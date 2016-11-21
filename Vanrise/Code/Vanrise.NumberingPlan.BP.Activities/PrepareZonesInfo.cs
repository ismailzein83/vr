using System;
using System.Activities;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Vanrise.BusinessProcess;
using Vanrise.NumberingPlan.Entities;

namespace Vanrise.NumberingPlan.BP.Activities
{
    public class PrepareZonesInfoInput
    {
        public IEnumerable<SaleZone> ExistingZoneEntities { get; set; }
    }

    public class PrepareZonesInfoOutput
    {
        public ExistingZoneInfoByZoneName ExistingZonesInfoByZoneName { get; set; }
    }
    public sealed class PrepareZonesInfo : BaseAsyncActivity<PrepareZonesInfoInput, PrepareZonesInfoOutput>
    {
        [RequiredArgument]
        public InArgument<IEnumerable<SaleZone>> ExistingZoneEntities { get; set; }

        protected override PrepareZonesInfoInput GetInputArgument(AsyncCodeActivityContext context)
        {
            return new PrepareZonesInfoInput()
            {
                ExistingZoneEntities = this.ExistingZoneEntities.Get(context)
            };
        }

        protected override PrepareZonesInfoOutput DoWorkWithResult(PrepareZonesInfoInput inputArgument, AsyncActivityHandle handle)
        {
            IEnumerable<SaleZone> existingZones = inputArgument.ExistingZoneEntities;
          
            ExistingZoneInfoByZoneName existingZonesInfoByZoneName = new ExistingZoneInfoByZoneName();

            ExistingZoneInfo existingZoneInfo;
            foreach (SaleZone saleZone in existingZones)
            {
                if (!existingZonesInfoByZoneName.TryGetValue(saleZone.Name, out existingZoneInfo))
                    existingZonesInfoByZoneName.Add(saleZone.Name, new ExistingZoneInfo() { CountryId = saleZone.CountryId });
            }

            return new PrepareZonesInfoOutput()
            {
                ExistingZonesInfoByZoneName = existingZonesInfoByZoneName
            };
        }

        protected override void OnWorkComplete(AsyncCodeActivityContext context, PrepareZonesInfoOutput result)
        {
            CPParametersContext cpParametersContext = context.GetCPParameterContext() as CPParametersContext;
            cpParametersContext.ExistingZonesInfoByZoneName = result.ExistingZonesInfoByZoneName;
        }
    }
}
