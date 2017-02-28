using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TOne.WhS.AccountBalance.Entities;

namespace TOne.WhS.AccountBalance.Data
{
    public interface IFinancialAccountDataManager:IDataManager
    {
        List<FinancialAccount> GetFinancialAccounts();
        bool Update(FinancialAccount financialAccount);
        bool Insert(FinancialAccount financialAccount, out int insertedId);
        bool AreFinancialAccountsUpdated(ref object updateHandle);
    }
}
