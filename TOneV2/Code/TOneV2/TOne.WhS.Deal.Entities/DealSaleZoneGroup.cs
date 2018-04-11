using System;
using System.Linq;
using System.Collections.Generic;
using Vanrise.Entities;

namespace TOne.WhS.Deal.Entities
{
	public abstract class BaseDealSaleZoneGroup : IDateEffectiveSettings
	{
		public int DealId { get; set; }

		public int DealSaleZoneGroupNb { get; set; }

		public int CustomerId { get; set; }

		public List<DealSaleZoneGroupZoneItem> Zones { get; set; }

		public DateTime BED { get; set; }

		public DateTime? EED { get; set; }
	}

	public class DealSaleZoneGroup : BaseDealSaleZoneGroup
	{
		public IOrderedEnumerable<DealSaleZoneGroupTier> Tiers { get; set; }
	}

	public class DealSaleZoneGroupWithoutRate : BaseDealSaleZoneGroup
	{
		public IOrderedEnumerable<DealSaleZoneGroupTierWithoutRate> Tiers { get; set; }
	}

	public class DealSaleZoneGroupZoneItem : IDateEffectiveSettings
	{
		public long ZoneId { get; set; }
		public DateTime BED { get; set; }
		public DateTime? EED { get; set; }
	}

	public abstract class BaseDealSaleZoneGroupTier
	{
		public int TierNumber { get; set; }

		public int? RetroActiveFromTierNumber { get; set; }

		public int? VolumeInSeconds { get; set; }
	}

	public class DealSaleZoneGroupTier : BaseDealSaleZoneGroupTier
	{
		public Dictionary<long, List<DealRate>> RatesByZoneId { get; set; }
	}

	public class DealSaleZoneGroupTierWithoutRate : BaseDealSaleZoneGroupTier
	{
	}

	public class DealRate
	{
		public long ZoneId { get; set; }
		public Decimal Rate { get; set; }
		public DateTime BED { get; set; }
		public DateTime? EED { get; set; }
		public int CurrencyId { get; set; }
	}
}