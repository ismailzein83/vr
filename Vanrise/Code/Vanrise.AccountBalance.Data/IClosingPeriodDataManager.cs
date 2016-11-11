using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Vanrise.AccountBalance.Entities;

namespace Vanrise.AccountBalance.Data
{
    public interface IClosingPeriodDataManager : IDataManager
    {
        void CreateClosingPeriod(DateTime balanceClosingPeriod, Guid accountTypeId, Guid usageTransactionTypeId);
        BalanceClosingPeriod GetLastClosingPeriod(Guid accountTypeId);
    }
}
