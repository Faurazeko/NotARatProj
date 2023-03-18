namespace Server.Models
{
	public class NotificationModel
	{
		public string Text { get; set; }
		public NotificationType Type { get; set; }
		public string? Url { get; set; }
	}

	public enum NotificationType
	{
		Info = 0,
		Warning,
		Danger,
		Success
	}
}
