using Microsoft.AspNetCore.SignalR.Client;
using Microsoft.AspNetCore.SignalR.Internal.Protocol;
using Microsoft.AspNetCore.Sockets;
using Microsoft.AspNetCore.Sockets.Client;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
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

namespace coreiot
{

    class Program
    {
        static DeviceClient deviceClient;
        static string iotHubUri = "electra.azure-devices.net";
        static string deviceKey = "dlZVBlSg9I4dW9/tsSRsobUoyYn+tybVLX0gBg09Yeg=";

        public static void Main(string[] args)
        {

            var NodePins = new Dictionary<int, bool>
            {
                {7 , false},
                {0 , false},
                {2 , false},
                {3 , false},
                {21, false},
                {22, false},
                {23, false},
                {24, false},
                {25, false},
                {1 , false},
                {4 , false},
                {5 , false},
                {6 , false},
                {10, false},
                {11, false},
            };
            var IdNodePins = new Dictionary<int, int>
            {
                { 3,  7  },
                { 4,  0  },
                { 5,  2  },
                { 6,  3  },
                { 7,  21 },
                { 8,  22 },
                { 9,  23 },
                { 10, 24 },
                { 11, 25 },
                { 12, 1  },
                { 13, 4  },
                { 14, 5  },
                { 15, 6  },
                { 16, 10 },
                { 17, 11 },
            };

            foreach (var pin in IdNodePins)
            {
                var status = false;
                Console.WriteLine($"Line 57: Your id {pin.Key} controller pin {pin.Value} and status {status}");

                Pi.Gpio[pin.Value].PinMode = GpioPinDriveMode.Output;
                Pi.Gpio[pin.Value].Write(!status);
                NodePins[pin.Value] = status;
            }

            Console.WriteLine("Simulated device\n");
            deviceClient = DeviceClient.Create(iotHubUri, new DeviceAuthenticationWithRegistrySymmetricKey("electra-iot-design.gr", deviceKey), Microsoft.Azure.Devices.Client.TransportType.Mqtt);
            deviceClient.ProductInfo = "Electra";
            ReceiveC2dAsync(NodePins);
            Console.ReadLine();

        }

        private static async void ReceiveC2dAsync(Dictionary<int, bool> dict)
        {
            Console.WriteLine("\nReceiving cloud to device messages from service");
            while (true)
            {
                Microsoft.Azure.Devices.Client.Message receivedMessage = await deviceClient.ReceiveAsync();
                if (receivedMessage == null) continue;

                Console.ForegroundColor = ConsoleColor.Yellow;
                var msg = Encoding.ASCII.GetString(receivedMessage.GetBytes());
                Console.WriteLine("Received message: {0}", msg);
                Console.ResetColor();

                await deviceClient.CompleteAsync(receivedMessage);

                try
                {
                    /*handle pinString to relay */
                    string[] check = msg.Split(',');
                    bool isOk = true;
                    var nums = new List<int>();

                    foreach (var c in check)
                    {
                        isOk = int.TryParse(c, out var num);
                        nums.Add(num);
                    }
                    if (isOk)
                    {
                        var keyValues = dict.Where(x => nums.Contains(x.Key))
                           .Select(x => x.Value).ToList();
                        foreach (var n in nums)
                        {

                            if (!dict[n] && n != 24) //opens special relay straight on pin :/
                            {
                                Console.WriteLine($"Open relay {n}");
                                Pi.Gpio[n].Write(false);
                                dict[n] = true;
                            }
                            else if (dict[n] && n != 24)
                            {
                                Console.WriteLine($"Close relay {n}");
                                Pi.Gpio[n].Write(true);
                                dict[n] = false;
                            }
                            else
                            {
                                Console.WriteLine($"Open relay {n}");
                                Pi.Gpio[n].Write(false);
                                dict[n] = true;

                                Thread.Sleep(50);

                                Console.WriteLine($"Close relay {n}");
                                Pi.Gpio[n].Write(true);
                                dict[n] = false;
                            }

                        }
                    }
                    /*handle pinString to relay */
                }
                catch (Exception ex)
                {
                    Console.WriteLine($"Error on piString to relay {ex}");
                }
            }
        }

        public static async Task<Boolean> GetStatus(int pin)
        {
            var baseUrl = $"http://iot-design.azurewebsites.net/api/NodePin/{pin}";
            using (HttpClient client = new HttpClient())
            using (HttpResponseMessage res = await client.GetAsync(baseUrl))
            using (HttpContent content = res.Content)
            {
                var modelString = await content.ReadAsStringAsync();


                Console.WriteLine($"Line 119: {modelString} {pin}");


                return JsonConvert.DeserializeObject<Boolean>(modelString);
            }
        }
    }
}
