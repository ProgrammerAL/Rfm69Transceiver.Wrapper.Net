using System;

namespace ProgrammerAl.HardwareSpecific.RF
{
    internal readonly struct OutgoingQueuedMessage : IEquatable<OutgoingQueuedMessage>
    {
        public OutgoingQueuedMessage(ReadOnlyMemory<byte> message, RfEndpoint connectionInfo)
        {
            Message = message;
            ConnectionInfo = connectionInfo;
        }

        public ReadOnlyMemory<byte> Message { get; }
        public RfEndpoint ConnectionInfo { get; }

        public override int GetHashCode() => (Message, ConnectionInfo).GetHashCode();
        public override bool Equals(object obj) => obj is OutgoingQueuedMessage otherObj && Equals(otherObj);

        public bool Equals(OutgoingQueuedMessage other)
        {
            return Message.Length == other.Message.Length
                && Message.Span.SequenceEqual(other.Message.Span)
                && ConnectionInfo == other.ConnectionInfo;
        }

        public static bool operator ==(OutgoingQueuedMessage result1, OutgoingQueuedMessage result2)
        {
            return result1.Equals(result2);
        }

        public static bool operator !=(OutgoingQueuedMessage result1, OutgoingQueuedMessage result2)
        {
            return !(result1 == result2);
        }
    }
}
