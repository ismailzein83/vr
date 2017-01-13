using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TOne.WhS.Sales.Entities;

namespace TOne.WhS.Sales.MainExtensions
{
	public class RoutingProductBulkActionType : BulkActionType
	{
		public override Guid ConfigId
		{
			get { return new Guid("67D0BD5E-8B7A-407E-B03B-5FAE05F10A01"); }
		}

		public override bool IsApplicableToZone(IActionApplicableToZoneContext context)
		{
			throw new NotImplementedException();
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
