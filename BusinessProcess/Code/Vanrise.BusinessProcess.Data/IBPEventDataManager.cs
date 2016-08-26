using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Vanrise.BusinessProcess.Data
{
    public interface IBPEventDataManager : IDataManager
    {
        IEnumerable<Entities.BPEvent> GetInstancesEvents(List<long> instancesIds);

        void DeleteEvent(long eventId);

        int InsertEvent(long processInstanceId, string bookmarkName, Object eventData);

        List<long> GetEventsDistinctProcessInstanceIds();

        void DeleteProcessInstanceEvents(long processInstanceId);
    }
}
