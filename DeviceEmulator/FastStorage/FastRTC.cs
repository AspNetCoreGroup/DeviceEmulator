using DeviceEmulator.BaseDevice;
using DeviceEmulator.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static DeviceEmulator.FastStorage.FastRTC;

namespace DeviceEmulator.FastStorage
{

    public interface IFastRtc
    {
        void Init(IncreaseRegister[] increaseRegisters, WriteProfile[] vriteProfiles);
    }
    public class FastRTC : RealTimeClockBase , IFastRtc
    {
        public FastRTC(DateTime startTimeClock, DateTime endTimeClock, int step) : base(startTimeClock, endTimeClock, step)
        {

        }

        public void Init(IncreaseRegister[] increaseRegisters, WriteProfile[] vriteProfiles)
        {
            this.increaseRegisters = increaseRegisters;
            this.vriteProfiles = vriteProfiles;
        }

        public IncreaseRegister[ ] increaseRegisters;
        public delegate void IncreaseRegister();

        public WriteProfile[] vriteProfiles ;
        public delegate void WriteProfile();

        override protected async Task Run(CancellationToken cancellationToken)
        {
            while (!cancellationToken.IsCancellationRequested)
            {
                I += Step;
                if (((DateTimeOffset)EndTimeClock).ToUnixTimeSeconds() < I)
                {
                    return;
                }
                foreach (var register in increaseRegisters) {
                    register();
                }
                foreach (var profile in vriteProfiles)
                {
                    profile();
                }

            }
            return;
        }

        public bool StopRtc()
        {
            throw new NotImplementedException();
        }
    }
}
