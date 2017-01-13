using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TOne.WhS.Sales.Entities;

namespace TOne.WhS.Sales.MainExtensions
{
	public class EEDBulkActionType : BulkActionType
	{
		public override Guid ConfigId
		{
			get { return new Guid("736034AB-115F-464B-919D-052EBFDEDD5C"); }
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
