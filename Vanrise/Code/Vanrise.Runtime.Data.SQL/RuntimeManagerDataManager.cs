using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Vanrise.Data.SQL;

namespace Vanrise.Runtime.Data.SQL
{
    public class RuntimeManagerDataManager : BaseSQLDataManager, IRuntimeManagerDataManager
    {
        public RuntimeManagerDataManager()
            : base(GetConnectionStringName("RuntimeConnStringKey", "RuntimeDBConnString"))
        {
        }

        public string GetRuntimeManagerServiceURL()
        {
            return ExecuteScalarSP("[runtime].[sp_RuntimeManager_GetServiceURL]") as string;
        }

        public bool TryUpdateHeartBeat(Guid serviceInstanceId, string serviceURL, TimeSpan heartbeatTimeOut)
        {
            return Convert.ToBoolean(ExecuteScalarSP("[runtime].[sp_RuntimeManager_TryUpdateHeartBeat]", serviceInstanceId, serviceURL, heartbeatTimeOut.TotalSeconds));
        }
    }
}
