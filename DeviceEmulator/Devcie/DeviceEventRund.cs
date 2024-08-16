using CommonTypeDevice;
using CommonTypeDevice.Event;
using CommonTypeDevice.MeasurumentData;
using CommonTypeDevice.Property;
using DeviceEmulator.Interfaces;
using System.Diagnostics;
using System.Net.Http.Json;
using System.Text;
using System.Text.Json;

namespace DeviceEmulator.Device
{

    public class DeviceEventRundWeb : DeviceEventRund
    {
        private IEnumerable<IProfile> profiles;
        private IEnumerable<IProperty>? properties;

        public DeviceEventRundWeb(int key, int chance, IEnumerable<IProfile> profiles, IEnumerable<IProperty>? properties) : base(key, chance)
        {
            this.profiles = profiles;
            this.properties = properties;
        }

        public override async Task DoEvent()
        {
            if (await Get() != null)
            {
                DeviceData data = new()
                {
                    DeviceEvents = new() { new() { DateTime = this.DateTime, EventParameters = this.EventParameters } },
                    Measurements = await GetAllValuesFromProfiles(profiles),
                    Properties = await GetAllProppertys(properties),

                };

                DeviceData s = new()
                {
                    DeviceEvents = new() { new() { DateTime = this.DateTime, EventParameters = this.EventParameters } },
                    Measurements = await GetLast(profiles),
                    Properties = await GetAllProppertys(properties),

                };
                //List<DeviceData> deviceDatas = await DeviceDataStorage.LoadAllDeviceDataAsync();
                //var deviceData = deviceDatas.Find(x => x?.Properties?.Find(y => y.Name == "SN")?.Value?.Contains(properties?.ToList()?.Find(t=>t.Name =="SN")?.Value??"") ?? false);

                Send(data);
                await DeviceDataStorage.SaveDeviceDataAsync(s);
                Debug.WriteLine("send");

            }
            else
            {
                //Debug.Write("no event ");
            }
        }

        private async Task<List<DeviceProperty>> GetAllProppertys(IEnumerable<IProperty> properties)
        {
            List<DeviceProperty> allProperty = new List<DeviceProperty>();
            foreach (IProperty properti in properties)
            {
                allProperty.Add(new(properti.Name, properti.Value));
            }
            return allProperty;
        }

        public async Task<List<MeasurementData>> GetAllValuesFromProfiles(IEnumerable<IProfile> profiles)
        {
            List<MeasurementData> allValues = new List<MeasurementData>();

            foreach (IProfile profile in profiles)
            {
                IEnumerable<IValue>? values = await profile.GetValues();
                if (values != null)
                {
                    foreach (IValue value in values)
                        allValues.Add(value.GetMeasurement());
                }
            }

            //Debug.WriteLine("Measurements Count");
            //Debug.WriteLine(allValues.Count());
            return allValues;
        }

        public async Task<List<MeasurementData>> GetLast(IEnumerable<IProfile> profiles)
        {
            List<MeasurementData> allValues = new List<MeasurementData>();

            foreach (IProfile profile in profiles)
            {
                IEnumerable<IValue>? values = await profile.GetLast();
                if (values != null)
                {
                    foreach (IValue value in values)
                        allValues.Add(value.GetMeasurement());
                }
            }

            //Debug.WriteLine("Measurements Count");
            //Debug.WriteLine(allValues.Count());
            return allValues;
        }


        public async Task Send(DeviceData? deviceData)
        {
            if (deviceData != null)
            {
                string _choosenDirectory = AppDomain.CurrentDomain.BaseDirectory;
                try
                {
                    using (HttpClient client2 = new HttpClient())
                    {
                        client2.BaseAddress = new Uri($"http://{ServerDataStorageConfig.ipAddres}:{ServerDataStorageConfig.port}");

                        string strD = JsonSerializer.Serialize(deviceData);
                        JsonContent content = JsonContent.Create(deviceData);
                        StringContent contentstr = new StringContent(strD, Encoding.UTF8, "application/json");

                        string? str = content.ToString();
                        HttpResponseMessage result = await client2.PostAsync("/DataFromDevice", contentstr);
                        if (result.IsSuccessStatusCode)
                        {
                            Debug.WriteLine("УСПЕХ ОТПРАВКИ");
                        }
                        else
                        {
                            Debug.WriteLine("Ошибка");
                        }
                    }

                }
                catch (Exception ex)
                {
                    Debug.WriteLine("Ошибка при подключении или обмене данными с сервером: " + ex.Message);
                   // await Task.Delay(2000); // Ожидание 2 секунд перед отправкой следующего запроса
                }
            }
        }
    }


