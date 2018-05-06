using System;
using System.Threading.Tasks;
using Microsoft.Azure.Devices.Client;
using System.Text;

namespace OpenTheLight
{
    class Program
    {
        static DeviceClient deviceClient;
        static string iotHubUri = "stpaulis.azure-devices.net";
        static string deviceKey = "qk80e5DcuYSPeGO/Hik8PMDEVWtxeKRpDIP3RFUYYio=";

        public static async Task Main()
        {
            // Authenticate device and set transport type
            deviceClient = DeviceClient.Create(iotHubUri,
             new DeviceAuthenticationWithRegistrySymmetricKey("ControlPowerWithAzure", deviceKey), Microsoft.Azure.Devices.Client.TransportType.Mqtt);
            deviceClient.ProductInfo = "Electra";

            //Handle Receiving on another Thread
            await Task.Factory.StartNew(() => ReceiveMessageAsync());

            //Wait
            Console.ReadLine();
        }

        private static async void ReceiveMessageAsync()
        {
            Console.WriteLine("\nReceiving messages from Azure");
            while (true)
            {
                Microsoft.Azure.Devices.Client.Message receivedMessage = await deviceClient.ReceiveAsync();
                if (receivedMessage == null) continue;

                Console.ForegroundColor = ConsoleColor.Yellow;

                var msg = Encoding.ASCII.GetString(receivedMessage.GetBytes());
                Console.WriteLine("Received message: {0}", msg);
                Console.ResetColor();

                await deviceClient.CompleteAsync(receivedMessage);
            }
        }
    }
}
