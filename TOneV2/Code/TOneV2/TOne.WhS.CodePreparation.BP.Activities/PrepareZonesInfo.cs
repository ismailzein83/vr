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
    public class PrepareZonesInfoInput
    {
        public IEnumerable<SaleZone> ExistingZoneEntities { get; set; }
    }

    public class PrepareZonesInfoOutput
    {
        public Dictionary<string, ExistingZoneInfo> ExistingZonesInfoByZoneName { get; set; }
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
            Dictionary<String, ExistingZoneInfo> existingZonesInfoDic = new Dictionary<string, ExistingZoneInfo>(StringComparer.InvariantCultureIgnoreCase);

            foreach (SaleZone saleZone in existingZones)
            {
                if (!existingZonesInfoDic.ContainsKey(saleZone.Name))
                    existingZonesInfoDic.Add(saleZone.Name, new ExistingZoneInfo() { CountryId = saleZone.CountryId });
            }

            return new PrepareZonesInfoOutput()
            {
                ExistingZonesInfoByZoneName = existingZonesInfoDic
            };
        }

        protected override void OnWorkComplete(AsyncCodeActivityContext context, PrepareZonesInfoOutput result)
        {
            CPParametersContext cpParametersContext = context.GetCPParameterContext() as CPParametersContext;
            cpParametersContext.ExistingZonesInfoByZoneName = result.ExistingZonesInfoByZoneName;
        }
    }
}
