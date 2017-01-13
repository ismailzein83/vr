﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TOne.WhS.Sales.Entities;

namespace TOne.WhS.Sales.MainExtensions
{
	public class RateBulkAction : BulkActionType
	{
		public override Guid ConfigId
		{
			get { return new Guid("A893F3C6-D4BF-4C60-BA7D-2A773791D7BD"); }
		}

		public Guid? RateCalculationCostColumnConfigId { get; set; }

		public RateCalculationMethod RateCalculationMethod { get; set; }

		public bool OverrideDraftNewRate { get; set; }

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
			ZoneItem zoneItem = context.GetZoneItem(context.ZoneDraft.ZoneId);
		}
	}
}
