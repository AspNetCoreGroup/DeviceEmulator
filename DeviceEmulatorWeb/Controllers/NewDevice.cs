using DeviceEmulator.Devcie;
using DeviceEmulatorWeb.Data;
using Microsoft.AspNetCore.Mvc;
namespace DeviceEmulatorWeb.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class NewDevice : ControllerBase
    {
        DeviceStorege Storege { get; }

        public NewDevice(DeviceStorege storege)
        {
            Storege = storege;
        }

        [HttpGet]
        public async Task<bool> CreateNewDevice()
        {
            EDeviceForeWeb device = new EDeviceForeWeb();
            Storege.ForeWebs.Add(device);
            using (CancellationTokenSource ct = new CancellationTokenSource())
                return await device.Init("", ct.Token);
        }
    }
}
