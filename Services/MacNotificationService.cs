using System;
using System.Diagnostics;
using System.Runtime.InteropServices;

namespace FileUploadApp.Services
{
    public class MacNotificationService : INotificationService
    {
        public void SendUploadCompleteNotification(string fileName)
        {
            SendNotification(
                title: "Upload Complete",
                message: $"{fileName} was uploaded successfully."
            );
        }

        public void SendUploadErrorNotification(string fileName, string errorMessage)
        {
            SendNotification(
                title: "Upload Failed",
                message: $"{fileName}: {errorMessage}"
            );
        }

        private static void SendNotification(string title, string message)
        {
            if (!RuntimeInformation.IsOSPlatform(OSPlatform.OSX))
                return;

            try
            {
                string script =
                    $"display notification \"{EscapeAppleScript(message)}\" " +
                    $"with title \"{EscapeAppleScript(title)}\"";

                var processInfo = new ProcessStartInfo
                {
                    FileName = "/usr/bin/osascript",
                    Arguments = $"-e \"{script}\"",
                    UseShellExecute = false,
                    CreateNoWindow = true
                };

                using var process = Process.Start(processInfo);
            }
            catch
            {
                // Do not crash the app if notification fails.
                // Upload logic should still complete normally.
            }
        }

        private static string EscapeAppleScript(string value)
        {
            return value
                .Replace("\\", "\\\\")
                .Replace("\"", "\\\"");
        }
    }
}