    public class DeviceEventRund : IFEvent
    {
        private int Key;
        private int Chance;

        public DeviceEventRund(int key, int chance)
        {
            this.Key = key;
            this.Chance = chance;

        }

        public List<EventItem> EventParameters { get; set; }
        public DateTime DateTime { get; set; }

        public virtual Task DoEvent()
        {
            throw new NotImplementedException();
        }



        /// <summary>
        /// With a Chance percentage, returns the result and fills EventParameters with a parameter with a timeout of 15 seconds
        /// </summary>
        /// <returns></returns>
        public async Task<IDeviceEvent?> Get()
        {
            Dictionary<int, string> dictionary = EventDictionary.dictionary;
            // await Task.Delay(15000); // Simulate 15 seconds timeout

            Random random = new Random();
            if (random.Next(100) < Chance) // If the random number is within the chance range
            {
                // Simulate filling EventParameters
                string? value = "";
                dictionary.TryGetValue(Key, out value);
                EventParameters = new List<EventItem>
                {
                    new EventItem { Key = Key, Name =  value??""} // Example event item
                };

                DateTime = DateTime.Now;

                return this;
            }

            return null; // If the event doesn't occur
        }
    }

    public static class ServerDataStorageConfig
    {
        public static string ipAddres { set; get; } = "127.0.0.1";
        public static string port { set; get; } = "5247";
    }


    public class DeviceDataStorage
    {
        private const string StorageFolder = "DeviceDataStorage";
        private static readonly object _lock = new object();

        public static async Task SaveDeviceDataAsync(DeviceData deviceData)
        {
            if (deviceData == null || deviceData.Properties == null)
            {
                throw new ArgumentException("Invalid device data");
            }

            // Найти SN
            DeviceProperty? snProperty = deviceData.Properties.FirstOrDefault(p => p.Name == "SN");
            if (snProperty == null)
            {
                throw new ArgumentException("Device SN not found");
            }

            // Последний Measurement
            MeasurementData? latestMeasurement = deviceData.Measurements?.LastOrDefault();
            if (latestMeasurement != null)
            {
                deviceData.Measurements = new List<MeasurementData> { latestMeasurement };
            }

            lock (_lock)
            {
                // Создать папку если не существует
                if (!Directory.Exists($"{AppDomain.CurrentDomain.BaseDirectory}{StorageFolder}"))
                {
                    Directory.CreateDirectory($"{AppDomain.CurrentDomain.BaseDirectory}{StorageFolder}");
                }

                List<DeviceData> deviceDatas = DeviceDataStorage.LoadAllDeviceDataAsync().Result;
                List<DeviceData> deviceDataf = deviceDatas.FindAll(x => !(x?.Properties?.Find(y => y.Name == "SN")?.Value?.Contains(deviceData.Properties?.ToList()?.Find(t => t.Name == "SN")?.Value ?? "") ?? false));
                deviceDataf.Add(deviceData);

                // Сохранить данные в JSON файл
                string filePath = Path.Combine($"{AppDomain.CurrentDomain.BaseDirectory}{StorageFolder}", $"SDevice.json");
                string json = JsonSerializer.Serialize(deviceDataf, new JsonSerializerOptions { WriteIndented = true });
                File.WriteAllText(filePath, json);
            }

        }

