using System;
using System.Net.WebSockets;
using System.Runtime.ExceptionServices;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Common;

var url = new Uri("ws://localhost:8080/ws");
ClientWebSocket connection;

while (true)
{
	try
	{
		connection = new ClientWebSocket();
		await connection.ConnectAsync(url, CancellationToken.None);
		break;
	}
	catch (Exception e)
	{
		Console.WriteLine($"Failed to connect. Retry after 3 sec... {e.Message}");
		await Task.Delay(TimeSpan.FromSeconds(3));
	}
}

Console.WriteLine($"Connected to the WS server {url}");

var message = Encoding.UTF8.GetBytes("Hello Server");
var tasks = new[]
{
	GetReceiveTextTask(),
	GetSendMessageTask(),
};

while (true)
{
	var finishedTask = await Task.WhenAny(tasks);
	if (finishedTask.Exception != null)
	{
		ExceptionDispatchInfo.Capture(finishedTask.Exception).Throw();
	}

	if (finishedTask == tasks[0])
	{
		tasks[0] = GetReceiveTextTask();
	}
	else
	{
		tasks[1] = GetSendMessageTask();
	}
}

async Task GetReceiveTextTask()
{
	var receivedMessage = await connection.ReceiveMessageAsync();
	if (receivedMessage is not TextMessage { Text: var text })
	{
		throw new InvalidOperationException("Invalid message");
	}

	Console.WriteLine($"Received from server: {text}");
}

async Task GetSendMessageTask()
{
	await Task.Delay(TimeSpan.FromSeconds(0.5));
	await connection.SendAsync(new ArraySegment<byte>(message), WebSocketMessageType.Text, true, CancellationToken.None);
}