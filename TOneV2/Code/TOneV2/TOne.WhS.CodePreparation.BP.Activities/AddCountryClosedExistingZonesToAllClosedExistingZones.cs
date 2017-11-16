using System;
using System.Activities;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TOne.WhS.CodePreparation.Business;
using TOne.WhS.CodePreparation.Entities;
using TOne.WhS.CodePreparation.Entities.Processing;
using Vanrise.BusinessProcess;

namespace TOne.WhS.CodePreparation.BP.Activities
{
    public class AddClosedExistingZonesToAllClosedExistingZonesInput
    {
        public int CountryId { get; set; }
        public Dictionary<string, List<ExistingZone>> ClosedExistingZones { get; set; }
        public ClosedExistingZonesByCountry ClosedExistingZonesByCountry { get; set; }
    }
    public sealed class AddCountryClosedExistingZonesToAllClosedExistingZones : BaseAsyncActivity<AddClosedExistingZonesToAllClosedExistingZonesInput>
    {
        #region Input Arguments
        [RequiredArgument]
        public InArgument<int> CountryId { get; set; }

        [RequiredArgument]
        public InArgument<ClosedExistingZonesByCountry> ClosedExistingZonesByCountry { get; set; }

        [RequiredArgument]
        public InArgument<Dictionary<string, List<ExistingZone>>> ClosedExistingZones { get; set; }
        #endregion

        protected override AddClosedExistingZonesToAllClosedExistingZonesInput GetInputArgument(AsyncCodeActivityContext context)
        {
            return new AddClosedExistingZonesToAllClosedExistingZonesInput()
            {
                CountryId = this.CountryId.Get(context),
                ClosedExistingZones = this.ClosedExistingZones.Get(context),
                ClosedExistingZonesByCountry = this.ClosedExistingZonesByCountry.Get(context)
            };
        }

        protected override void DoWork(AddClosedExistingZonesToAllClosedExistingZonesInput inputArgument, AsyncActivityHandle handle)
        {
            if(inputArgument.ClosedExistingZones != null && inputArgument.ClosedExistingZones.Count()>0)
            inputArgument.ClosedExistingZonesByCountry.TryAddValue(inputArgument.CountryId, inputArgument.ClosedExistingZones);
        }
                
    }
}
