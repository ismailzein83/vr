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
    class VRMailMessageTemplateDataManager : BaseSQLDataManager, IVRMailMessageTemplateDataManager
    {
        #region ctor/Local Variables

        public VRMailMessageTemplateDataManager()
            : base(GetConnectionStringName("ConfigurationDBConnStringKey", "ConfigurationDBConnString"))
        {

        }
        #endregion

        #region Public Methods

        public List<VRMailMessageTemplate> GetMailMessageTemplates()
        {
            return GetItemsSP("common.sp_MailMessageTemplate_GetAll", VRMailMessageTemplateMapper);
        }

        public bool AreMailMessageTemplateUpdated(ref object updateHandle)
        {
            return base.IsDataUpdated("common.MailMessageTemplate", ref updateHandle);
        }

        public bool Insert(VRMailMessageTemplate vrMailMessageTemplateItem)
        {
            string serializedSettings = vrMailMessageTemplateItem.Settings != null ? Vanrise.Common.Serializer.Serialize(vrMailMessageTemplateItem.Settings) : null;
            int affectedRecords = ExecuteNonQuerySP("common.sp_MailMessageTemplate_Insert", vrMailMessageTemplateItem.VRMailMessageTemplateId, vrMailMessageTemplateItem.Name, vrMailMessageTemplateItem.VRMailMessageTypeId, serializedSettings);

            if (affectedRecords > 0)
            {
                return true;
            }

            return false;
        }

        public bool Update(VRMailMessageTemplate vrMailMessageTemplateItem)
        {
            string serializedSettings = vrMailMessageTemplateItem.Settings != null ? Vanrise.Common.Serializer.Serialize(vrMailMessageTemplateItem.Settings) : null;
            int affectedRecords = ExecuteNonQuerySP("common.sp_MailMessageTemplate_Update", vrMailMessageTemplateItem.VRMailMessageTemplateId, vrMailMessageTemplateItem.Name, vrMailMessageTemplateItem.VRMailMessageTypeId, serializedSettings);
            return (affectedRecords > 0);
        }

        #endregion

        #region Mappers

        VRMailMessageTemplate VRMailMessageTemplateMapper(IDataReader reader)
        {
            VRMailMessageTemplate vrMailMessageTemplate = new VRMailMessageTemplate
            {
                VRMailMessageTemplateId = (Guid)reader["ID"],
                Name = reader["Name"] as string,
                VRMailMessageTypeId = (Guid)reader["MessageTypeID"],
                Settings = Vanrise.Common.Serializer.Deserialize<VRMailMessageTemplateSettings>(reader["Settings"] as string)
            };
            return vrMailMessageTemplate;
        }

        #endregion
    }

}
