using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Vanrise.Entities;

namespace Vanrise.Common.Business
{
    public class SMSManager
    {
        public void SendSMS(string mobileNumber, string message, DateTime messageTime)
        {
            SMSHandlerSendSMSContext context = new SMSHandlerSendSMSContext
            {
                Message = message,
                MessageTime = messageTime,
                MobileNumber = mobileNumber
            };

            SMSSendHandler handler = new ConfigManager().GetSMSSendHandler();
            handler.Settings.SendSMS(context);
        }

        public void SendSMS(Guid smsTemplateId, Dictionary<string, dynamic> objects)
        {
            var evaluatedSMSTemplate = EvaluateSMSTemplate(smsTemplateId, objects);
            SendSMS(evaluatedSMSTemplate.MobileNumber, evaluatedSMSTemplate.Message,DateTime.Now);
        }

        public SMSEvaluatedTemplate EvaluateSMSTemplate(Guid smsMessageTemplateId, Dictionary<string, dynamic> objects)
        {
            SMSMessageTemplate smsMessageTemplate = GetSMSMessageTemplate(smsMessageTemplateId);

            var smsMessageTypeSettings = GetSMSMessageTypeSettings(smsMessageTemplate.SMSMessageTypeId);

            var smsContext = new VRSMSContext(smsMessageTypeSettings, objects);


            return new SMSEvaluatedTemplate
            {
                MobileNumber = EvaluateExpression(smsMessageTemplateId, "MobileNumber", smsMessageTemplate.Settings.MobileNumber, smsContext),
                Message = EvaluateExpression(smsMessageTemplateId, "Message", smsMessageTemplate.Settings.Message, smsContext)
            };
        }

        #region Private Methods

        private SMSMessageTemplate GetSMSMessageTemplate(Guid smsMessageTemplateId)
        {
            SMSMessageTemplate smsMessageTemplate = new SMSMessageTemplateManager().GetSMSMessageTemplate(smsMessageTemplateId);
            if (smsMessageTemplate == null)
                throw new NullReferenceException(String.Format("smsMessageTemplate '{0}'", smsMessageTemplateId));
            if (smsMessageTemplate.Settings == null)
                throw new NullReferenceException(String.Format("smsMessageTemplate.Settings '{0}'", smsMessageTemplateId));
            return smsMessageTemplate;
        }

        private SMSMessageTypeSettings GetSMSMessageTypeSettings(Guid smsMessageTypeId)
        {
            SMSMessageTypeSettings smsMessageTypeSettings = new SMSMessageTypeManager().GetSMSMessageTypeSettings(smsMessageTypeId);
            if (smsMessageTypeSettings == null)
                throw new NullReferenceException(String.Format("smsMessageType '{0}'", smsMessageTypeId));
            if (smsMessageTypeSettings.Objects == null)
                throw new NullReferenceException(String.Format("smsMessageType.Settings.Objects '{0}'", smsMessageTypeId));
            return smsMessageTypeSettings;
        }
         private static string EvaluateExpression(Guid smsMessageTemplateId, string fieldName, VRExpression expression, VRSMSContext smsContext)
        {
            if (expression == null || expression.ExpressionString == null)
                return null;

            string cacheName = String.Format("SMSTemplate_{0}_{1}", smsMessageTemplateId, fieldName);
            string templateKey = Vanrise.Caching.CacheManagerFactory.GetCacheManager<SMSMessageTemplateManager.CacheManager>()
                .GetOrCreateObject(cacheName, () =>
                {
                    string templateKeyLocal = string.Format("TemplateKey_{0}", Guid.NewGuid());
                    var key = new RazorEngine.Templating.NameOnlyTemplateKey(templateKeyLocal, RazorEngine.Templating.ResolveType.Global, null);
                    RazorEngine.Engine.Razor.AddTemplate(key, new RazorEngine.Templating.LoadedTemplateSource(expression.ExpressionString));
                    RazorEngine.Engine.Razor.Compile(key, typeof(VRSMSContext));
                    return templateKeyLocal;
                });
            var keyName = new RazorEngine.Templating.NameOnlyTemplateKey(templateKey, RazorEngine.Templating.ResolveType.Global, null);
            StringBuilder sb = new StringBuilder();
            StringWriter sw = new StringWriter(sb);
            RazorEngine.Engine.Razor.RunCompile(keyName, sw, typeof(VRSMSContext), smsContext);
            return sb.ToString();
        }
        #endregion
    }

    public class SMSHandlerSendSMSContext : ISMSHandlerSendSMSContext
    {
        public string MobileNumber { get; set; }

        public string Message { get; set; }

        public DateTime MessageTime { get; set; }
    }
}
