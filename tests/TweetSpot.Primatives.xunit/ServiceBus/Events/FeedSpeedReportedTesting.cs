using System;
using System.Linq;
using System.Runtime.InteropServices.ComTypes;
using Xunit;
using Xunit.Abstractions;

namespace TweetSpot.ServiceBus.Events
{
    public class FeedSpeedReportedTesting
    {
        private readonly ITestOutputHelper _testOutput;


        // 
        // Timeline:
        //
        // | <------------------------------ 15 seconds ----------------------------------------> |
        // | <------------------------------ 700 tweets ----------------------------------------> |
        // | <------------------------------ 46.7 / sec ----------------------------------------> |
        // |       CYCLE ONE               |                CYCLE TWO                             |
        // | ----------------------------- | ---------------------------------------------------- | 
        // | <-----  5 seconds ----------> | <-------------------- 10 seconds ------------------> |
        // | <------ 300 Tweets ---------> | <-------------------- 400 Tweets ------------------> |
        // | <------- 60 / sec ----------> | <--------------------- 40 / sec -------------------> |
        // 

        private readonly int[] _secondDurationPerCycle = new[] { 5, 10 };
        private readonly int[] _tweetCountPerCycle = new [] { 300, 400 };
        private readonly TimeSpan[] _elapsedTimePerCycle;


        private readonly FeedSpeedReported _reportOne;
        private readonly FeedSpeedReported _reportTwo;


        public FeedSpeedReportedTesting(ITestOutputHelper testOutput)
        {
            _testOutput = testOutput;
            _elapsedTimePerCycle = new[]
            {
                TimeSpan.FromSeconds(_secondDurationPerCycle[0]),
                TimeSpan.FromSeconds(_secondDurationPerCycle.Take(2).Sum()),
            };
            _reportOne = new FeedSpeedReported(_elapsedTimePerCycle[0], (ulong) _tweetCountPerCycle[0], null);
            _reportTwo = new FeedSpeedReported(_elapsedTimePerCycle[1], (ulong) _tweetCountPerCycle.Take(2).Sum(), _reportOne);
        }
        [Fact]
        public void TimeSpanForAllCycles_One_Correct()
        {
            Assert.Equal(_elapsedTimePerCycle[0], _reportOne.TimeSpanForAllCycles );
        }

        [Fact]
        public void TimeSpanForAllCycles_Two_Correct()
        {
            Assert.Equal(_elapsedTimePerCycle[1], _reportTwo.TimeSpanForAllCycles);
        }

        [Fact]
        public void TweetsReceivedForAllCycles_One_Correct()
        {
            Assert.Equal((ulong) _tweetCountPerCycle[0], _reportOne.TweetsReceivedForAllCycles);
        }

        [Fact]
        public void TweetsReceivedForAllCycles_Two_Correct()
        {
            Assert.Equal((ulong) _tweetCountPerCycle.Take(2).Sum(), _reportTwo.TweetsReceivedForAllCycles);
        }

        [Fact]
        public void AverageSpeedTweetsPerSecond_One_Correct()
        {
            var tweets = Convert.ToDouble(_tweetCountPerCycle[0]);
            var seconds = Convert.ToDouble(_secondDurationPerCycle[0]);
            Assert.Equal(tweets/seconds, _reportOne.AverageSpeedTweetsPerSecond);
        }

        [Fact]
        public void AverageSpeedTweetsPerSecond_One_SameAsCurrent()
        {
            // On the first cycle, average speed is same as current speed because there is only one cycle to report
            Assert.Equal(_reportOne.CurrentSpeedTweetsPerSecond , _reportOne.AverageSpeedTweetsPerSecond);
        }

        [Fact]
        public void AverageSpeedTweetsPerSecond_Two_VariesFromCurrent()
        {
            // By the second cycle, the Average has diverged from the current
            Assert.NotEqual(_reportTwo.CurrentSpeedTweetsPerSecond, _reportTwo.AverageSpeedTweetsPerSecond);
        }

        [Fact]
        public void AverageSpeedTweetsPerSecond_Two_Correct()
        {
            var tweets = Convert.ToDouble(_tweetCountPerCycle.Take(2).Sum());
            var seconds = Convert.ToDouble(_secondDurationPerCycle.Take(2).Sum());
            Assert.Equal(tweets / seconds, _reportTwo.AverageSpeedTweetsPerSecond);
        }

        [Fact]
        public void CurrentSpeedTweetsPerSecond_One_Correct()
        {
            var tweets = Convert.ToDouble(_tweetCountPerCycle[0]);
            var seconds = Convert.ToDouble(_secondDurationPerCycle[0]);
            Assert.Equal(tweets / seconds, _reportOne.CurrentSpeedTweetsPerSecond);
        }

        [Fact]
        public void CurrentSpeedTweetsPerSecond_Two_Correct()
        {
            var tweets = Convert.ToDouble(_tweetCountPerCycle[1]);
            var seconds = Convert.ToDouble(_secondDurationPerCycle[1]);
            Assert.Equal(tweets / seconds, _reportTwo.CurrentSpeedTweetsPerSecond);
        }

    }
}