using System;
using System.Activities;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TOne.WhS.BusinessEntity.Business;
using TOne.WhS.BusinessEntity.Entities;
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

            var currencyExchangeRateManager = new CurrencyExchangeRateManager();
            var saleRateManager = new SaleRateManager();

            var existingRates = saleRates.MapRecords(saleRate => ExistingRateMapper(saleRate, existingZonesById, currencyExchangeRateManager, saleRateManager, currencyId, DateTime.Now));
            this.ExistingRates.Set(context, existingRates);
        }

        #region Private Methods

        private ExistingRate ExistingRateMapper
        (
            SaleRate saleRate,
            Dictionary<long, ExistingZone> existingZonesById,
            CurrencyExchangeRateManager currencyExchangeRateManager,
            SaleRateManager saleRateManager,
            int currencyId,
            DateTime effectiveOn
        )
        {
            ExistingZone existingZone;

            if (!existingZonesById.TryGetValue(saleRate.ZoneId, out existingZone))
                throw new NullReferenceException(String.Format("A rate exists for a missing or ineffective zone (Id: {0})", saleRate.ZoneId));

            decimal convertedRate =
                currencyExchangeRateManager.ConvertValueToCurrency(saleRate.NormalRate, saleRateManager.GetCurrencyId(saleRate), currencyId, effectiveOn);

            return new ExistingRate()
            {
                Rate = convertedRate,
                RateEntity = saleRate,
                ParentZone = existingZone
            };
        }

        #endregion
    }
}
