﻿using Microsoft.Azure.Devices;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SendCloud2Device
{
    class Program
    {
        static ServiceClient serviceClient;
        static string connectionString = "HostName=electra.azure-devices.net;SharedAccessKeyName=iothubowner;SharedAccessKey=12AhxT0etW53dGg5l2O2fAmzSB08o6OCGZpAephLwC0=";

        private async static Task SendCloudToDeviceMessageAsync(string s)
        {
            var commandMessage = new Message(Encoding.ASCII.GetBytes(s));
            await serviceClient.SendAsync("electra-iot-design.gr", commandMessage);
        }

        static void Main(string[] args)
        {
            Console.WriteLine("Send Cloud-to-Device message\n");
            serviceClient = ServiceClient.CreateFromConnectionString(connectionString);
            while (true)
            {
                Console.WriteLine("Press any key to send a C2D message.");
                var s = Console.ReadLine();
                SendCloudToDeviceMessageAsync(s).Wait();
            }
        }
    }
}