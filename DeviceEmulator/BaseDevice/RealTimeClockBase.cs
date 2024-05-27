using DeviceEmulator.Interfaces;

namespace DeviceEmulator.BaseDevice
{
    public class RealTimeClockBase : IRealTimeClock
    {
        private long _i;
        private int _step = 1;

        public long I { get => _i; private set => _i = value; }

        public int Step => _step;

        public RealTimeClockBase(object looker, DateTime startTimeClock)
        {
            this.Locker = looker;
            StartTimeClock = startTimeClock;
            I = ((DateTimeOffset)StartTimeClock).ToUnixTimeSeconds();
        }
        public DateTime EndTimeClock { get; }
        public object Locker { get; }

        public DateTime StartTimeClock { get; }

        public DateTime GetRealTimeClock()
        {
            return DateTimeOffset.FromUnixTimeSeconds(I).UtcDateTime;
        }

        private async Task Run(CancellationToken cancellationToken)
        {
            while (!cancellationToken.IsCancellationRequested)
            {
                I += _step;
                if (((DateTimeOffset)StartTimeClock).ToUnixTimeSeconds() > I)
                {
                    return;
                }
                await Task.Delay(1, cancellationToken);

            }
            return;
        }

        public bool StartRtc(CancellationToken cancellationToken)
        {
            _cts = CancellationTokenSource.CreateLinkedTokenSource(cancellationToken);
            _runTask = Run(_cts.Token);
            return true;
        }

        public bool StopRtc()
        {
            if (_cts == null)
            {
                return false; // Not running
            }

            _cts.Cancel();
            try
            {
                _runTask?.Wait();
            }
            catch (AggregateException ex) when (ex.InnerExceptions.All(e => e is TaskCanceledException))
            {
                // Ignore task canceled exceptions
            }
            finally
            {
                _cts.Dispose();
                _cts = null;
                _runTask = null;
            }

            return true;
        }
        private Task? _runTask;
        private CancellationTokenSource? _cts;
    }
}
