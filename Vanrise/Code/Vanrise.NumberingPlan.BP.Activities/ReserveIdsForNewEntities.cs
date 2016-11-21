using System;
using System.Activities;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Vanrise.NumberingPlan.Business;
using Vanrise.NumberingPlan.Entities;

namespace Vanrise.NumberingPlan.BP.Activities
{
    public class ReserveIdsForNewEntitiesInput
    {
        public IEnumerable<AddedZone> NewZones { get; set; }

        public IEnumerable<AddedCode> NewCodes { get; set; }

    }
    public sealed class ReserveIdsForNewEntities : Vanrise.BusinessProcess.BaseAsyncActivity<ReserveIdsForNewEntitiesInput>
    {
        [RequiredArgument]
        public InArgument<IEnumerable<AddedZone>> NewZones { get; set; }

        [RequiredArgument]
        public InArgument<IEnumerable<AddedCode>> NewCodes { get; set; }

        protected override void DoWork(ReserveIdsForNewEntitiesInput inputArgument, Vanrise.BusinessProcess.AsyncActivityHandle handle)
        {
            IEnumerable<AddedZone> zoneList = inputArgument.NewZones;
            IEnumerable<AddedCode> codeList = inputArgument.NewCodes;

            SaleZoneManager zoneManager = new SaleZoneManager();
            long zoneStartingId = zoneManager.ReserveIDRange(zoneList.Count());

            foreach (AddedZone zone in zoneList)
            {
                zone.ZoneId = zoneStartingId++;
            }

            SaleCodeManager codeManager = new SaleCodeManager();
            long codeStartingId = codeManager.ReserveIDRange(codeList.Count());

            foreach (AddedCode code in codeList)
            {
                code.CodeId = codeStartingId++;
            }

        }

        protected override ReserveIdsForNewEntitiesInput GetInputArgument(AsyncCodeActivityContext context)
        {
            return new ReserveIdsForNewEntitiesInput()
            {
                NewCodes = this.NewCodes.Get(context),
                NewZones = this.NewZones.Get(context)
            };
        }
    }
}
