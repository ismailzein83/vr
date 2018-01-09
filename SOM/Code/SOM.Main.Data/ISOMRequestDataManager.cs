using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SOM.Main.Data
{
    public interface ISOMRequestDataManager : IDataManager
    {
        void AddRequest(Guid requestTypeId, string entityId, string serializedSettings, out long requestId);

        void UpdateRequestProcessInstanceId(long requestId, long processInstanceId);
    }
}
