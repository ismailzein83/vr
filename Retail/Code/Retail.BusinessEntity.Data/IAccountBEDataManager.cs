using Retail.BusinessEntity.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Retail.BusinessEntity.Data
{
    public interface IAccountBEDataManager : IDataManager
    {
        IEnumerable<Account> GetAccounts(Guid accountBEDefinitionId);

        bool Insert(AccountToInsert accountToInsert, out long insertedId);

        bool Update(AccountToEdit accountToEdit);

        bool AreAccountsUpdated(Guid accountBEDefinitionId, ref object updateHandle);

        bool UpdateStatus(long accountId, Guid statusId);

        bool UpdateExtendedSettings(long accountId, Dictionary<string, BaseAccountExtendedSettings> extendedSettings);

    }
}
