using System;
using Vanrise.Notification.Entities;

namespace Retail.BusinessEntity.MainExtensions.AccountBalanceAlertRule
{
    public class FixedBalanceAlertThreshold : VRBalanceAlertThreshold
    {
        public override Guid ConfigId { get { return new Guid("497557d1-399e-4af5-ba10-a03338d1caf4"); } }

        public Decimal Threshold { get; set; }


        public override decimal GetThreshold(IVRBalanceAlertThresholdContext context)
        {
            return this.Threshold;
        }
    }
}
