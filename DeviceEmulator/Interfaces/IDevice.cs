using CommonTypeDevice;
using CommonTypeDevice.Event;
using CommonTypeDevice.Measurument;
using CommonTypeDevice.Property;

namespace DeviceEmulator.Interfaces
{
    public interface IDevice: IPropetryCollection
    {
        Task<bool> Init(string sn, CancellationToken cancellationToken);
        IRealTimeClock? RealTimeClock { get; }
        IEnumerable<IRegister> Registers { get; }
        IEnumerable<IProfile> Profiles { get; }

        IEnumerable<IDeviceEvent> DeviceEvents { get; }

    }

    public interface IRealTimeClock : IRtc
    {
        DateTime GetRealTimeClock();
        DateTime StartTimeClock { get; }
        DateTime EndTimeClock { get; }
    }
    public interface IRtc: IDeviceRtc
    {

        /// <summary>
        /// шаг времени в миллисекундах
        /// </summary>
        int Step { get; }
        bool StartRtc(CancellationToken cancellationToken);
        bool StopRtc();
        //object Locker { get; }
    }

    public interface IDeviceRtc
    {
        /// <summary>
        /// счётчик эмулирующий тики времени 
        /// </summary>
        public abstract long I { get; }
    }

    public interface IValue
    {
        string GetValue();
        Measurement GetMeasurement();
    }

    public interface IFRegister : IRegister
    {
        void IncreaseValue();
    }

    public interface IRegister : IValue
    {
        int MeasurumentId { get; set; }
        double Value { get; set; }
        IScaleAndUnit ScaleAndUnit { get; }
        Task StartWatch(CancellationToken token);
    }

    public interface IScaleAndUnit
    {
        
        sbyte Scale { get; set; }
        byte Unit { get; set; }
    }

    public interface IFProfile: IProfile
    {
        void WriteProfile();
    }

    public interface IFEvent : IDeviceEvent
    {
        Task DoEvent();
    }

    public interface IProfile
    {
        string Name { get; }

        uint Period { get; } //sek

        Task<IEnumerable<IValue>?> GetValues();
        Task<IEnumerable<IValue>?> GetLast();

        Task<IEnumerable<IValue>?> GetValues(DateTime from, DateTime to);
        Task StartMonitoring(CancellationToken token);
    }
}
