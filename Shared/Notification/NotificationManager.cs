namespace Shared.Notification
{
    using System.Collections.Generic;
    using RestSharp;
    using EntityActions;
    using Model;
    using System.Net;
    using NLog;
    using System;
    using Newtonsoft.Json.Linq;

    public class NotificationManager : INotificationManager
    {
        public NotificationManager(IEnumerable<string> adminEmails)
        {
            this.notificationServiceUrl = @"http://localhost:81";
            this.emailNotifyUrl = @"email/send";
            this.adminEmails = new List<string>(adminEmails);
        }

        public void EmailNotification(CommentEntityActionsArgs args)
        {
            var client = new RestClient(this.notificationServiceUrl);
            var request = new RestRequest(this.emailNotifyUrl, Method.POST, DataFormat.Json);
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

            try
            {
                client.ExecuteAsync(request, responce =>
                {
                    if (responce.StatusCode == HttpStatusCode.OK)
                    {
                        var jArr = JArray.Parse(responce.Content);
                        string result = jArr["success"].Value<string>();

                        if(!string.IsNullOrWhiteSpace(result))
                        {
                            this.logger.Info("Оповещения о добавлении комментария успешно отправлены.");
                            return;
                        }
                            
                        result = jArr["error"].Value<string>();

                        if (!string.IsNullOrWhiteSpace(result))
                        {
                            this.logger.Warn("Оповещения о добавлении комментария успешно отправлены.");
                            return;
                        }
                    }
                    else
                    {
                        this.logger.Error($"{responce.StatusCode} {responce.StatusDescription} Ошибка при запросе к сервису оповещений");
                    }
                });
            }
            catch (Exception ex)
            {
                this.logger.Error(ex, "Ошибка при запросе к сервису оповещений");
            }

        }

        public void SmsNotification(CommentEntityActionsArgs args)
        {
            throw new System.NotImplementedException();
        }

        private void GetMessageAndCaptionText(CommentEntityActionsArgs args, out string message, out string caption)
        {
            string userName = string.IsNullOrWhiteSpace(args.Sender.AuthorName) ? "Юзер" : args.Sender.AuthorName;

            switch (args.Type)
            {
                case EntityActionTypes.Create:
                    message = $"{userName} ({args.Sender.AuthorID}) оставил новый комментарий: \"{args.Sender.Text}\" к видеоматериалу {args.Sender.MaterialID}";
                    caption = NotificationStrings.GetString(StringNames.Comment_created_caption_admin);
                    break;
                case EntityActionTypes.Change:
                    message = $"{userName} ({args.Sender.AuthorID}) изменил текст комментария на: \"{args.Sender.Text}\" к видеоматериалу {args.Sender.MaterialID}";
                    caption = NotificationStrings.GetString(StringNames.Comment_changed_caption_admin);
                    break;
                case EntityActionTypes.Remove:
                    message = $"{userName} ({args.Sender.AuthorID}) удалил свой комментарий: \"{args.Sender.Text}\" из видеоматериала {args.Sender.MaterialID}";
                    caption = NotificationStrings.GetString(StringNames.Comment_removed_caption_admin);
                    break;
                default:
                    message = string.Empty;
                    caption = string.Empty;
                    break;
            }
        }

        private readonly IEnumerable<string> adminEmails;
        private readonly Logger logger = LogManager.GetCurrentClassLogger();
        private readonly string notificationServiceUrl;
        private readonly string emailNotifyUrl;
    }
}