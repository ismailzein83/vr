using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
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
    }
}
