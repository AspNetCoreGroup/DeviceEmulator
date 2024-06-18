namespace DeviceEmulator.Interfaces
{
    public interface IDevice
    {
        Task<bool> Init(string initStr, CancellationToken cancellationToken);
        IRealTimeClock? RealTimeClock { get; }
        IPropetryCollection? PuppetryCollection { get; }

        IEnumerable<IRegister> Registers { get; }
        IEnumerable<IProfile> Profiles { get; }

    }

    public interface IPropetryCollection
    {
        IEnumerable<IProperty> Properties { get; }
    }

    public interface IProperty
    {
        string Name { get; }
        string Value { get; }
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
    }

    public interface IFRegister : IRegister
    {
        void IncreaseValue();
    }

    public interface IRegister : IValue
    {
        string Name { get; set; }
        uint Value { get; set; }
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

    public interface IProfile
    {
        string Name { get; }

        uint Period { get; } //sek

        Task<IEnumerable<IValue>?> GetValues();

        Task<IEnumerable<IValue>?> GetValues(DateTime from, DateTime to);
        Task StartMonitoring(CancellationToken token);
    }
}
