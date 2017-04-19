using System;
using TOne.WhS.AccountBalance.Business;
using Vanrise.Notification.Entities;
using Vanrise.Common;
using Vanrise.Common.Business;

namespace TOne.WhS.AccountBalance.MainExtensions.VRBalanceAlertThresholds.PostPaid
{
    /// <summary>
    /// only applicable for PostPaid Customer AccountType
    /// </summary>
    public class CustTolerancePercThreshold : VRBalanceAlertThreshold
    {
        public override Guid ConfigId { get { return new Guid("9D1732F1-E595-41B8-8FA6-FBD0AE039EED"); } }

        public Decimal Percentage { get; set; }

        public override decimal GetThreshold(IVRBalanceAlertThresholdContext context)
        {
            context.ThrowIfNull("context");
            context.EntityBalanceInfo.ThrowIfNull("context.EntityBalanceInfo");
            int liveBalanceCurrencyId = context.EntityBalanceInfo.CurrencyId;

            int carrierCurrencyId;
            Decimal creditLimit = new AccountBalanceManager().GetCustomerCreditLimit(context.EntityBalanceInfo.EntityId, out carrierCurrencyId);
            Decimal convertedCreditLimit = new CurrencyExchangeRateManager().ConvertValueToCurrency(creditLimit, carrierCurrencyId, liveBalanceCurrencyId, DateTime.Now);

            return -(convertedCreditLimit * (100 - this.Percentage) / 100);
        }
    }
}
