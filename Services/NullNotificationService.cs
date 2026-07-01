namespace FileUploadApp.Services
{
	public class NullNotificationService : INotificationService
	{
		public void SendUploadCompleteNotification(string fileName)
		{
			// No notification support for this platform.
		}

		public void SendUploadErrorNotification(string fileName, string errorMessage)
		{
			// No notification support for this platform.
		}
	}
}