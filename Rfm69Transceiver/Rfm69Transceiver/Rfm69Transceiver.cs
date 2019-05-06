using System;
using System.Collections.Concurrent;
using System.Threading;
using System.Runtime.InteropServices;
using System.Buffers;
using System.Diagnostics;

namespace ProgrammerAl.HardwareSpecific.RF
{
    public class Rfm69Transceiver : IRFTransceiver
    {
        private const byte NodeIdForEveryone = 255;
        private const byte MaxMessageBytesCount = 61;

        private readonly ConcurrentQueue<OutgoingQueuedMessage> _messagesToSend;
        private readonly Thread _rfThread;

        public Rfm69Transceiver(Rf69FrequencyType frequencyType, byte nodeId, byte networkId, string nativeLibfileName)
        {
            NetworkId = networkId;
            NodeId = nodeId;
            _messagesToSend = new ConcurrentQueue<OutgoingQueuedMessage>();

            InitTransceiver(frequencyType, NodeId, NetworkId);

            _rfThread = new Thread(new ParameterizedThreadStart(HandleRfWork));
        }

        public byte NetworkId { get; }
        public byte NodeId { get; }
        public bool IsRunning { get; private set; }

        public event Action<IRFReceiver, RFMessage> MessageReceived;

        public void Dispose()
        {
            Stop();
        }

        public void Start()
        {
            if (!IsRunning)
            {
                IsRunning = true;

                if (_rfThread.IsAlive)
                {
                    _rfThread.Abort();
                }

                _rfThread.Start();
            }
        }

        public void Stop() => IsRunning = false;

        public void QueueStringToBeTransmitted(in ReadOnlyMemory<byte> message, RfEndpoint connectionInfo)
        {
            var queuedMessage = new OutgoingQueuedMessage(message, connectionInfo);
            _messagesToSend.Enqueue(queuedMessage);
        }

        public void QueueStringToBeTransmittedToEveryone(in ReadOnlyMemory<byte> message)
        {
            var queuedMessage = new OutgoingQueuedMessage(message, new RfEndpoint(NodeIdForEveryone));
            _messagesToSend.Enqueue(queuedMessage);
        }

        private static void InitTransceiver(Rf69FrequencyType frequencyType, byte nodeId, byte networkId)
        {
            _ = Rfm69LibWrapper.Initialize(frequencyType, nodeId, networkId);
            Rfm69LibWrapper.SetPowerLevel(TransmitPowerLevel.MaxPower);
        }

        private void HandleRfWork(object _)
        {
            //TODO: Figure out if 100 is good for max arrays per bucket. Right now it's just a magic number
            var arrayPool = ArrayPool<byte>.Create(MaxMessageBytesCount, 100);
            while (IsRunning)
            {
                if (_messagesToSend.TryDequeue(out OutgoingQueuedMessage message)
                    && MemoryMarshal.TryGetArray(message.Message, out ArraySegment<byte> messageBytes))
                {
                    Rfm69LibWrapper.Send(message.ConnectionInfo.NodeId, messageBytes.Array, (byte)message.Message.Length, false);
                }

                Rfm69LibWrapper.Receive();
                short receivedMessageLength = Rfm69LibWrapper.GetMessageBytesLength();
                if (receivedMessageLength > 0)
                {
                    Debug.WriteLine($"Read Message with Length: {receivedMessageLength}");
                    byte[] messageArray = arrayPool.Rent(MaxMessageBytesCount);
                    int rssi = Rfm69LibWrapper.GetMessageRSSI();
                    byte senderId = Rfm69LibWrapper.GetMessageSenderId();
                    Rfm69LibWrapper.GetMessageBytes(messageArray);

                    using (var messageMemoryInfo = new MessageMemoryInfo(messageArray, arrayPool))
                    {
                        MessageReceived?.Invoke(this, new RFMessage(messageMemoryInfo, rssi, new RfEndpoint(senderId)));
                    }
                }
            }
        }
    }
}
