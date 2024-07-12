using CommonTypeDevice.Property;
using DeviceEmulator.Devcie;
using DeviceEmulatorWeb.Data;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

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
            return true;
        }
    }
}
