using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
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

        long _accountId;
        public dynamic Account
        {
            get
            {
                throw new NotImplementedException();
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
