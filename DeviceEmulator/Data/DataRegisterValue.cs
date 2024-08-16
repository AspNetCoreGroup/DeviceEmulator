using CommonTypeDevice.MeasurumentData;
using DeviceEmulator.Interfaces;
using System.Collections.Generic;

namespace DeviceEmulator.Data
{
    public class DataRegisterValue : IValue
    {
        private readonly DateTime _timestamp;
        private readonly double _value;

        private readonly int _MeasurumentId;
        private readonly uint _unit;

        public DataRegisterValue(DateTime timestamp, double value, int name, uint unit)
        {
            _timestamp = timestamp;
            _value = value;
            _MeasurumentId = name;
            _unit = unit;
        }

        public MeasurementData GetMeasurement()
        {
            return new() { DateTime = _timestamp, Value = _value, MeasurumentId = _MeasurumentId, Unit = _unit };
        }

        public string GetValue()
        {
            return $"{_timestamp:O} - {_value}";
        }
    }
}
