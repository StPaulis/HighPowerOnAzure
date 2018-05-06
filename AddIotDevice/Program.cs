﻿using Microsoft.Azure.Devices;
using Microsoft.Azure.Devices.Common.Exceptions;
using System;
using System.Threading.Tasks;

namespace AddIotDevice
{
    class Program
    {
        static RegistryManager registryManager;
        static string connectionString = "HostName=stpaulis.azure-devices.net;SharedAccessKeyName=iothubowner;SharedAccessKey=EhkZvkWkB+W7WYALjkHD5K5XYXnsEGMF7RMo4etWsIE=";

        static void Main(string[] args)
        {
            registryManager = RegistryManager.CreateFromConnectionString(connectionString);
            AddDeviceAsync().Wait();
            Console.ReadLine();
        }

        private static async Task AddDeviceAsync()
        {
            string deviceId = "ControlPowerWithAzure";
            Device device;
            try
            {
                device = await registryManager.AddDeviceAsync(new Device(deviceId));
            }
            catch (DeviceAlreadyExistsException)
            {
                device = await registryManager.GetDeviceAsync(deviceId);
            }
            Console.WriteLine("Generated device key: {0}", device.Authentication.SymmetricKey.PrimaryKey);
        }
    }
}