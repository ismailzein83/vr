using RazorEngine;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net.Mail;
using System.Text;
using System.Threading.Tasks;
using Vanrise.Entities;

namespace Vanrise.Common.Business
{
    public class VRMailManager
    {
        #region Public Methods
        public void SendMail(Guid mailMessageTemplateId, Dictionary<string, dynamic> objects)
        {
            var evaluatedMailTemplate = EvaluateMailTemplate(mailMessageTemplateId, objects);
            SendMail(evaluatedMailTemplate.From, evaluatedMailTemplate.To, evaluatedMailTemplate.CC, evaluatedMailTemplate.BCC, evaluatedMailTemplate.Subject, evaluatedMailTemplate.Body);
        }

        public VRMailEvaluatedTemplate EvaluateMailTemplate(Guid mailMessageTemplateId, Dictionary<string, dynamic> objects)
        {
            VRMailMessageTemplate mailMessageTemplate = GetMailMessageTemplate(mailMessageTemplateId);

            var mailMessageType = GetMailMessageType(mailMessageTemplate.VRMailMessageTypeId);

            //Dictionary<string, dynamic> variableValuesObj = new VRObjectManager().EvaluateVariables(mailMessageTemplate.Settings.Variables, objects, mailMessageType.Settings.Objects);

            var mailContext = new VRMailContext(mailMessageType, objects);


            return new VRMailEvaluatedTemplate
            {
                From = EvaluateExpression(mailMessageTemplateId, "From", mailMessageTemplate.Settings.From, mailContext),
                To = EvaluateExpression(mailMessageTemplateId, "To", mailMessageTemplate.Settings.To, mailContext),
                CC = EvaluateExpression(mailMessageTemplateId, "CC", mailMessageTemplate.Settings.CC, mailContext),
                BCC = EvaluateExpression(mailMessageTemplateId, "BCC", mailMessageTemplate.Settings.BCC, mailContext),
                Subject = EvaluateExpression(mailMessageTemplateId, "Subject", mailMessageTemplate.Settings.Subject, mailContext),
                Body = EvaluateExpression(mailMessageTemplateId, "Body", mailMessageTemplate.Settings.Body, mailContext)
            };
        }

        public SmtpClient GetSMTPClient(EmailSettingData emailSettingData)
        {
            SmtpClient client = new SmtpClient();
            client.Port = emailSettingData.Port;
            client.Host = emailSettingData.Host;
            client.Timeout = emailSettingData.TimeoutInSeconds * 1000;
            client.DeliveryMethod = SmtpDeliveryMethod.Network;
            client.UseDefaultCredentials = false;
            client.Credentials = new System.Net.NetworkCredential(emailSettingData.SenderEmail, emailSettingData.SenderPassword);
            client.EnableSsl = emailSettingData.EnabelSsl;
            return client;
        }

        public VRMailAttachement ConvertToGeneralAttachement( long fileId)
        {
            var vrFileManager = new VRFileManager();
            var file = vrFileManager.GetFile(fileId);
            return new VRMailAttachmentGeneral
            {
                Name = file.Name,
                Content = file.Content
            };
        }
        #endregion


        #region Private Methods
        //private void SendMail(VRMailMessageTemplate mailMessageTemplate, Dictionary<string, dynamic> objects)
        //{
        //    var mailMessageType = GetMailMessageType(mailMessageTemplate.VRMailMessageTypeId);

        //    //Dictionary<string, dynamic> variableValuesObj = new VRObjectManager().EvaluateVariables(mailMessageTemplate.Settings.Variables, objects, mailMessageType.Settings.Objects);

        //    var mailContext = new VRMailContext(mailMessageType, objects);
        //    string to = EvaluateExpression(mailMessageTemplate.Settings.To, mailContext);
        //    string cc = EvaluateExpression(mailMessageTemplate.Settings.CC, mailContext);
        //    string subject = EvaluateExpression(mailMessageTemplate.Settings.Subject, mailContext);
        //    string body = EvaluateExpression(mailMessageTemplate.Settings.Body, mailContext);

        //    SendMail(to, cc, subject, body);
        //}

