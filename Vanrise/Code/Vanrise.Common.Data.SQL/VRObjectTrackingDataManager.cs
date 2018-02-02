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

        public long Insert(int userId, Guid loggableEntityId, string objectId, object obj, int actionId, string actionDescription,Object technicalInformation, VRActionAuditChangeInfo vrActionAuditChangeInfo)
        {
            object objectTrackingId;
           
            string serializedVRActionAuditChangeInfo = null;
            if (vrActionAuditChangeInfo != null)
                serializedVRActionAuditChangeInfo = Vanrise.Common.Serializer.Serialize(vrActionAuditChangeInfo);

            string serializedTechnicalInformation = null;
            if (technicalInformation != null)
                serializedTechnicalInformation = Vanrise.Common.Serializer.Serialize(technicalInformation);

            string serializedObj = null;
            if (obj != null)
                serializedObj = Vanrise.Common.Serializer.Serialize(obj);

            ExecuteNonQuerySP("logging.sp_ObjectTracking_Insert", out objectTrackingId, userId, loggableEntityId, objectId, serializedObj, actionId, actionDescription, technicalInformation, serializedVRActionAuditChangeInfo);
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
        public VRActionAuditChangeInfo GetVRActionAuditChangeInfoDetailById(int vrObjectTrackingId)
        {
            return GetItemSP("[logging].[sp_ObjectTracking_GetChangeInfoById]", VRActionAuditChangeInfoDetailMapper, vrObjectTrackingId);
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
                HasChangeInfo = GetReaderValue<bool>(reader, "HasChangeInfo"),
                ActionDescription = reader["ActionDescription"] as string,
            };

            return ObjectTracking;
        }

        private object ObjectDetailMapper(IDataReader reader)
        {
            return Vanrise.Common.Serializer.Deserialize<object>(reader["ObjectDetails"] as string);
        }
        private VRActionAuditChangeInfo VRActionAuditChangeInfoDetailMapper(IDataReader reader)
        {
            return Vanrise.Common.Serializer.Deserialize<VRActionAuditChangeInfo>(reader["ChangeInfo"] as string);
        }

    }
}
