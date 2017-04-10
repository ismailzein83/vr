using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Vanrise.Notification.Entities;

namespace TOne.WhS.AccountBalance.MainExtensions.VRBalanceAlertThresholds.PrePaid
{
    /// <summary>
    /// only applicable for PrePaid Customer AccountType
    /// </summary>
    public class CustFixedThreshold : VRBalanceAlertThreshold
    {
        public override Guid ConfigId
        {
            get { return new Guid("D188C0DF-A278-4F03-9D89-F5DF808AFD61"); }
        }

        public Decimal Threshold { get; set; }

        public override decimal GetThreshold(IVRBalanceAlertThresholdContext context)
        {
            return this.Threshold;
        }
    }
}
