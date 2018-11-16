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
    public class VRDynamicAPIDataManager : BaseSQLDataManager, IVRDynamicAPIDataManager
    {
        public VRDynamicAPIDataManager() :
            base(GetConnectionStringName("ConfigurationDBConnStringKey", "ConfigurationDBConnStringKey"))
        {
        }

        public List<VRDynamicAPI> GetVRDynamicAPIs()
        {
            return GetItemsSP("[common].[sp_VRDynamicAPI_GetAll]", VRDynamicAPIMapper);
        }

        public bool Insert(VRDynamicAPI vrDynamicAPI, out int insertedId)
        {
            object id;

            string serializedVRDynamicAPISettings = null;

            if (vrDynamicAPI.Settings != null)
                serializedVRDynamicAPISettings = Vanrise.Common.Serializer.Serialize(vrDynamicAPI.Settings);
            int nbOfRecordsAffected = ExecuteNonQuerySP("[common].[sp_VRDynamicAPI_Insert]", out id, vrDynamicAPI.Name, vrDynamicAPI.ModuleId, serializedVRDynamicAPISettings, vrDynamicAPI.CreatedBy, vrDynamicAPI.LastModifiedBy);
            insertedId = Convert.ToInt32(id);

            return (nbOfRecordsAffected > 0);
        }

        public bool Update(VRDynamicAPI vrDynamicAPI)
        {
            string serializedVRDynamicAPISettings = null;

            if (vrDynamicAPI.Settings != null)
                serializedVRDynamicAPISettings = Vanrise.Common.Serializer.Serialize(vrDynamicAPI.Settings);
            int nbOfRecordsAffected = ExecuteNonQuerySP("[common].[sp_VRDynamicAPI_Update]", vrDynamicAPI.VRDynamicAPIId, vrDynamicAPI.Name, vrDynamicAPI.ModuleId, serializedVRDynamicAPISettings, vrDynamicAPI.LastModifiedBy);
            return (nbOfRecordsAffected > 0);
        }

        public bool AreVRDynamicAPIsUpdated(ref object updateHandle)
        {
            return base.IsDataUpdated("[common].[VRDynamicAPI]", ref updateHandle);
        }

        VRDynamicAPI VRDynamicAPIMapper(IDataReader reader)
        {
            return new VRDynamicAPI
            {
                VRDynamicAPIId = GetReaderValue<long>(reader, "ID"),
                Name = GetReaderValue<string>(reader, "Name"),
                ModuleId= GetReaderValue<int>(reader, "ModuleId"),
                Settings = Vanrise.Common.Serializer.Deserialize<VRDynamicAPISettings>(reader["Settings"] as string),
                CreatedTime = GetReaderValue<DateTime>(reader, "CreatedTime"),
                CreatedBy = GetReaderValue<int>(reader, "CreatedBy"),
                LastModifiedTime = GetReaderValue<DateTime>(reader, "LastModifiedTime"),
                LastModifiedBy = GetReaderValue<int>(reader, "LastModifiedBy")
            }; 
        }

    }
}
