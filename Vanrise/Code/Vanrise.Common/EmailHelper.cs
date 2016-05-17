using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Mail;
using System.Text;
using System.Threading.Tasks;

namespace Vanrise.Common
{
    public class EmailHelper
    {
        private string Receiver { get; set; }
        private string Content { get; set; }
        private string Subject { get; set; }
        
        public EmailHelper(string to, string content, string subject)
        {
            Receiver = to;
            Content = content;
            Subject = subject;
        }

        public void Send()
        {
            if (false)
            {
                MailMessage mailMessage = new MailMessage();
                mailMessage.From = new MailAddress("testemail.vr@gmail.com");
                mailMessage.To.Add(new MailAddress(Receiver));
                mailMessage.Subject = Subject;
                mailMessage.Body = Content;
                mailMessage.IsBodyHtml = true;


                SmtpClient client = new SmtpClient();
                client.Port = 587;
                client.Host = "smtp.gmail.com";
                client.Timeout = 10000;
                client.DeliveryMethod = SmtpDeliveryMethod.Network;
                client.UseDefaultCredentials = false;
                client.Credentials = new System.Net.NetworkCredential("testemail.vr@gmail.com", "gmer2juIda");
                client.EnableSsl = true;

                client.Send(mailMessage);
            }
        }
    }
}
