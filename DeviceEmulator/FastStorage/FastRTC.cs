using CommonTypeDevice;
using DeviceEmulator.BaseDevice;
using System.Diagnostics;
using System.Text.Json;
using static DeviceEmulator.FastStorage.FastRTC;

namespace DeviceEmulator.FastStorage
{

    public interface IFastRtc
    {
        void Init(IncreaseRegister[] increaseRegisters, WriteProfile[] vriteProfiles, DoEvent[] eventDo);
    }
    public class FastRTC : RealTimeClockBase , IFastRtc
    {
        public FastRTC(DateTime startTimeClock, DateTime endTimeClock, int step) : base(startTimeClock, endTimeClock, step)
        {

        }

        public void Init(IncreaseRegister[] increaseRegisters, WriteProfile[] writeProfiles, DoEvent[] eventDo)
        {
            this.increaseRegisters = increaseRegisters;
            this.writeProfiles = writeProfiles;
            this.eventDo = eventDo;
        }

        public IncreaseRegister[ ] increaseRegisters;
        public delegate void IncreaseRegister();

        public WriteProfile[] writeProfiles ;
        public delegate void WriteProfile();

        public DoEvent[] eventDo;
        public delegate Task DoEvent();

        override protected async Task Run(CancellationToken cancellationToken)
        {
            while (!cancellationToken.IsCancellationRequested)
            {
                I += Step;
                if (((DateTimeOffset)EndTimeClock).ToUnixTimeSeconds() < I)
                {
                   break;
                }
                foreach (var register in increaseRegisters) {
                    register();
                }
                foreach (var profile in writeProfiles)
                {
                    profile();
                }
                //await Task.Delay(1);
            }
            while (!cancellationToken.IsCancellationRequested)
            {
                I += Step;
                foreach (var profile in writeProfiles)
                {
                    profile();
                }
                foreach (var register in increaseRegisters)
                {
                    register();
                }
                foreach (var thisevent in eventDo)
                {
                    thisevent();
                }
                //Debug.WriteLine("");
                //Debug.WriteLine("Task.Delay" + Step * 1000);
                await Task.Delay(Step*1000);
            }
            return;
        }

        public bool StopRtc()
        {
            throw new NotImplementedException();
        }
    }
}
