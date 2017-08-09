using Retail.BusinessEntity.Data;
using Retail.BusinessEntity.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Retail.BusinessEntity.Business
{
    public class AccountStatusHistoryManager
    {
        public bool TryAddAccountStatusHistory(AccountStatusHistory accountStatusHistory, out long accountId)
        {
            IAccountStatusHistoryDataManager dataManager = BEDataManagerFactory.GetDataManager<IAccountStatusHistoryDataManager>();
            if (dataManager.Insert(accountStatusHistory, out accountId))
            {
                return true;
            }
            else
            {
                return false;
            }
        }
    }
}
