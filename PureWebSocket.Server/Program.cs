using System;
using System.Net;
using System.Net.WebSockets;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Common;

var prefix = "http://localhost:8080/";

using var httpListener = new HttpListener();
httpListener.Prefixes.Add(prefix);
httpListener.Start();

Console.WriteLine($"Server started at: {prefix}");

while (true)
{
	var context = await httpListener.GetContextAsync();
	if (context.Request.Url.PathAndQuery.Equals("/ws", StringComparison.OrdinalIgnoreCase))
	{
		HandleWebSocketConnection(await context.AcceptWebSocketAsync(null, 4096, TimeSpan.FromSeconds(30)));
	}
	else
	{
		context.Response.StatusCode = 200;
		context.Response.Close();
	}
}

static async Task HandleWebSocketConnection(HttpListenerWebSocketContext webSocketContext)
{
	var message = Encoding.UTF8.GetBytes("Hello Client");
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
			Console.WriteLine($"Closing connection due to error: {finishedTask.Exception.Message}");
			await webSocketContext.WebSocket.CloseAsync(WebSocketCloseStatus.Empty, null, CancellationToken.None);
			break;
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
		var receivedMessage = await webSocketContext.WebSocket.ReceiveMessageAsync();
		if (receivedMessage is not TextMessage { Text: var text })
		{
			throw new InvalidOperationException($"Invalid message {receivedMessage}");
		}

		Console.WriteLine($"Received from client: {text}");
	}

	async Task GetSendMessageTask()
	{
		await Task.Delay(TimeSpan.FromSeconds(0.5));
		await webSocketContext.WebSocket.SendAsync(new ArraySegment<byte>(message), WebSocketMessageType.Text, true, CancellationToken.None);
	}
}