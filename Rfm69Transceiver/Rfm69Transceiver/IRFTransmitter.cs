using System;

namespace ProgrammerAl.HardwareSpecific.RF
{
    /// <summary>
    /// Interface for a hardware Radio Frequency Transmitter
    /// </summary>
    public interface IRFTransmitter : IDisposable
    {
        byte NetworkId { get; }

        void QueueStringToBeTransmitted(ReadOnlyMemory<byte> message, RfEndpoint connectionInfo);
        void QueueStringToBeTransmittedToEveryone(ReadOnlyMemory<byte> message);
    }
}
