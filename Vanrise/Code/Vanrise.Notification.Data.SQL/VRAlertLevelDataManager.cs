using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Vanrise.Data.SQL;
using Vanrise.Notification.Entities;

namespace Vanrise.Notification.Data.SQL
{
   
    public class VRAlertLevelDataManager : BaseSQLDataManager, IVRAlertLevelDataManager
    {

        #region ctor/Local Variables
        public VRAlertLevelDataManager()
            : base(GetConnectionStringName("ConfigurationDBConnStringKey", "ConfigurationDBConnStringKey"))
        {

        }

        #endregion

        #region Public Methods
        public List<VRAlertLevel> GetAlertLevel()
        {
            return GetItemsSP("VRNotification.sp_VRAlertLevel_GetAll", VRAlertLevelMapper);
        }
        public bool AreAlertLevelUpdated(ref object updateHandle)
        {
            return base.IsDataUpdated("VRNotification.VRAlertLevel", ref updateHandle);
        }
        public bool Insert(VRAlertLevel alertLevelItem)
        {
            string serializedSettings = alertLevelItem.Settings != null ? Vanrise.Common.Serializer.Serialize(alertLevelItem.Settings) : null;
            int affectedRecords = ExecuteNonQuerySP("VRNotification.sp_VRAlertLevel_Insert", alertLevelItem.VRAlertLevelId, alertLevelItem.Name, alertLevelItem.BusinessEntityDefinitionId, serializedSettings);
            return (affectedRecords > 0);

        }
        public bool Update(VRAlertLevel alertLevelItem)
        {
            string serializedSettings = alertLevelItem.Settings != null ? Vanrise.Common.Serializer.Serialize(alertLevelItem.Settings) : null;
            int affectedRecords = ExecuteNonQuerySP("VRNotification.sp_VRAlertLevel_Update", alertLevelItem.VRAlertLevelId, alertLevelItem.Name, alertLevelItem.BusinessEntityDefinitionId, serializedSettings);
            return (affectedRecords > 0);
        }
        #endregion

        #region Mappers
        VRAlertLevel VRAlertLevelMapper(IDataReader reader)
        {
            VRAlertLevel alertLevel = new VRAlertLevel
            {
                VRAlertLevelId = (Guid)reader["ID"],
                Name = reader["Name"] as string,
                BusinessEntityDefinitionId = GetReaderValue<Guid>(reader, "BusinessEntityDefinitionID"),
                Settings = reader["Settings"] as string != null ? Vanrise.Common.Serializer.Deserialize<VRAlertLevelSettings>(reader["Settings"] as string) : null,
            };
            return alertLevel;
        }

        #endregion
    }
}
