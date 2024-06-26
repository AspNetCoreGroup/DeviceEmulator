using DeviceEmulator.BaseDevice;
using DeviceEmulator.FastStorage;
using DeviceEmulator.Interfaces;
using DeviceEmulator.UseRTC;
using static DeviceEmulator.FastStorage.FastRTC;

namespace DeviceEmulator.Device
{
    public class EDeviceDelegat : DeviceBase
    {
        public override IPropetryCollection? PuppetryCollection { get; protected set; }
        public override IEnumerable<IRegister> Registers { get; protected set; } = new List<IRegister>();
        public override IEnumerable<IProfile> Profiles { get; protected set; } = new List<IProfile>();

        public override Task<bool> Init(string initStr, CancellationToken cancellationToken)
        {


            PuppetryCollection = new PropetryCollection(new List<IProperty>
            {
                new Property("SerialNumber", GenerateSerialNumber()),
                new Property("DeviceType", GenerateDeviceType())
            });
            RealTimeClock = new FastRTC(new DateTime(2022, 1, 1), DateTime.Now, 60);

            IRegister u = new RegisterUseRTC(RealTimeClock, "U", 230, new ScaleAndUnit() { Scale = 0, Unit = 1 }, IncrementTipe.UpDown);

            IRegister I = new RegisterUseRTC(RealTimeClock, "I", 50, new ScaleAndUnit() { Scale = 0, Unit = 2 }, IncrementTipe.UpDown);

            IRegister Ain = new RegisterUseRTC(RealTimeClock, "Ain", 10, new ScaleAndUnit() { Scale = 0, Unit = 3 }, IncrementTipe.Increment);
            IRegister Aout = new RegisterUseRTC(RealTimeClock, "Aout", 10, new ScaleAndUnit() { Scale = 0, Unit = 3}, IncrementTipe.Increment);
            IRegister Rin = new RegisterUseRTC(RealTimeClock, "Rin", 10, new ScaleAndUnit() { Scale = 0, Unit = 4 }, IncrementTipe.Increment);
            IRegister Rout = new RegisterUseRTC(RealTimeClock, "Rout", 10, new ScaleAndUnit() { Scale = 0, Unit = 4 }, IncrementTipe.Increment);




            Registers = new List<IRegister>()
            {
                u,
                I,
                Ain,
                Aout,
                Rin,
                Rout
            };
            List<IRegister> RegistersCurrent = new List<IRegister>()
            {
                u,
                I,
            };
            List<IRegister> RegistersHour = new List<IRegister>()
            {
                Ain,
                Aout,
                Rin,
                Rout
            };
            Profiles = new List<IProfile>()
            {
                new ProfileUseRTC(RealTimeClock,RegistersCurrent,"Current", 900 ),
                new ProfileUseRTC(RealTimeClock,RegistersHour,"Hour", 3600 )
            };


            List<IncreaseRegister> IncreaseRegister  = new List<IncreaseRegister>();
            foreach (var i in Registers)
            {
                IFRegister fRegister = (IFRegister)i;
                IncreaseRegister.Add(new FastRTC.IncreaseRegister(fRegister.IncreaseValue));
            }

            List<WriteProfile> WriteProfile = new List<WriteProfile>();
            foreach (var i in Profiles)
            {
                IFProfile fRegister = (IFProfile)i;
                IncreaseRegister.Add(new FastRTC.IncreaseRegister(fRegister.WriteProfile));
            }


            var IFastRtc  = (IFastRtc)RealTimeClock;

            IFastRtc.Init(IncreaseRegister.ToArray(), WriteProfile.ToArray());



            return Task.FromResult(RealTimeClock?.StartRtc(cancellationToken) ?? false);
        }
    }
}
