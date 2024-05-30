using DeviceEmulator.Interfaces;

namespace DeviceEmulator.Device
{
    public interface IGetProfile
    {
        Task<IEquatable<IValue>?> GetProfile(string name);
        Task<IEquatable<IValue>?> GetProfile(string name, DateTime begin, DateTime end);
    }
}