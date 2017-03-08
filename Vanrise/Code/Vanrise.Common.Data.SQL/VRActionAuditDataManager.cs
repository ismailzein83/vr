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
    public class VRActionAuditDataManager : BaseSQLDataManager, IVRActionAuditDataManager
    {
        #region public Methods
        public VRActionAuditDataManager()
            : base(GetConnectionStringName("LoggingDBConnStringKey", "LogDBConnString"))
        {

        }

        public void Insert(int? userId, int? urlId, int moduleId, int entityId, int actionId, string objectId, string objectName, long? objectTrackingId, string actionDescription)
        {
            ExecuteNonQuerySP("[logging].[sp_ActionAudit_Insert]", userId, urlId, moduleId, entityId, actionId, objectId, objectName, objectTrackingId, actionDescription);
        }
        public List<VRActionAudit> GetAll(VRActionAuditQuery query)
        {
            string userIds = null;
            string moduleIds = null;
            string actionIds = null;
            string entityIds = null;
            if (query.UserIds != null && query.UserIds.Count > 0)
                userIds = string.Join(",", query.UserIds);
            if (query.ModuleIds != null && query.ModuleIds.Count > 0)
                moduleIds = string.Join(",", query.ModuleIds);
            if (query.ActionIds != null && query.ActionIds.Count > 0)
                actionIds = string.Join(",", query.ActionIds);
            if (query.EntityIds != null && query.EntityIds.Count > 0)
                entityIds = string.Join(",", query.EntityIds);
            return GetItemsSP("[logging].[sp_ActionAudit_GetFilterd]", ActionAuditMapper,
                    query.TopRecord,
                    userIds,
                    moduleIds,
                    actionIds,
                    entityIds,
                    query.ObjectId,
                    query.ObjectName,
                    query.FromTime,
                    query.ToTime
                );
        }
        #endregion

        #region Mappers
        private VRActionAudit ActionAuditMapper(IDataReader reader)
        {
           VRActionAudit ActionAudit = new VRActionAudit
            {
                VRActionAuditId = (long)reader["ID"],
                UserId = GetReaderValue<int?>(reader, "UserId"),
                ModuleId = GetReaderValue<int>(reader, "ModuleID") ,
                EntityId = GetReaderValue<int>(reader, "EntityID") ,
                ActionId = GetReaderValue<int>(reader, "ActionId"),
                UrlId = GetReaderValue<int>(reader, "URLID") ,
                ObjectId = GetReaderValue<string>(reader, "ObjectID"),
                ObjectName = GetReaderValue<string>(reader, "ObjectName"),
                LogTime = GetReaderValue<DateTime>(reader, "LogTime")

            };

            return ActionAudit;
        }
        #endregion
    }
}
