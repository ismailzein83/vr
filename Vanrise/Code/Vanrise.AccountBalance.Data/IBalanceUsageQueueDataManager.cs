using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Vanrise.AccountBalance.Entities;

namespace Vanrise.AccountBalance.Data
{
    public interface IBalanceUsageQueueDataManager:IDataManager
    {
        void LoadUsageBalanceUpdate(Action<BalanceUsageQueue> onUsageBalanceUpdateReady);
        bool UpdateUsageBalance(BalanceUsageDetail balanceUsageDetail);
    }
}
