using System;

namespace ProgrammerAl.HardwareSpecific.RF
{
    /// <summary>
    /// Interface for a hardware Radio Frequency Receiver
    /// </summary>
    public interface IRFReceiver : IDisposable
    {
        event Action<IRFReceiver, RFMessage> MessageReceived;

        byte NetworkId { get; }

        bool IsRunning { get; }
        void Start();
        void Stop();
    }
}
