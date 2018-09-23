namespace ProgrammerAl.HardwareSpecific.RF
{
    public class RFMessage
    {
        public RFMessage(MessageMemoryInfo messageMemory, int rssi, RfEndpoint connectionInfo)
        {
            MessageMemory = messageMemory;
            RSSI = rssi;
            ConnectionInfo = connectionInfo;
        }

        public MessageMemoryInfo MessageMemory { get; }
        public int RSSI { get; }
        public RfEndpoint ConnectionInfo { get; }
    }
}
