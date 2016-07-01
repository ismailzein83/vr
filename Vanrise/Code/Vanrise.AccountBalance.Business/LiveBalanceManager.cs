using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Vanrise.AccountBalance.Entities;

namespace Vanrise.AccountBalance.Business
{
    public class LiveBalanceManager
    {
        bool _isLocked;

        public void CreateBalances(IEnumerable<BalanceCreate> balances)
        {
            CheckLocked();
        }

        public void UpdateBalances(IEnumerable<BalanceUpdate> balances)
        {
            CheckLocked();
        }

        private void CheckLocked()
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
