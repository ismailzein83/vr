using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Vanrise.Entities;
using Vanrise.Common;
using Vanrise.Data.SQL;
using System.Data;

namespace Vanrise.Common.Data.SQL
{
    class VRApplicationVisibilityDataManager : BaseSQLDataManager, IVRApplicationVisibilityDataManager
    {
        #region ctor/Local Variables
        public VRApplicationVisibilityDataManager()
            : base(GetConnectionStringName("ConfigurationDBConnStringKey", "ConfigurationDBConnString"))
        {

        }

        #endregion

        #region Public Methods

        public List<VRApplicationVisibility> GetVRApplicationVisibilities()
        {
            return GetItemsSP("common.sp_VRAppVisibility_GetAll", VRApplicationVisibilityMapper);
        }

        public bool AreVRApplicationVisibilityUpdated(ref object updateHandle)
        {
            return base.IsDataUpdated("common.VRAppVisibility", ref updateHandle);
        }

        public bool Insert(VRApplicationVisibility vrApplicationVisibilityItem)
        {
            string serializedSettings = vrApplicationVisibilityItem.Settings != null ? Vanrise.Common.Serializer.Serialize(vrApplicationVisibilityItem.Settings) : null;
            int affectedRecords = ExecuteNonQuerySP("common.sp_VRAppVisibility_Insert", vrApplicationVisibilityItem.VRApplicationVisibilityId, vrApplicationVisibilityItem.Name, serializedSettings);

            if (affectedRecords > 0)
            {
                return true;
            }

            return false;
        }

        public bool Update(VRApplicationVisibility vrApplicationVisibilityItem)
        {
            string serializedSettings = vrApplicationVisibilityItem.Settings != null ? Vanrise.Common.Serializer.Serialize(vrApplicationVisibilityItem.Settings) : null;
            int affectedRecords = ExecuteNonQuerySP("common.sp_VRAppVisibility_Update", vrApplicationVisibilityItem.VRApplicationVisibilityId, vrApplicationVisibilityItem.Name, serializedSettings);
            return (affectedRecords > 0);
        }

        #endregion

        #region Mappers

        VRApplicationVisibility VRApplicationVisibilityMapper(IDataReader reader)
        {
            VRApplicationVisibility vrApplicationVisibility = new VRApplicationVisibility
            {
                VRApplicationVisibilityId = (Guid)reader["ID"],
                Name = reader["Name"] as string,
                Settings = Vanrise.Common.Serializer.Deserialize<VRApplicationVisibilitySettings>(reader["Settings"] as string)
            };
            return vrApplicationVisibility;
        }

        #endregion
    }
}
