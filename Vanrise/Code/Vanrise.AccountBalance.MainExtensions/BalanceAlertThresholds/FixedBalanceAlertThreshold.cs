using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Vanrise.AccountBalance.Entities;

namespace Vanrise.AccountBalance.MainExtensions.BalanceAlertThresholds
{
    public class FixedBalanceAlertThreshold : BalanceAlertThreshold
    {
        public Decimal Threshold { get; set; }

        public override decimal GetThreshold(IBalanceAlertThresholdContext context)
        {
            return this.Threshold;
        }
    }
}
