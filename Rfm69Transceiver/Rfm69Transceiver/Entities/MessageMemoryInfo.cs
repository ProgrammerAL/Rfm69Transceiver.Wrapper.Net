using System;
using System.Buffers;

namespace ProgrammerAl.HardwareSpecific.RF
{
    public readonly struct MessageMemoryInfo : IDisposable
    {
        public MessageMemoryInfo(byte[] messageBytes, ArrayPool<byte> parentMemoryPool)
        {
            MessageBytes = messageBytes;
            ParentMemoryPool = parentMemoryPool;
        }

        public byte[] MessageBytes { get; }
        public ArrayPool<byte> ParentMemoryPool { get; }

        public void Dispose() => ParentMemoryPool.Return(MessageBytes);
    }
}
