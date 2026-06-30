using Microsoft.Toolkit.Uwp.Notifications;

namespace FileUploadApp.Services
{
    public class WindowsNotificationService : INotificationService
    {
        public void SendUploadCompleteNotification(string fileName)
        {
            new ToastContentBuilder()
                .AddText("Upload Complete")
                .AddText($"{fileName} was uploaded successfully.")
                .Show();
        }

        public void SendUploadErrorNotification(string fileName, string errorMessage)
        {
            new ToastContentBuilder()
                .AddText("Upload Failed")
                .AddText($"{fileName}: {errorMessage}")
                .Show();
        }
    }
}