using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Vanrise.Entities;

namespace Vanrise.Common.Data.SQL
{
    public class VRLoggableEntityDataManager : Vanrise.Data.SQL.BaseSQLDataManager, IVRLoggableEntityDataManager
    {
        public VRLoggableEntityDataManager()
            : base(GetConnectionStringName("LoggingDBConnStringKey", "LogDBConnString"))
        {

        }
        public int AddOrUpdateLoggableEntity(string entityUniqueName, VRLoggableEntitySettings loggableEntitySettings)
        {
            return (int)ExecuteScalarSP("[logging].[sp_LoggableEntity_InsertOrUpdate]", entityUniqueName, Vanrise.Common.Serializer.Serialize(loggableEntitySettings));
        }

        public List<VRLoggableEntity> GetAll()
        {
            return GetItemsSP("[logging].[sp_LoggableEntity_GetAll]", 
                (reader) =>
                {
                    return new VRLoggableEntity
                    {
                        VRLoggableEntityId = (int)reader["ID"],
                        UniqueName = reader["UniqueName"] as string,
                        Settings = Vanrise.Common.Serializer.Deserialize<VRLoggableEntitySettings>(reader["Settings"] as string)
                    };
                });
        }
    }
}
