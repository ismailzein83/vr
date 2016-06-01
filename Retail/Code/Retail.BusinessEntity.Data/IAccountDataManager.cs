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

        bool Insert(Account account, out int insertedId);

        bool Update(AccountToEdit account, int? parentId);

        bool AreAccountsUpdated(ref object updateHandle);
    }
}
