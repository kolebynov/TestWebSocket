namespace Common;

public sealed class TextMessage : IWsMessage
{
	public string Text { get; }

	public TextMessage(string text)
	{
		Text = text;
	}
}