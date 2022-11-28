using System;
using System.Net.WebSockets;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace Common;

public static class WebSocketExtensions
{
	public static async Task<IWsMessage> ReceiveMessageAsync(this WebSocket webSocket)
	{
		var resultArray = new byte[256];
		var result = await webSocket.ReceiveAsync(new ArraySegment<byte>(resultArray), CancellationToken.None);
		return result.MessageType switch
		{
			WebSocketMessageType.Binary => new BinaryMessage(await ReadMessageDataToEnd(webSocket, result, resultArray)),
			WebSocketMessageType.Text => new TextMessage(GetString(await ReadMessageDataToEnd(webSocket, result, resultArray))),
			WebSocketMessageType.Close => new CloseMessage(result.CloseStatus, result.CloseStatusDescription),
			_ => throw new InvalidOperationException("Invalid message type"),
		};
	}

	private static async Task<ArraySegment<byte>> ReadMessageDataToEnd(WebSocket webSocket, WebSocketReceiveResult result, byte[] resultArray)
	{
		var readCount = result.Count;
		while (!result.EndOfMessage)
		{
			if (resultArray.Length <= readCount)
			{
				var newResultArray = new byte[resultArray.Length * 2];
				resultArray.CopyTo(newResultArray, 0);
				resultArray = newResultArray;
			}

			result = await webSocket.ReceiveAsync(
				new ArraySegment<byte>(resultArray, readCount, resultArray.Length - readCount),
				CancellationToken.None);
			readCount += result.Count;
		}

		return new ArraySegment<byte>(resultArray, 0, readCount);
	}

	private static string GetString(ArraySegment<byte> arraySegment) =>
		Encoding.UTF8.GetString(arraySegment.Array, arraySegment.Offset, arraySegment.Count);
}