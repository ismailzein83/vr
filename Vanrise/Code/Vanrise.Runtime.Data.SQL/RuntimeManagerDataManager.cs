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

        public string GetRuntimeManagerServiceURL(out Guid runtimeNodeInstanceId)
        {
            string serviceURL = null;
            runtimeNodeInstanceId = Guid.Empty;
            Guid runtimeNodeInstanceId_Local = Guid.Empty;
            ExecuteReaderSP("[runtime].[sp_RuntimeManager_GetServiceURL]", (reader) =>
                {
                    if(reader.Read())
                    {
                        serviceURL = reader["ServiceURL"] as string;
                        runtimeNodeInstanceId_Local = (Guid)reader["RuntimeNodeInstanceID"];
                    }
                });
            runtimeNodeInstanceId = runtimeNodeInstanceId_Local;
            return serviceURL;
        }

        public bool TryTakePrimaryNode(Guid serviceInstanceId, TimeSpan heartbeatTimeOut)
        {
            return Convert.ToBoolean(ExecuteScalarSP("[runtime].[sp_RuntimeManager_TryTakePrimaryNode]", serviceInstanceId, heartbeatTimeOut.TotalSeconds));
        }
    }
}
