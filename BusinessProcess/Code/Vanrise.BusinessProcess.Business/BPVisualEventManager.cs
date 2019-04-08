using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Vanrise.Common;
using Vanrise.BusinessProcess.Data;
using Vanrise.BusinessProcess.Entities;

namespace Vanrise.BusinessProcess.Business
{
    public class BPVisualEventManager
    {
        IBPVisualEventDataManager s_dataManager = BPDataManagerFactory.GetDataManager<IBPVisualEventDataManager>();

        public void InsertVisualEvent(long processInstanceId, Guid activityId, string title, Guid eventTypeId, BPVisualEventPayload eventPayload)
        {
            s_dataManager.InsertVisualEvent(processInstanceId, activityId, title, eventTypeId, Serializer.Serialize(eventPayload));
        }
    }
}
