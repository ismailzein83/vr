using System;
using System.Collections.Generic;
using System.Data;
using Vanrise.Entities;
using Vanrise.Data.SQL;

namespace Vanrise.Common.Data.SQL
{
    public class VRConnectionDataManager : BaseSQLDataManager, IVRConnectionDataManager
    {

        public VRConnectionDataManager()
            : base(GetConnectionStringName("ConfigurationDBConnStringKey", "ConfigurationDBConnString"))
        {

        }

        public List<VRConnection> GetVRConnections()
        {
            return GetItemsSP("common.sp_Connection_GetAll", VRConnectionMapper);
        }


        public bool Update(VRConnection connection)
        {
            string serializedSettings = connection.Settings != null ? Vanrise.Common.Serializer.Serialize(connection.Settings) : null;

            return ExecuteNonQuerySP("common.sp_Connection_Update", connection.VRConnectionId, connection.Name, serializedSettings) > 0;
        }

        public bool Insert(VRConnection connection)
        {
            string serializedSettings = connection.Settings != null ? Vanrise.Common.Serializer.Serialize(connection.Settings) : null;
            return ExecuteNonQuerySP("common.sp_Connection_Insert", connection.VRConnectionId, connection.Name, serializedSettings) > 0;
        }


        public bool AreVRConnectionsUpdated(ref object updateHandle)
        {
            return base.IsDataUpdated("common.Connection", ref updateHandle);
        }

        private VRConnection VRConnectionMapper(IDataReader reader)
        {
            VRConnection vrConnection = new VRConnection
            {
                VRConnectionId = (Guid)reader["ID"],
                Name = reader["Name"] as string,
                Settings = Vanrise.Common.Serializer.Deserialize<VRConnectionSettings>(reader["Settings"] as string)
            };

            return vrConnection;
        }
    }
}
