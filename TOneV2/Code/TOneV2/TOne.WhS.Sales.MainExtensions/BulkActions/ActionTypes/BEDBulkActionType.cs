using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TOne.WhS.Sales.Entities;

namespace TOne.WhS.Sales.MainExtensions
{
	public class BEDBulkActionType : BulkActionType
	{
		public override Guid ConfigId
		{
			get { return new Guid("310EAF9D-68B5-466A-9CC4-96121B03A5FD"); }
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
