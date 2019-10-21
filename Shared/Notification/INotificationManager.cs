namespace Shared.Notification
{
    using EntityActions;

    public interface INotificationManager
    {
        void EmailNotification(CommentEntityActionsArgs args);
        void SmsNotification(CommentEntityActionsArgs args);
    }
}