        public void SendMail(string to, string cc, string bcc, string subject, string body)
        {
            SendMail(to, cc, bcc, subject, body, null,false);
        }
        public void SendMail(string from, string to, string cc, string bcc, string subject, string body)
        {
            SendMail(from, to, cc, bcc, subject, body, null);
        }

        public void SendTestMail(EmailSettingData emailSettingData, string from, string to, string subject, string body)
        {
            if (String.IsNullOrWhiteSpace(to))
                throw new NullReferenceException("to");

            if (String.IsNullOrWhiteSpace(subject))
                throw new NullReferenceException("subject");

            MailMessage mailMessage = new MailMessage();
            mailMessage.From = GetSenderMailAddress(from, emailSettingData);
            string[] toAddresses = to.Split(';', ',', ':');
            foreach (var toAddress in toAddresses)
            {
                if (!String.IsNullOrEmpty(toAddress))
                    mailMessage.To.Add(new MailAddress(toAddress));
            }

            mailMessage.Subject = subject;
            mailMessage.Body = body;
            mailMessage.IsBodyHtml = true;

            SmtpClient client = GetSMTPClient(emailSettingData);

            client.Send(mailMessage);
        }

        public void SendMail(string to, string cc, string bcc, string subject, string body, List<VRMailAttachement> attachements, bool compressAttachement = false)
        {
            SendMail(null, to, cc, bcc, subject, body, attachements, compressAttachement);
        }
        public void SendMail(string from, string to, string cc, string bcc, string subject, string body, List<VRMailAttachement> attachements, bool compressAttachement = false)
        {
            if (String.IsNullOrWhiteSpace(to))
                throw new NullReferenceException("to");
            ConfigManager configManager = new ConfigManager();
            EmailSettingData emailSettingData = configManager.GetSystemEmail();

            MailMessage mailMessage = new MailMessage ();
            mailMessage.From = GetSenderMailAddress(from, emailSettingData);

            string[] toAddresses = to.Split(';', ',', ':');
            foreach (var toAddress in toAddresses)
            {
                if (!string.IsNullOrEmpty(toAddress))
                    mailMessage.To.Add(new MailAddress(toAddress));
            }

            if (!String.IsNullOrWhiteSpace(cc))
            {
                string[] ccAddresses = cc.Split(';', ',', ':');
                foreach (var ccAddress in ccAddresses)
                {
                    if (!string.IsNullOrEmpty(ccAddress))
                        mailMessage.CC.Add(new MailAddress(ccAddress));
                }
            }

            if (!String.IsNullOrWhiteSpace(bcc))
            {
                string[] bccAddresses = bcc.Split(';', ',', ':');
                foreach (var bccAddress in bccAddresses)
                {
                    if (!string.IsNullOrEmpty(bccAddress))
                        mailMessage.Bcc.Add(new MailAddress(bccAddress));
                }
            }

            mailMessage.Subject = subject;
            mailMessage.Body = body;
            mailMessage.IsBodyHtml = true;

            SmtpClient client = GetSMTPClient(emailSettingData);
            SetAttachement(mailMessage, attachements, compressAttachement);
            string logEmailDescription = String.Format("Email Information - To: '{0}'. Subject: '{1}'", to, subject);
            string logEventType = "Email Notification";
            try
            {
                client.Send(mailMessage);
                LoggerFactory.GetLogger().WriteEntry(logEventType, null, LogEntryType.Information, "Email sent successfully. {0}", logEmailDescription);
            }
            catch (Exception ex)
            {
                var businessException = new VRBusinessException(string.Format("Email is not sent (click to check error). {0}", logEmailDescription), ex);
                LoggerFactory.GetExceptionLogger().WriteException(logEventType, null, businessException);
                throw;
            }
        }

        private MailAddress GetSenderMailAddress(string from, EmailSettingData emailSettingData)
        {
            if (!string.IsNullOrEmpty(from))
                return new MailAddress(from);

            if (!string.IsNullOrEmpty(emailSettingData.AlternativeSenderEmail))
                return new MailAddress(emailSettingData.AlternativeSenderEmail);

            return new MailAddress(emailSettingData.SenderEmail);
        }

