using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Vanrise.AccountBalance.Data
{
    public interface IBalanceHistoryDataManager : IDataManager
    {
        bool InsertBalanceHistoryFromLiveBalance(long closingPeriodID, Guid accountTypeId);
    }
}
