using DeviceEmulator.Interfaces;

namespace DeviceEmulator.Device
{
    public interface IGetProperty
    {
        Task<List<IProperty>> GetProperty();
        Task<List<IProperty>> GetProperty(string name);
    }
}