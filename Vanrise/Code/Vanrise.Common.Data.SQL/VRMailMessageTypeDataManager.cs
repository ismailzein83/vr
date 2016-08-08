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
    class VRMailMessageTypeDataManager : BaseSQLDataManager, IVRMailMessageTypeDataManager
    {
        #region ctor/Local Variables
        public VRMailMessageTypeDataManager()
            : base(GetConnectionStringName("ConfigurationDBConnStringKey", "ConfigurationDBConnString"))
        {

        }
        #endregion


        #region Public Methods

        public List<VRMailMessageType> GetMailMessageTypes()
        {
            return GetItemsSP("common.sp_MailMessageType_GetAll", VRMailMessageTypeMapper);
        }

        public bool AreMailMessageTypeUpdated(ref object updateHandle)
        {
            return base.IsDataUpdated("common.MailMessageType", ref updateHandle);
        }

        public bool Insert(VRMailMessageType vrMailMessageTypeItem)
        {
            string serializedSettings = vrMailMessageTypeItem.Settings != null ? Vanrise.Common.Serializer.Serialize(vrMailMessageTypeItem.Settings) : null;
            int affectedRecords = ExecuteNonQuerySP("common.sp_MailMessageType_Insert", vrMailMessageTypeItem.VRMailMessageTypeId, vrMailMessageTypeItem.Name, serializedSettings);

            if (affectedRecords > 0)
            {
                return true;
            }

            return false;
        }

        public bool Update(VRMailMessageType vrMailMessageTypeItem)
        {
            string serializedSettings = vrMailMessageTypeItem.Settings != null ? Vanrise.Common.Serializer.Serialize(vrMailMessageTypeItem.Settings) : null;
            int affectedRecords = ExecuteNonQuerySP("common.sp_MailMessageType_Update", vrMailMessageTypeItem.VRMailMessageTypeId, vrMailMessageTypeItem.Name, serializedSettings);
            return (affectedRecords > 0);
        }

        #endregion


        #region Mappers

        VRMailMessageType VRMailMessageTypeMapper(IDataReader reader)
        {
            VRMailMessageType vrMailMessageType = new VRMailMessageType
            {
                VRMailMessageTypeId = (Guid) reader["ID"],
                Name = reader["Name"] as string,
                Settings = Vanrise.Common.Serializer.Deserialize<VRMailMessageTypeSettings>(reader["Settings"] as string) 
            };
            return vrMailMessageType;
        }

        #endregion
    }
}
