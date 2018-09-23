namespace ProgrammerAl.HardwareSpecific.RF
{
    public static class Rfm69LibWrapper
    {
        public delegate int Initialize(Rf69FrequencyType frequencyBand, byte nodeId, byte networkId);
        public delegate void SetPowerLevel(TransmitPowerLevel powerLevel);
        public delegate void Send(byte targetNodeId, byte[] buffer, byte bufferSize, bool requestACK);
        public delegate void Receive();

        public delegate short GetMessageBytesLength();
        public delegate int GetMessageRSSI();
        public delegate void GetMessageBytes(byte[] messageReceived);
        public delegate byte GetMessageSenderId();
    }
}
