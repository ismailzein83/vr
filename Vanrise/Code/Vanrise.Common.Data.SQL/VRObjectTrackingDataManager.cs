using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

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
    }
}
