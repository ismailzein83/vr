using Retail.BusinessEntity.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Retail.BusinessEntity.Business
{
    public class AccountLiveBalanceManager
    {
        bool _isLocked;

        public void UpdateBalances(IEnumerable<AccountBalanceUpdate> balances)
        {
            if (!_isLocked)
                throw new Exception("AccountLiveBalance is not locked!");
        }

        public bool TryLock()
        {
            _isLocked = true;
            return true;
        }

        public void UnLock()
        {

        }
    }
}
