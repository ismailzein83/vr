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
        public override Guid ConfigId { get { return new Guid("497557d1-399e-4af5-ba10-a03338d1caf4"); } }

        public Decimal Threshold { get; set; }

        public override decimal GetThreshold(IBalanceAlertThresholdContext context)
        {
            return this.Threshold;
        }
    }
}
