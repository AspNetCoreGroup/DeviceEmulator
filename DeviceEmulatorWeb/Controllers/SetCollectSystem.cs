using DeviceEmulator.Device;
using DeviceEmulatorWeb.Data;
using Microsoft.AspNetCore.Mvc;
using System.Net;

namespace DeviceEmulatorWeb.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class SetCollectSystem : ControllerBase
    {
        SystemSetings SystemSetings { get; }

        public SetCollectSystem(SystemSetings systemSetings)
        {
            SystemSetings = systemSetings;
        }

        [HttpGet("{ipPort}")]
        public async Task<bool> CreateNewDevice(string ipPort)
        {
            SystemSetings.IpPortCollectSystem = ipPort;



            IPAddress? iPAddress;
            int iPort;
            IPAddress.TryParse(ipPort, out iPAddress);
            var portstr = ipPort.Split(':').ToList().Last();
            int.TryParse(portstr, out iPort);

            ServerDataStorageConfig.port = iPort.ToString();
            ServerDataStorageConfig.ipAddres = iPAddress.ToString();
            return true;
        }
    }
}
