using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Vanrise.Data.SQL;
using Vanrise.Entities;

namespace Vanrise.Common.Data.SQL
{
    public class SMSMessageTemplateDataManager : BaseSQLDataManager, ISMSMessageTemplateDataManager
    {
        #region ctor/Local Variables

        public SMSMessageTemplateDataManager()
            : base(GetConnectionStringName("ConfigurationDBConnStringKey", "ConfigurationDBConnString"))
        {

        }
        #endregion

        #region Public Methods
        public List<SMSMessageTemplate> GetSMSMessageTemplates()
        {
            return GetItemsSP("common.sp_SMSMessageTemplate_GetAll", SMSMessageTemplateMapper);
        }
        public bool AreSMSMessageTemplateUpdated(ref object updateHandle)
        {
            return base.IsDataUpdated("common.SMSMessageTemplate", ref updateHandle);
        }

        public bool Insert(SMSMessageTemplate smsMessageTemplateItem)
        {
            string serializedSettings = smsMessageTemplateItem.Settings != null ? Vanrise.Common.Serializer.Serialize(smsMessageTemplateItem.Settings) : null;
            int affectedRecords = ExecuteNonQuerySP("common.sp_SMSMessageTemplate_Insert", smsMessageTemplateItem.SMSMessageTemplateId, smsMessageTemplateItem.Name, smsMessageTemplateItem.SMSMessageTypeId, serializedSettings);

            if (affectedRecords > 0)
            {
                return true;
            }

            return false;
        }

        public bool Update(SMSMessageTemplate smsMessageTemplateItem)
        {
            string serializedSettings = smsMessageTemplateItem.Settings != null ? Vanrise.Common.Serializer.Serialize(smsMessageTemplateItem.Settings) : null;
            int affectedRecords = ExecuteNonQuerySP("common.sp_SMSMessageTemplate_Update", smsMessageTemplateItem.SMSMessageTemplateId, smsMessageTemplateItem.Name, smsMessageTemplateItem.SMSMessageTypeId, serializedSettings);
            return (affectedRecords > 0);
        }

        #endregion

        #region Mappers

        SMSMessageTemplate SMSMessageTemplateMapper(IDataReader reader)
        {
            SMSMessageTemplate smsMessageTemplate = new SMSMessageTemplate
            {
                SMSMessageTemplateId = (Guid)reader["ID"],
                Name = reader["Name"] as string,
                SMSMessageTypeId = (Guid)reader["SMSMessageTypeId"],
                Settings = Vanrise.Common.Serializer.Deserialize<SMSMessageTemplateSettings>(reader["Settings"] as string)
            };
            return smsMessageTemplate;
        }

        #endregion
    }
}
