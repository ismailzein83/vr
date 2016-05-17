using System.Collections.Generic;
using Vanrise.Entities;

namespace Vanrise.Common.Data
{
    public interface IEmailTemplateDataManager : IDataManager
    {
        IEnumerable<EmailTemplate> GetEmailTemplates();
        
        bool UpdateEmailTemplate(EmailTemplate emailTemplate);

        bool AreEmailTemplatesUpdated(ref object updateHandle);
    }
}
