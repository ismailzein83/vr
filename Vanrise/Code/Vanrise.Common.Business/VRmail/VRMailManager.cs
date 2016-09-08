using System;
using System.Collections.Generic;
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
            VRMailMessageTemplate mailMessageTemplate = GetMailMessageTemplate(mailMessageTemplateId);
            SendMail(mailMessageTemplate, objects);
        }
        #endregion


        #region Private Methods
        private void SendMail(VRMailMessageTemplate mailMessageTemplate, Dictionary<string, dynamic> objects)
        {
            var mailMessageType = GetMailMessageType(mailMessageTemplate.VRMailMessageTypeId);

            //Dictionary<string, dynamic> variableValuesObj = new VRObjectManager().EvaluateVariables(mailMessageTemplate.Settings.Variables, objects, mailMessageType.Settings.Objects);

            var mailContext = new VRMailContext(mailMessageType, objects);
            string to = EvaluateExpression(mailMessageTemplate.Settings.To, mailContext);
            string cc = EvaluateExpression(mailMessageTemplate.Settings.CC, mailContext);
            string subject = EvaluateExpression(mailMessageTemplate.Settings.Subject, mailContext);
            string body = EvaluateExpression(mailMessageTemplate.Settings.Body, mailContext);

            SendMail(to, cc, subject, body);
        }

        public void SendMail(string to, string cc, string subject, string body)
        {
            ConfigManager configManager = new ConfigManager();
            EmailSettingData emailSettingData = configManager.GetSystemEmail();

            MailMessage mailMessage = new MailMessage();
            mailMessage.From = new MailAddress(emailSettingData.SenderEmail);
            mailMessage.To.Add(new MailAddress(to));
            mailMessage.Subject = subject;
            mailMessage.Body = body;
            mailMessage.IsBodyHtml = true;

            SmtpClient client = new SmtpClient();
            client.Port = emailSettingData.Port;
            client.Host = emailSettingData.Host;
            client.Timeout = emailSettingData.Timeout;
            client.DeliveryMethod = SmtpDeliveryMethod.Network;
            client.UseDefaultCredentials = false;
            client.Credentials = new System.Net.NetworkCredential(emailSettingData.SenderEmail, emailSettingData.SenderPassword);
            client.EnableSsl = true;

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

        private static string EvaluateExpression(VRExpression expression, VRMailContext mailContext)
        {
            return expression.ExpressionString != null ? RazorEngine.Razor.Parse<VRMailContext>(expression.ExpressionString, mailContext) : null;
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
