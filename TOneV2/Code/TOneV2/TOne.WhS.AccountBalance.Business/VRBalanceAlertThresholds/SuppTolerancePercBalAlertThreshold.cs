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
    /// only applicable for PostPaid Supplier AccountType
    /// </summary>
    public class SuppTolerancePercBalAlertThreshold : VRBalanceAlertThreshold
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
            Decimal creditLimit = new AccountBalanceManager().GetSupplierCreditLimit(context.EntityBalanceInfo.EntityId);
            context.IsUpThreshold = true;
            return  creditLimit * (100 - this.Percentage) / 100;
        }
    }
}
