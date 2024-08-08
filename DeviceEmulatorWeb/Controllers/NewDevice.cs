using DeviceEmulator.BaseDevice;
using DeviceEmulator.Devcie;
using DeviceEmulatorWeb.Data;
using Microsoft.AspNetCore.Mvc;
using System.Collections.Generic;
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
            string sn = DeviceBase.GenerateSerialNumber();
            while(Storege.ListSn.Find(x=>x.Contains(sn) )!=null)
            {
                sn = DeviceBase.GenerateSerialNumber();
            }
            Storege.ListSn.Add(sn);
            using (CancellationTokenSource ct = new CancellationTokenSource())
                return await device.Init(sn, ct.Token);
        }
    }
}
