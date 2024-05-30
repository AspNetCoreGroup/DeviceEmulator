using DeviceEmulator.Interfaces;

namespace DeviceEmulator.Data
{
    public class DataRegisterValue : IValue
    {
        private readonly DateTime _timestamp;
        private readonly uint _value;

        public DataRegisterValue(DateTime timestamp, uint value)
        {
            _timestamp = timestamp;
            _value = value;
        }

        public string GetValue()
        {
            return $"{_timestamp:O} - {_value}";
        }
    }
}
