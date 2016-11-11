using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Vanrise.AccountBalance.Data;
using Vanrise.AccountBalance.Entities;

namespace Vanrise.AccountBalance.Business
{
    public class UsageBalanceManager
    {
        public void UpdateUsageBalance(Guid accountTypeId, BalanceUsageDetail balanceUsageDetail)
        {
            IBalanceUsageQueueDataManager dataManager = AccountBalanceDataManagerFactory.GetDataManager<IBalanceUsageQueueDataManager>();
            dataManager.UpdateUsageBalance(accountTypeId, balanceUsageDetail);
        }
    }
}
