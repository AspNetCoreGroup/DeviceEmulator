using DeviceEmulator.Interfaces;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DeviceEmulator.BaseDevice
{
    public class Profile : IProfile
    {
        private IRealTimeClock Rtc { get; }
        private IEnumerable<IRegister> _register { get; }
        private readonly List<IValue> _values;
        private readonly CancellationTokenSource _cancellationTokenSource;

        public Profile(IRealTimeClock deviceRtc, IEnumerable<IRegister> register, string name, uint period)
        {
            Rtc = deviceRtc;
            _register = register;
            Name = name;
            Period = period;
            _values = new List<IValue>();
            _cancellationTokenSource = new CancellationTokenSource();
        }

        public string Name { get; }
        public uint Period { get; }

        public IEnumerable<IValue> GetValues() => _values;

        public IEnumerable<IValue> GetValues(DateTime from, DateTime to)
        {
            return _values.Where(value =>
            {
                if (DateTime.TryParse(value.GetValue(), out var timestamp))
                {
                    return timestamp >= from && timestamp <= to;
                }
                return false;
            });
        }

        public async Task StartMonitoring(CancellationToken token)
        {
             MonitorLoop(token);
        }

        public void StopMonitoring()
        {
            _cancellationTokenSource.Cancel();
        }

        private async Task MonitorLoop(CancellationToken token)
        {
            long lastDT = 0;
            while (!token.IsCancellationRequested)
            {
                if (lastDT < Rtc.I)
                {
                    Debug.WriteLine(Rtc.I);
                    if (Rtc.I % Period == 0)
                    {
                        var timestamp = Rtc.GetRealTimeClock();
                        foreach(var value in _register)
                        {
                            _values.Add(new RegisterValue(timestamp, value.Value));
                            Debug.WriteLine(value.Name +": "+ _values.Last().GetValue());
                        }

                    }
                    lastDT = Rtc.I;
                }
                await Task.Delay(1);
            }
        }
    }

    public class RegisterValue : IValue
    {
        private readonly DateTime _timestamp;
        private readonly uint _value;

        public RegisterValue(DateTime timestamp, uint value)
        {
            _timestamp = timestamp;
            _value = value;
        }

        public string GetValue()
        {
            return $"{_timestamp:O} - {_value}";
        }
    }
}
