using System;

namespace TOne.WhS.BusinessEntity.Entities
{
	public class SalePricelistRateChange
	{
		public string ZoneName { get; set; }
		public long? ZoneId { get; set; }
		public int RoutingProductId { get; set; }
		public int CountryId { get; set; }
		public Decimal Rate { get; set; }
		public Decimal? RecentRate { get; set; }
		public Decimal? RecentRateConverted { get; set; }
		public int? RecentCurrencyId { get; set; }
		public RateChangeType ChangeType { get; set; }
		public int? RateTypeId { get; set; }
		public long PricelistId { get; set; }
		public DateTime BED { get; set; }
		public DateTime? EED { get; set; }
		public int? CurrencyId { get; set; }
	}
}
