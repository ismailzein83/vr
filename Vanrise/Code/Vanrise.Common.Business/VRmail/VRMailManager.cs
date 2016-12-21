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
            SendMail(evaluatedMailTemplate.To, evaluatedMailTemplate.CC, evaluatedMailTemplate.Subject, evaluatedMailTemplate.Body);           
        }

        public VRMailEvaluatedTemplate EvaluateMailTemplate(Guid mailMessageTemplateId, Dictionary<string, dynamic> objects)
        {
            VRMailMessageTemplate mailMessageTemplate = GetMailMessageTemplate(mailMessageTemplateId);

            var mailMessageType = GetMailMessageType(mailMessageTemplate.VRMailMessageTypeId);

            //Dictionary<string, dynamic> variableValuesObj = new VRObjectManager().EvaluateVariables(mailMessageTemplate.Settings.Variables, objects, mailMessageType.Settings.Objects);

            var mailContext = new VRMailContext(mailMessageType, objects);


            return new VRMailEvaluatedTemplate
            {
                To = EvaluateExpression(mailMessageTemplateId, "To", mailMessageTemplate.Settings.To, mailContext),
                CC = EvaluateExpression(mailMessageTemplateId, "CC", mailMessageTemplate.Settings.CC, mailContext),
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

        public void SendMail(string to, string cc, string subject, string body)
        {
            SendMail(to, cc, subject, body, null);
        }

        public void SendMail(string to, string cc, string subject, string body, List<VRMailAttachement> attachements)
        {
            if (String.IsNullOrWhiteSpace(to))
                throw new NullReferenceException("to");
            ConfigManager configManager = new ConfigManager();
            EmailSettingData emailSettingData = configManager.GetSystemEmail();

            MailMessage mailMessage = new MailMessage();
            mailMessage.From = new MailAddress(emailSettingData.SenderEmail);
            string[] toAddresses = to.Split(';', ',', ':');
            foreach (var toAddress in toAddresses)
            {
                mailMessage.To.Add(new MailAddress(toAddress));
            }

            if (!String.IsNullOrWhiteSpace(cc))
            {
                string[] ccAddresses = cc.Split(';', ',', ':');
                foreach (var ccAddress in ccAddresses)
                {
                    mailMessage.CC.Add(new MailAddress(ccAddress));
                }
            }

            mailMessage.Subject = subject;
            mailMessage.Body = body;
            mailMessage.IsBodyHtml = true;

            if(attachements != null)
            {
                foreach(var vrAttachment in attachements)
                {
                    MemoryStream memStr = new MemoryStream(vrAttachment.Content);
                    var attachment = new Attachment(memStr, vrAttachment.Name);
                    //attachment.ContentType = new ContentType("application/vnd.ms-excel");
                    //attachment.TransferEncoding = TransferEncoding.Base64;
                    //attachment.NameEncoding = Encoding.UTF8;
                    //attachment.Name = "SalePriceList.xls";

                    mailMessage.Attachments.Add(attachment);
                }
            }

            SmtpClient client = GetSMTPClient(emailSettingData);
            
            client.Send(mailMessage);
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
            if (expression.ExpressionString == null)
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
