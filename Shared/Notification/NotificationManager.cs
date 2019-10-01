namespace Shared.Notification
{
    using System.Collections.Generic;
    using RestSharp;
    using EntityActions;
    using Model;

    public class NotificationManager : INotificationManager
    {
        public NotificationManager(IEnumerable<string> adminEmails) 
        {
            this.notificationServiceUrl = @"http://localhost:12345";
            this.emailNotifyUrl = @"email/send";
            this.adminEmails = new List<string>(adminEmails);
        }

        public void EmailNotification(CommentEntityActionsArgs args)
        {
            var client = new RestClient(this.notificationServiceUrl);
            var request = new RestRequest(this.emailNotifyUrl, DataFormat.Json);
            this.GetMessageAndCaptionText(args, out string message, out string caption);

            List<string> destinations = new List<string>(adminEmails);

            if (!string.IsNullOrWhiteSpace(args.Sender.ParentAuthorEmail))
                destinations.Add(args.Sender.ParentAuthorEmail);

            SendMessageModel model = new SendMessageModel
            {
                Body = message,
                Caption = caption,
                Destinations = destinations
            };
            request.AddJsonBody(model);
            var responce = client.Get(request);
        }

        public void SmsNotification(CommentEntityActionsArgs args)
        {
            throw new System.NotImplementedException();
        }

        private void GetMessageAndCaptionText(CommentEntityActionsArgs args, out string message, out string caption)
        {
            switch (args.Type)
            {
                case EntityActionTypes.Create:
                    message = NotificationStrings.GetString(StringNames.Comment_created_text_admin);
                    caption = NotificationStrings.GetString(StringNames.Comment_created_caption_admin);
                    break;
                case EntityActionTypes.Change:
                    message = NotificationStrings.GetString(StringNames.Comment_changed_text_admin);
                    caption = NotificationStrings.GetString(StringNames.Comment_changed_caption_admin);
                    break;
                case EntityActionTypes.Remove:
                    message = NotificationStrings.GetString(StringNames.Comment_removed_text_admin);
                    caption = NotificationStrings.GetString(StringNames.Comment_removed_caption_admin);
                    break;
                default:
                    message = string.Empty;
                    caption = string.Empty;
                    break;
            }
        }

        private readonly IEnumerable<string> adminEmails;
        private readonly string notificationServiceUrl;
        private readonly string emailNotifyUrl;
    }
}