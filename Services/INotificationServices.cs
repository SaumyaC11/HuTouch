namespace FileUploadApp.Services
{

	public interface INotificationService
{
	void SendUploadCompleteNotification(string fileName);
	void SendUploadErrorNotification(string fileName, string errorMessage);
}
}