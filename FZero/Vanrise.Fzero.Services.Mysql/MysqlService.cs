using System;
using System.Collections.Generic;
using System.Configuration;
using System.Diagnostics;
using System.Linq;
using System.Net;
using System.Net.Mail;
using System.Net.Mime;
using System.ServiceProcess;
using System.Timers;
using System.Xml.Linq;
using MySql.Data.MySqlClient;
using Vanrise.CommonLibrary;


namespace Vanrise.Fzero.Services.Mysql
{
    public partial class MysqlService : ServiceBase
    {
        private static System.Timers.Timer aTimer;

        public MysqlService()
        {
            try
            {
                InitializeComponent();
            }
            catch (Exception ex)
            {
                ErrorLog("MysqlService() " + ex.Message);
            }

        }

        private void ErrorLog(string message)
        {
            string cs = "GOIPtoFMS";
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

            try
            {
                base.RequestAdditionalTime(int.Parse(System.Configuration.ConfigurationManager.AppSettings["RequestAdditionalTime"].ToString()));
                //Debugger.Launch(); // launch and attach debugger

                // Create a timer with a ten second interval.
                aTimer = new System.Timers.Timer(int.Parse(System.Configuration.ConfigurationManager.AppSettings["TimerInterval"].ToString()));// 60 minutes
                // Hook up the Elapsed event for the timer.
                aTimer.Elapsed += new ElapsedEventHandler(OnTimedEvent);
                aTimer.Interval = int.Parse(System.Configuration.ConfigurationManager.AppSettings["TimerInterval"].ToString());// 60 minutes
                aTimer.Enabled = true;
                GC.KeepAlive(aTimer);
                OnTimedEvent(null, null);
            }
            catch (Exception ex)
            {
                ErrorLog("OnStart() " + ex.Message);
                ErrorLog("OnStart() " + ex.ToString());
                ErrorLog("OnStart() " + ex.InnerException.ToString());
            }

        }

        private void OnTimedEvent(object source, ElapsedEventArgs e)
        {

            try
            {
                if (HttpHelper.CheckInternetConnection("smtp.gmail.com", 587))
                {
                    DBConnect db = new DBConnect();
                    List<string>[] listcalls = new List<string>[0];
                    List<string>[] listcallsfailed = new List<string>[0];

                    listcalls = db.SelectCalls(db.LastCallID());
                    listcallsfailed = db.SelectCallsFailed(db.LastCallFailedID());


                    if (listcalls[0].Count() > 0 || listcallsfailed[0].Count() > 0)
                    {
                        XElement root = new XElement("CDRs");
                        List<XElement> cdrs = new List<XElement>();

                        for (int i = 0; i < listcalls[0].Count(); i++)
                        {
                            XElement cdr;
                            List<XElement> elements = new List<XElement>();
                            elements.Add(new XElement("ID", listcalls[1][i]));
                            elements.Add(new XElement("caller_id", listcalls[2][i]));
                            elements.Add(new XElement("called_number", listcalls[3][i]));
                            elements.Add(new XElement("call_start", listcalls[4][i]));
                            elements.Add(new XElement("duration", listcalls[5][i]));
                            elements.Add(new XElement("Origination", listcalls[6][i]));
                            elements.Add(new XElement("RouteID", listcalls[7][i]));
                            elements.Add(new XElement("ClientName", listcalls[8][i]));
                            elements.Add(new XElement("Type", listcalls[9][i]));
                            cdr = new XElement("CDR", elements);
                            cdrs.Add(cdr);
                        }


                        for (int i = 0; i < listcallsfailed[0].Count(); i++)
                        {
                            XElement cdr;
                            List<XElement> elements = new List<XElement>();
                            elements.Add(new XElement("ID", listcallsfailed[1][i]));
                            elements.Add(new XElement("caller_id", listcallsfailed[2][i]));
                            elements.Add(new XElement("called_number", listcallsfailed[3][i]));
                            elements.Add(new XElement("call_start", listcallsfailed[4][i]));
                            elements.Add(new XElement("duration", listcallsfailed[5][i]));
                            elements.Add(new XElement("Origination", listcallsfailed[6][i]));
                            elements.Add(new XElement("RouteID", listcallsfailed[7][i]));
                            elements.Add(new XElement("ClientName", listcallsfailed[8][i]));
                            elements.Add(new XElement("Type", listcallsfailed[9][i]));
                            cdr = new XElement("CDR", elements);
                            cdrs.Add(cdr);
                        }



                        XElement xmlTree1 = new XElement("CDRs", cdrs);
                        Random r = new Random();
                        string Path = ConfigurationManager.AppSettings["XmlPath"] + "GOIPFile" + r.Next().ToString() + ".xml";
                        new XDocument(xmlTree1).Save(Path);

                        db.Insert(db.NewLastCallID, db.NewLastCallFailedID);
                        db.SendMail(System.Configuration.ConfigurationManager.AppSettings["ToAddress"].ToString(), "Mysql Calls", "Administrator", "Please find attached XML of Calls", Path, string.Empty, null);
                    }
                }

            }
            catch (Exception ex)
            {
                ErrorLog("OnTimedEvent() " + ex.Message);
                ErrorLog("OnTimedEvent() " + ex.ToString());
                ErrorLog("OnTimedEvent() " + ex.InnerException.ToString());
            }




        }

