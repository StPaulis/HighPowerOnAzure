# Controlling your home with IoT Hub, Raspberry PI & Resin.IO

## AzureHeads 12#:Azure Machine Learning & IoT Hub

This repository contains the 4 pieces that we need to stick together to
control high power (the software part...)

### Sample Code:
#### - AddIotDevice (to IoT Hub)
#### - OpenTheLight [Iot Device code: Publish for rasberry pi 3+:
For rasberry pi 3+
````
dotnet publish . -r ubuntu.16.04-arm
````
#### - ReceiveIotHubMessage
#### - SendMsg2IotDevice

### Instructions: 
If you want to play with this sample, replace the static variables with your 
keys, connections strings and IoT Device Name.

Install .Net Core 2 sdk

Build & Publish

#### Just in case
If raspberry doesn't have .net core sdk you can publish from
your personal computer the files and run the publish executable
on your raspberry directly.

***libunwind8 (dependency)
````
sudo apt-get update
sudo apt-get install libunwind8
````
