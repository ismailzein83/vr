using SOM.Main.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SOM.Main.Data
{
    public interface ISOMRequestDataManager : IDataManager
    {
        void AddRequest(Guid requestId, Guid requestTypeId, string entityId, string title, string serializedSettings);

        void UpdateRequestProcessInstanceId(Guid requestId, long processInstanceId);

        long? GetRequestProcessInstanceId(Guid somRequestId);

        List<SOMRequestHeader> GetRecentSOMRequestHeaders(string entityId, int nbOfRecords, long? lessThanSequenceNb);
    }
}
