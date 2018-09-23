namespace ProgrammerAl.HardwareSpecific.RF
{
    /// <summary>
    /// Interface for a Radio Frequency transceiver (both a transmitter and a receiver)
    /// </summary>
    public interface IRFTransceiver : IRFReceiver, IRFTransmitter
    {
    }
}
