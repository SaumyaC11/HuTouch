using Microsoft.Toolkit.Uwp.Notifications;
using System;
using System.Diagnostics;
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
            catch (Exception ex)
            {
                // Toast failures were previously silent. Log so they're visible during diagnosis.
                Debug.WriteLine($"[WindowsNotificationService] Failed to show completion toast: {ex}");
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
            catch (Exception ex)
            {
                Debug.WriteLine($"[WindowsNotificationService] Failed to show error toast: {ex}");
            }
        }
    }
}