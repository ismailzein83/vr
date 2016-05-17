using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Vanrise.Entities;

namespace Vanrise.Common.Data.SQL
{
    public class EmailTemplateDataManager : Vanrise.Data.SQL.BaseSQLDataManager, IEmailTemplateDataManager
    {
        public EmailTemplateDataManager()
            : base(GetConnectionStringName("ConfigurationDBConnStringKey", "ConfigurationDBConnString"))
        {

        }

        public IEnumerable<EmailTemplate> GetEmailTemplates()
        {
            return GetItemsSP("common.sp_EmailTemplate_GetAll", EmailTemplateMapper);
        }

        public bool UpdateEmailTemplate(EmailTemplate emailTemplate)
        {
            int recordsEffected = ExecuteNonQuerySP("common.sp_EmailTemplate_Update", emailTemplate.EmailTemplateId, emailTemplate.Name, emailTemplate.SubjectTemplate, 
                emailTemplate.BodyTemplate);
            return (recordsEffected > 0);
        }

        public bool AreEmailTemplatesUpdated(ref object updateHandle)
        {
            return base.IsDataUpdated("common.EmailTemplate", ref updateHandle);
        }


        #region Mappers
        public EmailTemplate EmailTemplateMapper(IDataReader reader)
        {
            EmailTemplate currency = new EmailTemplate
            {
                EmailTemplateId = (int)reader["ID"],
                Name = reader["Name"] as string,
                BodyTemplate = reader["BodyTemplate"] as string,
                SubjectTemplate = reader["SubjectTemplate"] as string,
                Type = reader["Type"] as string
            };

            return currency;
        }
        #endregion
    }
}