        private void eventLog1_EntryWritten(object sender, EntryWrittenEventArgs e)
        {

        }

    }
    class DBConnect
    {
        private MySqlConnection connection;
        private string server;
        private string database;
        private string uid;
        private string password;
        public int NewLastCallID = 0;
        public int NewLastCallFailedID = 0;

        private void ErrorLog(string message)
        {
            string cs = "GOIPtoFMS";
            EventLog elog = new EventLog();
            if (!EventLog.SourceExists(cs))
            {
                EventLog.CreateEventSource(cs, cs);
            }
            elog.Source = cs;
            elog.EnableRaisingEvents = true;
            elog.WriteEntry(message);
        }

        //Constructor
        public DBConnect()
        {
            try
            {
                Initialize();
            }
            catch (Exception ex)
            {
                ErrorLog("DBConnect() " + ex.Message);
                ErrorLog("DBConnect() " + ex.ToString());
                ErrorLog("DBConnect() " + ex.InnerException.ToString());
            }

        }

        //Initialize values
        private void Initialize()
        {
            try
            {
                server = System.Configuration.ConfigurationManager.AppSettings["SERVER"].ToString();
                database = System.Configuration.ConfigurationManager.AppSettings["DATABASE"].ToString();
                uid = System.Configuration.ConfigurationManager.AppSettings["UID"].ToString();
                password = System.Configuration.ConfigurationManager.AppSettings["PASSWORD"].ToString();
                string connectionString;
                connectionString = "SERVER=" + server + ";" + "DATABASE=" +
                database + ";" + "UID=" + uid + ";" + "PASSWORD=" + password + ";";

                connection = new MySqlConnection(connectionString);
            }
            catch (Exception ex)
            {
                ErrorLog("Initialize() " + ex.Message);
                ErrorLog("Initialize() " + ex.ToString());
                ErrorLog("Initialize() " + ex.InnerException.ToString());
            }

        }

        //open connection to database
        private bool OpenConnection()
        {
            try
            {
                if (connection.State != System.Data.ConnectionState.Open)
                    connection.Open();
                return true;
            }
            catch (MySqlException ex)
            {
                ErrorLog("OpenConnection() " + ex.Message);
                ErrorLog("OpenConnection() " + ex.ToString());
                ErrorLog("OpenConnection() " + ex.InnerException.ToString());
                return false;
            }
        }

        //Close connection
        private bool CloseConnection()
        {
            try
            {
                connection.Close();
                return true;
            }
            catch (MySqlException ex)
            {
                ErrorLog("CloseConnection() " + ex.Message);
                ErrorLog("CloseConnection() " + ex.ToString());
                ErrorLog("CloseConnection() " + ex.InnerException.ToString());
                return false;
            }
        }

