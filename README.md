# Rfm69Transceiver
This repo is a .NET Wrapper for working with an RFM69 Transceiver on a Raspberry Pi. This is a wrapper around the code in https://github.com/ProgrammerAl/RaspberryPiRfm69Wrapper repository. The compiled native library from that repo is already included in this one. Ys, it's not reccomended. But there won't be a lot of changes to this repo, so it's fine.

## How to Compile and Run
The Demo project has an example on getting started with sending and receiving messages. If you would like to compile that project, use the following instructions. The instructions publish a self-contained .NET Core application, but you will still need to have the .NET Core runtime installed on your Raspberry Pi. If you have not already done so, download and install the runtime for the Linux ARM32 verion from https://www.microsoft.com/net/download. I've only run the below instructions from a Windows 10 machine, but they should work on any machine that supports .NET Core.

1. Open command line to root directory of Rfm69Transceiver.sln solution file
1. To compile a debug version run: dotnet publish ./Demo -c Debug -r linux-arm --self-contained
1. Or, to compile a release version run: dotnet publish ./Demo -c Release -r linux-arm --self-contained
1. Navigate to the output directory from your chosen publish command: ~/Demo/bin/<debug or release>/netcoreapp2.1/linux-arm/publish
1. Note: If the first command you run is publish, it's possible the ~/publish/NativeLibs folder will not be output containing the native RaspberryPiRfm69Wrapper.o file used to control the transceiver. If this happens run the `dotnet build` command from the root directory where the Rfm69Transceiver.sln file is located. Then run the publish command again. 
1. Copy all files in the publish folder and paste them to your desired location on a raspberry Pi (ex: /home/pi/Desktop/Practice)
1. On the Raspberry Pi, open a terminal command line window and navigate to the folder you copy/pasted the above files to
1. Run the demo project with the command: dotnet ProgrammerAl.HardwareSpecific.RF.Demo.dll

Reminder: This is hardware specific and will only work on a Raspberry Pi with an RFM69 Transceiver properly wired up.

