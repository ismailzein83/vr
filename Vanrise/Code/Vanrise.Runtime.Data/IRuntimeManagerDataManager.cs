using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Vanrise.Runtime.Data
{
    public interface IRuntimeManagerDataManager : IDataManager
    {
        string GetRuntimeManagerServiceURL(out Guid runtimeNodeInstanceId);

        bool TryTakePrimaryNode(Guid serviceInstanceId, TimeSpan heartbeatTimeOut);
    }
}
