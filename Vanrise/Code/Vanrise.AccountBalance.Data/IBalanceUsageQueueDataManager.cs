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
        void LoadUsageBalanceUpdate(Guid accountTypeId, Action<BalanceUsageQueue> onUsageBalanceUpdateReady);
        bool UpdateUsageBalance(Guid accountTypeId, BalanceUsageDetail balanceUsageDetail);
    }
}