        //Insert statement
        public void Insert(int NewLastCallID, int NewLastCallFailedID)
        {

            try
            {
                string query = "INSERT INTO sentcalls ( LastCallID, LastCallFailedID) VALUES( '" + NewLastCallID.ToString() + "','" + NewLastCallFailedID.ToString() + "')";

                //open connection
                if (this.OpenConnection() == true)
                {
                    //create command and assign the query and connection from the constructor
                    MySqlCommand cmd = new MySqlCommand(query, connection);
                    //Execute command
                    cmd.ExecuteNonQuery();
                    //close connection
                    this.CloseConnection();
                }
            }
            catch (MySqlException ex)
            {
                ErrorLog("Insert() " + ex.Message);
                ErrorLog("Insert() " + ex.ToString());
                ErrorLog("Insert() " + ex.InnerException.ToString());
            }


        }


        //Select statement
        public List<string>[] SelectCalls(int LastCallID)
        {
            NewLastCallID = LastCallID;
            string querycalls = "SELECT   a.id_call as ID,  IFNULL(c.Name,'') as ClientName,  a.id_call, a.caller_id, SUBSTRING(a.called_number, 7) as called_number, a.call_start, a.duration , IFNULL(b.Origination,'') as Origination ,    IFNULL(b.Carrier,'') as RouteID,    IFNULL(b.Type,'SIP') as Type    FROM         calls as a left outer join CarrierPrefixes as b on    Left(a.called_number, 6) = b.Prefix      left outer join clients as c on    a.id_Client = c.ID where id_call>" + LastCallID.ToString();

            //Create a list to store the result
            List<string>[] listcalls = new List<string>[10];
            listcalls[0] = new List<string>();
            listcalls[1] = new List<string>();
            listcalls[2] = new List<string>();
            listcalls[3] = new List<string>();
            listcalls[4] = new List<string>();
            listcalls[5] = new List<string>();
            listcalls[6] = new List<string>();
            listcalls[7] = new List<string>();
            listcalls[8] = new List<string>();
            listcalls[9] = new List<string>();

            //Open connection
            if (this.OpenConnection() == true)
            {
                //Create Command
                MySqlCommand cmd = new MySqlCommand(querycalls, connection);
                cmd.CommandTimeout = 600000;
                //Create a data reader and Execute the command
                MySqlDataReader dataReader = cmd.ExecuteReader();


                //Read the data and store them in the list
                while (dataReader.Read())
                {
                    listcalls[0].Add(dataReader["ID"] + "");
                    listcalls[1].Add(dataReader["id_call"] + "");
                    listcalls[2].Add(dataReader["caller_id"] + "");
                    listcalls[3].Add(dataReader["called_number"] + "");
                    listcalls[4].Add(dataReader["call_start"] + "");
                    listcalls[5].Add(dataReader["duration"] + "");
                    listcalls[6].Add(dataReader["Origination"] + "");
                    listcalls[7].Add(dataReader["RouteID"] + "");
                    listcalls[8].Add(dataReader["ClientName"] + "");
                    listcalls[9].Add(dataReader["Type"] + "");
                }
                if (listcalls[0].LastOrDefault() != null)
                    NewLastCallID = int.Parse(listcalls[0].Last());
                //close Data Reader
                dataReader.Close();




                //close Connection
                this.CloseConnection();

                //return list to be displayed
                return listcalls;
            }
            else
            {
                return listcalls;
            }
        }


