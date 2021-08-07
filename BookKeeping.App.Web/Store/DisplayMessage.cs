namespace BookKeeping.App.Web.Store
{
	public record DisplayMessage
	{
		public string Message { get; set; } = string.Empty;
		public MessageType Type { get; set; }
		public enum MessageType
		{
			Error = 0,
			Information = 1
		}

		public DisplayMessage() { }
		public DisplayMessage(MessageType type)
			=> Type = type;
		public DisplayMessage(string message, MessageType type)
		{
			Type = type;
			Message = message;
		}
	}
}