        public static  Task<List<DeviceData>> LoadAllDeviceDataAsync()
        {

            lock (_lock)
            {
                List<DeviceData> deviceDataList = new List<DeviceData>();

                if (!Directory.Exists($"{AppDomain.CurrentDomain.BaseDirectory}{StorageFolder}"))
                {
                    Directory.CreateDirectory($"{AppDomain.CurrentDomain.BaseDirectory}{StorageFolder}");
                }
                string filePath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, StorageFolder, "SDevice.json");
                if (File.Exists(filePath))
                {
                    string json = File.ReadAllText(filePath);
                    List<DeviceData>? deviceData = JsonSerializer.Deserialize<List<DeviceData>>(json);
                    if (deviceData != null)
                    {
                        deviceDataList.AddRange(deviceData);
                    }
                }

                return Task.FromResult(deviceDataList);
            }

        }
    }

    //public class DeviceDataStorage
    //{
    //    private const string StorageFolder = "DeviceDataStorage";
    //    private static readonly SemaphoreSlim semaphoreWrite = new SemaphoreSlim(1, 1000);
    //    private static readonly SemaphoreSlim semaphoreLoad = new SemaphoreSlim(1, 1000);

    //    public static async Task SaveDeviceDataAsync(DeviceData deviceData)
    //    {
    //        if (deviceData == null || deviceData.Properties == null)
    //        {
    //            throw new ArgumentException("Invalid device data");
    //        }

    //            List<DeviceData> deviceDatas = await LoadAllDeviceDataAsync();
    //        try
    //        {
    //            await semaphoreWrite.WaitAsync();
    //            List<DeviceData> deviceDataf = deviceDatas.FindAll(x => !(x?.Properties?.Find(y => y.Name == "SN")?.Value?.Contains(deviceData.Properties?.ToList()?.Find(t => t.Name == "SN")?.Value ?? "") ?? false));

    //            // Найти SN
    //            DeviceProperty? snProperty = deviceData.Properties.FirstOrDefault(p => p.Name == "SN");
    //            if (snProperty == null)
    //            {
    //                throw new ArgumentException("Device SN not found");
    //            }

    //            // Последний Measurement
    //            Measurement? latestMeasurement = deviceData.Measurements?.LastOrDefault();
    //            if (latestMeasurement != null)
    //            {
    //                deviceData.Measurements = new List<Measurement> { latestMeasurement };
    //            }

    //            // Создать папку если не существует
    //            if (!Directory.Exists($"{AppDomain.CurrentDomain.BaseDirectory}{StorageFolder}"))
    //            {
    //                Directory.CreateDirectory($"{AppDomain.CurrentDomain.BaseDirectory}{StorageFolder}");
    //            }

    //            deviceDataf.Add(deviceData);

    //            // Сохранить данные в JSON файл
    //            string filePath = Path.Combine($"{AppDomain.CurrentDomain.BaseDirectory}{StorageFolder}", $"SDevice.json");
    //            string json = JsonSerializer.Serialize(deviceDataf, new JsonSerializerOptions { WriteIndented = true });
    //            await File.WriteAllTextAsync(filePath, json);
    //        }
    //        finally
    //        {
    //            semaphoreWrite.Release();
    //        }
    //    }

    //    public static async Task<List<DeviceData>> LoadAllDeviceDataAsync()
    //    {
    //        await semaphoreLoad.WaitAsync();
    //        try
    //        {
    //            List<DeviceData> deviceDataList = new List<DeviceData>();

    //            if (!Directory.Exists($"{AppDomain.CurrentDomain.BaseDirectory}{StorageFolder}"))
    //            {
    //                Directory.CreateDirectory($"{AppDomain.CurrentDomain.BaseDirectory}{StorageFolder}");
    //            }

    //            string filePath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, StorageFolder, "SDevice.json");
    //            if (File.Exists(filePath))
    //            {
    //                string json = await File.ReadAllTextAsync(filePath);
    //                List<DeviceData>? deviceData = JsonSerializer.Deserialize<List<DeviceData>>(json);
    //                if (deviceData != null)
    //                {
    //                    deviceDataList.AddRange(deviceData);
    //                }
    //            }

    //            return deviceDataList;
    //        }
    //        finally
    //        {
    //            semaphoreLoad.Release();
    //        }
    //    }
    //}
}