        //Select statement
        public List<string>[] SelectCallsFailed(int LastCallFailedID)
        {
            NewLastCallFailedID = LastCallFailedID;
            string querycallsfailed = "SELECT     a.id_failed_call as ID, IFNULL(c.Name,'') as ClientName, a.id_failed_call, a.caller_id, SUBSTRING(a.called_number, 7) as called_number, a.call_start, 0 as duration,  IFNULL(b.Origination,'') as Origination ,    IFNULL(b.Carrier,'') as RouteID,    IFNULL(b.Type,'SIP') as Type    FROM         callsfailed as a left outer join CarrierPrefixes as b on    Left(a.called_number, 6) = b.Prefix       left outer join clients as c on    a.id_Client = c.ID  where id_failed_call>" + LastCallFailedID.ToString();

            //Create a list to store the result
            List<string>[] list_failedcalls = new List<string>[10];
            list_failedcalls[0] = new List<string>();
            list_failedcalls[1] = new List<string>();
            list_failedcalls[2] = new List<string>();
            list_failedcalls[3] = new List<string>();
            list_failedcalls[4] = new List<string>();
            list_failedcalls[5] = new List<string>();
            list_failedcalls[6] = new List<string>();
            list_failedcalls[7] = new List<string>();
            list_failedcalls[8] = new List<string>();
            list_failedcalls[9] = new List<string>();

            //Open connection
            if (this.OpenConnection() == true)
            {
                //Create Command
                MySqlCommand cmdfailed = new MySqlCommand(querycallsfailed, connection);
                cmdfailed.CommandTimeout = 600000;
                //Create a data reader and Execute the command
                MySqlDataReader dataReaderfailed = cmdfailed.ExecuteReader();
                //Read the data and store them in the list
                while (dataReaderfailed.Read())
                {
                    list_failedcalls[0].Add(dataReaderfailed["ID"] + "");
                    list_failedcalls[1].Add(dataReaderfailed["id_failed_call"] + "");
                    list_failedcalls[2].Add(dataReaderfailed["caller_id"] + "");
                    list_failedcalls[3].Add(dataReaderfailed["called_number"] + "");
                    list_failedcalls[4].Add(dataReaderfailed["call_start"] + "");
                    list_failedcalls[5].Add(dataReaderfailed["duration"] + "");
                    list_failedcalls[6].Add(dataReaderfailed["Origination"] + "");
                    list_failedcalls[7].Add(dataReaderfailed["RouteID"] + "");
                    list_failedcalls[8].Add(dataReaderfailed["ClientName"] + "");
                    list_failedcalls[9].Add(dataReaderfailed["Type"] + "");
                }
                if (list_failedcalls[0].LastOrDefault() != null)
                    NewLastCallFailedID = int.Parse(list_failedcalls[0].Last());
                //close Data Reader
                dataReaderfailed.Close();

                //close Connection
                this.CloseConnection();

                //return list to be displayed
                return list_failedcalls;
            }
            else
            {
                return list_failedcalls;
            }
        }

        //Last statement
        public int LastCallID()
        {
            string query = "SELECT LastCallID FROM sentcalls ORDER BY ID DESC Limit 1";
            int LastCallID = 0;

            //Open connection
            if (this.OpenConnection() == true)
            {
                //Create Command
                MySqlCommand cmd = new MySqlCommand(query, connection);
                //Create a data reader and Execute the command
                MySqlDataReader dataReader = cmd.ExecuteReader();
                //Read the data and store them in the list
                if (dataReader.Read())
                {
                    LastCallID = int.Parse(dataReader["LastCallID"].ToString());
                }

                //close Data Reader
                dataReader.Close();

                //close Connection
                this.CloseConnection();

                return LastCallID;

            }
            else
            {
                return 0;
            }
        }

        //Last statement
        public int LastCallFailedID()
        {
            string query = "SELECT LastCallFailedID FROM sentcalls ORDER BY ID DESC Limit 1";
            int LastCallFailedID = 0;

            //Open connection
            if (this.OpenConnection() == true)
            {
                //Create Command
                MySqlCommand cmd = new MySqlCommand(query, connection);
                //Create a data reader and Execute the command
                MySqlDataReader dataReader = cmd.ExecuteReader();
                //Read the data and store them in the list
                if (dataReader.Read())
                {
                    LastCallFailedID = int.Parse(dataReader["LastCallFailedID"].ToString());
                }


                //close Data Reader
                dataReader.Close();

                //close Connection
                this.CloseConnection();

                return LastCallFailedID;

            }
            else
            {
                return 0;
            }
        }


