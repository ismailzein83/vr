using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Activities;
using TOne.WhS.SupplierPriceList.Entities.SPL;
using TOne.WhS.BusinessEntity.Entities;
using Vanrise.Common;
using TOne.WhS.BusinessEntity.Business;
using Vanrise.Common.Business;

namespace TOne.WhS.SupplierPriceList.BP.Activities
{
    public sealed class PrepareExistingRates : CodeActivity
    {

        [RequiredArgument]
        public InArgument<IEnumerable<SupplierRate>> ExistingRateEntities { get; set; }

        [RequiredArgument]
        public InArgument<int> CurrencyId { get; set; }

        [RequiredArgument]
        public InArgument<Dictionary<long, ExistingZone>> ExistingZonesByZoneId { get; set; }

        [RequiredArgument]
        public OutArgument<IEnumerable<ExistingRate>> ExistingRates { get; set; }

        protected override void Execute(CodeActivityContext context)
        {
            IEnumerable<SupplierRate> existingRateEntities = this.ExistingRateEntities.Get(context);
            Dictionary<long, ExistingZone> existingZonesByZoneId = this.ExistingZonesByZoneId.Get(context);
            int currencyId = this.CurrencyId.Get(context);

            IEnumerable<ExistingRate> existingRates = existingRateEntities.OrderBy(item => item.BED).Where(x => existingZonesByZoneId.ContainsKey(x.ZoneId)).MapRecords(
              (rateEntity) => ExistingRateMapper(rateEntity, existingZonesByZoneId, currencyId));
            
            ExistingRates.Set(context, existingRates);
        }

        ExistingRate ExistingRateMapper(SupplierRate rateEntity, Dictionary<long, ExistingZone> existingZonesByZoneId, int currencyId)
        {
            ExistingZone existingZone;

            if (!existingZonesByZoneId.TryGetValue(rateEntity.ZoneId, out existingZone))
                throw new Exception(String.Format("Rate Entity with Id {0} is not linked to Zone Id {1}", rateEntity.SupplierRateId, rateEntity.ZoneId));

            CurrencyExchangeRateManager currencyExchangeRateManager = new CurrencyExchangeRateManager();
            SupplierRateManager supplierRateManager = new SupplierRateManager();
            int rateCurrencyId = supplierRateManager.GetCurrencyId(rateEntity);


            ExistingRate existingRate = new ExistingRate()
            {
                ConvertedRate = currencyExchangeRateManager.ConvertValueToCurrency(rateEntity.Rate, rateCurrencyId, currencyId, DateTime.Now),
                RateEntity = rateEntity,
                ParentZone = existingZone
            };
            
            existingRate.ParentZone.ExistingRates.Add(existingRate);
            return existingRate;
        }
                                                                                                                                       
    }
}
