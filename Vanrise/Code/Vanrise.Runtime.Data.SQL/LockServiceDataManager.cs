using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Vanrise.Data.SQL;
using Vanrise.Runtime.Entities;

namespace Vanrise.Runtime.Data.SQL
{
    public class LockServiceDataManager : BaseSQLDataManager, ILockServiceDataManager
    {
        public LockServiceDataManager()
            : base(GetConnectionStringName("RuntimeConnStringKey", "RuntimeDBConnString"))
        {
        }

        public bool TryLock(int currentRuntimeProcessId, IEnumerable<int> runningRuntimeProcessesIds, out Entities.TransactionLockingDetails lockingDetails)
        {
            TransactionLockingDetails lockingDetails_Local = null;
            bool isLocked = false;
            ExecuteReaderSP("[runtime].[sp_LockService_TryLock]", (reader) => {
                if (reader.Read())
                {
                    isLocked = true;
                    var serializiedLockingDetails = reader["LockingDetails"] as string;
                    if (serializiedLockingDetails != null)
                        lockingDetails_Local = Common.Serializer.Deserialize<TransactionLockingDetails>(serializiedLockingDetails);
                }
            }, currentRuntimeProcessId, String.Join(",", runningRuntimeProcessesIds));
            lockingDetails = lockingDetails_Local;
            return isLocked;
        }

        public void UpdateServiceURL(string serviceUrl)
        {
            ExecuteNonQuerySP("[runtime].[sp_LockService_UpdateServiceURL]", serviceUrl);
        }

        public bool UpdateLockingDetails(int currentRuntimeProcessId, Entities.TransactionLockingDetails lockingDetails)
        {
            return ExecuteNonQuerySP("[runtime].[sp_LockService_UpdateLockingDetails]", currentRuntimeProcessId, lockingDetails != null ? Common.Serializer.Serialize(lockingDetails) : null) > 0;
        }

        public string GetServiceURL()
        {
            return ExecuteScalarSP("[runtime].[sp_LockService_GetServiceURL]") as string;
        }
    }
}
