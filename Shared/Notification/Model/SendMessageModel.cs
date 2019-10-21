namespace Shared.Notification.Model
{
    using System.Collections.Generic;

    public class SendMessageModel
    {
        public IEnumerable<string> Destinations { get; set; }
        public string Caption { get; set; }
        public string Body { get; set; }
    }
}