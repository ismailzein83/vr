using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Data;
using Vanrise.Data.SQL;
using Vanrise.Data;
using Vanrise.Entities;

namespace Vanrise.Common.Data.SQL
{
        public class VRDynamicAPIModuleDataManager : BaseSQLDataManager, IVRDynamicAPIModuleDataManager
        {
            public VRDynamicAPIModuleDataManager() :
                base(GetConnectionStringName("ConfigurationDBConnStringKey", "ConfigurationDBConnStringKey"))
            {
            }

            public List<VRDynamicAPIModule> GetVRDynamicAPIModules()
            {
                return GetItemsSP("[common].[sp_VRDynamicAPIModule_GetAll]", VRDynamicAPIModuleMapper);
            }

            public bool Insert(VRDynamicAPIModule vrDynamicAPIModule, out int insertedId)
            {
                object id;

                int nbOfRecordsAffected = ExecuteNonQuerySP("[common].[sp_VRDynamicAPIModule_Insert]", out id, vrDynamicAPIModule.Name, vrDynamicAPIModule.CreatedBy, vrDynamicAPIModule.LastModifiedBy);
                insertedId = Convert.ToInt32(id);

                return (nbOfRecordsAffected > 0);
            }

            public bool Update(VRDynamicAPIModule vrDynamicAPIModule)
            {
               
                int nbOfRecordsAffected = ExecuteNonQuerySP("[common].[sp_VRDynamicAPIModule_Update]", vrDynamicAPIModule.VRDynamicAPIModuleId, vrDynamicAPIModule.Name, vrDynamicAPIModule.LastModifiedBy);
                return (nbOfRecordsAffected > 0);
            }

            public bool AreVRDynamicAPIModulesUpdated(ref object updateHandle)
            {
                return base.IsDataUpdated("[common].[VRDynamicAPIModule]", ref updateHandle);
            }
        
            VRDynamicAPIModule VRDynamicAPIModuleMapper(IDataReader reader)
            {
                return new VRDynamicAPIModule
                {
                    VRDynamicAPIModuleId = GetReaderValue<int>(reader, "ID"),
                    Name = GetReaderValue<string>(reader, "Name"),
                    CreatedTime= GetReaderValue<DateTime>(reader, "CreatedTime"),
                    CreatedBy=GetReaderValue<int>(reader, "CreatedBy"),
                    LastModifiedTime= GetReaderValue<DateTime>(reader, "LastModifiedTime"),
                    LastModifiedBy= GetReaderValue<int>(reader, "LastModifiedBy")
                };
            }

        } 
}
