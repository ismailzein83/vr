using System;
using System.Activities;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TOne.WhS.BusinessEntity.Business;
using TOne.WhS.BusinessEntity.Entities;
using TOne.WhS.CodePreparation.Business;
using TOne.WhS.CodePreparation.Entities;
using TOne.WhS.CodePreparation.Entities.Processing;
using Vanrise.Common;
using Vanrise.BusinessProcess;
using Vanrise.Entities;
namespace TOne.WhS.CodePreparation.BP.Activities
{
    public class GetChanges : CodeActivity
    {
        [RequiredArgument]
        public InArgument<int> SellingNumberPlanId { get; set; }

        [RequiredArgument]
        public OutArgument<Dictionary<int, IEnumerable<SaleCode>>> ZoneBySaleCodes { get; set; }

        [RequiredArgument]
        public OutArgument<Changes> Changes { get; set; }

        protected override void Execute(CodeActivityContext context)
        {

            int sellingNumberPlanId = SellingNumberPlanId.Get(context);
            SaleCodeManager saleCodeManager = new SaleCodeManager();
            CodePreparationManager codePreparationManager = new CodePreparationManager();
            Changes changes = codePreparationManager.GetChanges(sellingNumberPlanId);


            List<int> zoneIds = new List<int>();
            zoneIds.AddRange(changes.DeletedZones.Select(x => x.ZoneId));

            if (changes.RenamedZones.Count > 0)
                foreach (RenamedZone renamedZone in changes.RenamedZones)
                {
                    if (!zoneIds.Contains(renamedZone.ZoneId.Value))
                        zoneIds.Add(renamedZone.ZoneId.Value);
                }

            List<SaleCode> saleCodes = saleCodeManager.GetSaleCodesByZoneIDs(zoneIds, DateTime.Now);
            Dictionary<int, IEnumerable<SaleCode>> zoneBySaleCodes = new Dictionary<int, IEnumerable<SaleCode>>();

            foreach (var zone in zoneIds)
            {
                zoneBySaleCodes.Add(zone, saleCodes.FindAllRecords(x => x.ZoneId == zone));
            }

            ZoneBySaleCodes.Set(context, zoneBySaleCodes);
            Changes.Set(context, changes);
        }
    }
}
