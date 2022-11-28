using System;

namespace Common;

public sealed class BinaryMessage : IWsMessage
{
	public ArraySegment<byte> Data { get; }

	public BinaryMessage(ArraySegment<byte> data)
	{
		Data = data;
	}
}