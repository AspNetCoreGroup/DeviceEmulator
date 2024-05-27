using DeviceEmulator.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DeviceEmulator.BaseDevice
{
    public class PropetryCollection : IPropetryCollection
    {
        public PropetryCollection(IEnumerable<IProperty> properties)
        {
            Properties = properties;
        }

        public IEnumerable<IProperty> Properties { get; }
    }

    public class Property : IProperty
    {
        public Property(string name, string value)
        {
            Name = name;
            _value = value;
        }

        public string Name { get; }
        private string _value;

        public string GetProperty()
        {
            return _value;
        }
    }
}
