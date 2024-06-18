using DeviceEmulator.Interfaces;

namespace DeviceEmulator.BaseDevice
{
    public class RealTimeClockBase : IRealTimeClock
    {
        // private int _step = 60;
        private Task? _runTask;
        private CancellationTokenSource? _cts;

        public int Step { get; }
        public long I { get; protected set; }
        public DateTime EndTimeClock { get; }
        //public object Locker { get; }
        public DateTime StartTimeClock { get; }
        public RealTimeClockBase(DateTime startTimeClock, DateTime endTimeClock, int step)
        {
            StartTimeClock = startTimeClock;
            I = ((DateTimeOffset)StartTimeClock).ToUnixTimeSeconds();
            Step = step;
            EndTimeClock = endTimeClock;
        }

        public DateTime GetRealTimeClock()
        {
            return DateTimeOffset.FromUnixTimeSeconds(I).UtcDateTime;
        }

        protected virtual  async Task Run(CancellationToken cancellationToken)
        {
            while (!cancellationToken.IsCancellationRequested)
            {
                I += Step;
                if (((DateTimeOffset)EndTimeClock).ToUnixTimeSeconds() < I)
                {
                    return;
                }
                await Task.Delay(10, cancellationToken);
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
    }
}
