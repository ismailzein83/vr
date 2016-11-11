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
    public class VRComponentTypeDataManager : BaseSQLDataManager, IVRComponentTypeDataManager
    {

        public VRComponentTypeDataManager()
            : base(GetConnectionStringName("ConfigurationDBConnStringKey", "ConfigurationDBConnString"))
        {

        }


        public List<VRComponentType> GetComponentTypes()
        {
            return GetItemsSP("[common].[sp_VRComponentType_GetAll]", VRComponentTypeMapper);
        }

        VRComponentType VRComponentTypeMapper(IDataReader reader)
        {
            return new VRComponentType
            {
                VRComponentTypeId = GetReaderValue<Guid>(reader, "ID"),
                Name = reader["Name"] as string,
                Settings = Serializer.Deserialize<VRComponentTypeSettings>(reader["Settings"] as string)
            };
        }

        public bool Insert(VRComponentType componentType)
        {
            string serializedSettings = componentType.Settings != null ? Vanrise.Common.Serializer.Serialize(componentType.Settings) : null;
            int affectedRecords = ExecuteNonQuerySP("[common].[sp_VRComponentType_Insert]", componentType.VRComponentTypeId, componentType.Name, componentType.Settings.VRComponentTypeConfigId, serializedSettings);

            if (affectedRecords > 0)
                return true;

            return false;
        }

        public bool Update(VRComponentType componentType)
        {
            string serializedSettings = componentType.Settings != null ? Vanrise.Common.Serializer.Serialize(componentType.Settings) : null;
            int affectedRecords = ExecuteNonQuerySP("[common].[sp_VRComponentType_Update]", componentType.VRComponentTypeId, componentType.Name, componentType.Settings.VRComponentTypeConfigId, serializedSettings);
            return (affectedRecords > 0);
        }

        public bool AreVRComponentTypeUpdated(ref object updateHandle)
        {
            return base.IsDataUpdated("common.VRComponentType", ref updateHandle);
        }
    }
}
