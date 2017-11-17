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
    public class VRLocalizationModuleDataManager: BaseSQLDataManager,IVRLocalizationModuleDataManager
    {
                public VRLocalizationModuleDataManager()
            : base(GetConnectionStringName("VRLocalizationDBConnStringKey", "VRLocalizationDBConnString"))
        {

        }

        public List<VRLocalizationModule> GetVRLocalizationModules()
        {
            return GetItemsSP("[VRLocalization].[sp_Module_GetAll]", VRLocalizationModuleMapper);
        }

        public bool Update(VRLocalizationModule localizationModule)
        {
             return ExecuteNonQuerySP("[VRLocalization].[sp_Module_Update]", localizationModule.VRLocalizationModuleId, localizationModule.Name) > 0;
        }

        public bool Insert(VRLocalizationModule localizationModule)
        {
            return ExecuteNonQuerySP("[VRLocalization].[sp_Module_Insert]", localizationModule.VRLocalizationModuleId, localizationModule.Name) > 0;
        }

        public bool AreVRLocalizationModulesUpdated(ref object updateHandle)
        {
            return base.IsDataUpdated("[VRLocalization].[Module]", ref updateHandle);
        }

        private VRLocalizationModule VRLocalizationModuleMapper(IDataReader reader)
        {
            VRLocalizationModule vrLocalizationModule = new VRLocalizationModule
            {
                VRLocalizationModuleId = (Guid)reader["ID"],
                Name = reader["Name"] as string
            };

            return vrLocalizationModule;
        }
    }
}
