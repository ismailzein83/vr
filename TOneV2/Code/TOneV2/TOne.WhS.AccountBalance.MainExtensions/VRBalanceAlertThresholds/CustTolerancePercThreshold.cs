using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TOne.WhS.AccountBalance.Business;
using Vanrise.Notification.Entities;
using Vanrise.Common;

namespace TOne.WhS.AccountBalance.MainExtensions
{
    /// <summary>
    /// only applicable for PostPaid Customer AccountType
    /// </summary>
    public class CustTolerancePercThreshold : VRBalanceAlertThreshold
    {
        public override Guid ConfigId
        {
            get { return new Guid("F16B3576-0622-48F1-9180-E7582C6B39EA"); }
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
