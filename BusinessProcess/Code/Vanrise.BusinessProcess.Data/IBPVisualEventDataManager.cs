using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Vanrise.BusinessProcess.Entities;

namespace Vanrise.BusinessProcess.Data
{
    public interface IBPVisualEventDataManager : IDataManager
    {
        void InsertVisualEvent(long processInstanceId, Guid activityId, string title, Guid eventTypeId, string eventPayload);

        List<BPVisualEvent> GetAfterId(BPVisualEventDetailUpdateInput input);
    }
}
