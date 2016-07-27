using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Vanrise.AccountBalance.Entities;

namespace Vanrise.AccountBalance.Data
{
    public interface ILiveBalanceDataManager:IDataManager
    {
        LiveBalance GetLiveBalance(long accountId);

        bool UpdateBalance(long accountId, List<long> billingTransactionIds, decimal amount);
    }
}
