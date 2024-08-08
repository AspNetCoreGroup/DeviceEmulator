using DeviceEmulator.BaseDevice;
using DeviceEmulator.Devcie;
using DeviceEmulatorWeb.Data;
using Microsoft.AspNetCore.Mvc;
using System.Diagnostics;
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
            while (Storege.ListSn.Find(x => x.Contains(sn)) != null)
            {
                sn = DeviceBase.GenerateSerialNumber();
            }
            Storege.ListSn.Add(sn);
            using (CancellationTokenSource ct = new CancellationTokenSource())
                return await device.Init(sn, ct.Token);
        }
    }

    [Route("api/[controller]")]
    [ApiController]
    public class New1000Device : ControllerBase
    {
        DeviceStorege Storege { get; }

        public New1000Device(DeviceStorege storege)
        {
            Storege = storege;
        }

        [HttpGet]
        public async Task<bool> CreateNew1000Device()
        {
            using (CancellationTokenSource ct = new CancellationTokenSource())
                for (int i = 0; i < 1000; i++)
                {
                    EDeviceForeWeb device = new EDeviceForeWeb();
                    Storege.ForeWebs.Add(device);
                    string sn = DeviceBase.GenerateSerialNumber();
                    while (Storege.ListSn.Find(x => x.Contains(sn)) != null)
                    {
                        sn = DeviceBase.GenerateSerialNumber();
                    }
                    Storege.ListSn.Add(sn);
                    Debug.WriteLine("Start " + i + " " + sn);
                    device.Init(sn, ct.Token);
                }
            return true;
        }
    }
}

