using System;
using Vanrise.Common.Business;
using TOne.WhS.Sales.Business;
using TOne.WhS.Sales.Entities;

namespace TOne.WhS.Sales.MainExtensions
{
    public class FixedRateThreshold : RateThreshold
    {
        public override Guid ConfigId => new Guid("066FCA8E-F4B8-4643-957D-AFAC44339470");

        public decimal Rate { get; set; }
        public int CurrencyId { get; set; }

        public override void Execute(ISellingRuleContext context)
        {
            //convert new rate to rule currency 
            var currencyExchangeRateManager = new CurrencyExchangeRateManager();
            int longPrecision = new GeneralSettingsManager().GetLongPrecisionValue();
            Decimal convertedRate = UtilitiesManager.ConvertToCurrencyAndRound(context.NewRate, context.CurrentCurrencyId
                , CurrencyId, DateTime.Now, longPrecision, currencyExchangeRateManager);

            context.ViolateRateRule = convertedRate < Rate;
        }
        public override string GetDescription()
        {
            string currencySymbol = new CurrencyManager().GetCurrencySymbol(CurrencyId);
            return $"{Rate} ({currencySymbol})";
        }
    }
}
