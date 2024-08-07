using DeviceEmulator.Devcie;

namespace DeviceEmulatorWeb.Data
{
    public  class SystemSetings
    {
        public  string IpPortCollectSystem { set; get; } = "localhost:5456";
    }
    public class DeviceStorege
    {
        public List<EDeviceForeWeb> ForeWebs { set; get; } = new();
    }
}
