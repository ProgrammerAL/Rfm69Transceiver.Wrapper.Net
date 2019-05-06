using System;

namespace ProgrammerAl.HardwareSpecific.RF
{
    /// <summary>
    /// Interface for a hardware Radio Frequency Transmitter
    /// </summary>
    public interface IRFTransmitter : IDisposable
    {
        byte NetworkId { get; }

        void QueueStringToBeTransmitted(in ReadOnlyMemory<byte> message, RfEndpoint connectionInfo);
        void QueueStringToBeTransmittedToEveryone(in ReadOnlyMemory<byte> message);
    }
}
