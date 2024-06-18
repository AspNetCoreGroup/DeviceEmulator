using System;
using System.Threading;
using System.Threading.Tasks;
using DeviceEmulator.BaseDevice;
using Xunit;

namespace DeviceEmulator.Tests
{
    public class RealTimeClockBaseTests
    {
        [Fact]
        public void InitializationTest()
        {
            var startTime = DateTime.UtcNow;
            startTime = new DateTime(startTime.Year, startTime.Month, startTime.Day, startTime.Hour, startTime.Minute, 0, DateTimeKind.Utc);
            var rtc = new RealTimeClockBase(startTime, DateTime.UtcNow.AddYears(1), 100);

            Xunit.Assert.Equal(startTime, rtc.StartTimeClock);
            Xunit.Assert.Equal(startTime, rtc.GetRealTimeClock());
        }

        [Fact]
        public async Task StartStopRtcTest()
        {
            var startTime = DateTime.UtcNow;
            var rtc = new RealTimeClockBase(startTime, DateTime.UtcNow.AddYears(1), 100);
            var cancellationToken = new CancellationTokenSource();

            bool started = rtc.StartRtc(cancellationToken.Token);
            Xunit.Assert.True(started);

            await Task.Delay(50); // Let the clock run for a bit

            bool stopped = rtc.StopRtc();
            Xunit.Assert.True(stopped);

            DateTime updatedTime = rtc.GetRealTimeClock();
            Xunit.Assert.True(updatedTime > startTime);
        }

        [Fact]
        public async Task RtcRunsCorrectlyTest()
        {
            var startTime = DateTime.UtcNow;
            var rtc = new RealTimeClockBase(startTime, DateTime.UtcNow.AddYears(1), 100);
            var cancellationToken = new CancellationTokenSource();

            rtc.StartRtc(cancellationToken.Token);

            await Task.Delay(100); // Let the clock run for 100 ms
            rtc.StopRtc();

            DateTime updatedTime = rtc.GetRealTimeClock();
            TimeSpan elapsed = updatedTime - startTime;

            Xunit.Assert.True(elapsed.TotalSeconds >= 0.1); // Since the clock step is 1 second per tick
        }

        [Fact]
        public void StopRtcNotRunningTest()
        {
            var rtc = new RealTimeClockBase(DateTime.UtcNow, DateTime.UtcNow.AddYears(1), 100);
            bool result = rtc.StopRtc();

            Xunit.Assert.False(result); // Should return false if RTC was not running
        }
    }
}
