using System;
using System.Collections.Generic;
using System.ComponentModel;
using Vanrise.Common;

namespace TOne.WhS.BusinessEntity.Entities
{
	public class SaleRate : IRate, Vanrise.Entities.IDateEffectiveSettings
	{
		public long SaleRateId { get; set; }

		public long ZoneId { get; set; }

		public int PriceListId { get; set; }

		public int? CurrencyId { get; set; }

		public int? RateTypeId { get; set; }

		public decimal Rate { get; set; }

		public DateTime BED { get; set; }

		public DateTime? EED { get; set; }

		public string SourceId { get; set; }
		public RateChangeType RateChange { get; set; }
	}

	public class SaleRatePriceList
	{
		public SaleRate Rate { get; set; }

		public Dictionary<int, SaleRate> RatesByRateType { get; set; }

		//TODO: Remove this property
		//public SalePriceList PriceList { get; set; }

		public string SourceId { get; set; }
	}

	public enum RateChangeType
	{

		[Description("Not Changed")]
		NotChanged = 0,

		[Description("New")]
		New = 1,

		[Description("Deleted")]
		Deleted = 2,

		[Description("Increase")]
		Increase = 3,

		[Description("Decrease")]
		Decrease = 4
	}

	public class RatesByZone : Vanrise.Common.VRDictionary<long, ZoneRates>
	{

	}

	public class ZoneRates
	{
		public List<SaleRate> NormalRates { get; set; }

		public Vanrise.Common.VRDictionary<int, List<SaleRate>> OtherRatesByType { get; set; }

		public IEnumerable<SaleRate> GetOverlappedNormalRates(DateTime beginEffectiveDate, DateTime? endEffectiveDate)
		{
			if (NormalRates == null)
				return null;
			return NormalRates.FindAllRecords(x => IsOverlapped(x, beginEffectiveDate, endEffectiveDate));
		}

		public IEnumerable<SaleRate> GetOverlappedOtherRates(int rateTypeId, DateTime beginEffectiveDate, DateTime? endEffectiveDate)
		{
			if (OtherRatesByType == null)
				return null;
			List<SaleRate> otherRatesByType;
			if (!OtherRatesByType.TryGetValue(rateTypeId, out otherRatesByType))
				return null;
			return otherRatesByType.FindAllRecords(x => IsOverlapped(x, beginEffectiveDate, endEffectiveDate));
		}

		private bool IsOverlapped(SaleRate saleRate, DateTime beginEffectiveDate, DateTime? endEffectiveDate)
		{
			return (endEffectiveDate.VRGreaterThan(saleRate.BED) && saleRate.EED.VRGreaterThan(beginEffectiveDate));
		}
	}
}

