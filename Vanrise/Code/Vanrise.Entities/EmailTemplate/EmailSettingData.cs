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
    }
}