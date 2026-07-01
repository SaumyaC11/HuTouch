using System.Runtime.InteropServices;

namespace FileUploadApp.Services
{
    public static class NotificationServiceFactory
    {
        public static INotificationService Create()
        {
            if (RuntimeInformation.IsOSPlatform(OSPlatform.Windows))
            {
#if WINDOWS10_0_19041_0_OR_GREATER
                return new WindowsNotificationService();
#else
                return new NullNotificationService();
#endif
            }

            if (RuntimeInformation.IsOSPlatform(OSPlatform.OSX))
            {
                return new MacNotificationService();
            }

            return new NullNotificationService();
        }
    }
}