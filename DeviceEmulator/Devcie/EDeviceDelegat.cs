using CommonTypeDevice.Event;
using CommonTypeDevice.Property;
using DeviceEmulator.BaseDevice;
using DeviceEmulator.FastStorage;
using DeviceEmulator.Interfaces;
using DeviceEmulator.UseRTC;
using static DeviceEmulator.FastStorage.FastRTC;

namespace DeviceEmulator.Device
{



    public class EDeviceDelegat : DeviceBase
    {
        public override IEnumerable<IProperty>? Properties { get; protected set; } = new List<IProperty>();
        public override IEnumerable<IRegister> Registers { get; protected set; } = new List<IRegister>();
        public override IEnumerable<IProfile> Profiles { get; protected set; } = new List<IProfile>();
        public override IEnumerable<IDeviceEvent> DeviceEvents { get; protected set; } = new List<IDeviceEvent>();

        public override Task<bool> Init(string initStr, CancellationToken cancellationToken)
        {


            Properties = new PropetryCollection(new List<IProperty>
            {
                new DeviceProperty("SerialNumber", GenerateSerialNumber()),
                new DeviceProperty("DeviceType", GenerateDeviceType())
            }).Properties;
            return GenerateProfile(cancellationToken);
        }

        private Task<bool> GenerateProfile(CancellationToken cancellationToken)
        {
            RealTimeClock = new FastRTC(new DateTime(2022, 1, 1), DateTime.Now, 60);

            IRegister u = new RegisterUseRTC(RealTimeClock, "U", 230, new ScaleAndUnit() { Scale = 0, Unit = 1 }, IncrementTipe.UpDown);

            IRegister I = new RegisterUseRTC(RealTimeClock, "I", 50, new ScaleAndUnit() { Scale = 0, Unit = 2 }, IncrementTipe.UpDown);

            IRegister Ain = new RegisterUseRTC(RealTimeClock, "Ain", 10, new ScaleAndUnit() { Scale = 0, Unit = 3 }, IncrementTipe.Increment);
            IRegister Aout = new RegisterUseRTC(RealTimeClock, "Aout", 10, new ScaleAndUnit() { Scale = 0, Unit = 3 }, IncrementTipe.Increment);
            IRegister Rin = new RegisterUseRTC(RealTimeClock, "Rin", 10, new ScaleAndUnit() { Scale = 0, Unit = 4 }, IncrementTipe.Increment);
            IRegister Rout = new RegisterUseRTC(RealTimeClock, "Rout", 10, new ScaleAndUnit() { Scale = 0, Unit = 4 }, IncrementTipe.Increment);






            Registers = new List<IRegister>()
            {
                u,
                I,
                Ain,
                Aout,
                Rin,
                Rout
            };
            List<IRegister> RegistersCurrent = new List<IRegister>()
            {
                u,
                I,
            };
            List<IRegister> RegistersHour = new List<IRegister>()
            {
                Ain,
                Aout,
                Rin,
                Rout
            };
            Profiles = new List<IProfile>()
            {
                new ProfileUseRTC(RealTimeClock,RegistersCurrent,"Current", 900 ),
                new ProfileUseRTC(RealTimeClock,RegistersHour,"Hour", 3600 )
            };

            IDeviceEvent Event1 = new DeviceEventRundWeb(1, 1, Profiles,Properties);//"Перезагрузка устройства" );
            IDeviceEvent Event2 = new DeviceEventRundWeb(2, 1, Profiles,Properties);//"Вскрытие пломбы клеммной крышки");
            IDeviceEvent Event3 = new DeviceEventRundWeb(3, 1, Profiles,Properties);//"Вскрытие пломбы корпуса" );
            IDeviceEvent Event4 = new DeviceEventRundWeb(4, 1, Profiles,Properties);//"Вскрытие пломбы отсека сменного модуля" );
            IDeviceEvent Event5 = new DeviceEventRundWeb(5, 1, Profiles,Properties);//"Сброс состояний пломб" );
            IDeviceEvent Event6 = new DeviceEventRundWeb(6, 1, Profiles,Properties);//"Попытка несанкционированного доступа" );
            IDeviceEvent Event7 = new DeviceEventRundWeb(7, 1, Profiles,Properties);//"Отключение реле нагрузки по превышению лимита активной мощности" );
            IDeviceEvent Event8 = new DeviceEventRundWeb(8, 1, Profiles,Properties);//"Отключение реле нагрузки по превышению напряжения" );
            IDeviceEvent Event9 = new DeviceEventRundWeb(9, 1, Profiles, Properties);//"Изменение заводского номера счетчика" );
            IDeviceEvent Event10 = new DeviceEventRundWeb(10, 1, Profiles,Properties);// "Изменение связного адреса счетчика" );
            IDeviceEvent Event11 = new DeviceEventRundWeb(11, 1, Profiles,Properties);// "Время изменено" );
            IDeviceEvent Event12 = new DeviceEventRundWeb(12, 1, Profiles,Properties);// "Пропадание фазного напряжения фазы A" );
            IDeviceEvent Event13 = new DeviceEventRundWeb(13, 1, Profiles,Properties);// "Пропадание фазного напряжения фазы B" );
            IDeviceEvent Event14 = new DeviceEventRundWeb(14, 1, Profiles,Properties);// "Пропадание фазного напряжения фазы C" );
            IDeviceEvent Event15 = new DeviceEventRundWeb(15, 1, Profiles,Properties);// "Фаза А - превышение максимального тока" );
            IDeviceEvent Event16 = new DeviceEventRundWeb(16, 1, Profiles,Properties);// "Фаза В - превышение максимального тока" );
            IDeviceEvent Event17 = new DeviceEventRundWeb(17, 1, Profiles,Properties);// "Фаза С - превышение максимального тока" );
            IDeviceEvent Event18 = new DeviceEventRundWeb(18, 1, Profiles,Properties);// "Превышение напряжения - порог №1" );
            IDeviceEvent Event19 = new DeviceEventRundWeb(19, 1, Profiles,Properties);// "Превышение напряжения - порог №2" );
            IDeviceEvent Event20 = new DeviceEventRundWeb(20, 1, Profiles,Properties);// "Превышение максимального тока прибора" );
            IDeviceEvent Event21 = new DeviceEventRundWeb(21, 1, Profiles,Properties);// "Превышение установленного порога" );
            IDeviceEvent Event22 = new DeviceEventRundWeb(22, 1, Profiles,Properties);// "Батарея заряжена" );
            IDeviceEvent Event23 = new DeviceEventRundWeb(23, 1, Profiles, Properties);// "Срабатывание сигнализации");


            DeviceEvents = new List<IDeviceEvent>()
            {
                Event1,
                Event2,
                Event3,
                Event4,
                Event5,
                Event6,
                Event7,
                Event8,
                Event9,
                Event10,
                Event11,
                Event12,
                Event13,
                Event14,
                Event15,
                Event16,
                Event17,
                Event18,
                Event19,
                Event20,
                Event21,
                Event22,
                Event23
            };

            List<IncreaseRegister> IncreaseRegister = new List<IncreaseRegister>();
            foreach (var i in Registers)
            {
                IFRegister fRegister = (IFRegister)i;
                IncreaseRegister.Add(new FastRTC.IncreaseRegister(fRegister.IncreaseValue));
            }

            List<WriteProfile> WriteProfile = new List<WriteProfile>();
            foreach (var i in Profiles)
            {
                IFProfile fRegister = (IFProfile)i;
                WriteProfile.Add(new FastRTC.WriteProfile(fRegister.WriteProfile));
            }

            List<DoEvent> doEvents = new List<DoEvent>();
            foreach (var i in DeviceEvents)
            {
                IFEvent fEvent = (IFEvent)i;
                doEvents.Add(new FastRTC.DoEvent(fEvent.DoEvent));
            }



            var IFastRtc = (IFastRtc)RealTimeClock;

            IFastRtc.Init(IncreaseRegister.ToArray(), WriteProfile.ToArray(), doEvents.ToArray());



            return Task.FromResult(RealTimeClock?.StartRtc(cancellationToken) ?? false);
        }
    }
}
