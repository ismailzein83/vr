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
        public void AddAccountStatusHistory(Guid accountDefinitionId, long accountId, Guid statusDefinitionId)
        {
            IAccountStatusHistoryDataManager dataManager = BEDataManagerFactory.GetDataManager<IAccountStatusHistoryDataManager>();
            dataManager.Insert(accountDefinitionId, accountId, statusDefinitionId);
        }
    }
}
