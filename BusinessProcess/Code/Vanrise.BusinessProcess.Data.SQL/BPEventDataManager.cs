using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Vanrise.BusinessProcess.Entities;
using Vanrise.Common;
using Vanrise.Data.SQL;

namespace Vanrise.BusinessProcess.Data.SQL
{
    public class BPEventDataManager : BaseSQLDataManager, IBPEventDataManager
    {
        public BPEventDataManager()
            : base(GetConnectionStringName("BusinessProcessDBConnStringKey", "BusinessProcessDBConnString"))
        {

        }

        public IEnumerable<Entities.BPEvent> GetDefinitionEvents(int definitionId)
        {
            return GetItemsSP("[bp].[sp_BPEvent_GetByDefinitionID]", BPEventMapper, definitionId);
        }

        public IEnumerable<Entities.BPEvent> GetInstancesEvents(int definitionId, List<long> instancesIds)
        {
            string instanceIdsString = null;
            if (instancesIds != null)
                instanceIdsString = String.Join(",", instancesIds);
            return GetItemsSP("[bp].[sp_BPEvent_GetByInstanceIDs]", BPEventMapper, definitionId, instanceIdsString);
        }

        public void DeleteEvent(long eventId)
        {
            ExecuteNonQuerySP("bp.sp_BPEvent_Delete", eventId);
        }

        public int InsertEvent(long processInstanceId, string bookmarkName, object eventData)
        {
            return ExecuteNonQuerySP("bp.sp_BPEvent_Insert", processInstanceId, bookmarkName, eventData != null ? Serializer.Serialize(eventData) : null);
        }

        #region Private Methods

        BPEvent BPEventMapper(IDataReader reader)
        {
            BPEvent instance = new BPEvent
            {
                BPEventID = (long)reader["ID"],
                ProcessInstanceID = (long)reader["ProcessInstanceID"],
                ProcessDefinitionID = (int)reader["DefinitionID"],
                Bookmark = reader["Bookmark"] as string
            };
            string payload = reader["Payload"] as string;
            if (!String.IsNullOrWhiteSpace(payload))
                instance.Payload = Serializer.Deserialize(payload);
            return instance;
        }

        #endregion
    }
}
