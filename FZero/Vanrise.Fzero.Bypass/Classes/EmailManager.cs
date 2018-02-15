using System.Collections.Generic;

namespace Vanrise.Fzero.Bypass
{
    public static class EmailManager
    {

        public static void SendForgetPassword(string verificationCode, string toEmail, string CC)
        {
            int ID = (int)Enums.EmailTemplates.ForgetPassword;
            EmailTemplate template = EmailTemplate.Load(ID);
            if (template.IsActive)
            {
                Email email = new Email() { EmailTemplateID = ID };
                email.DestinationEmail = toEmail;
                email.Subject = template.Subject;
                email.Body = template.MessageBody.Replace("%VerificationCode%", verificationCode);
                email.CC = CC;
                Email.SendMail(email, "FMS_Profile");
            }
        }


        public static void SendAutoBlockReport(string toEmail, HashSet<string> CLIs, string ReportID, string profile_name)
        {
            if (CLIs.Count > 0)
            {
                string CLIConcatenated = string.Empty;
                int ID = (int)Enums.EmailTemplates.AutoBlockReport;
                EmailTemplate template = EmailTemplate.Load(ID);
                if (template.IsActive)
                {
                    Email email = new Email() { EmailTemplateID = ID };
                    email.DestinationEmail = toEmail;
                    email.Subject = "FM_BYPASS_ALERT";
                    email.CC = string.Empty;


                    foreach (var i in CLIs)
                        CLIConcatenated += "<br />" + i + "<br />";

                    email.Body = template.MessageBody.Replace("%CLIs%", CLIConcatenated).Replace("%Total%", CLIs.Count.ToString());
                    Email.SendMailWithAttachement(email, string.Empty, profile_name);
                }
            }
        }


        public static void SendReporttoMobileOperator(int Total, string AttachedPath, string toEmail, string OperatorLink, string CC, string ReportID, string profile_name)
        {
            if (Total > 0)
            {
                int ID = (int)Enums.EmailTemplates.ReporttoMobileOperator;
                EmailTemplate template = EmailTemplate.Load(ID);
                if (template.IsActive)
                {
                    Email email = new Email() { EmailTemplateID = ID };
                    email.DestinationEmail = toEmail;
                    email.Subject = template.Subject.Replace("%ReportID%", ReportID).Replace("%Total%", Total.ToString());
                    email.CC = CC;
                    email.Body = template.MessageBody.Replace("%OperatorLink%", OperatorLink).Replace("%Total%", Total.ToString());
                    Email.SendMailWithAttachement(email, AttachedPath, profile_name);
                }
            }
        }

        public static void SendReporttoMobileOperatorNoBlock(int Total, string AttachedPath, string toEmail, string OperatorLink, string CC, string ReportID, string profile_name)
        {
            int ID = (int)Enums.EmailTemplates.ReporttoMobileOperatorNoBLock;
            EmailTemplate template = EmailTemplate.Load(ID);
            if (template.IsActive)
            {
                Email email = new Email() { EmailTemplateID = ID };
                email.DestinationEmail = toEmail;
                email.Subject = template.Subject.Replace("%ReportID%", ReportID).Replace("%Total%", Total.ToString());
                email.CC = CC;
                email.Body = template.MessageBody.Replace("%OperatorLink%", OperatorLink).Replace("%Total%", Total.ToString());
                Email.SendMailWithAttachement(email, AttachedPath, profile_name);
            }
        }

        public static void SendReporttoMobileSyrianOperator(int Total, string AttachedPath, string toEmail, string OperatorLink, string CC, string ReportID, string profile_name)
        {
            if (Total > 0)
            {
                int ID = (int)Enums.EmailTemplates.ReporttoMobileSyrianOperator;
                EmailTemplate template = EmailTemplate.Load(ID);
                if (template.IsActive)
                {
                    Email email = new Email() { EmailTemplateID = ID };
                    email.DestinationEmail = toEmail;
                    email.Subject = template.Subject.Replace("%ReportID%", ReportID).Replace("%Total%", Total.ToString());
                    email.CC = CC;
                    email.Body = template.MessageBody.Replace("%OperatorLink%", OperatorLink).Replace("%Total%", Total.ToString());
                    Email.SendMailWithAttachement(email, AttachedPath, profile_name);
                }   
            }
        }

        public static void SendReporttoMobileSyrianOperatorNoBlock(int Total, string AttachedPath, string toEmail, string OperatorLink, string CC, string ReportID, string profile_name)
        {
            int ID = (int)Enums.EmailTemplates.ReporttoMobileSyrianOperatorNoBlock;
            EmailTemplate template = EmailTemplate.Load(ID);
            if (template.IsActive)
            {
                Email email = new Email() { EmailTemplateID = ID };
                email.DestinationEmail = toEmail;
                email.Subject = template.Subject.Replace("%ReportID%", ReportID).Replace("%Total%", Total.ToString());
                email.CC = CC;
                email.Body = template.MessageBody.Replace("%OperatorLink%", OperatorLink).Replace("%Total%", Total.ToString());
                Email.SendMailWithAttachement(email, AttachedPath, profile_name);
            }
        }

