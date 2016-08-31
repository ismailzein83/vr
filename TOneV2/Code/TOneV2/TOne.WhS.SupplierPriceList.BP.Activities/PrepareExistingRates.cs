using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Activities;
using TOne.WhS.SupplierPriceList.Entities.SPL;
using TOne.WhS.BusinessEntity.Entities;
using Vanrise.Common;
using TOne.WhS.BusinessEntity.Business;

namespace TOne.WhS.SupplierPriceList.BP.Activities
{

    public sealed class PrepareExistingRates : CodeActivity
    {

        [RequiredArgument]
        public InArgument<IEnumerable<SupplierRate>> ExistingRateEntities { get; set; }

        [RequiredArgument]
        public InArgument<Dictionary<long, ExistingZone>> ExistingZonesByZoneId { get; set; }

        [RequiredArgument]
        public OutArgument<ExistingRateGroupByZoneName> ExistingRatesGroupsByZoneName { get; set; }

        protected override void Execute(CodeActivityContext context)
        {
            IEnumerable<SupplierRate> existingRateEntities = this.ExistingRateEntities.Get(context);
            Dictionary<long, ExistingZone> existingZonesByZoneId = this.ExistingZonesByZoneId.Get(context);

            IEnumerable<ExistingRate> existingRates = existingRateEntities.OrderBy(item => item.BED).Where(x => existingZonesByZoneId.ContainsKey(x.ZoneId)).MapRecords(
              (rateEntity) => ExistingRateMapper(rateEntity, existingZonesByZoneId));
            ExistingRateGroupByZoneName existingRatesGroupsByZoneName = new ExistingRateGroupByZoneName();
            StructureExistingRatesByRatesGroups( existingRatesGroupsByZoneName, existingRates);

            ExistingRatesGroupsByZoneName.Set(context, existingRatesGroupsByZoneName);
        }

        ExistingRate ExistingRateMapper(SupplierRate rateEntity, Dictionary<long, ExistingZone> existingZonesByZoneId)
        {
            ExistingZone existingZone;

            if (!existingZonesByZoneId.TryGetValue(rateEntity.ZoneId, out existingZone))
                throw new Exception(String.Format("Rate Entity with Id {0} is not linked to Zone Id {1}", rateEntity.SupplierRateId, rateEntity.ZoneId));

            ExistingRate existingRate = new ExistingRate()
            {
                RateEntity = rateEntity,
                ParentZone = existingZone
            };

            existingRate.ParentZone.ExistingRates.Add(existingRate);
            return existingRate;
        }
        ExistingRateGroupByZoneName StructureExistingRatesByRatesGroups(ExistingRateGroupByZoneName existingRatesGroupsByZoneName, IEnumerable<ExistingRate> existingRates)
        {
            SupplierZoneManager supplierZoneManager = new SupplierZoneManager();

            List<ExistingRate> existingOtherRates;
            ExistingRateGroup existingRateGroup;

            foreach (ExistingRate existingRate in existingRates)
            {
                string zoneName = supplierZoneManager.GetSupplierZoneName(existingRate.RateEntity.ZoneId);
               
                if (!existingRatesGroupsByZoneName.TryGetValue(zoneName, out existingRateGroup))
                {
                    existingRateGroup = new ExistingRateGroup();
                    existingRateGroup.ZoneName = zoneName;

                    if (existingRate.RateEntity.RateTypeId.HasValue)
                    {
                        existingOtherRates = new List<ExistingRate>();
                        existingOtherRates.Add(existingRate);
                        existingRateGroup.OtherRates.Add(existingRate.RateEntity.RateTypeId.Value, existingOtherRates);
                    }
                    else
                        existingRateGroup.NormalRates.Add(existingRate);

                    existingRatesGroupsByZoneName.Add(zoneName, existingRateGroup);
                }
                else
                {
                    if (existingRate.RateEntity.RateTypeId.HasValue)
                    {
                        if (existingRateGroup.OtherRates.TryGetValue(existingRate.RateEntity.RateTypeId.Value, out existingOtherRates))
                            existingOtherRates.Add(existingRate);
                        else
                        {
                            existingOtherRates = new List<ExistingRate>();
                            existingOtherRates.Add(existingRate);
                            existingRateGroup.OtherRates.Add(existingRate.RateEntity.RateTypeId.Value, existingOtherRates);
                        }
                    }
                    else
                        existingRateGroup.NormalRates.Add(existingRate);
                }
            }
            return existingRatesGroupsByZoneName;
        }
       
    }
}
