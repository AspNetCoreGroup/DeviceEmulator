using DeviceEmulator.BaseDevice;
using DeviceEmulator.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Sockets;
using System.Text;
using System.Threading.Tasks;

namespace DeviceEmulator.Device
{
    public class EDevice : DeviceBase
    {
        public override IPropetryCollection? PuppetryCollection { get; protected set; }
        public override IEnumerable<IRegister> Registers { get; protected set; } = new List<IRegister>();
        public override IEnumerable<IProfile> Profiles { get; protected set; } = new List<IProfile>();

        public override Task<bool> Init(string initStr, CancellationToken cancellationToken)
        {

            RealTimeClock = new RealTimeClockBase(Locker, new DateTime(2022, 1, 1));

            PuppetryCollection = new PropetryCollection(new List<IProperty>
            {
                new Property("SerialNumber", GenerateSerialNumber()),
                new Property("DeviceType", GenerateDeviceType())
            });

            IRegister u = new Register(RealTimeClock, "U", 230, new ScaleAndUnit() { Scale = 0, Unit = 1 }, IncrementTipe.UpDown);
            IRegister Ain = new Register(RealTimeClock, "Ain", 10, new ScaleAndUnit() { Scale = 0, Unit = 2 }, IncrementTipe.Increment);
            IRegister Qin = new Register(RealTimeClock, "Qin", 10, new ScaleAndUnit() { Scale = 0, Unit = 3 }, IncrementTipe.Increment);

            Registers = new List<IRegister>()
            {
                u,
                Ain
            };
            List<IRegister> Registers3 = new List<IRegister>()
            {
                u,
                Ain
            };


            Profiles = new List<IProfile>()
            {
                new Profile(RealTimeClock,Registers3,"I", 900 )
            };

            foreach (IRegister i in Registers)
            {
                i.StartWatch(cancellationToken);
            }

            foreach (IProfile i in Profiles)
            {
                i.StartMonitoring(cancellationToken);
            }




            return Task.FromResult(RealTimeClock?.StartRtc(cancellationToken) ?? false);
        }
    }
}
