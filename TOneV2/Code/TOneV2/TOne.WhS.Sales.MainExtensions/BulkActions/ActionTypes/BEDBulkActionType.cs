using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TOne.WhS.Sales.Entities;
using Vanrise.Common;

namespace TOne.WhS.Sales.MainExtensions
{
	public class BEDBulkActionType : BulkActionType
	{
		public override Guid ConfigId
		{
			get { return new Guid("310EAF9D-68B5-466A-9CC4-96121B03A5FD"); }
		}

		public DateTime BED { get; set; }

		public override bool IsApplicableToZone(IActionApplicableToZoneContext context)
		{
			// The bulk action is applicable to the zone if a new normal rate exists
			return (context.ZoneDraft != null && context.ZoneDraft.NewRates != null && context.ZoneDraft.NewRates.Any(x => !x.RateTypeId.HasValue));
		}

		public override void ApplyBulkActionToZoneItem(IApplyBulkActionToZoneItemContext context)
		{
			throw new NotImplementedException();
		}

		public override void ApplyBulkActionToZoneDraft(IApplyBulkActionToZoneDraftContext context)
		{
			throw new NotImplementedException();
		}
	}
}
