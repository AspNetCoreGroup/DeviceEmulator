using CommonTypeDevice.Event;
using DeviceEmulator.Interfaces;
using System.Net.Sockets;
using System.Net;
using System.Diagnostics;
using System.Text.RegularExpressions;
using CommonTypeDevice;
using CommonTypeDevice.Property;
using CommonTypeDevice.Measurument;

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
                    Properties  = await GetAllProppertys(properties),

                };
                Send(data);
            }
        }

        private async Task<List<DeviceProperty>> GetAllProppertys(IEnumerable<IProperty> properties)
        {
            var allProperty = new List<DeviceProperty>();
            foreach (var properti in properties)
            {
                allProperty.Add(new(properti.Name, properti.Value));
            }
            return allProperty;
        }

        public async Task<List<Measurement>> GetAllValuesFromProfiles(IEnumerable<IProfile> profiles)
        {
            var allValues = new List<Measurement>();

            foreach (var profile in profiles)
            {
                var values = await profile.GetValues();
                if (values != null)
                {
                    foreach (var value in values)
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
                ServerDataStorageConfig? server_config = new();

                int port = 5247;
                int.TryParse(server_config?.port, out port); // Порт, на котором будет слушать сервер

                string host = Dns.GetHostName();
                IPAddress? ipAddress;
                if (!IPAddress.TryParse(server_config?.ipAddres, out ipAddress))
                {
                    ipAddress = Dns.GetHostAddresses(host).Last<IPAddress>(f => f.AddressFamily == System.Net.Sockets.AddressFamily.InterNetwork);
                }
                try
                {
                    using (TcpClient client = new TcpClient())
                    {

                        await client.ConnectAsync(ipAddress, port);
                        using (NetworkStream stream = client.GetStream())
                        {
                            await Writer<DeviceData>.Write(stream, deviceData);
                        }
                    }

                }
                catch (Exception ex)
                {
                    Debug.WriteLine("Ошибка при подключении или обмене данными с сервером: " + ex.Message);
                    await Task.Delay(2000); // Ожидание 2 секунд перед отправкой следующего запроса

                    Debug.WriteLine($"Для закрытия нажмите любую клавишу");
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

        public virtual async  Task DoEvent()
        {

        }

        /// <summary>
        /// With a Chance percentage, returns the result and fills EventParameters with a parameter with a timeout of 15 seconds
        /// </summary>
        /// <returns></returns>
        public async Task<IDeviceEvent?> Get()
        {
            var dictionary = EventDictionary.dictionary;
            await Task.Delay(15000); // Simulate 15 seconds timeout

            Random random = new Random();
            if (random.Next(100) < Chance) // If the random number is within the chance range
            {
                // Simulate filling EventParameters
                var value = "";
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

    public class ServerDataStorageConfig
    {
        public string ipAddres { set; get; } = "127.0.0.1";
        public string port { set; get; } = "1944";
    }
}
