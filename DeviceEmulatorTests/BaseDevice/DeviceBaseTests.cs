using DeviceEmulator.BaseDevice;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Xunit;

namespace DeviceEmulator.Tests
{
    public class DeviceBaseTests
    {
        [Fact]
        public async Task Init_DeviceBase_Success()
        {
            // Arrange
            var deviceBase = new DeviceBase();
            var cancellationTokenSource = new CancellationTokenSource();
            var initStr = "Init String";

            // Act
            var result = await deviceBase.Init(initStr, cancellationTokenSource.Token);

            // Assert
            Xunit.Assert.True(result);
            Xunit.Assert.NotNull(deviceBase.RealTimeClock);
            Xunit.Assert.NotNull(deviceBase.PuppetryCollection);
            Xunit.Assert.NotNull(deviceBase.Registers);
            Xunit.Assert.Single(deviceBase.Registers);
        }

        [Fact]
        public async Task StartStop_DeviceBase_Success()
        {
            // Arrange
            var deviceBase = new DeviceBase();
            var cancellationTokenSource = new CancellationTokenSource();
            await deviceBase.Init("Init String", cancellationTokenSource.Token);

            // Act
            var startResult =  deviceBase.RealTimeClock?.StartRtc(cancellationTokenSource.Token);
            deviceBase.Registers.ToList().ForEach(r => r.StartWatch(cancellationTokenSource.Token));
            cancellationTokenSource.Cancel();
            var stopResult =  deviceBase.RealTimeClock?.StopRtc();

            // Assert
            Xunit.Assert.True(startResult);
            Xunit.Assert.True(stopResult);
        }

        [Fact]
        public async Task RealTimeClock_StartStop_Success()
        {
            // Arrange
            var deviceBase = new DeviceBase();
            var cancellationTokenSource = new CancellationTokenSource();
            await deviceBase.Init("Init String", cancellationTokenSource.Token);

            // Act
            var startResult =  deviceBase.RealTimeClock?.StartRtc(cancellationTokenSource.Token);
            await Task.Delay(10000);
            var stopResult =  deviceBase.RealTimeClock?.StopRtc();

            // Assert
            Xunit.Assert.True(startResult);
            Xunit.Assert.True(stopResult);
        }
    }
}
