using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Vanrise.Common;

namespace TOne.WhS.Deal.Entities
{
	public struct DealZoneGroup
	{
		public int DealId { get; set; }
		public int ZoneGroupNb { get; set; }
	}

	public struct DealZoneGroupTier
	{
		public int DealId { get; set; }
		public int ZoneGroupNb { get; set; }
		public int? TierNb { get; set; }
	}

	public struct AccountZoneGroup
	{
		public int AccountId { get; set; }
		public long ZoneId { get; set; }
	}

	public struct DealDetailedZoneGroupTier
	{
		public int DealId { get; set; }

		public int ZoneGroupNb { get; set; }

		public int? TierNb { get; set; }

		public int? RateTierNb { get; set; }

		public DateTime FromTime { get; set; }

		public DateTime ToTime { get; set; }
	}

	public struct DealDetailedZoneGroupTierWithoutRate
	{
		public int DealId { get; set; }

		public int ZoneGroupNb { get; set; }

		public int? TierNb { get; set; }

		public DateTime FromTime { get; set; }

		public DateTime ToTime { get; set; }
	}

	public class DealZoneGroupTierDetailsWithoutRate
	{
		public int TierNb { get; set; }
		public decimal? VolumeInSeconds { get; set; }
		public int? RetroActiveFromTierNb { get; set; }
	}

	public class DealZoneGroupTierDetails
	{
		public int TierNb { get; set; }
		public decimal? VolumeInSeconds { get; set; }
		public decimal Rate { get; set; }
		public int CurrencyId { get; set; }
		public int? RetroActiveFromTierNb { get; set; }
	}

	public class DealProgressData
	{
		public int DealId { get; set; }

		public int ZoneGroupNb { get; set; }

		public bool IsSale { get; set; }

		public int CurrentTierNb { get; set; }

		public decimal ReachedDurationInSeconds { get; set; }

		public decimal? TargetDurationInSeconds { get; set; }
	}


	public class DealZoneRateByZoneGroup : Dictionary<DealZoneGroup, DealZoneRateByZoneId>
	{

	}
	public class DealZoneRateByZoneId : Dictionary<long, DealZoneRateByTireNB>
	{

	}
	public class DealZoneRateByTireNB : Dictionary<int, List<DealZoneRate>>
	{

	}
}