        public void SendMail(string ToAddress, string subject, string Name, string Content, string AttachmentLink, string Url, IList<string> ToBccAddresses)
        {

            try
            {



                // Gmail Address from where you send the mail
                var fromAddress = System.Configuration.ConfigurationManager.AppSettings["FromMail"].ToString();
                // any address where the email will be sending
                var toAddress = ToAddress;
                //Password of your gmail address




                string fromPassword = System.Configuration.ConfigurationManager.AppSettings["FromMailPassword"].ToString();
                // Passing the values and make a email formate to display
                string body = "Dear " + Name + ",<br/>";
                body += "<br/>" + Content + "<br/>";

                if (Url != string.Empty)
                {
                    body += "<a href=\"" + Url + "\" target=\"_blank\">Check it!</a> ";
                }


                string htmlBody = "<html><body><table style=\"border: 1px solid #9FB2C7;  width:650px\"> <tr> <td bgcolor=\"#E2EBF5\"  style=\"height: 40px; padding:3px 3px 3px 3px;   color: white;  font-size: xx-large;\"   ><img src=\"cid:Pic1\"></td> </tr>  <tr> <td   bgcolor=\"#FFFFFF\" style=\"height: 15px;  padding:8px 8px 8px 8px;   color: #25313F;  font-size: 13px;\"  >" + body + "</td>   </tr>  <tr>    <td bgcolor=\"#9FB2C7\" style=\"height: 20px;  padding:2px 2px 2px 2px; color: white;  font-size: 12px;\">Fzero Copyright &#9400;</td>   </tr>   </table></body></html>";
                //string htmlBody = "<html><body><h1>Picture</h1><br><img src=\"cid:Pic1\"></body></html>";
                AlternateView avHtml = AlternateView.CreateAlternateViewFromString
                    (htmlBody, null, MediaTypeNames.Text.Html);


                System.Net.Mime.ContentType mimeType = new System.Net.Mime.ContentType("text/html");

                // smtp settings
                var smtp = new System.Net.Mail.SmtpClient();
                {
                    smtp.Host = System.Configuration.ConfigurationManager.AppSettings["host"].ToString();
                    smtp.Port = int.Parse(System.Configuration.ConfigurationManager.AppSettings["port"]);
                    smtp.EnableSsl = bool.Parse(System.Configuration.ConfigurationManager.AppSettings["EnableSsl"].ToString());
                    smtp.DeliveryMethod = System.Net.Mail.SmtpDeliveryMethod.Network;



                    NetworkCredential networkCredential = new NetworkCredential();
                    networkCredential.UserName = fromAddress;
                    networkCredential.Password = fromPassword;
                    //networkCredential.Domain = fromAddress;

                    smtp.Credentials = networkCredential;
                    smtp.Timeout = 20000000;

                }


                MailMessage mailMessage = new MailMessage();
                mailMessage.From = new MailAddress(fromAddress);

                if (toAddress != null)
                {
                    mailMessage.To.Add(new MailAddress(toAddress));
                }






                if (ToBccAddresses != null)
                {
                    foreach (string bcc in ToBccAddresses)
                    {
                        mailMessage.Bcc.Add(new MailAddress(bcc));
                    }
                }



                mailMessage.Subject = subject;
                mailMessage.Body = htmlBody;
                mailMessage.Priority = MailPriority.High;
                //mailMessage.IsBodyHtml = true;



                mailMessage.AlternateViews.Add(avHtml);



                if (AttachmentLink != string.Empty)
                {
                    mailMessage.Attachments.Add(new System.Net.Mail.Attachment(AttachmentLink));
                }


                // Passing values to smtp object
                //smtp.SendAsync(mailMessage,null);

                if (mailMessage.To.Count > 0 || mailMessage.Bcc.Count > 0)
                {
                    smtp.Send(mailMessage);
                }

            }
            catch (Exception ex)
            {
                ErrorLog("SendMail " + ex.Message);
                ErrorLog("SendMail " + ex.InnerException.ToString());
                ErrorLog("SendMail " + ex.ToString());
            }




        }

    }

}









