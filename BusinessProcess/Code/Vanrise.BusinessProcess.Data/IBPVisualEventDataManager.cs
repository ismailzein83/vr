using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Vanrise.BusinessProcess.Data
{
    public interface IBPVisualEventDataManager : IDataManager
    {
        void InsertVisualEvent(long processInstanceId, Guid activityId, string title, Guid eventTypeId, string eventPayload);
    }
}
