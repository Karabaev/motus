namespace NotificationService.Common
{
    public class Configuration
    {
        public string SmtpHost { get; set; }
        public int SmtpPort { get; set; }
        public string SenderEmail { get; set; }
        public string SenderPublicName { get; set; }
        public string Login { get; set; }
        public string Password { get; set; }
        public bool UseSsl { get; set; }
    }
}
