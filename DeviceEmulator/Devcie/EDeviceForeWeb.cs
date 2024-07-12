using CommonTypeDevice.Property;
using DeviceEmulator.Device;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json;
using System.Text.Json.Nodes;
using System.Text.Json.Serialization;
using System.Threading.Tasks;

namespace DeviceEmulator.Devcie
{
    public class EDeviceForeWeb: EDeviceDelegat
    {
        public override async Task<bool> Init(string initStr, CancellationToken cancellationToken)
        {
            IPropetryCollection? propetryCollection =  JsonSerializer.Deserialize<IPropetryCollection>(initStr);
            Properties = propetryCollection?.Properties;
            return await base.Init(initStr, cancellationToken);
        }
    }
}
