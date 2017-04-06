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
    public class VRObjectTrackingDataManager : Vanrise.Data.SQL.BaseSQLDataManager, IVRObjectTrackingDataManager
    {
        public VRObjectTrackingDataManager()
            : base(GetConnectionStringName("LoggingDBConnStringKey", "LogDBConnString"))
        {

        }

        public long Insert(int userId, Guid loggableEntityId, string objectId, object obj, int actionId, string actionDescription)
        {
            object objectTrackingId;
            ExecuteNonQuerySP("logging.sp_ObjectTracking_Insert", out objectTrackingId, userId, loggableEntityId, objectId, obj != null ? Vanrise.Common.Serializer.Serialize(obj) : null, actionId, actionDescription);
            return (long)objectTrackingId;
        }

        public List<VRObjectTrackingMetaData> GetAll(Guid loggableEntityId, string objectId)
        {

            return GetItemsSP("[logging].[sp_ObjectTracking_GetFiltered]", ObjectTrackingMapper,
                    loggableEntityId,
                    objectId
                );
        }

        public object GetObjectDetailById(int VRObjectTrackingId)
        {
            return GetItemSP("[logging].[sp_ObjectTracking_GetObjectDetailsById]", ObjectDetailMapper, VRObjectTrackingId);
        }

        private VRObjectTrackingMetaData ObjectTrackingMapper(IDataReader reader)
        {
            VRObjectTrackingMetaData ObjectTracking = new VRObjectTrackingMetaData
            {
                VRObjectTrackingId = (long)reader["ID"],
                Time = GetReaderValue<DateTime>(reader, "LogTime"),
                UserId = GetReaderValue<int>(reader, "UserID"),
                ActionId = GetReaderValue<int>(reader, "ActionID"),
                HasDetail = GetReaderValue<bool>(reader, "HasDetail"),
            };

            return ObjectTracking;
        }

        private object ObjectDetailMapper(IDataReader reader)
        {
            return Vanrise.Common.Serializer.Deserialize<object>(reader["ObjectDetails"] as string);
        }


    }
}
