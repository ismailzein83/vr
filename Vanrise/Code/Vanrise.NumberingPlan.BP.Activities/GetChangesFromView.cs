using System;
using System.Activities;
using System.Collections.Generic;
using System.Linq;
using Vanrise.Common;
using Vanrise.NumberingPlan.Business;
using Vanrise.NumberingPlan.Entities;
namespace Vanrise.NumberingPlan.BP.Activities
{
	public class GetChangesFromView : CodeActivity
	{
		[RequiredArgument]
		public InArgument<int> SellingNumberPlanId { get; set; }

		[RequiredArgument]
		public OutArgument<Dictionary<long, IEnumerable<SaleCode>>> SaleCodesByZoneId { get; set; }

		[RequiredArgument]
		public OutArgument<Changes> Changes { get; set; }

		[RequiredArgument]
		public OutArgument<AllNewZones> AllNewZones { get; set; }

		protected override void Execute(CodeActivityContext context)
		{

			int sellingNumberPlanId = SellingNumberPlanId.Get(context);
			SaleCodeManager saleCodeManager = new SaleCodeManager();
			CodePreparationManager codePreparationManager = new CodePreparationManager();
			Changes changes = codePreparationManager.GetChanges(sellingNumberPlanId);

			HashSet<long> zoneIds = new HashSet<long>();

			foreach (DeletedZone deletedZone in changes.DeletedZones)
			{
				zoneIds.Add(deletedZone.ZoneId);
			}

			foreach (RenamedZone renamedZone in changes.RenamedZones)
			{
				zoneIds.Add(renamedZone.ZoneId);
			}

			List<SaleCode> saleCodes = saleCodeManager.GetSaleCodesByZoneIDs(zoneIds.ToList(), DateTime.Now);
			Dictionary<long, IEnumerable<SaleCode>> saleCodesByZoneId = new Dictionary<long, IEnumerable<SaleCode>>();

			foreach (var zone in zoneIds)
			{
				saleCodesByZoneId.Add(zone, saleCodes.FindAllRecords(x => x.ZoneId == zone));
			}

			SaleCodesByZoneId.Set(context, saleCodesByZoneId);
			Changes.Set(context, changes);
			AllNewZones.Set(context, new AllNewZones() { Zones = changes.NewZones });
		}
	}
}
