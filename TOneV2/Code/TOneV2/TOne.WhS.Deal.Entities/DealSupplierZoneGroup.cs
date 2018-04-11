using System;
using System.Collections.Generic;
using System.Linq;
using Vanrise.Entities;

namespace TOne.WhS.Deal.Entities
{
	public abstract class BaseDealSupplierZoneGroup : IDateEffectiveSettings
	{
		public int DealId { get; set; }

		public int DealSupplierZoneGroupNb { get; set; }

		public int SupplierId { get; set; }

		public List<DealSupplierZoneGroupZoneItem> Zones { get; set; }

		public DateTime BED { get; set; }

		public DateTime? EED { get; set; }
	}

	public class DealSupplierZoneGroup : BaseDealSupplierZoneGroup
	{
		public IOrderedEnumerable<DealSupplierZoneGroupTier> Tiers { get; set; }
	}

	public class DealSupplierZoneGroupWithoutRate : BaseDealSupplierZoneGroup
	{
		public IOrderedEnumerable<DealSupplierZoneGroupTierWithoutRate> Tiers { get; set; }
	}


	public class DealSupplierZoneGroupZoneItem : IDateEffectiveSettings
	{
		public long ZoneId { get; set; }
		public DateTime BED { get; set; }
		public DateTime? EED { get; set; }
	}


	public abstract class BaseDealSupplierZoneGroupTier
	{
		public int TierNumber { get; set; }

		public int? RetroActiveFromTierNumber { get; set; }

		public int? VolumeInSeconds { get; set; }
	}

	public class DealSupplierZoneGroupTier : BaseDealSupplierZoneGroupTier
	{
		public Dictionary<long, List<DealRate>> RatesByZoneId { get; set; }
	}

	public class DealSupplierZoneGroupTierWithoutRate : BaseDealSupplierZoneGroupTier
	{
	}

}