using Retail.BusinessEntity.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Retail.BusinessEntity.Data
{
    public interface IAccountTypeDataManager : IDataManager
    {
        IEnumerable<AccountType2> GetAccountTypes();

        bool Insert(AccountType2 accountType, out int insertedId);

        bool Update(AccountTypeToEdit accountType);

        bool AreAccountTypesUpdated(ref object updateHandle);
    }
}
