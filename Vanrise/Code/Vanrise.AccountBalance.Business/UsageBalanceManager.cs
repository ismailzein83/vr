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
        public void UpdateUsageBalance(Guid accountTypeId, UpdateUsageBalancePayload updateUsageBalancePayload)
        {
            IBalanceUsageQueueDataManager dataManager = AccountBalanceDataManagerFactory.GetDataManager<IBalanceUsageQueueDataManager>();
            dataManager.UpdateUsageBalance(accountTypeId,BalanceUsageQueueType.UpdateUsageBalance, updateUsageBalancePayload);
        }
        public Guid InitializeUpdateUsageBalance()
        {
            return new Guid();
        }
        public void CorrectUsageBalance(Guid accountTypeId, CorrectUsageBalancePayload correctUsageBalancePayload)
        {
            IBalanceUsageQueueDataManager dataManager = AccountBalanceDataManagerFactory.GetDataManager<IBalanceUsageQueueDataManager>();
            dataManager.UpdateUsageBalance(accountTypeId,BalanceUsageQueueType.CorrectUsageBalance , correctUsageBalancePayload);
        }
        public void LoadUpdatesUsageBalance(Guid accountTypeId, Action<BalanceUsageQueue<UpdateUsageBalancePayload>> onUsageBalanceUpdateReady)
        {
            IBalanceUsageQueueDataManager dataManager = AccountBalanceDataManagerFactory.GetDataManager<IBalanceUsageQueueDataManager>();
            dataManager.LoadUsageBalance<UpdateUsageBalancePayload>(accountTypeId, BalanceUsageQueueType.UpdateUsageBalance, onUsageBalanceUpdateReady);
        }
        public void LoadCorrectUsageBalance(Guid accountTypeId, Action<BalanceUsageQueue<CorrectUsageBalancePayload>> onUsageBalanceUpdateReady)
        {
            IBalanceUsageQueueDataManager dataManager = AccountBalanceDataManagerFactory.GetDataManager<IBalanceUsageQueueDataManager>();
            dataManager.LoadUsageBalance<CorrectUsageBalancePayload>(accountTypeId, BalanceUsageQueueType.CorrectUsageBalance, onUsageBalanceUpdateReady);
        }

    }
}
