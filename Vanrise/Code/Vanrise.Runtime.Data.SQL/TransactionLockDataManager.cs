using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Vanrise.Data.SQL;
using Vanrise.Runtime.Entities;

namespace Vanrise.Runtime.Data.SQL
{
    public class TransactionLockDataManager : BaseSQLDataManager, ITransactionLockDataManager
    {
        public TransactionLockDataManager()
            : base(GetConnectionStringName("RuntimeConnStringKey", "RuntimeDBConnString"))
        {
        }

        public void Add(TransactionLockItem lockItem)
        {
            ExecuteNonQuerySP("[runtime].[sp_TransactionLock_Insert]", lockItem.LockItemUniqueId, lockItem.TransactionUniqueName, lockItem.ProcessId);
        }

        public void Delete(Guid lockItemId)
        {
            ExecuteNonQuerySP("[runtime].[sp_TransactionLock_Delete]", lockItemId);
        }

        public List<TransactionLockItem> GetLocksForNotRunningProcesses(List<int> runningProcessesIds)
        {
            return GetItemsSP("[runtime].[sp_TransactionLock_GetForNotRunningProcesses]", TransactionLockItemMapper, String.Join(",", runningProcessesIds));
        }

        public List<TransactionLockItem> GetAll()
        {
            return GetItemsSP("[runtime].[sp_TransactionLock_GetAll]", TransactionLockItemMapper);
        }

        #region Private Methods

        private TransactionLockItem TransactionLockItemMapper(IDataReader reader)
        {
            return new TransactionLockItem
            {
                LockItemUniqueId = (Guid)reader["ID"],
                TransactionUniqueName = reader["TransactionUniqueName"] as string,
                ProcessId = (int)reader["ProcessID"]
            };
        }

        #endregion
    }
}