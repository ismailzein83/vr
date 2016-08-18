using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TOne.WhS.BusinessEntity.Entities;
using TOne.WhS.CodePreparation.Entities;
using TOne.WhS.CodePreparation.Entities.Processing;

namespace TOne.WhS.CodePreparation.Business
{
    public class FixedZoneRateLocator : NewZoneRateLocator
    {
        public FixedZoneRateLocator(int sellingNumberPlanId)
            : base(sellingNumberPlanId)
        {

        }

        public override IEnumerable<NewZoneRateEntity> GetRates(IEnumerable<Entities.Processing.CodeToAdd> codes, Dictionary<SaleZoneTypeEnum, IEnumerable<ExistingZone>> zonesByType)
        {
            if (zonesByType[SaleZoneTypeEnum.Fixed].Count() == 0)
                return null;

            IEnumerable<ExistingZone> matchedZones = base.GetMatchedExistingZones(codes, zonesByType[SaleZoneTypeEnum.Fixed]);
            return base.GetHighestRatesFromZoneMatchesSaleEntities(matchedZones);
        }
    }
}
