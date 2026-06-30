namespace FileUploadApp.Services
{
    public static class NotificationServiceFactory
    {
        public static INotificationService Create()
        {
#if WINDOWS10_0_19041_0_OR_GREATER
            return new WindowsNotificationService();
#else
            return new MacNotificationService();
#endif
        }
    }
}