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
using System.Data;


namespace Vanrise.Fzero.Services.CDRImport
{
    public partial class CDRImportService : ServiceBase
    {
        private static System.Timers.Timer aTimer;

        public CDRImportService()
        {
            InitializeComponent();
        }

        private void ErrorLog(string message)
        {
            string cs = "CDRImport Service";
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
                int SourceId = 0;
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
                        SourceId = Source.GetByEmail(message.Headers.From.Address).ID;
                        if (SourceId != 0)
                        {
                            NewEmailRecieved.SourceID = SourceId;

                            foreach (var attachment in message.FindAllAttachments())
                            {
                                string name = attachment.ContentType.Name;
                                if (name.Contains(".xml"))
                                {
                                    using (MemoryStream ms = new MemoryStream(attachment.Body))
                                    {
                                        DataSet ds = new DataSet("GeneratedCalls");
                                        ds.ReadXml(ms);

                                        DataTable dt = ds.Tables[0];

                                        List<SourceMapping> listSourceMappingXml = SourceMapping.GetSourceMappings(SourceId);

                                        int colNumberXml = 0;

                                        foreach (DataColumn DC in dt.Columns)
                                        {
                                            SourceMapping sourceMapping = listSourceMappingXml.Where(x => x.ColumnName == dt.Columns[colNumberXml].ColumnName).FirstOrDefault();

                                            if (sourceMapping != null)
                                            {
                                                DC.ColumnName = sourceMapping.PredefinedColumn.Name;
                                            }
                                            else
                                            {
                                                DC.ColumnName = DC.ColumnName + " : UnMapped";
                                            }

                                            colNumberXml++;
                                        }

                                        if (dt != null)
                                            GeneratedCall.Confirm(SourceId, dt, null);
                                    }
                                }
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









