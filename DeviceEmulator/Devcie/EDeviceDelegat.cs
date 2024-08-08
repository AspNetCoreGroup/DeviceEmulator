using CommonTypeDevice;
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

        List<DeviceProperty>? _properties = new();
        string sn = "";
        DeviceData? deviceData;
        public override async Task<bool> Init(string sn, CancellationToken cancellationToken)
        {
            this.sn = sn;

            List<DeviceData> deviceDatas = await DeviceDataStorage.LoadAllDeviceDataAsync();
            deviceData = deviceDatas.Find(x => x?.Properties?.Find(y => y.Name == "SN")?.Value?.Contains(sn) ?? false);

            if (deviceData == null)
            {
                _properties = new List<DeviceProperty>
                {
                    new DeviceProperty("SN", sn),
                    new DeviceProperty("DeviceType", GenerateDeviceType())
                };
                Properties = new PropetryCollection(_properties).Properties;

            }
            else
            {
                Properties = deviceData.Properties;
                _properties = deviceData.Properties;

            }
            return await GenerateProfile(cancellationToken);
        }

        private async Task<bool> GenerateProfile(CancellationToken cancellationToken)
        {


            if (deviceData == null)
                RealTimeClock = new FastRTC(new DateTime(2024, 1, 1), DateTime.Now, 60);
            else
            {

                var dateTime = deviceData.Measurements?.ElementAtOrDefault(0)?.DateTime;
                RealTimeClock = new FastRTC(dateTime ?? new DateTime(2024, 1, 1), DateTime.Now, 60);
            }

            IRegister Ain;
            IRegister Aout;
            IRegister Rin;
            IRegister Rout;
            if (deviceData == null)
            {
                Ain = new RegisterUseRTC(RealTimeClock, 1, 10, new ScaleAndUnit() { Scale = 0, Unit = 3 }, IncrementTipe.Increment);
                Aout = new RegisterUseRTC(RealTimeClock, 2, 10, new ScaleAndUnit() { Scale = 0, Unit = 3 }, IncrementTipe.Increment);
                Rin = new RegisterUseRTC(RealTimeClock, 3, 10, new ScaleAndUnit() { Scale = 0, Unit = 4 }, IncrementTipe.Increment);
                Rout = new RegisterUseRTC(RealTimeClock, 4, 10, new ScaleAndUnit() { Scale = 0, Unit = 4 }, IncrementTipe.Increment);
            }
            else
            {
                Ain = new RegisterUseRTC(RealTimeClock, 1, Convert.ToUInt32(deviceData?.Measurements?.Find(x => x.MeasurumentId == 1)?.Value ?? 10), new ScaleAndUnit() { Scale = 0, Unit = 3 }, IncrementTipe.Increment);
                Aout = new RegisterUseRTC(RealTimeClock, 2, Convert.ToUInt32(deviceData?.Measurements?.Find(x => x.MeasurumentId == 2)?.Value ?? 10), new ScaleAndUnit() { Scale = 0, Unit = 3 }, IncrementTipe.Increment);
                Rin = new RegisterUseRTC(RealTimeClock, 3, Convert.ToUInt32(deviceData?.Measurements?.Find(x => x.MeasurumentId == 3)?.Value ?? 10), new ScaleAndUnit() { Scale = 0, Unit = 4 }, IncrementTipe.Increment);
                Rout = new RegisterUseRTC(RealTimeClock, 4, Convert.ToUInt32(deviceData?.Measurements?.Find(x => x.MeasurumentId == 4)?.Value ?? 10), new ScaleAndUnit() { Scale = 0, Unit = 4 }, IncrementTipe.Increment);
            }

            Registers = new List<IRegister>()
            {
                //u,
                //I,
                Ain,
                Aout,
                Rin,
                Rout
            };
            //List<IRegister> RegistersCurrent = new List<IRegister>()
            //{
            //    u,
            //    I,
            //};
            List<IRegister> RegistersHour = new List<IRegister>()
            {
                Ain,
                Aout,
                Rin,
                Rout
            };
            Profiles = new List<IProfile>()
            {
                //new ProfileUseRTC(RealTimeClock,RegistersCurrent,"Current", 900 ),
                new ProfileUseRTC(RealTimeClock,RegistersHour,"Hour", 3600 )
            };

            IDeviceEvent Event1 = new DeviceEventRundWeb(1, 1, Profiles, Properties);//"Перезагрузка устройства" );
            IDeviceEvent Event2 = new DeviceEventRundWeb(2, 1, Profiles, Properties);//"Вскрытие пломбы клеммной крышки");
            IDeviceEvent Event3 = new DeviceEventRundWeb(3, 1, Profiles, Properties);//"Вскрытие пломбы корпуса" );
            IDeviceEvent Event4 = new DeviceEventRundWeb(4, 1, Profiles, Properties);//"Вскрытие пломбы отсека сменного модуля" );
            IDeviceEvent Event5 = new DeviceEventRundWeb(5, 1, Profiles, Properties);//"Сброс состояний пломб" );
            IDeviceEvent Event6 = new DeviceEventRundWeb(6, 1, Profiles, Properties);//"Попытка несанкционированного доступа" );
            IDeviceEvent Event7 = new DeviceEventRundWeb(7, 1, Profiles, Properties);//"Отключение реле нагрузки по превышению лимита активной мощности" );
            IDeviceEvent Event8 = new DeviceEventRundWeb(8, 1, Profiles, Properties);//"Отключение реле нагрузки по превышению напряжения" );
            IDeviceEvent Event9 = new DeviceEventRundWeb(9, 1, Profiles, Properties);//"Изменение заводского номера счетчика" );
            IDeviceEvent Event10 = new DeviceEventRundWeb(10, 1, Profiles, Properties);// "Изменение связного адреса счетчика" );
            IDeviceEvent Event11 = new DeviceEventRundWeb(11, 1, Profiles, Properties);// "Время изменено" );
            IDeviceEvent Event12 = new DeviceEventRundWeb(12, 1, Profiles, Properties);// "Пропадание фазного напряжения фазы A" );
            IDeviceEvent Event13 = new DeviceEventRundWeb(13, 1, Profiles, Properties);// "Пропадание фазного напряжения фазы B" );
            IDeviceEvent Event14 = new DeviceEventRundWeb(14, 1, Profiles, Properties);// "Пропадание фазного напряжения фазы C" );
            IDeviceEvent Event15 = new DeviceEventRundWeb(15, 1, Profiles, Properties);// "Фаза А - превышение максимального тока" );
            IDeviceEvent Event16 = new DeviceEventRundWeb(16, 1, Profiles, Properties);// "Фаза В - превышение максимального тока" );
            IDeviceEvent Event17 = new DeviceEventRundWeb(17, 1, Profiles, Properties);// "Фаза С - превышение максимального тока" );
            IDeviceEvent Event18 = new DeviceEventRundWeb(18, 1, Profiles, Properties);// "Превышение напряжения - порог №1" );
            IDeviceEvent Event19 = new DeviceEventRundWeb(19, 1, Profiles, Properties);// "Превышение напряжения - порог №2" );
            IDeviceEvent Event20 = new DeviceEventRundWeb(20, 1, Profiles, Properties);// "Превышение максимального тока прибора" );
            IDeviceEvent Event21 = new DeviceEventRundWeb(21, 1, Profiles, Properties);// "Превышение установленного порога" );
            IDeviceEvent Event22 = new DeviceEventRundWeb(22, 1, Profiles, Properties);// "Батарея заряжена" );
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
            foreach (IRegister i in Registers)
            {
                IFRegister fRegister = (IFRegister)i;
                IncreaseRegister.Add(new FastRTC.IncreaseRegister(fRegister.IncreaseValue));
            }

            List<WriteProfile> WriteProfile = new List<WriteProfile>();
            foreach (IProfile i in Profiles)
            {
                IFProfile fRegister = (IFProfile)i;
                WriteProfile.Add(new FastRTC.WriteProfile(fRegister.WriteProfile));
            }

            List<DoEvent> doEvents = new List<DoEvent>();

            foreach (IDeviceEvent i in DeviceEvents)
            {
                IFEvent fEvent = (IFEvent)i;
                doEvents.Add(new FastRTC.DoEvent(fEvent.DoEvent));
            }



            IFastRtc IFastRtc = (IFastRtc)RealTimeClock;

            IFastRtc.Init(IncreaseRegister.ToArray(), WriteProfile.ToArray(), doEvents.ToArray());

            DeviceData data = new()
            {
                DeviceEvents = new() { new() { DateTime = this.RealTimeClock.GetRealTimeClock(), EventParameters = new() { new() { Key = 22, Name = "СТАРТ" } } } },
                //Measurements = await GetAllValuesFromProfiles(profiles),
                Properties = _properties,

            };
            DeviceDataStorage.SaveDeviceDataAsync(data);

            return Task.FromResult(RealTimeClock?.StartRtc(cancellationToken) ?? false).Result;
        }
    }
}
