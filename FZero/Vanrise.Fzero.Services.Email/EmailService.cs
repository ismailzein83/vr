using System;
using System.Collections.Generic;
using System.Configuration;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.ServiceProcess;
using System.Timers;
using System.Web;
using OpenPop.Mime;
using OpenPop.Pop3;
using Vanrise.Fzero.Bypass;


namespace Vanrise.Fzero.Services.Email
{
    public partial class EmailService : ServiceBase
    {
        private static System.Timers.Timer aTimer;

        public EmailService()
        {
            InitializeComponent();
        }

        private void ErrorLog(string message)
        {
            string cs = "Email Service";
            EventLog elog = new EventLog();
            if (!EventLog.SourceExists(cs))
            {
                EventLog.CreateEventSource(cs, cs);
            }
            elog.Source = cs;
            elog.EnableRaisingEvents = true;
            elog.WriteEntry(message);
        }

        protected override void OnStart(string[] args)
        {
            base.RequestAdditionalTime(60000); // 10 minutes timeout for startup
            aTimer = new System.Timers.Timer(300000);// 5 minutes
            aTimer.Elapsed += new ElapsedEventHandler(OnTimedEvent);
            aTimer.Interval = 300000;// 5 minutes
            aTimer.Enabled = true;
            GC.KeepAlive(aTimer);
            OnTimedEvent(null, null);
        }

        private void OnTimedEvent(object source, ElapsedEventArgs e)
        {
            RecievedEmail NewEmailRecieved = new RecievedEmail();
          
            try
            {
                RecievedEmail LastEmailRecieved = null;
                int SourceID = 0;
                Pop3Client pop3Client;
                pop3Client = new Pop3Client();
                pop3Client.Connect(ConfigurationManager.AppSettings["HostName"], int.Parse(ConfigurationManager.AppSettings["Port"]), bool.Parse(ConfigurationManager.AppSettings["EnableSSL"]));
                pop3Client.Authenticate(ConfigurationManager.AppSettings["ListentoEmail"], ConfigurationManager.AppSettings["EmailPassword"], AuthenticationMethod.TryBoth);
                if (pop3Client.Connected)
                {
                    int count = pop3Client.GetMessageCount();
                    for (int i = 1; i <= count; i++)
                    {
                        Message message = pop3Client.GetMessage(i);

                        NewEmailRecieved.DateSent = message.Headers.DateSent;
                        NewEmailRecieved.MessageID = message.Headers.MessageId;
                        NewEmailRecieved.ReadDate = DateTime.Now;
                        SourceID = Source.GetByEmail(message.Headers.From.Address).ID;
                        if (SourceID != 0)
                        {
                            DateTime? LastDateTime = null;
                            LastEmailRecieved = RecievedEmail.GetLast(SourceID);
                            if (LastEmailRecieved != null)
                            {
                                LastDateTime = LastEmailRecieved.DateSent;
                            }

                            if (LastDateTime == null || message.Headers.DateSent > LastEmailRecieved.DateSent)
                            {
                                NewEmailRecieved.SourceID = SourceID;
                                List<MessagePart> Attachments = message.FindAllAttachments();
                                foreach (MessagePart attachment in Attachments)
                                {
                                    if (attachment.ContentType.Name.Contains(".xml") || attachment.ContentType.Name.Contains(".xls") || attachment.ContentType.Name.Contains(".xslx"))
                                    {
                                        File.WriteAllBytes(ConfigurationManager.AppSettings["UploadPath"] + "\\" + Source.GetByEmail(message.Headers.From.Address).Name + "\\" + message.Headers.MessageId + "_" + attachment.FileName, attachment.Body); //overwrites MessagePart.Body with attachment 
                                    }
                                }
                            }
                            else
                            {
                                break;
                            }
                        }

                    }
                    RecievedEmail.Save(NewEmailRecieved);
                    pop3Client.Disconnect();
                    pop3Client.Dispose();
                }
            }
            catch (Exception ex)
            {
                ErrorLog("10.1: " + ex.Message);
                ErrorLog("10.2: " + ex.InnerException);
                ErrorLog("10.3: " + ex.Data);
                ErrorLog("10.4: " + ex.ToString());
                RecievedEmail.Save(NewEmailRecieved);
            }
        }






        private void eventLog1_EntryWritten(object sender, EntryWrittenEventArgs e)
        {

        }

    }

    [Serializable]
    public class Email
    {
        public Email()
        {
            this.Attachments = new List<Attachment>();
        }
        public int MessageNumber { get; set; }
        public string From { get; set; }
        public string Subject { get; set; }
        public string Body { get; set; }
        public DateTime DateSent { get; set; }
        public List<Attachment> Attachments { get; set; }
    }
    [Serializable]
    public class Attachment
    {
        public string FileName { get; set; }
        public string ContentType { get; set; }
        public byte[] Content { get; set; }
    }
}









