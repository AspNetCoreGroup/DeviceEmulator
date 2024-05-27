namespace DeviceEmulator.Interfaces
{
    public interface IDevice
    {
        Task<bool> Init(string initStr, CancellationToken cancellationToken);
        IRealTimeClock? RealTimeClock { get; }
        IPropetryCollection? PuppetryCollection { get; }

        IEnumerable<IRegister> Registers { get; }
    }

    public interface IPropetryCollection
    {
        IEnumerable<IProperty> Properties { get; }
    }

    public interface IProperty
    {
        string Name { get; }
        string GetProperty();
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
        object Locker { get; }
    }

    public interface IDeviceRtc
    {
        /// <summary>
        /// счётчик эмулирующий тики времени 
        /// </summary>
        public abstract long I { get; }
    }

    public interface IRegister
    {
        
        string SogialName { get; set; }
        uint Value { get; set; }
        IScaleAndUnit ScaleAndUnit { get; }
        Task StartWatch(CancellationToken token);
    }
    public interface IScaleAndUnit
    {
        
        sbyte Scale { get; set; }
        byte Unit { get; set; }
    }

}
