using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Activities;
using TOne.WhS.SupplierPriceList.Entities.SPL;
using TOne.WhS.BusinessEntity.Entities;
using Vanrise.Common;

namespace TOne.WhS.SupplierPriceList.BP.Activities
{

    public sealed class PrepareExistingRates : CodeActivity
    {

        [RequiredArgument]
        public InArgument<IEnumerable<SupplierRate>> ExistingRateEntities { get; set; }

        [RequiredArgument]
        public InArgument<Dictionary<long, ExistingZone>> ExistingZonesByZoneId { get; set; }

        [RequiredArgument]
        public OutArgument<IEnumerable<ExistingRate>> ExistingRates { get; set; }

        protected override void Execute(CodeActivityContext context)
        {
            IEnumerable<SupplierRate> existingRateEntities = this.ExistingRateEntities.Get(context);
            Dictionary<long, ExistingZone> existingZonesByZoneId = this.ExistingZonesByZoneId.Get(context);

            IEnumerable<ExistingRate> existingRates = existingRateEntities.MapRecords((rateEntity) => ExistingRateMapper(rateEntity, existingZonesByZoneId));
            ExistingRates.Set(context, existingRates);
        }

        ExistingRate ExistingRateMapper(SupplierRate rateEntity, Dictionary<long, ExistingZone> existingZonesByZoneId)
        {
            return new ExistingRate()
            {
                RateEntity = rateEntity,
                ParentZone = existingZonesByZoneId[rateEntity.ZoneId]
            };
        }
    }
}
