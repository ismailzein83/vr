using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Vanrise.Data.SQL;
using Vanrise.Entities;
using System.Data;

namespace Vanrise.Common.Data.SQL
{
    public class VRLocalizationTextResourceDataManager: BaseSQLDataManager,IVRLocalizationTextResourceDataManager
    {
        #region public methods
        public VRLocalizationTextResourceDataManager()
            : base(GetConnectionStringName("VRLocalizationDBConnStringKey", "VRLocalizationDBConnString"))
        {

        }

        public List<VRLocalizationTextResource> GetVRLocalizationTextResources()
        {
            return GetItemsSP("[VRLocalization].[sp_TextResource_GetAll]", VRLocalizationTextResourceMapper);
        }

        public bool Update(VRLocalizationTextResource localizationTextResource)
        {
            return ExecuteNonQuerySP("[VRLocalization].[sp_TextResource_Update]", localizationTextResource.VRLocalizationTextResourceId, localizationTextResource.ResourceKey, localizationTextResource.ModuleId) > 0;
        }

        public bool Insert(VRLocalizationTextResource localizationTextResource)
        {
            return ExecuteNonQuerySP("[VRLocalization].[sp_TextResource_Insert]", localizationTextResource.VRLocalizationTextResourceId, localizationTextResource.ResourceKey, localizationTextResource.ModuleId) > 0;
        }

        public bool AreVRLocalizationTextResourcesUpdated(ref object updateHandle)
        {
            return base.IsDataUpdated("[VRLocalization].[TextResource]", ref updateHandle);
        }
        #endregion 

        #region private methods
        private VRLocalizationTextResource VRLocalizationTextResourceMapper(IDataReader reader)
        {
            VRLocalizationTextResource vrLocalizationTextResource = new VRLocalizationTextResource
            {
                VRLocalizationTextResourceId = (Guid)reader["ID"],
                ResourceKey = reader["ResourceKey"] as string,
                ModuleId = (Guid)reader["ModuleID"],
                Settings = Vanrise.Common.Serializer.Deserialize<VRLocalizationTextResourceSettings>(reader["Settings"] as string),
            };

            return vrLocalizationTextResource;
        }
        #endregion

    }
}
