using System;
using System.Activities;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TOne.WhS.BusinessEntity.Entities;
using TOne.WhS.CodePreparation.Entities.Processing;
using Vanrise.BusinessProcess;

namespace TOne.WhS.CodePreparation.BP.Activities
{
    public class PrepareZonesInput
    {
        public IEnumerable<SaleZone> ExistingZoneEntities { get; set; }
    }

    public class PrepareZonesOutput
    {
        public Dictionary<string, ExistingZone> ExistingZonesByZoneName { get; set; }
    }
    public sealed class PrepareZones : BaseAsyncActivity<PrepareZonesInput, PrepareZonesOutput>
    {
        [RequiredArgument]
        public InArgument<IEnumerable<SaleZone>> ExistingZoneEntities { get; set; }

        protected override PrepareZonesInput GetInputArgument(AsyncCodeActivityContext context)
        {
            return new PrepareZonesInput()
            {
                ExistingZoneEntities = this.ExistingZoneEntities.Get(context)
            };
        }

        protected override PrepareZonesOutput DoWorkWithResult(PrepareZonesInput inputArgument, AsyncActivityHandle handle)
        {
            IEnumerable<SaleZone> existingZones = inputArgument.ExistingZoneEntities;

            Dictionary<String, ExistingZone> existingZoneDic = existingZones.ToDictionary<BusinessEntity.Entities.SaleZone, string, ExistingZone>((zoneEntity) =>
                zoneEntity.Name, (zoneEntity) => new ExistingZone { ZoneEntity = zoneEntity });

            return new PrepareZonesOutput()
            {
                ExistingZonesByZoneName = existingZoneDic
            };
        }

        protected override void OnWorkComplete(AsyncCodeActivityContext context, PrepareZonesOutput result)
        {
            CPParametersContext cpParametersContext = context.GetCPParameterContext() as CPParametersContext;
            cpParametersContext.ExistingZonesByZoneName = result.ExistingZonesByZoneName;
        }
    }
}
