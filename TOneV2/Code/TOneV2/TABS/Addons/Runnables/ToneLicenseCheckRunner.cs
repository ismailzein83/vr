using System;
using System.Linq;

namespace TABS.Addons.Runnables
{
    [NamedAddon("T.One - Licensing", "Manages User Licenses.")]
    public class ToneLicenseCheckRunner : RunnableBase
    {
        public static string Warning { get; set; }
        public static string LicenceType { set; get; }
        internal static log4net.ILog log = log4net.LogManager.GetLogger(typeof(ToneLicenseCheckRunner));

        public override void Run()
        {
            TABS.SpecialSystemParameters.TOneLicenseChecker license = TABS.SpecialSystemParameters.TOneLicenseChecker.Get(TABS.SystemParameter.TOneLicenseCheckerParameter).FirstOrDefault();

            if (license.CheckLicense())
            {
                Warning = null;
                LicenceType = license.Type.ToString();
            }
            else
            {
                TABS.SpecialSystemParameters.SmtpInfo info = new TABS.SpecialSystemParameters.SmtpInfo();
                System.Net.Mail.SmtpClient smtp = new System.Net.Mail.SmtpClient(info.Host);
                smtp.Timeout = (int)TABS.SystemParameter.SMTP_Timeout.NumericValue * 60 * 1000;
                System.Net.Mail.MailMessage mail = new System.Net.Mail.MailMessage();
                mail.From = new System.Net.Mail.MailAddress(license.FromEmails);

                foreach (var mailto in license.Emails.Split(',', ';'))
                    mail.To.Add(mailto.Trim());

                mail.Subject = string.Format("License Activation {0:yyyy-MM-dd hh:mm:ss}", DateTime.Now);
                mail.IsBodyHtml = true;
                mail.Body = license.MessageBody;
                Exception ex;
                TABS.Components.EmailSender.Send(mail, out ex);
                Warning = license.Warning;
                LicenceType = license.Type.ToString();

                if (ex != null)
                    log.Error("unable to send mails to clients", ex);
            }

        }

        public override string Status
        {
            get { return Warning; }
        }
    }
}
