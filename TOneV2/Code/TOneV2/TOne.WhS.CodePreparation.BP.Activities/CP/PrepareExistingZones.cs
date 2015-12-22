using System;
using System.Activities;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TOne.WhS.BusinessEntity.Entities;
using TOne.WhS.CodePreparation.Entities.CP.Processing;

namespace TOne.WhS.CodePreparation.BP.Activities
{
    public sealed class PrepareExistingZones : CodeActivity
    {
        [RequiredArgument]
        public InArgument<IEnumerable<SaleZone>> ExistingZoneEntities { get; set; }

        [RequiredArgument]
        public OutArgument<Dictionary<long, ExistingZone>> ExistingZonesByZoneId { get; set; }

        protected override void Execute(CodeActivityContext context)
        {
            IEnumerable<SaleZone> existingZones = context.GetValue(this.ExistingZoneEntities);

            Dictionary<long, ExistingZone> existingZoneDic = existingZones.ToDictionary<BusinessEntity.Entities.SaleZone, long, ExistingZone>((zoneEntity) =>
                zoneEntity.SaleZoneId, (zoneEntity) => new ExistingZone { ZoneEntity = zoneEntity });

            ExistingZonesByZoneId.Set(context, existingZoneDic);
        }
    }
}
