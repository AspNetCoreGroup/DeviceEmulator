using CommonTypeDevice;
using CommonTypeDevice.Event;
using CommonTypeDevice.Measurument;
using CommonTypeDevice.Property;
using DeviceEmulator.Interfaces;
using System.Diagnostics;
using System.Net;
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
                Debug.WriteLine("send");
                Send(data);
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

        public async Task<List<Measurement>> GetAllValuesFromProfiles(IEnumerable<IProfile> profiles)
        {
            List<Measurement> allValues = new List<Measurement>();

            foreach (IProfile profile in profiles)
            {
                IEnumerable<IValue>? values = await profile.GetValues();
                if (values != null)
                {
                    foreach (IValue value in values)
                        allValues.Add(value.GetMeasurement());
                }
            }


            return allValues;
        }


        public async Task Send(DeviceData? deviceData)
        {
            if (deviceData != null)
            {
                string _choosenDirectory = AppDomain.CurrentDomain.BaseDirectory;
                //ServerDataStorageConfig? server_config = new();
                try
                {
                    //using (TcpClient client = new TcpClient())
                    //{
                    using (HttpClient client2 = new HttpClient())
                    {
                        //await client.ConnectAsync(ipAddress, port);
                        client2.BaseAddress = new Uri($"http://{ServerDataStorageConfig.ipAddres}:{ServerDataStorageConfig.port}");
                        string propertiesJson = JsonSerializer.Serialize(deviceData.Properties);
                        string deviceEventsJson = JsonSerializer.Serialize(deviceData.DeviceEvents);
                        string measurementsJson = JsonSerializer.Serialize(deviceData.Measurements);

                        // Create the content for the POST request
                        FormUrlEncodedContent content = new FormUrlEncodedContent(new[]
                                                {
                            new KeyValuePair<string, string>("Properties", propertiesJson),
                            new KeyValuePair<string, string>("DeviceEvents", deviceEventsJson),
                            new KeyValuePair<string, string>("Measurements", measurementsJson)
                        });
                        
                        HttpResponseMessage result = await client2.PostAsync("/DataFromDevice", content);
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
                    await Task.Delay(2000); // Ожидание 2 секунд перед отправкой следующего запроса
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

        public virtual async Task DoEvent()
        {

        }

        /// <summary>
        /// With a Chance percentage, returns the result and fills EventParameters with a parameter with a timeout of 15 seconds
        /// </summary>
        /// <returns></returns>
        public async Task<IDeviceEvent?> Get()
        {
            Dictionary<int, string> dictionary = EventDictionary.dictionary;
            await Task.Delay(15000); // Simulate 15 seconds timeout

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
        public static string port { set; get; } = "5000";
    }
}
