using DeviceEmulator.Interfaces;
using System.Diagnostics;
using System.Threading;

namespace DeviceEmulator.BaseDevice
{
    public class RegisterUseRTC : IFRegister, IRegister
    {
        IncrementTipe _incrementType;
        IDeviceRtc Rtc { get; }
        uint _startValue;
        public RegisterUseRTC(IDeviceRtc rtc, string sogialName, uint startValue, IScaleAndUnit scaleAndUnit, IncrementTipe incrementTipe)
        {
            Rtc = rtc;
            Name = sogialName;
            Value = startValue;
            ScaleAndUnit = scaleAndUnit;
            _startValue = startValue;
            _incrementType = incrementTipe;
        }

        public string Name { get; set; }
        public uint Value { get; set; }
        public IScaleAndUnit ScaleAndUnit { get; private set; }

        Task? WatchTask;

        public Task StartWatch(CancellationToken token)
        {

            WatchTask = WatchLoop(token);
            return Task.CompletedTask;
        }

        private async Task WatchLoop(CancellationToken token)
        {
            long oldRtc = Rtc.I;
            while (!token.IsCancellationRequested)
            {
                if (oldRtc != Rtc.I)
                {
                    oldRtc = Rtc.I;
                    IncreaseValue();
                    // Debug.WriteLine("Register " + Name + ": " + Value);
                }

                await Task.Delay(1, token);// Добавляем небольшую задержку
            }
        }

        protected void IncreaseValue()
        {
            if (_incrementType == IncrementTipe.UpDown)
            {
                Value = _startValue + (uint)rrrr();
            }
            else if (_incrementType == IncrementTipe.Increment)
            {
                Value += Convert.ToUInt32(Math.Abs(rrrr()));
            }
            else if (_incrementType == IncrementTipe.Decrement)
            {
                if (Value > 0)
                {
                    Value -= Convert.ToUInt32(Math.Abs(rrrr()));
                }
            }
        }

        private static double rrrr()
        {
            Random random = new Random();
            double[] probabilities = new double[] { 0.5, 0.3, 0.15, 0.05 };
            int[] values = new int[] { 10, 50, 75, 100 };

            double cumulative = 0.0;
            double r = random.NextDouble();

            for (int i = 0; i < probabilities.Length; i++)
            {
                cumulative += probabilities[i];
                if (r < cumulative)
                {
                    return values[i];
                }
            }

            return values[values.Length - 1]; // fallback
        }

        public string GetValue()
        {
            return Convert.ToString(Value * Math.Pow(10, ScaleAndUnit.Scale));
        }

        void IFRegister.IncreaseValue() => IncreaseValue();
    }

    public enum IncrementTipe
    {
        UpDown,
        Increment,
        Decrement
    }

    public class ScaleAndUnit : IScaleAndUnit
    {
        public sbyte Scale { get; set; }
        public byte Unit { get; set; }
    }
}
