using System;
using System.Collections.Concurrent;
using System.Threading;
using NativeLibraryLoader;
using System.Runtime.InteropServices;
using System.Buffers;
using System.Diagnostics;

namespace ProgrammerAl.HardwareSpecific.RF
{
    public class Rfm69Transceiver : IRFTransceiver
    {
        private const string LibRelativeFolderPath = "NativeLibs";
        private const string InitMethodName = "rfm69_initialize";
        private const string SetPowerLevelMethodName = "rfm69_setPowerLevel";
        private const string SendMethodName = "rfm69_send";
        private const string ReceiveMethodName = "rfm69_receive";

        private const string GetMessageLengthMethodName = "rfm69_getDataLen";
        private const string GetMessageRSSIMethodName = "rfm69_getRssi";
        private const string GetMessageBytesMethodName = "rfm69_getData";
        private const string GetMessageSenderIdMethodName = "rfm69_getSenderId";

        private const byte NodeIdForEveryone = 255;
        private const byte MaxMessageBytesCount = 61;

        private ConcurrentQueue<OutgoingQueuedMessage> _messagesToSend;
        private static readonly TimeSpan ReceiveCallbackCheck = TimeSpan.FromMilliseconds(10);

        private readonly Rfm69LibWrapper.Send _send;
        private readonly Rfm69LibWrapper.Receive _receive;

        private readonly Rfm69LibWrapper.GetMessageBytesLength _getMessageBytesLength;
        private readonly Rfm69LibWrapper.GetMessageRSSI _getMessageRssi;
        private readonly Rfm69LibWrapper.GetMessageBytes _getMessageBytes;
        private readonly Rfm69LibWrapper.GetMessageSenderId _getMessageSenderId;

        private readonly NativeLibrary _nativeLibrary;

        private Thread _rfThread;

        public Rfm69Transceiver(Rf69FrequencyType frequencyType, byte nodeId, byte networkId, string nativeLibfileName)
        {
            NetworkId = networkId;
            NodeId = nodeId;
            _messagesToSend = new ConcurrentQueue<OutgoingQueuedMessage>();

            var pathResolver = new RelativePathResolver(LibRelativeFolderPath);
            var libraryLoader = LibraryLoader.GetPlatformDefaultLoader();
            _nativeLibrary = new NativeLibrary(nativeLibfileName, libraryLoader, pathResolver);

            _send = _nativeLibrary.LoadFunction<Rfm69LibWrapper.Send>(SendMethodName);
            _receive = _nativeLibrary.LoadFunction<Rfm69LibWrapper.Receive>(ReceiveMethodName);

            _getMessageBytesLength = _nativeLibrary.LoadFunction<Rfm69LibWrapper.GetMessageBytesLength>(GetMessageLengthMethodName);
            _getMessageRssi = _nativeLibrary.LoadFunction<Rfm69LibWrapper.GetMessageRSSI>(GetMessageRSSIMethodName);
            _getMessageBytes = _nativeLibrary.LoadFunction<Rfm69LibWrapper.GetMessageBytes>(GetMessageBytesMethodName);
            _getMessageSenderId = _nativeLibrary.LoadFunction<Rfm69LibWrapper.GetMessageSenderId>(GetMessageSenderIdMethodName);

            InitTransceiver(frequencyType, NodeId, NetworkId, _nativeLibrary);

            _rfThread = new Thread(new ParameterizedThreadStart(HandleRfWork));
        }

        public byte NetworkId { get; }
        public byte NodeId { get; }
        public bool IsRunning { get; private set; }

        public event Action<IRFReceiver, RFMessage> MessageReceived;

        public void Dispose()
        {
            Stop();
            _nativeLibrary.Dispose();
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

        public void QueueStringToBeTransmitted(ReadOnlyMemory<byte> message, RfEndpoint connectionInfo)
        {
            var queuedMessage = new OutgoingQueuedMessage(message, connectionInfo);
            _messagesToSend.Enqueue(queuedMessage);
        }

        public void QueueStringToBeTransmittedToEveryone(ReadOnlyMemory<byte> message)
        {
            var queuedMessage = new OutgoingQueuedMessage(message, new RfEndpoint(NodeIdForEveryone));
            _messagesToSend.Enqueue(queuedMessage);
        }

        private static void InitTransceiver(Rf69FrequencyType frequencyType, byte nodeId, byte networkId, NativeLibrary loader)
        {
            Rfm69LibWrapper.Initialize initMethod = loader.LoadFunction<Rfm69LibWrapper.Initialize>(InitMethodName);
            initMethod.Invoke(frequencyType, nodeId, networkId);

            Rfm69LibWrapper.SetPowerLevel setPowerLevelMethod = loader.LoadFunction<Rfm69LibWrapper.SetPowerLevel>(SetPowerLevelMethodName);
            setPowerLevelMethod.Invoke(TransmitPowerLevel.MaxPower);
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
                    _send(message.ConnectionInfo.NodeId, messageBytes.Array, (byte)message.Message.Length, false);
                }

                _receive.Invoke();
                short receivedMessageLength = _getMessageBytesLength();
                if (receivedMessageLength > 0)
                {
                    Debug.WriteLine($"Read Message with Length: {receivedMessageLength}");
                    byte[] messageArray = arrayPool.Rent(MaxMessageBytesCount);
                    int rssi = _getMessageRssi();
                    byte senderId = _getMessageSenderId();
                    _getMessageBytes(messageArray);

                    using (var messageMemoryInfo = new MessageMemoryInfo(messageArray, arrayPool))
                    {
                        MessageReceived?.Invoke(this, new RFMessage(messageMemoryInfo, rssi, new RfEndpoint(senderId)));
                    }
                }
            }
        }
    }
}