        private void SetAttachement(MailMessage mailMessage, List<VRMailAttachement> attachements, bool compressAttachement)
        {
            if (attachements != null)
            {
                if (compressAttachement)
                {
                    ZipUtility zipUtility = new ZipUtility();
                    MemoryStream zipStream = zipUtility.ZipFiles(attachements.Select(ConvertAttachementToFileInfo));
                    mailMessage.Attachments.Add(new System.Net.Mail.Attachment(zipStream, "Pricelist.zip", "application/zip"));
                    zipStream.Seek(0, System.IO.SeekOrigin.Begin);
                }
                else
                {
                    foreach (var vrAttachment in attachements)
                    {
                        MemoryStream memStr = new MemoryStream(vrAttachment.Content);
                        var attachment = new Attachment(memStr, vrAttachment.Name);
                        mailMessage.Attachments.Add(attachment);
                    }
                }
            }
        }

        private ZipFileInfo ConvertAttachementToFileInfo(VRMailAttachement mailAttachement)
        {
            return new ZipFileInfo
            {
                Content = mailAttachement.Content,
                FileName = mailAttachement.Name
            };
        }
        private VRMailMessageTemplate GetMailMessageTemplate(Guid mailMessageTemplateId)
        {
            VRMailMessageTemplate mailMessageTemplate = new VRMailMessageTemplateManager().GetMailMessageTemplate(mailMessageTemplateId);
            if (mailMessageTemplate == null)
                throw new NullReferenceException(String.Format("mailMessageTemplate '{0}'", mailMessageTemplateId));
            if (mailMessageTemplate.Settings == null)
                throw new NullReferenceException(String.Format("mailMessageTemplate.Settings '{0}'", mailMessageTemplateId));
            return mailMessageTemplate;
        }

        private VRMailMessageType GetMailMessageType(Guid mailMessageTypeId)
        {
            VRMailMessageType mailMessageType = new VRMailMessageTypeManager().GetMailMessageType(mailMessageTypeId);
            if (mailMessageType == null)
                throw new NullReferenceException(String.Format("mailMessageType '{0}'", mailMessageTypeId));
            if (mailMessageType.Settings == null)
                throw new NullReferenceException(String.Format("mailMessageType.Settings '{0}'", mailMessageTypeId));
            if (mailMessageType.Settings.Objects == null)
                throw new NullReferenceException(String.Format("mailMessageType.Settings.Objects '{0}'", mailMessageTypeId));
            return mailMessageType;
        }

        private static string EvaluateExpression(Guid mailMessageTemplateId, string fieldName, VRExpression expression, VRMailContext mailContext)
        {
            if (expression == null || expression.ExpressionString == null)
                return null;

            string cacheName = String.Format("EmailTemplate_{0}_{1}", mailMessageTemplateId, fieldName);
            string templateKey = Vanrise.Caching.CacheManagerFactory.GetCacheManager<VRMailMessageTemplateManager.CacheManager>()
                .GetOrCreateObject(cacheName, () =>
                {
                    string templateKeyLocal = string.Format("TemplateKey_{0}", Guid.NewGuid());
                    var key = new RazorEngine.Templating.NameOnlyTemplateKey(templateKeyLocal, RazorEngine.Templating.ResolveType.Global, null);
                    RazorEngine.Engine.Razor.AddTemplate(key, new RazorEngine.Templating.LoadedTemplateSource(expression.ExpressionString));
                    RazorEngine.Engine.Razor.Compile(key, typeof(VRMailContext));
                    return templateKeyLocal;
                });
            var keyName = new RazorEngine.Templating.NameOnlyTemplateKey(templateKey, RazorEngine.Templating.ResolveType.Global, null);
            StringBuilder sb = new StringBuilder();
            StringWriter sw = new StringWriter(sb);
            RazorEngine.Engine.Razor.RunCompile(keyName, sw, typeof(VRMailContext), mailContext);
            return sb.ToString();
        }
        #endregion
    }

    public class VRMailContext
    {
        VRObjectEvaluator vrObjectEvaluator;

        public VRMailContext(VRMailMessageType mailMessageType, Dictionary<string, dynamic> objects)
        {
            vrObjectEvaluator = new VRObjectEvaluator(mailMessageType.Settings.Objects, objects);
        }

        public dynamic GetVal(string objectName, string propertyName)
        {
            return vrObjectEvaluator.GetPropertyValue(objectName, propertyName);
        }
    }
}
