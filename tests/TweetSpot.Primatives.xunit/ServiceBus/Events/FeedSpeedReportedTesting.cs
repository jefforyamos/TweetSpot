using System;
using System.Collections.Generic;
using System.Linq;
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
        // |       CYCLE ONE               |                CYCLE TWO                             |         CYCLE THREE        |
        // | ----------------------------- | ---------------------------------------------------- | -------------------------- |
        // | <-----  5 seconds ----------> | <-------------------- 10 seconds ------------------> | <-----  4 seconds -------> |
        // | <------ 300 Tweets ---------> | <-------------------- 400 Tweets ------------------> | <------ 300 Tweets ------> |
        // | <------- 60 / sec ----------> | <--------------------- 40 / sec -------------------> | <------- 75 / sec -------> |

        // | <-------------------------------- 15 seconds --------------------------------------> |
        // | <-------------------------------- 700 tweets --------------------------------------> |
        // | <-------------------------------- 46.7 / sec --------------------------------------> |

        // | <--------------------------------------------------- 19 seconds ------------------------------------------------> |
        // | <-------------------------------------------------- 1000 tweets ------------------------------------------------> |
        // | <-------------------------------------------------- 52.6 / sec -------------------------------------------------> |
        // 

        private readonly int[] _secondDurationPerCycle = new[] { 5, 10, 4 };
        private readonly int[] _tweetCountPerCycle = new[] { 300, 400, 300 };
        private readonly TimeSpan[] _elapsedTimePerCycle;

        private readonly FeedSpeedReported[] _report;


        public FeedSpeedReportedTesting(ITestOutputHelper testOutput)
        {
            _testOutput = testOutput;
            var elapsedList = new List<TimeSpan>();
            for (var x = 0; x < _secondDurationPerCycle.Length; x++)
                elapsedList.Add(TimeSpan.FromSeconds(_secondDurationPerCycle.Take(x + 1).Sum()));
            _elapsedTimePerCycle = elapsedList.ToArray();
            var reportList = new List<FeedSpeedReported>();
            reportList.Add(new FeedSpeedReported(_elapsedTimePerCycle[0], (ulong)_tweetCountPerCycle[0], null));
            for (int x = 1; x < _secondDurationPerCycle.Length; x++)
            {
                var rpt = new FeedSpeedReported(_elapsedTimePerCycle[x], (ulong)_tweetCountPerCycle.Take(x + 1).Sum(), reportList[x - 1]);
                reportList.Add(rpt);
            }
            _report = reportList.ToArray();
        }

        [Theory]
        [InlineData(0)]
        [InlineData(1)]
        [InlineData(2)]
        public void TimeSpanForAllCycles_Any_Correct(int cycleNumber)
        {
            _testOutput.WriteLine(_report[cycleNumber].TimeSpanForAllCycles.ToString());
            Assert.Equal(_elapsedTimePerCycle[cycleNumber], _report[cycleNumber].TimeSpanForAllCycles);
        }


        [Theory]
        [InlineData(0)]
        [InlineData(1)]
        [InlineData(2)]
        public void TweetsReceivedForAllCycles_Any_Correct(int cycleNumber)
        {
            _testOutput.WriteLine(_report[cycleNumber].TweetsReceivedForAllCycles.ToString());
            Assert.Equal((ulong)_tweetCountPerCycle.Take(cycleNumber + 1).Sum(), _report[cycleNumber].TweetsReceivedForAllCycles);
        }

        [Fact]
        public void AverageSpeedTweetsPerSecond_FirstCycle_Correct()
        {
            _testOutput.WriteLine(_report[0].AverageSpeedTweetsPerSecond.ToString("N1"));
            var tweets = Convert.ToDouble(_tweetCountPerCycle[0]);
            var seconds = Convert.ToDouble(_secondDurationPerCycle[0]);
            Assert.Equal(tweets / seconds, _report[0].AverageSpeedTweetsPerSecond);
        }

        [Fact]
        public void AverageSpeedTweetsPerSecond_FirstCycle_SameAsCurrent()
        {
            _testOutput.WriteLine(_report[0].AverageSpeedTweetsPerSecond.ToString("N1"));
            // On the first cycle, average speed is same as current speed because there is only one cycle to report
            Assert.Equal(_report[0].CurrentSpeedTweetsPerSecond, _report[0].AverageSpeedTweetsPerSecond);
        }

        [Theory]
        [InlineData(1)]
        [InlineData(2)]
        public void AverageSpeedTweetsPerSecond_NormalCycle_VariesFromCurrent(int cycleNumber)
        {
            // By the second cycle, the Average has diverged from the current
            _testOutput.WriteLine(_report[cycleNumber].AverageSpeedTweetsPerSecond.ToString("N1"));
            Assert.NotEqual(_report[cycleNumber].CurrentSpeedTweetsPerSecond, _report[cycleNumber].AverageSpeedTweetsPerSecond);
        }

        [Theory]
        [InlineData(1)]
        [InlineData(2)]
        public void AverageSpeedTweetsPerSecond_NormalCycle_Correct(int cycleNumber)
        {
            _testOutput.WriteLine(_report[cycleNumber].AverageSpeedTweetsPerSecond.ToString("N1"));
            var tweets = Convert.ToDouble(_tweetCountPerCycle.Take(cycleNumber + 1).Sum());
            var seconds = Convert.ToDouble(_secondDurationPerCycle.Take(cycleNumber + 1).Sum());
            Assert.Equal(tweets / seconds, _report[cycleNumber].AverageSpeedTweetsPerSecond);
        }

        [Theory]
        [InlineData(0)]
        [InlineData(1)]
        [InlineData(2)]
        public void CurrentSpeedTweetsPerSecond_Any_Correct(int cycleNumber)
        {
            _testOutput.WriteLine(_report[cycleNumber].CurrentSpeedTweetsPerSecond.ToString("N1"));
            var tweets = Convert.ToDouble(_tweetCountPerCycle[cycleNumber]);
            var seconds = Convert.ToDouble(_secondDurationPerCycle[cycleNumber]);
            Assert.Equal(tweets / seconds, _report[cycleNumber].CurrentSpeedTweetsPerSecond);
        }


    }
}