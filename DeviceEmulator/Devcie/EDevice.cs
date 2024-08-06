using CommonTypeDevice.Event;
using CommonTypeDevice.Property;
using DeviceEmulator.BaseDevice;
using DeviceEmulator.Interfaces;
using DeviceEmulator.UseRTC;

namespace DeviceEmulator.Device
{
    public class EDevice : DeviceBase
    {
        //public override IPropetryCollection? PuppetryCollection { get; protected set; }
        public override IEnumerable<IRegister> Registers { get; protected set; } = new List<IRegister>();
        public override IEnumerable<IProfile> Profiles { get; protected set; } = new List<IProfile>();
        public override IEnumerable<IProperty>? Properties { get; protected set; } = new List<IProperty>();
        public override IEnumerable<IDeviceEvent> DeviceEvents { get; protected set ; } = new List<IDeviceEvent>();

        public override Task<bool> Init(string initStr, CancellationToken cancellationToken)
        {

            RealTimeClock = new RealTimeClockBase(new DateTime(2022, 1, 1), DateTime.Now, 60);

            Properties = new PropetryCollection(new List<IProperty>
            {
                new DeviceProperty("SerialNumber", GenerateSerialNumber()),
                new DeviceProperty("DeviceType", GenerateDeviceType())
            }).Properties;

            IRegister u = new RegisterUseRTC(RealTimeClock, "U", 230, new ScaleAndUnit() { Scale = 0, Unit = 1 }, IncrementTipe.UpDown);
            IRegister Ain = new RegisterUseRTC(RealTimeClock, "Ain", 10, new ScaleAndUnit() { Scale = 0, Unit = 2 }, IncrementTipe.Increment);
            IRegister Qin = new RegisterUseRTC(RealTimeClock, "Qin", 10, new ScaleAndUnit() { Scale = 0, Unit = 3 }, IncrementTipe.Increment);

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
                new ProfileUseRTC(RealTimeClock,Registers3,"I", 900 )
            };


            IDeviceEvent deviceEvent = new DeviceEvent()
            {
                EventParameters = new() { new() { Key = 1 } }
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
