using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Vanrise.Notification.Entities;

namespace Retail.BusinessEntity.MainExtensions.AccountBalanceAlertRule
{
    public class FixedBalanceAlertThreshold : VRBalanceAlertThreshold
    {
        public Decimal Threshold { get; set; }
        public override Guid ConfigId
        {
            get { return new Guid("497557d1-399e-4af5-ba10-a03338d1caf4"); }
        }

        public override decimal GetThreshold(IVRBalanceAlertThresholdContext context)
        {
            return this.Threshold;
        }
    }
}
