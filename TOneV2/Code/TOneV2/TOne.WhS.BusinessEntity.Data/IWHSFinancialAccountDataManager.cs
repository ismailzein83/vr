using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TOne.WhS.BusinessEntity.Entities;

namespace TOne.WhS.BusinessEntity.Data
{
    public interface IWHSFinancialAccountDataManager:IDataManager
    {
        List<WHSFinancialAccount> GetFinancialAccounts();
        bool Update(WHSFinancialAccountToEdit financialAccountToEdit);
        bool Insert(WHSFinancialAccount financialAccount, out int insertedId);
        bool AreFinancialAccountsUpdated(ref object updateHandle);
    }
}
