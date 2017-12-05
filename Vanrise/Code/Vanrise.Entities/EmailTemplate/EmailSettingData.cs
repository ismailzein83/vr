namespace Vanrise.Entities
{
    public class EmailSettingData : SettingData
    {
        public string SenderEmail { get; set; }
        public string SenderPassword { get; set; }
        public string Host { get; set; }
        public int Port { get; set; }
        public int TimeoutInSeconds { get; set; }
        public bool EnabelSsl { get; set; }
        public string AlternativeSenderEmail { get; set; }
    }

    public class EmailSettingDetail
    {
        public EmailSettingData EmailSettingData { get; set; }
        public string Subject { get; set; }
        public string Body { get; set; }
        public string ToEmail { get; set; }
        public string FromEmail { get; set; }
    }
}