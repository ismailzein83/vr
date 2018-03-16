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
        IEnumerable<Account> GetAccounts(List<Guid> accountTypeIds, byte[] afterLastTimeStamp);

        bool Insert(AccountToInsert accountToInsert, out long insertedId);

        bool Update(AccountToEdit accountToEdit);

        bool AreAccountsUpdated(List<Guid> accountTypeIds, ref object updateHandle);

        bool UpdateStatus(long accountId, Guid statusId, int lastModifiedBy);

        bool UpdateExtendedSettings(long accountId, Dictionary<string, BaseAccountExtendedSettings> extendedSettings, int lastModifiedBy);


        byte[] GetMaxTimeStamp(List<Guid> accountTypeIds);
    }
}
