using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Vanrise.Notification.Entities;

namespace TOne.WhS.AccountBalance.MainExtensions.VRBalanceAlertThresholds
{
    /// <summary>
    /// only applicable for PrePaid Customer AccountType
    /// </summary>
    public class CustFixedBalAlertThreshold : VRBalanceAlertThreshold
    {
        public override Guid ConfigId
        {
            get { throw new NotImplementedException(); }
        }

        public Decimal Threshold { get; set; }

        public override decimal GetThreshold(IVRBalanceAlertThresholdContext context)
        {
            return this.Threshold;
        }
    }
}
