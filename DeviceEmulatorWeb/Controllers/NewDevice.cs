
using CommonTypeDevice.Property;
using DeviceEmulator.Devcie;
using Microsoft.AspNetCore.Mvc;
using System.Text.Json;

// For more information on enabling Web API for empty projects, visit https://go.microsoft.com/fwlink/?LinkID=397860

namespace DeviceEmulatorWeb.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class NewDevice : ControllerBase
    {
        [HttpGet]
        public IEnumerable<string> Get()
        {
            return new string[] { "value1", "value2" };
        }

        [HttpGet("{IPropetryCollection}")]
        public async Task<bool> CreateNewDevice(PropetryCollection sn)
        {
            var device = new EDeviceForeWeb();
            using(var ct = new CancellationTokenSource()) 
            return await device.Init(JsonSerializer.Serialize(sn), ct.Token);
        }
    }
}
