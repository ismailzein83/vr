using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Vanrise.AccountBalance.Entities;
using Vanrise.Entities;

namespace Vanrise.AccountBalance.Business
{
    public class BalanceAlertEventPayload : IVRActionEventPayload
    {
        public BalanceAlertEventPayload(long accountId, Decimal threshold)
        {
            this._accountId = accountId;
            this._threshold = threshold;
        }

        dynamic _account;
        long _accountId;
        public dynamic Account
        {
            get
            {
                if (_account == null)
                {
                    _account = new AccountManager().GetAccount(this._accountId);
                }
                return _account;
            }
        }

        Decimal _threshold;
        public Decimal Threshold
        {
            get
            {
                return _threshold;
            }
        }
    }
}
