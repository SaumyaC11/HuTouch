namespace FileUploadApp.Services
{
    public class MacNotificationService : INotificationService
    {
        public void SendUploadCompleteNotification(string fileName)
        {
            // macOS notification via NSUserNotification / UNUserNotificationCenter
            // To be implemented when testing on macOS.
        }

        public void SendUploadErrorNotification(string fileName, string errorMessage)
        {
            // macOS notification via NSUserNotification / UNUserNotificationCenter
        }
    }
}