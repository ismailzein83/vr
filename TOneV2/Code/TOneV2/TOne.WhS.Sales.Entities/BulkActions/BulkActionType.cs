﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TOne.WhS.Sales.Entities
{
	public abstract class BulkActionType
	{
		public abstract Guid ConfigId { get; }

		public abstract bool IsApplicableToZone(IActionApplicableToZoneContext context);

		public abstract void ApplyBulkActionToZoneItem(IApplyBulkActionToZoneItemContext context);

		public abstract void ApplyBulkActionToZoneDraft(IApplyBulkActionToZoneDraftContext context);
	}

	public interface IActionApplicableToZoneContext
	{
		long ZoneId { get; }

		ZoneChanges ZoneDraft { get; }
	}

	public interface IApplyBulkActionToZoneItemContext
	{
		ZoneItem ZoneItem { get; }
	}

	public interface IApplyBulkActionToZoneDraftContext
	{
		ZoneChanges ZoneDraft { get; }

		ZoneItem GetZoneItem(long zoneId);
	}
}
