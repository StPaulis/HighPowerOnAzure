using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Threading.Tasks;
using Unosquare.RaspberryIO;
using Unosquare.RaspberryIO.Gpio;
using Microsoft.Azure.Devices;
using Microsoft.Azure.Devices.Client;
using System.Text;
using System.Threading;

namespace OpenTheLight
{
    class Program
    {
        static DeviceClient deviceClient;
        static string iotHubUri = "stpaulis.azure-devices.net";
        static string deviceKey = "qk80e5DcuYSPeGO/Hik8PMDEVWtxeKRpDIP3RFUYYio=";
        static bool status = false;

        public static async Task Main()
        {
            // Set pin 24 as output and set initial state to close
            Pi.Gpio[24].PinMode = GpioPinDriveMode.Output;
            Pi.Gpio[24].Write(status);

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

                // Switch the Light
                try
                {
                    if (msg == "24")
                    {
                        Console.WriteLine($"Switch The light on pin 24");
                        Pi.Gpio[24].Write(!status);
                        status = !status;
                    }
                }
                catch (Exception ex)
                {
                    Console.WriteLine($"Error on switching the light to relay {ex}");
                }
            }
        }
    }
}
