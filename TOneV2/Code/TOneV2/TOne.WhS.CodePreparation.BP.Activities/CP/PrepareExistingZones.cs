using System;
using System.Activities;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TOne.WhS.BusinessEntity.Entities;
using TOne.WhS.CodePreparation.Entities.CP.Processing;
using Vanrise.BusinessProcess;

namespace TOne.WhS.CodePreparation.BP.Activities
{
    public class PrepareExistingZonesInput
    {
        public IEnumerable<SaleZone> ExistingZoneEntities { get; set; }
    }

    public class PrepareExistingZonesOutput
    {
        public Dictionary<long, ExistingZone> ExistingZonesByZoneId { get; set; }
    }
    public sealed class PrepareExistingZones : BaseAsyncActivity<PrepareExistingZonesInput, PrepareExistingZonesOutput>
    {
        [RequiredArgument]
        public InArgument<IEnumerable<SaleZone>> ExistingZoneEntities { get; set; }

        [RequiredArgument]
        public InOutArgument<Dictionary<long, ExistingZone>> ExistingZonesByZoneId { get; set; }

        protected override PrepareExistingZonesInput GetInputArgument(AsyncCodeActivityContext context)
        {
            return new PrepareExistingZonesInput()
            {
                ExistingZoneEntities = this.ExistingZoneEntities.Get(context)
            };
        }

        protected override void OnBeforeExecute(AsyncCodeActivityContext context, AsyncActivityHandle handle)
        {
            if (this.ExistingZonesByZoneId.Get(context) == null)
                this.ExistingZonesByZoneId.Set(context, new Dictionary<long, ExistingZone>());
            base.OnBeforeExecute(context, handle);
        }

        protected override PrepareExistingZonesOutput DoWorkWithResult(PrepareExistingZonesInput inputArgument, AsyncActivityHandle handle)
        {
            IEnumerable<SaleZone> existingZones = inputArgument.ExistingZoneEntities;

            Dictionary<long, ExistingZone> existingZoneDic = existingZones.ToDictionary<BusinessEntity.Entities.SaleZone, long, ExistingZone>((zoneEntity) =>
                zoneEntity.SaleZoneId, (zoneEntity) => new ExistingZone { ZoneEntity = zoneEntity });

            return new PrepareExistingZonesOutput()
            {
                ExistingZonesByZoneId = existingZoneDic
            };
        }

        protected override void OnWorkComplete(AsyncCodeActivityContext context, PrepareExistingZonesOutput result)
        {
            this.ExistingZonesByZoneId.Set(context, result.ExistingZonesByZoneId);
        }
    }
}
