using System;
using System.Linq;
using System.Net;
using System.Net.WebSockets;
using System.Reflection;
using System.ServiceModel;
using System.ServiceModel.Channels;
using System.Threading;
using System.Threading.Tasks;
using System.Xml;
using Common;
using Wcf.PureWsServer;

var messageEncoderFactoryType = typeof(MessageEncoderFactory).Assembly.DefinedTypes
    .First(x => x.FullName == "System.ServiceModel.Channels.BinaryMessageEncoderFactory");
var binaryVersionType = typeof(MessageEncoderFactory).Assembly.DefinedTypes
    .First(x => x.FullName == "System.ServiceModel.Channels.BinaryVersion");
var constructor = messageEncoderFactoryType.GetConstructors(BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic).First();
var encoderFactory = (MessageEncoderFactory)constructor.Invoke(new[]
{
    MessageVersion.CreateVersion(EnvelopeVersion.Soap12), 64, 16, 2048,
    new XmlDictionaryReaderQuotas { MaxDepth = 32, MaxArrayLength = 16384, MaxBytesPerRead = 4096, MaxStringContentLength = 8192, MaxNameTableCharCount = 16384 },
    2147483647, binaryVersionType.GetField("Version1", BindingFlags.Static | BindingFlags.Public | BindingFlags.NonPublic).GetValue(null),
    CompressionFormat.None,
});
var messageEncoder = encoderFactory.CreateSessionEncoder();

var prefix = "http://*:6060/";

using var httpListener = new HttpListener();
httpListener.Prefixes.Add(prefix);
httpListener.Start();

Console.WriteLine($"Server started at: {prefix}");

while (true)
{
    var context = await httpListener.GetContextAsync();
    HandleWebSocketConnection(await context.AcceptWebSocketAsync("soap", 4096, TimeSpan.FromSeconds(30)));
}

async Task HandleWebSocketConnection(HttpListenerWebSocketContext webSocketContext)
{
    var bufferManager = BufferManager.CreateBufferManager(2147483647, 2147483647);

    while (true)
    {
        var receivedMessage = await webSocketContext.WebSocket.ReceiveMessageAsync();
        if (receivedMessage is not BinaryMessage { Data: var data })
        {
            continue;
        }

        var wcfMessage = messageEncoder.ReadMessage(data, bufferManager);
        Console.WriteLine($"Received from client: {wcfMessage}");

        var responseMessage = wcfMessage.Headers.Action switch
        {
            "http://tempuri.org/IRecordEventsService/IsAlive" => MessageUtils.CreateResponseMessage(wcfMessage),
            "http://tempuri.org/IRecordEventsService/SubmitEvents" => MessageUtils.CreateResponseMessage(wcfMessage),
            _ => null,
        };

        if (responseMessage != null)
        {
            await webSocketContext.WebSocket.SendAsync(
                messageEncoder.WriteMessage(responseMessage, int.MaxValue, bufferManager), WebSocketMessageType.Binary, true,
                CancellationToken.None);
        }
    }
}