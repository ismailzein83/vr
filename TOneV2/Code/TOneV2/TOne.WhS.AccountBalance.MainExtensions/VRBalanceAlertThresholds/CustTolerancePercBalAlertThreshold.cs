using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TOne.WhS.AccountBalance.Business;
using Vanrise.Notification.Entities;
using Vanrise.Common;

namespace TOne.WhS.AccountBalance.MainExtensions.VRBalanceAlertThresholds
{
    /// <summary>
    /// only applicable for PostPaid Customer AccountType
    /// </summary>
    public class CustTolerancePercBalAlertThreshold : VRBalanceAlertThreshold
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
            Decimal creditLimit = new AccountBalanceManager().GetCustomerCreditLimit(context.EntityBalanceInfo.EntityId);
            return -(creditLimit * (100 - this.Percentage) / 100);
        }
    }
}
