using System;

namespace TOne.WhS.BusinessEntity.Entities
{
	public class OtherSaleRateQuery
	{
		public string ZoneName { get; set; }

		public long ZoneId { get; set; }

		public int CountryId { get; set; }

		public SalePriceListOwnerType OwnerType { get; set; }

		public int OwnerId { get; set; }

		public DateTime EffectiveOn { get; set; }

		public int CurrencyId { get; set; }

		public bool IsSystemCurrency { get; set; }
	}
}
