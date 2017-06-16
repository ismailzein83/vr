using System;
using System.Activities;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TOne.WhS.BusinessEntity.Business;
using TOne.WhS.BusinessEntity.Entities;
using TOne.WhS.Sales.Business;
using TOne.WhS.Sales.Entities;
using Vanrise.Common;
using Vanrise.Common.Business;

namespace TOne.WhS.Sales.BP.Activities
{
    public class PrepareExistingRates : CodeActivity
    {
        #region Input Arguments

        [RequiredArgument]
        public InArgument<int> CurrencyId { get; set; }

        [RequiredArgument]
        public InArgument<IEnumerable<SaleRate>> ExistingSaleRates { get; set; }

        [RequiredArgument]
        public InArgument<Dictionary<long, ExistingZone>> ExistingZonesById { get; set; }

        #endregion

        #region Output Arguments

        [RequiredArgument]
        public OutArgument<IEnumerable<ExistingRate>> ExistingRates { get; set; }

        #endregion

        protected override void Execute(CodeActivityContext context)
        {
            int currencyId = CurrencyId.Get(context);
            IEnumerable<SaleRate> saleRates = ExistingSaleRates.Get(context);
            Dictionary<long, ExistingZone> existingZonesById = ExistingZonesById.Get(context);

            IRatePlanContext ratePlanContext = context.GetRatePlanContext();

            var exchangeRateManager = new CurrencyExchangeRateManager();
            var saleRateManager = new SaleRateManager();

            var existingRates = saleRates.MapRecords(saleRate => ExistingRateMapper(saleRate, existingZonesById, saleRateManager, currencyId, DateTime.Now, ratePlanContext.LongPrecision, exchangeRateManager));
            this.ExistingRates.Set(context, existingRates);
        }

        #region Private Methods

        private ExistingRate ExistingRateMapper(SaleRate saleRate, Dictionary<long, ExistingZone> existingZonesById, SaleRateManager saleRateManager, int currencyId, DateTime effectiveOn, int longPrecision, CurrencyExchangeRateManager exchangeRateManager)
        {
            ExistingZone existingZone;

            if (!existingZonesById.TryGetValue(saleRate.ZoneId, out existingZone))
                throw new NullReferenceException(String.Format("A rate exists for a missing or not effective zone (Id: {0})", saleRate.ZoneId));

            decimal convertedRate = UtilitiesManager.ConvertToCurrencyAndRound(saleRate.Rate, saleRateManager.GetCurrencyId(saleRate), currencyId, effectiveOn, longPrecision, exchangeRateManager);

            var existingRate = new ExistingRate()
            {
                ConvertedRate = convertedRate,
                RateEntity = saleRate,
                ParentZone = existingZone
            };
            existingZone.ExistingRates.Add(existingRate);
            return existingRate;
        }

        #endregion
    }
}
