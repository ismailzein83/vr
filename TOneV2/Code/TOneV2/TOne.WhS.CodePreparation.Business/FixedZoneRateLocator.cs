﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TOne.WhS.BusinessEntity.Entities;
using TOne.WhS.CodePreparation.Entities;
using TOne.WhS.CodePreparation.Entities.Processing;

namespace TOne.WhS.CodePreparation.Business
{
    //TODO to remove this class because it's no longer used
    public class FixedZoneRateLocator : NewZoneRateLocator
    {
        public FixedZoneRateLocator(int sellingNumberPlanId)
            : base(sellingNumberPlanId)
        {

        }

        public override IEnumerable<NewZoneRateEntity> GetRates(IEnumerable<Entities.Processing.CodeToAdd> codes, Dictionary<SaleZoneTypeEnum, IEnumerable<ExistingZone>> zonesByType, ExistingRatesByZoneName existingRatesByZoneName)
        {
            if (zonesByType[SaleZoneTypeEnum.Fixed].Count() == 0)
                return null;

            List<ExistingZone> matchedZones = base.GetMatchedExistingZones(codes, zonesByType[SaleZoneTypeEnum.Fixed]);

            if (matchedZones.Count() == 0)
                matchedZones.AddRange(zonesByType[SaleZoneTypeEnum.Fixed]);

            return base.GetHighestRatesFromZoneMatchesSaleEntities(matchedZones, existingRatesByZoneName);
        }
    }
}
