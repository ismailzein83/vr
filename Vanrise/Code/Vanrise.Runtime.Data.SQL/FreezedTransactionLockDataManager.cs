using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Vanrise.Data.SQL;
using Vanrise.Runtime.Entities;

namespace Vanrise.Runtime.Data.SQL
{
    public class FreezedTransactionLockDataManager : BaseSQLDataManager, IFreezedTransactionLockDataManager
    {
        public FreezedTransactionLockDataManager()
            : base(GetConnectionStringName("RuntimeConnStringKey", "RuntimeDBConnString"))
        {

        }

        public void SaveFreezedLockTransaction(IEnumerable<Guid> transactionLockItemId)
        {
            ExecuteNonQuerySP("[runtime].[sp_FreezedTransactionLock_Insert]", String.Join(",", transactionLockItemId));
        }

        public List<Entities.FreezedTransactionLock> GetFreezedTransactionLocks()
        {
            return GetItemsSP("[runtime].[sp_FreezedTransactionLock_GetAll]", (reader) =>
                {
                    return new FreezedTransactionLock
                    {
                        FreezedTransactionLockId = (long)reader["ID"],
                        TransactionLockItemIds = (reader["TransactionLockItemIds"] as string).Split(',').Select(itm => Guid.Parse(itm)).ToList()
                    };
                });
        }

        public void DeleteBeforeId(long beforeId)
        {
            ExecuteNonQuerySP("[runtime].[sp_FreezedTransactionLock_DeleteBeforeId]", beforeId);
        }
    }
}
