using DeviceEmulator.Device;
using DeviceEmulator.Interfaces;

namespace DeviceEmulator.BaseDevice
{
    public abstract class DeviceBase : IDevice, IGetProfile, IGetProperty
    {

        protected object Locker = new();
        public IRealTimeClock? RealTimeClock { get; protected set; }

        public abstract IPropetryCollection? PuppetryCollection { get; protected set; }

        public abstract IEnumerable<IRegister> Registers { get; protected set; }

        public abstract IEnumerable<IProfile> Profiles { get; protected set; }

        public abstract Task<bool> Init(string initStr, CancellationToken cancellationToken);

        protected string GenerateSerialNumber()
        {
            Random random = new Random();
            return new string(Enumerable.Repeat("0123456789", 10).Select(s => s[random.Next(s.Length)]).ToArray());
        }

        protected virtual string GenerateDeviceType()
        {
            string[] deviceTypes = { "TypeA1", "TypeA2", "TypeA3" };
            Random random = new Random();
            return deviceTypes[random.Next(deviceTypes.Length)];
        }

        public async Task<IEquatable<IValue>?> GetProfile(string name)
        {
            IProfile? profile = Profiles.FirstOrDefault(x => x.Name == name);
            IEnumerable<IValue>? values = profile != null
                ? await profile.GetValues()
                : Enumerable.Empty<IValue>();
            return (IEquatable<IValue>?)values;
        }

        public async Task<IEquatable<IValue>?> GetProfile(string name, DateTime begin, DateTime end)
        {
            IProfile? profile = Profiles.FirstOrDefault(x => x.Name == name);
            IEnumerable<IValue>? values = profile != null
                ? await profile.GetValues(begin, end)
                : Enumerable.Empty<IValue>();
            return (IEquatable<IValue>?)values;
        }

        public Task<List<IProperty>> GetProperty()
        {
            throw new NotImplementedException();
        }

        public Task<List<IProperty>> GetProperty(string name)
        {
            throw new NotImplementedException();
        }
    }
}
