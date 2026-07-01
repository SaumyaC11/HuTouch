using Microsoft.Toolkit.Uwp.Notifications;
using System;
using System.Diagnostics;
//Toast notification
namespace FileUploadApp.Services
{
    public class WindowsNotificationService : INotificationService
    {
        public void SendUploadCompleteNotification(string fileName)
        {
            try
            {
                new ToastContentBuilder()
                    .AddText("Upload Complete")
                    .AddText($"{fileName} was uploaded successfully.")
                    .Show();
            }
            catch
            {
                // Notification failure should not crash the upload flow.

            }
        }

        public void SendUploadErrorNotification(string fileName, string errorMessage)
        {
            try
            {
                new ToastContentBuilder()
                    .AddText("Upload Failed")
                    .AddText($"{fileName}: {errorMessage}")
                    .Show();
            }
            catch
            {
                // Notification failure should not crash the upload flow.
            }
        }
    }
}