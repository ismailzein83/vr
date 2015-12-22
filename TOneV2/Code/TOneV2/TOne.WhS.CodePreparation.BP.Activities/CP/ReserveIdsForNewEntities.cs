using System;
using System.Activities;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TOne.WhS.BusinessEntity.Business;
using TOne.WhS.CodePreparation.Entities.CP.Processing;

namespace TOne.WhS.CodePreparation.BP.Activities
{
    public sealed class ReserveIdsForNewEntities : CodeActivity
    {
        [RequiredArgument]
        public InArgument<IEnumerable<AddedZone>> NewZones { get; set; }

        [RequiredArgument]
        public InArgument<IEnumerable<AddedCode>> NewCodes { get; set; }

        protected override void Execute(CodeActivityContext context)
        {
            IEnumerable<AddedZone> zoneList = NewZones.Get(context);
            IEnumerable<AddedCode> codeList = NewCodes.Get(context);

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

    }
}
