using System.Diagnostics;

namespace FileUploadApp.Services
{
    public static class NotificationServiceFactory
    {
        public static INotificationService Create()
        {
#if WINDOWS10_0_19041_0_OR_GREATER
            Debug.WriteLine("[NotificationServiceFactory] Building with WINDOWS10_0_19041_0_OR_GREATER defined -> using WindowsNotificationService.");
            return new WindowsNotificationService();
#else
            Debug.WriteLine("[NotificationServiceFactory] WINDOWS10_0_19041_0_OR_GREATER is NOT defined for this build -> using MacNotificationService (no-op on Windows). Check which TargetFramework you're running.");
            return new MacNotificationService();
#endif
        }
    }
}
