using System.Runtime.InteropServices;

namespace ProgrammerAl.HardwareSpecific.RF
{
    public static class Rfm69LibWrapper
    {
        [DllImport("./NativeLibs/RaspberryPiRfm69Wrapper.o", EntryPoint = "rfm69_initialize", SetLastError = true)]
        public static extern int Initialize(Rf69FrequencyType frequencyBand, byte nodeId, byte networkId);

        [DllImport("./NativeLibs/RaspberryPiRfm69Wrapper.o", EntryPoint = "rfm69_setPowerLevel", SetLastError = true)]
        public static extern void SetPowerLevel(TransmitPowerLevel powerLevel);

        [DllImport("./NativeLibs/RaspberryPiRfm69Wrapper.o", EntryPoint = "rfm69_send", SetLastError = true)]
        public static extern void Send(byte targetNodeId, byte[] buffer, byte bufferSize, bool requestACK);

        [DllImport("./NativeLibs/RaspberryPiRfm69Wrapper.o", EntryPoint = "rfm69_receive", SetLastError = true)]
        public static extern void Receive();

        [DllImport("./NativeLibs/RaspberryPiRfm69Wrapper.o", EntryPoint = "rfm69_getDataLen", SetLastError = true)]
        public static extern short GetMessageBytesLength();

        [DllImport("./NativeLibs/RaspberryPiRfm69Wrapper.o", EntryPoint = "rfm69_getRssi", SetLastError = true)]
        public static extern int GetMessageRSSI();

        [DllImport("./NativeLibs/RaspberryPiRfm69Wrapper.o", EntryPoint = "rfm69_getData", SetLastError = true)]
        public static extern void GetMessageBytes(byte[] messageReceived);

        [DllImport("./NativeLibs/RaspberryPiRfm69Wrapper.o", EntryPoint = "rfm69_getSenderId", SetLastError = true)]
        public static extern byte GetMessageSenderId();
    }
}
