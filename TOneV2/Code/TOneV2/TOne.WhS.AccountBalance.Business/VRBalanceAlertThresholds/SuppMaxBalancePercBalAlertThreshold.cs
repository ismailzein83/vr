using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Vanrise.Notification.Entities;
using Vanrise.Common;

namespace TOne.WhS.AccountBalance.Business
{
    /// <summary>
    /// only applicable for PrePaid Supplier AccountType
    /// </summary>
    class SuppMaxBalancePercBalAlertThreshold : VRBalanceAlertThreshold
    {
        public override Guid ConfigId
        {
            get { throw new NotImplementedException(); }
        }

        public Decimal Percentage { get; set; }

        public override decimal GetThreshold(IVRBalanceAlertThresholdContext context)
        {
            context.ThrowIfNull("context");
            context.EntityBalanceInfo.ThrowIfNull("context.EntityBalanceInfo");
            Decimal maxBalance = new AccountBalanceManager().GetSupplierEstimatedMaxBalance(context.EntityBalanceInfo.EntityId);
            //context.IsUpThreshold = true;
            return -(maxBalance * this.Percentage / 100);
        }
    }
}
