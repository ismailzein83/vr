using System;
using System.Collections.Generic;
using System.Configuration;
using System.Diagnostics;
using System.Linq;
using System.Net;
using System.Net.Mail;
using System.Net.Mime;
using System.Timers;
using System.Xml.Linq;
using MySql.Data.MySqlClient;
using MySql.Data;





public partial class _Default : System.Web.UI.Page
{
    protected void Page_Load(object sender, EventArgs e)
    {
        try
        {
            DBConnect db = new DBConnect();
            db.Load();

        }
        catch (Exception ex)
        {
           
        }
    }


     private static System.Timers.Timer aTimer;

     

        
       

    
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
           
            return false;
        }
    }


    //Insert statement
    public void Load()
    {

        try
        {
            //string query = "LOAD DATA INFILE './x1.DAT' INTO TABLE filesDAT CHARACTER SET UTF8  LINES TERMINATED BY '\n'";
            string query = "LOAD DATA INFILE './x1.DAT' INTO TABLE filesDAT CHARACTER SET UTF8  LINES TERMINATED BY '\n';del './x1.DAT';";

            //open connection
            if (this.OpenConnection() == true)
            {
                //create command and assign the query and connection from the constructor
                MySqlCommand cmd = new MySqlCommand(query, connection);
                cmd.CommandTimeout = 0;
                //Execute command
                cmd.ExecuteNonQuery();
                //close connection
                this.CloseConnection();
            }
        }
        catch (MySqlException ex)
        {
         
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
          
        }




    }

}