using Retail.BusinessEntity.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Retail.BusinessEntity.Data
{
    public interface IAccountDataManager : IDataManager
    {
        IEnumerable<Account> GetAccounts();
        bool UpdateStatus(long accountId, Guid statusId);
        bool UpdateExecutedActions(long accountId, ExecutedActions executedAction);
        bool Insert(Account account, out long insertedId);

        bool Update(AccountToEdit account, long? parentId);

        bool AreAccountsUpdated(ref object updateHandle);
    }
}
