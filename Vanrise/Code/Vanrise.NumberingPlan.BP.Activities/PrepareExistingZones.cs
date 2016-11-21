using System;
using System.Activities;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Vanrise.BusinessProcess;
using Vanrise.Common;
using Vanrise.NumberingPlan.Entities;

namespace Vanrise.NumberingPlan.BP.Activities
{
    public class PrepareExistingZonesInput
    {
        public IEnumerable<SaleZone> ExistingZoneEntities { get; set; }
        public int CountryId { get; set; }
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
        public InArgument<int> CountryId { get; set; }

        [RequiredArgument]
        public InOutArgument<Dictionary<long, ExistingZone>> ExistingZonesByZoneId { get; set; }

        protected override PrepareExistingZonesInput GetInputArgument(AsyncCodeActivityContext context)
        {
            return new PrepareExistingZonesInput()
            {
                ExistingZoneEntities = this.ExistingZoneEntities.Get(context),
                CountryId = this.CountryId.Get(context)
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
            int countryId = inputArgument.CountryId;
             Dictionary<long, ExistingZone> existingZoneDic = existingZones.Where(item => item.CountryId == countryId).ToDictionary<SaleZone, long, ExistingZone>((zoneEntity) =>
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
