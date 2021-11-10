# TweetSpot - A Twitter processing feed architecture example

The concept is this: Let's use processing of Twitter *tweets* as a use case to discover and prove the *sweet spot* in enterprise architecture, hence the name *TweetSpot*.

The initial effort is being done as an illustration to a potential employer.  However in the long term, I want to make this an ever-evolving testing ground and showcase for architecture techniques.  It is constructed in a way that's maintainable and horizontally and vertically scalable.  My overall philosophy can be attributed to a statement I once read in the introduction of a book years ago:

> Lofty heights are oft achived, while standing on the shoulders of giants.

To a large degree, I stand on the shoulders of giants that preceeded me establishing design methodology, and giants that produced enterprise tools I can effectively leverage.

Inspiration for this implementation includes the folowing:

- [SOLID](https://www.c-sharpcorner.com/UploadFile/damubetha/solid-principles-in-C-Sharp/) design principles.
- [Roy Oserove naming standards](https://osherove.com/blog/2005/4/3/naming-standards-for-unit-tests.html) for naming of unit tests.
- [Event Sourcing and DDD](https://www.youtube.com/watch?v=-iuMjjKQnhg) presentation by Udi Dahan, or anything else by Udi Dahan.
- [CQRS - Command Query Responsibility Segregation](https://martinfowler.com/bliki/CQRS.html) by Martin Fowler.

## Tools Used in Implementation

The tools and technologies employed in this technology are as follows:

| Tool                                                                      | Purpose                                                                                                                                            |
| ------------------------------------------------------------------------- | -------------------------------------------------------------------------------------------------------------------------------------------------- |
| [C# .NET 5 Framework](https://dotnet.microsoft.com/download/dotnet/5.0)   | All server based code.                                                                                                                             |
| Twitter [API Access](<https://developer.twitter.com/en/apply-for-access>) | A live stream of data that most people can relate to as a valid example.                                                                           |
| [Mass Transit](masstransit-project.com) service bus                       | For the benefit of scalability, it is my intent to use an enterprise service bus as the foundation of interaction with message queues.             |
| [MOQ](https://github.com/moq/moq4)                                        | Used for mocking dependencies on unit tests.                                                                                                       |
| [Microsoft Entity Framework](https://docs.microsoft.com/en-us/ef/core/)   | Used as a mediator between code and database.  Provides framework for managing migrations.  Has the ability to talk to diverse database back ends. |

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
| Persistence          | Declarations of persistence interfaces.                                                                                                     |
| Persistence.EF       | Persistence implementations using Microsoft Entity Framework Core.                                                                          |
| Persistence.InMemory | Persistence implementations using in-memory mechanisms to maintain state.                                                                   |

## Guidelines for Assembly Types

| Type     | Philosophy and Approach                                                                                                                                                                                                                                                                                                                                                                                                                              |
| -------- | ---------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------- |
| BASE-DLL | This single class library contains all of the primative data types and interfaces for the enterprise. Only with reluctance should any implementaton be placed in this library.  It's general purpose is for global declarations.                                                                                                                                                                                                                     |
| IMPL-DLL | There can be an infinite number of these.  Since IMPL-DLL-A *never* references IMPL-DLL-B, they can only reference dependencies on each other by using dependency interfaces declared in BASE-DLL.  If IMPL-DLL-A.SomeClass has a need at runtime for IMPL-DLL-B.SomeOtherClass, that dependency should be facilitated through BASE-DLL.ISomePurpose.                                                                                                |
| EXE      | An executable assembly should relate to a defined set of services for a given deployment scenario.  In the same way a conductor assembles and orchestra, brass section, the woodwinds, etc., the EXE should establish on startup the dependency relationship of BASE-DLL.IAbc being implemented by IMPL-DLL.Abc.  Any implementation in this assembly beyond assembling and starting the orchestra of services should be considered an anti-pattern. |
| TEST-DLL | This is for unit testing.  There should be exactly one test assembly for every one IMPL-DLL and one for the BASE-DLL. They should strive toward 100% code coverage.                                                                                                                                                                                                                                                                                  |

## Assemblies / Projects

Implementations are placed in varying assemblies to allow for diversity in deployment scenarios.

| Assembly                             | Type     | Purpose / Deployment                                                                                                                                                                                                                                                                |
| ------------------------------------ | -------- | ----------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------- |
| TweetSpot.Primatives                 | BASE-DLL | Provides primative data types and interfaces to be used throughout this implementation.                                                                                                                                                                                             |
| TweetSpot.Primatives.xunit           | TEST-DLL | Provides testing on any implementations done in TweetSpot.Primatives.                                                                                                                                                                                                               |
| TweetSpot.AllInOne                   | EXE      | Provides the simplest integration of all the provided services with minimal configuration required.                                                                                                                                                                                 |
| TweetSpot.Feed.Services              | IMPL-DLL | All implementation for the incoming feed from Twitter and it's subsequent processing and persistence is implemented here.                                                                                                                                                           |
| TweetSpot.Feed                       | EXE      | Provides the enterprise version of the Twitter Feed.  It has one job to do, receive the tweets and place them in on the service bus. It employs only a single service from TweetSpot.Feed.Services.dll.  It can be run on a separate machine for the sake of scalability if needed. |
| TweetSpot.Feed.Services.xunit        | TEST-DLL | Provides testing for all classes implemented in TweetSpot.Feed.Services.                                                                                                                                                                                                            |
| TweetSpot.Processing.Services        | IMPL-DLL | Provides all analysis services that evaluate and report upon feed data.                                                                                                                                                                                                             |
| TweetSpot.Processing                 | EXE      | Provides enterprise version of processing services.  May be multiple instances to scale-out.                                                                                                                                                                                        |
| TweetSpot.Persistence.EF             | IMPL-DLL | Beginning of implementation using Entity Frameworks to persist to a database.                                                                                                                                                                                                       |
| TweetSpot.Persistence.EF.xunit       | TEST-DLL | Testing for above.  Incomplete.  Having difficulties making Sqlite in-memory fixture working as expected.                                                                                                                                                                           |
| TweetSpot.Persistence.InMemory       | IMPL-DLL | Persistence implementations that either maintain their own in-memory state or derive state from embedded resources.                                                                                                                                                                 |
| TweetSpot.Persistence.InMemory.xunit | TEST-DLL | Testing for TweetSpot.Persistence.InMemory.                                                                                                                                                                                                                                         |

## Compare / contrast EXE alternatives

In general, the only executable currently working is the TweetSpot.AllInOne.exe.  The enterprise-ready scalable alternatives require services that are not yet implemented.

| EXE Assembly         | Status  | Instances | Transport | Persist   | Caching   | Feed | Analysis | Sagas |
| -------------------- | ------- | --------- | --------- | --------- | --------- | ---- | -------- | ----- |
| TweetSpot.AllInOne   | Working | Single    | In-Memory | In-Memory | In-Memory | Yes  | Yes      | No    |
| TweetSpot.Feed       | Future  | Single    | RabbitMQ  | Postgres  | Redis     | Yes  | No       | No    |
| TweetSpot.Processing | Future  | Multiple  | RabbitMQ  | Postgres  | Redis     | No   | Yes      | Yes   |

## Running from the developer command prompt

```bash
dotnet user-secrets -p src/TweetSpot.Feed.Services/ set TwitterBearerToken [YOUR TOKEN FROM TWITTER]
cd src/TweetSpot.AllInOne
dotnet run
```

## What you should see

You'll see several informational startup events that are not detailed here.  In addition, you should see:

`
TWITTER Feed provider is opening access to feed using token AAA...27F.
`
This means the feed provider published an event to the bus that it is starting up.  It is giving you an abbreviation of the first/last characters of the bearer token it used to authenticate.

`
info: TweetSpot.ServiceBus.Consumers.LogEventsToConsoleConsumer[0]
`
This just identifies the originator of the given log message. Whatever line that follows is logged by it.

`
Current speed: 53.7/sec. Average 49.2/sec.
`
This indicates the speed achieved by the last 100 transactions, and the average speed for the duration of our uptime.

`
Crypto Tweet [#ADA,Bitcoin] [Raw data from tweet]
`
This indicates an event was produced by a handler that a crypto-related tweet was detected.  An event was published and our display handler, as a subscriber to the event, is displaying it's occurrence.  The values in brackets are keywords that were detected in the body of the tweet.

## Some useful links

- Twitter-provided [Stream Endpoint](https://developer.twitter.com/en/docs/basics/getting-started)
- [Sampled stream endpoint](https://developer.twitter.com/en/docs/twitter-api/tweets/sampled-stream/introduction)
- Twitter [API Access] (<https://developer.twitter.com/en/apply-for-access>)
