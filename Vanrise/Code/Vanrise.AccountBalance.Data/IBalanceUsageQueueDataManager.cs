using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Vanrise.AccountBalance.Entities;

namespace Vanrise.AccountBalance.Data
{
    public interface IBalanceUsageQueueDataManager : IDataManager
    {
        void LoadUsageBalance<T>(Guid accountTypeId,BalanceUsageQueueType balanceUsageQueueType, Action<BalanceUsageQueue<T>> onUsageBalanceUpdateReady);
        bool UpdateUsageBalance<T>(Guid accountTypeId, BalanceUsageQueueType balanceUsageQueueType , T updateUsageBalancePayload);
    }
}
