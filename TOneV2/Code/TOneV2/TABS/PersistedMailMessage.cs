using System;

namespace TABS
{
    public class PersistedMailMessage
    {
        public int ID { get; set; }
        public int MessageID { get; set; }
        public string Subject { get; set; }
        public string From { get; set; }
        public string To { get; set; }
        public string Body { get; set; }
        public DateTime DeliveryDate { get; set; }
        public string CC { get; set; }
        public string BCC { get; set; }
        public byte[] Attachment { get; set; }
        public string AttachmentFileName { get; set; }
        public string ContentType { get; set; }
        public string ContentDisposition { get; set; }
        public bool Processed { get; set; }
        

        public PersistedMailMessage()
        {

        }
    }
}