        public static void SendRepeatedReporttoMobileOperator(string AttachedPath, string toEmail, string OperatorLink, string CC, string ReportID, string profile_name)
        {
            int ID = (int)Enums.EmailTemplates.RepeatedReporttoMobileOperator;
            EmailTemplate template = EmailTemplate.Load(ID);
            if (template.IsActive)
            {
                Email email = new Email() { EmailTemplateID = ID };
                email.DestinationEmail = toEmail;
                email.Subject = template.Subject.Replace("%ReportID%", ReportID);
                email.CC = CC;
                email.Body = template.MessageBody.Replace("%OperatorLink%", OperatorLink);
                Email.SendMailWithAttachement(email, AttachedPath, profile_name);
            }
        }

        public static void SendRepeatedReporttoMobileSyrianOperator(string AttachedPath, string toEmail, string OperatorLink, string CC, string ReportID, string profile_name)
        {
            int ID = (int)Enums.EmailTemplates.RepeatedReporttoMobileSyrianOperator;
            EmailTemplate template = EmailTemplate.Load(ID);
            if (template.IsActive)
            {
                Email email = new Email() { EmailTemplateID = ID };
                email.DestinationEmail = toEmail;
                email.Subject = template.Subject.Replace("%ReportID%", ReportID);
                email.CC = CC;
                email.Body = template.MessageBody.Replace("%OperatorLink%", OperatorLink);
                Email.SendMailWithAttachement(email, AttachedPath, profile_name);
            }
        }

        public static void SendDailyReport(string AttachedPath, string toEmail, string CC, string profile_name)
        {
            int ID = (int)Enums.EmailTemplates.DailyReport;
            EmailTemplate template = EmailTemplate.Load(ID);
            if (template.IsActive)
            {
                Email email = new Email() { EmailTemplateID = ID };
                email.DestinationEmail = toEmail;
                email.Subject = template.Subject;
                email.CC = CC;
                email.Body = template.MessageBody;
                Email.SendMailWithAttachement(email, AttachedPath, profile_name);
            }
        }

        public static void SendSyrianDailyReport(string AttachedPath, string toEmail, string CC, string profile_name)
        {
            int ID = (int)Enums.EmailTemplates.SyrianDailyReport;
            EmailTemplate template = EmailTemplate.Load(ID);
            if (template.IsActive)
            {
                Email email = new Email() { EmailTemplateID = ID };
                email.DestinationEmail = toEmail;
                email.Subject = template.Subject;
                email.CC = CC;
                email.Body = template.MessageBody;
                Email.SendMailWithAttachement(email, AttachedPath, profile_name);
            }
        }

        public static void SendWeeklyReport(string AttachedPath, string toEmail, string CC, string profile_name)
        {
            int ID = (int)Enums.EmailTemplates.WeeklyReport;
            EmailTemplate template = EmailTemplate.Load(ID);
            if (template.IsActive)
            {
                Email email = new Email() { EmailTemplateID = ID };
                email.DestinationEmail = toEmail;
                email.Subject = template.Subject;
                email.CC = CC;
                email.Body = template.MessageBody;
                Email.SendMailWithAttachement(email, AttachedPath, profile_name);
            }
        }

        public static void SendSyrianWeeklyReport(string AttachedPath, string toEmail, string CC, string profile_name)
        {
            int ID = (int)Enums.EmailTemplates.SyrianWeeklyReport;
            EmailTemplate template = EmailTemplate.Load(ID);
            if (template.IsActive)
            {
                Email email = new Email() { EmailTemplateID = ID };
                email.DestinationEmail = toEmail;
                email.Subject = template.Subject;
                email.CC = CC;
                email.Body = template.MessageBody;
                Email.SendMailWithAttachement(email, AttachedPath, profile_name);
            }
        }

        public static void SendMobileOperatorFeedback(string toEmail, List<string> cases, string ReportID, string profile_name)
        {
            int id = (int)Enums.EmailTemplates.MobileOperatorFeedbackonReportedCases;
            EmailTemplate template = EmailTemplate.Load(id);
            if (template.IsActive)
            {
                string Cases = string.Empty;


                foreach (string i in cases)
                {
                    Cases += "<li>" + i + "</li>";
                }

                Cases = "<ul>" + Cases + "</ul>";


                Email email = new Email() { EmailTemplateID = id };
                email.DestinationEmail = toEmail;
                email.Subject = template.Subject;
                email.Body = template.MessageBody.Replace("%Cases%", Cases).Replace("%ReportID%", ReportID);

                Email.SendMail(email, profile_name);
            }
        }

    }
}
