using System;
using Vanrise.Notification.Entities;
using Vanrise.Common;
using Vanrise.Common.Business;

namespace TOne.WhS.AccountBalance.MainExtensions.VRBalanceAlertThresholds.PrePaid
{
    /// <summary>
    /// only applicable for PrePaid Customer AccountType
    /// </summary>
    public class CustFixedThreshold : VRBalanceAlertThreshold
    {
        public override Guid ConfigId { get { return new Guid("D188C0DF-A278-4F03-9D89-F5DF808AFD61"); } }

        public Decimal Threshold { get; set; }

        public int CurrencyId { get; set; }

        public override decimal GetThreshold(IVRBalanceAlertThresholdContext context)
        {
            context.ThrowIfNull("IVRBalanceAlertThresholdContext");
            context.EntityBalanceInfo.ThrowIfNull("context.EntityBalanceInfo");
            int liveBalanceCurrencyId = context.EntityBalanceInfo.CurrencyId;

            return new CurrencyExchangeRateManager().ConvertValueToCurrency(this.Threshold, this.CurrencyId, liveBalanceCurrencyId, DateTime.Now);
        }
    }
}
