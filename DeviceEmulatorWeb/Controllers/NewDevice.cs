using CommonTypeDevice.Property;
using DeviceEmulator.Devcie;
using Microsoft.AspNetCore.Mvc;
using System.Data;
using System.Text.Json;
namespace DeviceEmulatorWeb.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class NewDevice : ControllerBase
    {

        [HttpGet]
        public async Task<bool> CreateNewDevice()
            {
            var device = new EDeviceForeWeb();
            using(var ct = new CancellationTokenSource()) 
            return await device.Init("", ct.Token);
        }
    }
}
