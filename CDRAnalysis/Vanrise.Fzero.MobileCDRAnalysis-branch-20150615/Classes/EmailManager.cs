using System.Collections.Generic;

namespace Vanrise.Fzero.MobileCDRAnalysis
{
    public static class EmailManager
    {

        public static void SendForgetPassword(string verificationCode, string profile_name)
        {
            int ID = (int)Enums.EmailTemplates.ForgetPassword;
            EmailTemplate template = EmailTemplate.Load(ID);
            if (template.IsActive)
            {

                string To = string.Empty;
                string CC = string.Empty;
                string BCC = string.Empty;


                foreach (var i in template.EmailReceivers)
                {
                    if (i.EmailReceiverTypeID == (int)Enums.EmailRecieverTypes.To)
                        To = To + i.Email + ";";

                    if (i.EmailReceiverTypeID == (int)Enums.EmailRecieverTypes.To)
                        CC = CC + i.Email + ";";

                    if (i.EmailReceiverTypeID == (int)Enums.EmailRecieverTypes.To)
                        BCC = BCC + i.Email + ";";
                }

                prGetEmails_Result.SendMail(profile_name, template.Subject, template.MessageBody.Replace("%VerificationCode%", verificationCode), To, CC, BCC);
            }
        }

        public static void SendReporttoITPC(string AttachedPath, string ReportID, string profile_name)
        {
            int ID = (int)Enums.EmailTemplates.ReporttoITPC;
            EmailTemplate template = EmailTemplate.Load_With_Receivers(ID);
          

            if (template.IsActive)
            {
                string To = string.Empty;
                string CC = string.Empty;
                string BCC = string.Empty;


                foreach (var i in template.EmailReceivers)
                {
                    if (i.EmailReceiverTypeID == (int)Enums.EmailRecieverTypes.To)
                        To = To + i.Email + ";";

                    if (i.EmailReceiverTypeID == (int)Enums.EmailRecieverTypes.CC)
                        CC = CC + i.Email + ";";

                    if (i.EmailReceiverTypeID == (int)Enums.EmailRecieverTypes.BCC)
                        BCC = BCC + i.Email + ";";
                }

                prGetEmails_Result.SendMailWithAttachement(profile_name,To,AttachedPath,  template.Subject.Replace("%ReportID%", ReportID),CC, template.MessageBody.Replace("%ReportID%", ReportID),  BCC);
            }
        }
    }
}
