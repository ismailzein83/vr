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
    public class VRTempPayloadDataManager:BaseSQLDataManager, IVRTempPayloadDataManager
    {
        public VRTempPayloadDataManager()
            : base(GetConnectionStringName("ConfigurationDBConnStringKey", "ConfigurationDBConnStringKey"))
        {
        }

        public bool Insert(VRTempPayload vrTempPayload)
        {
            string serializedSettings = null;
            if (vrTempPayload.Settings != null)
                serializedSettings = Vanrise.Common.Serializer.Serialize(vrTempPayload.Settings);

            int recordsEffected = ExecuteNonQuerySP("[common].[sp_VRTempPayload_Insert]", vrTempPayload.VRTempPayloadId, serializedSettings, vrTempPayload.CreatedBy);
            return (recordsEffected > 0);
        }
        public bool DeleteVRTempPayload(Guid vrTempPayloadId)
        {
            int recordsEffected = ExecuteNonQuerySP("[common].[sp_VRTempPayload_Delete]", vrTempPayloadId);
            return (recordsEffected > 0);
        }
        public VRTempPayload GetVRTempPayload(Guid vrTempPayloadId)
        {
            return GetItemSP("[common].[sp_VRTempPayload_Get]", VRTempPayloadMapper);
        }

        VRTempPayload VRTempPayloadMapper(IDataReader reader)
        {
            return new VRTempPayload
            {
                VRTempPayloadId = GetReaderValue<Guid>(reader, "ID"),
                Settings = Vanrise.Common.Serializer.Deserialize<VRTempPayloadSettings>(reader["Settings"] as string),
                CreatedBy = GetReaderValue<int>(reader, "CreatedBy"),
                CreatedTime = GetReaderValue<DateTime>(reader, "CreatedTime"),
            };
            
        }

    }
}
