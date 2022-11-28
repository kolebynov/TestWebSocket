using System.Net.WebSockets;

namespace Common;

public sealed class CloseMessage : IWsMessage
{
	public WebSocketCloseStatus? CloseStatus { get; }

	public string? CloseDescription { get; }

	public CloseMessage(WebSocketCloseStatus? closeStatus, string? closeDescription)
	{
		CloseStatus = closeStatus;
		CloseDescription = closeDescription;
	}
}