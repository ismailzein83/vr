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
    public class BPVisualEventDataManager : BaseSQLDataManager, IBPVisualEventDataManager
    {
        public BPVisualEventDataManager()
            : base(GetConnectionStringName("BusinessProcessTrackingDBConnStringKey", "BusinessProcessTrackingDBConnString"))
        {
        }

        public void InsertVisualEvent(long processInstanceId, Guid activityId, string title, Guid eventTypeId, string eventPayload)
        {
            ExecuteNonQuerySP("bp.sp_BPVisualEvent_Insert", processInstanceId, activityId, title, eventTypeId, eventPayload);
        }

        public List<BPVisualEvent> GetAfterId(BPVisualEventDetailUpdateInput input)
        {
            List<BPVisualEvent> bpVisualEvent = new List<BPVisualEvent>();

            ExecuteReaderSP("[bp].[sp_BPVisualEvent_GetAfterId]", (reader) =>
            {
                while (reader.Read())
                    bpVisualEvent.Add(BPVisualEventMapper(reader));
            },
              ToDBNullIfDefault(input.GreaterThanID), input.BPInstanceID);

            return bpVisualEvent;
        }

        #region Private Methods

        BPVisualEvent BPVisualEventMapper(IDataReader reader)
        {
            var bpTrackingMessage = new BPVisualEvent
            {
                BPVisualEventId = (long)reader["ID"],
                ProcessInstanceId = (long)reader["ProcessInstanceID"],
                ActivityId = (Guid)reader["ActivityID"],
                Title = reader["Title"] as string,
                EventTypeId = (Guid)reader["EventTypeID"],
                EventPayload = Serializer.Deserialize<BPVisualEventPayload>(reader["EventPayload"] as string),
                CreatedTime = (DateTime)reader["CreatedTime"] 
            };

            return bpTrackingMessage;
        }

        #endregion
    }
}
