using DeviceEmulator.Data;
using DeviceEmulator.Interfaces;
using System.Diagnostics;

namespace DeviceEmulator.UseRTC
{
    public class ProfileUseRTC : IProfile, IFProfile
    {
        private IRealTimeClock Rtc { get; }
        private IEnumerable<IRegister> Registers { get; }
        private readonly List<IValue> _values;
        private readonly CancellationTokenSource _cancellationTokenSource;
        private Task? loop;
        public string Name { get; }
        public uint Period { get; }

        public ProfileUseRTC(IRealTimeClock deviceRtc, IEnumerable<IRegister> register, string name, uint period)
        {
            Rtc = deviceRtc;
            Registers = register;
            Name = name;
            Period = period;
            _values = new List<IValue>();
            _cancellationTokenSource = new CancellationTokenSource();
        }


        public Task<IEnumerable<IValue>?> GetValues() => Task.FromResult<IEnumerable<IValue>?>(_values);

        public Task<IEnumerable<IValue>?> GetValues(DateTime from, DateTime to)
        {
            return Task.FromResult(_values.Where(value =>
            {
                if (DateTime.TryParse(value.GetValue(), out DateTime timestamp))
                {
                    return timestamp >= from && timestamp <= to;
                }
                return false;
            }) ?? null);
        }
        public Task StartMonitoring(CancellationToken token)
        {
            loop = MonitorLoop(token);

            return loop;
        }

        public void StopMonitoring()
        {
            _cancellationTokenSource.Cancel();
        }

        private async Task MonitorLoop(CancellationToken token)
        {
            long lastDT = 0;
            while (!token.IsCancellationRequested && !_cancellationTokenSource.IsCancellationRequested)
            {
                if (lastDT < Rtc.I)
                {
                    // Debug.WriteLine(Rtc.I);
                    if (Rtc.I % Period == 0)
                    {
                        SaveValueProfile();
                    }
                    lastDT = Rtc.I;
                }
                await Task.Delay(1);
            }
        }


        public void SaveValueProfile()
        {
            DateTime timestamp = Rtc.GetRealTimeClock();
            foreach (IRegister value in Registers)
            {

                _values.Add(new DataRegisterValue(timestamp, value.Value));
                if (i == 0)
                {
                    Debug.WriteLine(value.Name + ": " + _values.Last().GetValue());
                    i = 1000;
                }
                else i--;
            }
        }

        uint i = 1000;
        long lastDTF = 0;
        public void WriteProfile()
        {

            if (lastDTF < Rtc.I)
            {
                // Debug.WriteLine(Rtc.I);
                if (Rtc.I % Period == 0)
                {
                    SaveValueProfile();
                }
                lastDTF = Rtc.I;
            }
        }
    }
}
