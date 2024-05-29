using DeviceEmulator.Interfaces;

namespace DeviceEmulator.BaseDevice
{
    public class DeviceBase : IDevice
    {
        
        object Locker = new();
        public IRealTimeClock? RealTimeClock { get; private set; }

        public IPropetryCollection? PuppetryCollection { get; private set; }

        public IEnumerable<IRegister>? Registers { get; private set; }

        public IEnumerable<IProfile>? Profiles { get; private set; }

        public Task<bool> Init(string initStr, CancellationToken cancellationToken)
        {
            RealTimeClock = new RealTimeClockBase(Locker, new DateTime(2022, 1, 1));

            PuppetryCollection = new PropetryCollection(new List<IProperty>
            {
                new Property("SerialNumber", GenerateSerialNumber()),
                new Property("DeviceType", GenerateDeviceType())
            });

            var u = new Register(RealTimeClock, "U", 230, new ScaleAndUnit() { Scale = 0, Unit = 1 }, IncrementTipe.UpDown);
            var Ain = new Register(RealTimeClock, "Ain", 10, new ScaleAndUnit() { Scale = 0, Unit = 2 }, IncrementTipe.Increment);

            Registers = new List<IRegister>()
            {
                u,
                Ain
            };
            var Registers3 = new List<IRegister>()
            {
                u,
                Ain
            };


            Profiles = new List<IProfile>()
            {
                new Profile(RealTimeClock,Registers3,"I", 5 )
            };

            foreach (var i in Registers)
            {
                i.StartWatch(cancellationToken);
            }

            foreach (var i in Profiles)
            {
                i.StartMonitoring(cancellationToken);
            }




            return Task.FromResult(RealTimeClock?.StartRtc(cancellationToken)??false);
        }
        private static string GenerateSerialNumber()
        {
            Random random = new Random();
            return new string(Enumerable.Repeat("0123456789", 10).Select(s => s[random.Next(s.Length)]).ToArray());
        }

        private static string GenerateDeviceType()
        {
            string[] deviceTypes = { "TypeA1", "TypeA2", "TypeA3" };
            Random random = new Random();
            return deviceTypes[random.Next(deviceTypes.Length)];
        }
    }
}
