# TweetSpot - A Twitter Processing Feed Architecture Example

Hey, let's use Twitter *tweets* as a use case to discover and prove the *sweet spot* in architecture, hence the name *TweetSpot*.

This is a solution to demonstrate one possible enterprise architecture.  It does so in a way that's maintainable and horizontally scalable.  My overall philosophy can be attributed to a statement I once read in the introduction of a book years ago that stuck with me:

> Lofty heights are oft achived, while standing on the shoulders of giants.

It is my intention to use this project as an ongoing workplace and eventually a showcase of good architecture and good practices.  To a large degree, I stand on the shoulders of giants that preceeded me establishing design methodology, and giants that produced enterprise tools I can effectively leverage.

Inspiration for this implementation includes the folowing:

- [SOLID](https://www.c-sharpcorner.com/UploadFile/damubetha/solid-principles-in-C-Sharp/) design principles.
- [Roy Oserove naming standards](https://osherove.com/blog/2005/4/3/naming-standards-for-unit-tests.html) for naming of unit tests.
- [Event Sourcing and DDD](https://www.youtube.com/watch?v=-iuMjjKQnhg) presentation by Udi Dahan, or anything else by Udi Dahan.
- [CQRS - Command Query Responsibility Segregation](https://martinfowler.com/bliki/CQRS.html) by Martin Fowler.

## Tools Used in Implementation

The tools and technologies employed in this technology are as follows:

| Tool                                                                      | Purpose                                                                                                                                |
| ------------------------------------------------------------------------- | -------------------------------------------------------------------------------------------------------------------------------------- |
| [C# .NET 5 Framework](https://dotnet.microsoft.com/download/dotnet/5.0)   | All server based code.                                                                                                                 |
| Twitter [API Access](<https://developer.twitter.com/en/apply-for-access>) | A live stream of data that most people can relate to as a valid example.                                                               |
| [Mass Transit](masstransit-project.com) service bus                       | For the benefit of scalability, it is my intent to use an enterprise service bus as the foundation of interaction with message queues. |
| [MOQ](https://github.com/moq/moq4)                                        | Used for mocking dependencies on unit tests.                                                                                           |

## .NET Namespaces

All assemblies in this application suite are rooted in the same namespace.  The namespaces below should be understood to be prefixed with "TweetSpot.".

| Namespace            | Purpose                                                                                                                                     |
| -------------------- | ------------------------------------------------------------------------------------------------------------------------------------------- |
| BackgroundServices   | Processing services.  Any service that subclasses from BackGroundService or otherwise implements IHostedService.                            |
| Models               | Data containers.                                                                                                                            |
| ServiceBus.Commands  | Service bus interfaces intended to be implemented as CQRS commands.  They should be named as present tense verbs.                           |
| ServiceBus.Events    | Service bus interfaces intended to be CQRS events.  They should be named as past tense verbs.                                               |
| ServiceBus.Consumers | Handlers that implement the IConsumer\<T\> interface to respond to service bus commands or events.                                          |
| Exceptions           | Any exception types that are unique to this system.                                                                                         |
| Net                  | Any implementations that extend or act as containers for classes in the System.Net namespaces.                                              |
| Delegates            | Any delegates that are used as types in various interfaces.                                                                                 |
| Extensions           | Extension method classes.  Although the folder is located here, the namespace should be modified to match that of the class being extended. |


## Guidelines for Assembly Types

| Type     | Philosophy and Approach                                                                                                                                                                                                                                                                                                                                                                                                                              |
| -------- | ---------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------- |
| BASE-DLL | This single class library contains all of the primative data types and interfaces for the enterprise. Only with reluctance should any implementaton be placed in this library.  It's general purpose is for global declarations.                                                                                                                                                                                                                     |
| IMPL-DLL | There can be an infinite number of these.  Since IMPL-DLL-A *never* references IMPL-DLL-B, they can only reference dependencies on each other by using dependency interfaces declared in BASE-DLL.  If IMPL-DLL-A.SomeClass has a need at runtime for IMPL-DLL-B.SomeOtherClass, that dependency should be facilitated through BASE-DLL.ISomePurpose.                                                                                                |
| EXE      | An executable assembly should relate to a defined set of services for a given deployment scenario.  In the same way a conductor assembles and orchestra, brass section, the woodwinds, etc., the EXE should establish on startup the dependency relationship of BASE-DLL.IAbc being implemented by IMPL-DLL.Abc.  Any implementation in this assembly beyond assembling and starting the orchestra of services should be considered an anti-pattern. |
| TEST-DLL | This is for unit testing.  There should be exactly one test assembly for every one IMPL-DLL and one for the BASE-DLL. They should strive toward 100% code coverage.                                                                                                                                                                                                                                                                                  |

## Assemblies / Projects

Though all assemblies are rooted in the same namespace, implementation is placed in different assemblies according to deployment scenarios.
| Assembly                      | Type     | Purpose / Deployment                                                                                                                                                                                                                                                                |
| ----------------------------- | -------- | ----------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------- |
| TweetSpot.AllInOne            | EXE      | Provides the simplest integration of all the provided services with minimal configuration required.                                                                                                                                                                                 |
| TweetSpot.Feed.Services       | IMPL-DLL | All implementation for the incoming feed from Twitter and it's subsequent processing and persistence is implemented here.                                                                                                                                                           |
| TweetSpot.Feed                | EXE      | Provides the enterprise version of the Twitter Feed.  It has one job to do, receive the tweets and place them in on the service bus. It employs only a single service from TweetSpot.Feed.Services.dll.  It can be run on a separate machine for the sake of scalability if needed. |
| TweetSpot.Feed.Services.xunit | TEST-DLL | Provides testing for all classes implemented in TweetSpot.Feed.Services.                                                                                                                                                                                                            |
| TweetSpot.Processing.Services | IMPL-DLL | Provides all analysis services that evaluate and report upon feed data.                                                                                                                                                                                                             |
| TweetSpot.Processing          | EXE      | Provides enterprise version of processing services.  May be multiple instances to scale-out.                                                                                                                                                                                        |
| TweetSpot.Primatives          | BASE-DLL | Provides primative data types and interfaces to be used throughout this implementation.                                                                                                                                                                                             |
| TweetSpot.Primatives.xunit    | TEST-DLL | Provides testing on any implementations done in TweetSpot.Primatives.                                                                                                                                                                                                               |

## Compare / Contrast EXE Alternatives

In general, the only executable currently working is the AllInOne.  The enterprise-ready scalable alternatives require services that are not yet implemented.
| EXE Assembly         | Status  | Instances | Transport | Persist   | Caching   | Feed | Analysis | Sagas |
| -------------------- | ------- | --------- | --------- | --------- | --------- | ---- | -------- | ----- |
| TweetSpot.AllInOne   | Working | Single    | In-Memory | In-Memory | In-Memory | Yes  | Yes      | No    |
| TweetSpot.Feed       | Future  | Single    | RabbitMQ  | Postgres  | Redis     | Yes  | No       | No    |
| TweetSpot.Processing | Future  | Multiple  | RabbitMQ  | Postgres  | Redis     | No   | Yes      | Yes   |

## Some useful links

- Twitter-provided [Stream Endpoint](https://developer.twitter.com/en/docs/basics/getting-started)
- [Sampled stream endpoint](https://developer.twitter.com/en/docs/twitter-api/tweets/sampled-stream/introduction)
- Twitter [API Access] (<https://developer.twitter.com/en/apply-for-access>)
