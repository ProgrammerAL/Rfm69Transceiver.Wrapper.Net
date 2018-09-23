using System;

namespace ProgrammerAl.HardwareSpecific.RF
{
    public readonly struct RfEndpoint : IEquatable<RfEndpoint>
    {
        public RfEndpoint(byte nodeId) => NodeId = nodeId;

        public byte NodeId { get; }

        public override string ToString() => $"{nameof(RfEndpoint)}({nameof(NodeId)}: {NodeId})";

        public override int GetHashCode() => (NodeId).GetHashCode();
        public override bool Equals(object obj) => obj is RfEndpoint otherObj && Equals(otherObj);
        public bool Equals(RfEndpoint other) => NodeId == other.NodeId;

        public static bool operator ==(RfEndpoint result1, RfEndpoint result2)
        {
            return result1.Equals(result2);
        }

        public static bool operator !=(RfEndpoint result1, RfEndpoint result2)
        {
            return !(result1 == result2);
        }
    }
}
