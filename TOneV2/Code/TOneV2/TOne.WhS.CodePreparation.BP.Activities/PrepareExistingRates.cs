using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Activities;
using TOne.WhS.CodePreparation.Entities.Processing;
using TOne.WhS.BusinessEntity.Entities;
using Vanrise.Common;

namespace TOne.WhS.CodePreparation.BP.Activities
{

    public sealed class PrepareExistingRates : CodeActivity
    {

        [RequiredArgument]
        public InArgument<IEnumerable<SaleRate>> ExistingRateEntities { get; set; }

        [RequiredArgument]
        public InArgument<Dictionary<long, ExistingZone>> ExistingZonesByZoneId { get; set; }

        [RequiredArgument]
        public OutArgument<IEnumerable<ExistingRate>> ExistingRates { get; set; }

        protected override void Execute(CodeActivityContext context)
        {
            IEnumerable<SaleRate> existingRateEntities = this.ExistingRateEntities.Get(context);
            Dictionary<long, ExistingZone> existingZonesByZoneId = this.ExistingZonesByZoneId.Get(context);

            IEnumerable<ExistingRate> existingRates = existingRateEntities.Where(x => existingZonesByZoneId.ContainsKey(x.ZoneId)).MapRecords(
                (rateEntity) => ExistingRateMapper(rateEntity, existingZonesByZoneId));

            ExistingRates.Set(context, existingRates);
        }

        ExistingRate ExistingRateMapper(SaleRate rateEntity, Dictionary<long, ExistingZone> existingZonesByZoneId)
        {
            ExistingZone existingZone;

            if (!existingZonesByZoneId.TryGetValue(rateEntity.ZoneId, out existingZone))
                throw new Exception(String.Format("Rate Entity with Id {0} is not linked to Zone Id {1}", rateEntity.SaleRateId, rateEntity.ZoneId));

            ExistingRate existingRate = new ExistingRate()
            {
                RateEntity = rateEntity,
                ParentZone = existingZone
            };

            existingRate.ParentZone.ExistingRates.Add(existingRate);
            return existingRate;
        }
    }
}
