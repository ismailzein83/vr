using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Mail;
using Vanrise.Common.Data;
using Vanrise.Entities;

namespace Vanrise.Common.Business
{
    public class EmailTemplateManager
    {
        public void SendEmail(string receiver, string content, string subject)
        {
            ConfigManager configManager = new ConfigManager();
            EmailSettingData emailSettingData = configManager.GetSystemEmail();

            MailMessage mailMessage = new MailMessage();
            mailMessage.From = new MailAddress(emailSettingData.SenderEmail);
            mailMessage.To.Add(new MailAddress(receiver));
            mailMessage.Subject = subject;
            mailMessage.Body = content;
            mailMessage.IsBodyHtml = true;


            SmtpClient client = new SmtpClient();
            client.Port = emailSettingData.Port;
            client.Host = emailSettingData.Host;
            client.Timeout = emailSettingData.Timeout;
            client.DeliveryMethod = SmtpDeliveryMethod.Network;
            client.UseDefaultCredentials = false;
            client.Credentials = new System.Net.NetworkCredential(emailSettingData.SenderEmail, emailSettingData.SenderPassword);
            client.EnableSsl = emailSettingData.EnabelSsl;

            client.Send(mailMessage);
        }

        public IDataRetrievalResult<EmailTemplateDetail> GetFilteredEmailTemplates(DataRetrievalInput<EmailTemplateQuery> input)
        {
            var emailTemplates = GetCachedEmailTemplates();

            Func<EmailTemplate, bool> filterExpression = (itemObject) =>
                 (string.IsNullOrEmpty(input.Query.Name) || itemObject.Name.ToLower().Contains(input.Query.Name.ToLower()));

            return DataRetrievalManager.Instance.ProcessResult(input, emailTemplates.ToBigResult(input, filterExpression, EmailTemplateDetailMapper));
        }

        public UpdateOperationOutput<EmailTemplateDetail> UpdateEmailTemplate(EmailTemplate emailTemplate)
        {
            IEmailTemplateDataManager dataManager = CommonDataManagerFactory.GetDataManager<IEmailTemplateDataManager>();

            bool updateActionSucc = dataManager.UpdateEmailTemplate(emailTemplate);
            UpdateOperationOutput<EmailTemplateDetail> updateOperationOutput = new UpdateOperationOutput<EmailTemplateDetail>();

            updateOperationOutput.Result = UpdateOperationResult.Failed;
            updateOperationOutput.UpdatedObject = null;

            if (updateActionSucc)
            {
                Vanrise.Caching.CacheManagerFactory.GetCacheManager<CacheManager>().SetCacheExpired();
                updateOperationOutput.Result = UpdateOperationResult.Succeeded;
                updateOperationOutput.UpdatedObject = EmailTemplateDetailMapper(emailTemplate);
            }
            return updateOperationOutput;

        }

        public EmailTemplate GetEmailTemplate(int emailTemplateId)
        {
            var allEmailTemplates = GetCachedEmailTemplates();
            return allEmailTemplates.GetRecord(emailTemplateId);
        }

        public EmailTemplate GeEmailTemplateByType(string type)
        {
            var allEmailTemplates = GetCachedEmailTemplates();
            if (allEmailTemplates == null)
                return null;

            return allEmailTemplates.FindRecord(itm => itm.Type == type);
        }

        public IEnumerable<EmailTemplate> GetAllEmailTemplates()
        {
            var allEmailTemplates = GetCachedEmailTemplates();
            if (allEmailTemplates == null)
                return null;

            return allEmailTemplates.Values;
        }

        private Dictionary<int, EmailTemplate> GetCachedEmailTemplates()
        {
            return Vanrise.Caching.CacheManagerFactory.GetCacheManager<CacheManager>().GetOrCreateObject("GetEmailTemplates",
               () =>
               {
                   IEmailTemplateDataManager dataManager = CommonDataManagerFactory.GetDataManager<IEmailTemplateDataManager>();
                   IEnumerable<EmailTemplate> emailTemplates = dataManager.GetEmailTemplates();
                   return emailTemplates.ToDictionary(item => item.EmailTemplateId, item => item);
               });
        }

        private EmailTemplateDetail EmailTemplateDetailMapper(EmailTemplate emailTemplate)
        {
            return new EmailTemplateDetail()
            {
                Entity = emailTemplate
            };
        }
        #region Private Members

        private class CacheManager : Vanrise.Caching.BaseCacheManager
        {
            IEmailTemplateDataManager _dataManager = CommonDataManagerFactory.GetDataManager<IEmailTemplateDataManager>();
            object _updateHandle;

            protected override bool ShouldSetCacheExpired(object parameter)
            {
                return _dataManager.AreEmailTemplatesUpdated(ref _updateHandle);
            }
        }

        #endregion
    }
}
