using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Vanrise.BusinessProcess.Data
{
    public interface IBPEventDataManager : IDataManager
    {
        IEnumerable<Entities.BPEvent> GetDefinitionEvents(int definitionId);

        IEnumerable<Entities.BPEvent> GetInstancesEvents(int definitionId, List<long> instancesIds);

        void DeleteEvent(long eventId);

        void InsertEvent(long processInstanceId, string bookmarkName, Object eventData);
    }
}
