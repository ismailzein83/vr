﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TOne.WhS.BusinessEntity.Entities;
using TOne.WhS.Routing.Entities;

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
		SalePriceListOwnerType OwnerType { get; }

		int OwnerId { get; }

		SaleZone SaleZone { get; }

		ZoneChanges ZoneDraft { get; }

		SaleEntityZoneRate GetSellingProductZoneRate(int sellingProductId, long zoneId);

		SaleEntityZoneRate GetCustomerZoneRate(int customerId, int sellingProductId, long zoneId);
	}

	public interface IApplyBulkActionToZoneItemContext
	{
		ZoneItem ZoneItem { get; }

		ZoneChanges ZoneDraft { get; set; }

		ZoneItem GetContextZoneItem(long zoneId);

		int? GetCostCalculationMethodIndex(Guid costCalculationMethodConfigId);
	}

	public interface IApplyBulkActionToZoneDraftContext
	{
		ZoneChanges ZoneDraft { get; }

		ZoneItem GetZoneItem(long zoneId);

		int? GetCostCalculationMethodIndex(Guid costCalculationMethodConfigId);
	}
}
