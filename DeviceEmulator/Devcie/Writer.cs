using System.Net.Sockets;
using System.Diagnostics;
using System.Text.Json;
using System.Text;

namespace DeviceEmulator.Device
{
    static public class Writer<T>
        {
            static public async Task Write(NetworkStream stream, T request)
            {
                string requestJson = JsonSerializer.Serialize(request);


                byte[] requestData;
                
                requestData = Encoding.UTF8.GetBytes(requestJson);
                await stream.WriteAsync(requestData, 0, requestData.Length);
                
                Debug.WriteLine($"Отправил: " + requestJson);
            }
        }
}
