using System;
using System.Activities;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TOne.WhS.BusinessEntity.Business;
using TOne.WhS.Sales.Business;
using TOne.WhS.Sales.Entities;
using Vanrise.Common.Business;

namespace TOne.WhS.Sales.BP.Activities
{
   public class SetDefaultRates : CodeActivity
    {
        #region Input Arguments

        [RequiredArgument]
        public InArgument<List<RateToChange>> RatesToChange { get; set; }
        public InArgument<List<long>> ZoneIdsWithMissingRates { get; set; }

        #endregion

        protected override void Execute(CodeActivityContext context)
        {
            var zoneIdsWithMissingRates = ZoneIdsWithMissingRates.Get(context);
                    IRatePlanContext ratePlanContext = context.GetRatePlanContext();
                    TOne.WhS.BusinessEntity.Business.ConfigManager configManager = new TOne.WhS.BusinessEntity.Business.ConfigManager();
                    decimal roundDefaultRate;// = configManager.GetRoundedDefaultRate();
                    if(ratePlanContext.OwnerType==TOne.WhS.BusinessEntity.Entities.SalePriceListOwnerType.SellingProduct)
                    {
                        var sellingProductManager = new SellingProductManager();
                        roundDefaultRate=sellingProductManager.GetSellingProductRoundedDefaultRate(ratePlanContext.OwnerId);
                    }
                    else
                    {
                        var customerManager = new CarrierAccountManager();
                        roundDefaultRate=customerManager.GetCustomerRoundedDefaultRate(ratePlanContext.OwnerId);
                    }
                    var systemCurrency = new Vanrise.Common.Business.ConfigManager().GetSystemCurrencyId();
                    CurrencyExchangeRateManager currencyExchangeRateManager = new CurrencyExchangeRateManager();
                    var convertedDefaultRate = currencyExchangeRateManager.ConvertValueToCurrency(roundDefaultRate,systemCurrency, ratePlanContext.SellingProductCurrencyId,DateTime.Now);
                    List<RateToChange> ratesToChange = this.RatesToChange.Get(context);
                    SaleZoneManager saleZoneManager = new SaleZoneManager();
                    foreach (var zoneId in zoneIdsWithMissingRates)
                    {
                        var zone = saleZoneManager.GetSaleZone(zoneId);
                        RateToChange rateToChange = new RateToChange()
                        {
                            ZoneId = zoneId,
                            ZoneName = zone.Name,
                            NormalRate = convertedDefaultRate,
                            CurrencyId = ratePlanContext.CurrencyId,
                            BED = zone.BED,
                            EED =(zone.EED !=null)?zone.EED:null
                        };
                        ratesToChange.Add(rateToChange);
                    }
        }
    }
}
