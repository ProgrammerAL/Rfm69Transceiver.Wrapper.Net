using System;
using System.Text;
using System.Threading;

namespace ProgrammerAl.HardwareSpecific.RF.Demo
{
    public static class Program
    {
        static void Main()
        {
            try
            {
                // To compile this to run on Raspberry Pi
                //   Open command line to root directory of solution file
                //      For Debug run:   dotnet publish ./Demo -c Release -r linux-arm --self-contained
                //      For Release run: dotnet publish ./Demo -c Debug -r linux-arm --self-contained
                //   Copy/paste the files from the /publish folder to a specific location on the Raspberry Pi
                //   On the Raspberry Pi, open a terminal command line window to the directory you copy/pasted the above files to
                //   Run this project with: dotnet ProgrammerAl.HardwareSpecific.RF.Demo.dll
                //   Note: This is hardware specific and will only work on a Raspberry Pi with an RFM69 Transceiver properly wired up
                Console.WriteLine("Starting ---");
                byte myNodeId = 1;
                byte networkId = 100;
                byte targetNodeId = 2;

                using (var transceiver = new Rfm69Transceiver(Rf69FrequencyType.RF69Frequency915, myNodeId, networkId, "RaspberryPiRfm69Wrapper.o"))
                {
                    Console.WriteLine("Completed Transceiver Init ---");
                    transceiver.MessageReceived += Transceiver_MessageReceived;
                    transceiver.Start();

                    Console.WriteLine("Transceiver now listening for messages");

                    var endpoint = new RfEndpoint(targetNodeId);

                    int i = 0;
                    while (true)
                    {
                        i++;
                        Console.WriteLine("Transmitting --- " + i);
                        string messageToSend = $"From Raspberry Pi with .NET Core! - {i}";
                        byte[] bytes = UTF8Encoding.UTF8.GetBytes(messageToSend);

                        transceiver.QueueStringToBeTransmitted(bytes, endpoint);
                        Thread.Sleep(1000);
                    }
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.ToString());
            }
        }

        private static void Transceiver_MessageReceived(IRFReceiver sender, RFMessage message)
        {
            Console.WriteLine($"Received Bytes: {message.MessageMemory.MessageBytes}");
            Console.Write("[");
            foreach (byte item in message.MessageMemory.MessageBytes)
            {
                Console.Write(item + ",");
            }
            Console.WriteLine("]");
            Console.WriteLine($"Received Bytes as String: {UTF8Encoding.ASCII.GetString(message.MessageMemory.MessageBytes)}");
            Console.WriteLine($"    Sender Node {message.ConnectionInfo.NodeId}");
            Console.WriteLine($"    RSSI {message.RSSI}");
        }
    }
}
