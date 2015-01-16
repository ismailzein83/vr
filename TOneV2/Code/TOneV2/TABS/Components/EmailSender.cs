using System;
using System.Collections.Generic;
using System.Net.Mail;
using System.Linq;

namespace TABS.Components
{
    public class EmailSender
    {
        static log4net.ILog log = log4net.LogManager.GetLogger(typeof(EmailSender));

        SpecialSystemParameters.SmtpInfo info = new TABS.SpecialSystemParameters.SmtpInfo();

        protected EmailSender()
        {
        }

        protected void SendMessage(MailMessage message)
        {
            message = ToActiveUser(message);
            System.Net.Mail.SmtpClient client = new SmtpClient(info.Host, info.Port);

            client.Timeout = (int)TABS.SystemParameter.SMTP_Timeout.NumericValue * 60 * 1000;

            if (!string.IsNullOrEmpty(info.User))
            {
                client.UseDefaultCredentials = false;
                client.Credentials = new System.Net.NetworkCredential(info.User, info.Password);
            }
            client.EnableSsl = info.EnableSsl;

            if (TABS.SystemParameter.SMTP_GetDirectoryFromIIS.BooleanValue.Value)
                client.DeliveryMethod = SmtpDeliveryMethod.PickupDirectoryFromIis;

            if (message.From == null) message.From = new MailAddress(info.Default_From);

            message.IsBodyHtml = true;

            client.Send(message);
        }

        public static bool Send(MailMessage message, out Exception ex)
        {
            ex = null;
            try
            {
                EmailSender sender = new EmailSender();
                sender.SendMessage(message);
                return true;
            }
            catch (Exception e)
            {
                ex = e;
                log.Error("Error Sending Mail: " + message, ex);
                return false;
            }
        }

        private MailMessage ToActiveUser(MailMessage msg)
        {
            Dictionary<string, bool> Users = new Dictionary<string, bool>();
            foreach (var item in TABS.Security.User.All)
                if (!string.IsNullOrEmpty(item.Login))
                    Users[item.Login] = item.IsActive;

            List<MailAddress> validMails = new List<MailAddress>();

            try
            {
                foreach (MailAddress mail in msg.To)
                {
                    if (Users.ContainsKey(mail.Address.Trim()) && !Users[mail.Address.Trim()])
                    { }
                    else
                        validMails.Add(mail);

                }

                for (int i = 0; i < msg.To.Count - 1; i++)
                    msg.To.RemoveAt(i);

                foreach (var item in validMails)
                {
                    if (!msg.To.Any(i => i.Address.Trim().Equals(item.Address.Trim())))
                        msg.To.Add(item);
                }

                if (msg.To.Count == 0)
                    log.ErrorFormat("All recipients are inactive users please consider to change the recipients addresses");
            }
            catch (Exception ex)
            {
                log.Warn("Email have not been sent: " + msg, ex);
            }
            return msg;
        }
    }
}
