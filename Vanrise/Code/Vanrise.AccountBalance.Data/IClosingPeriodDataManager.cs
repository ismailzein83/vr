using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Vanrise.AccountBalance.Data
{
    public interface IClosingPeriodDataManager:IDataManager
    {
        void CreateClosingPeriod(DateTime balanceClosingPeriod, Guid usageTransactionTypeId);
    }
}
