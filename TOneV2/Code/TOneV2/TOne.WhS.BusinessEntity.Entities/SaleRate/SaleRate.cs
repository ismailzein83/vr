using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
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

	public class OverlappedRatesByZone : Dictionary<long, ZoneOverlappedRates>
	{

	}

	public class ZoneOverlappedRates
	{
		public List<SaleRate> NormalRates { get; set; }

		public Dictionary<int, List<SaleRate>> OtherRatesByType { get; set; }

		#region Normal Rate Methods

		public DateTime? GetLastOverlappedNormalRateEED(DateTime inheritedRateBED, DateTime? inheritedRateEED)
		{
			IEnumerable<SaleRate> overlappedNormalRates = GetOverlappedNormalRates(inheritedRateBED, inheritedRateEED);
			return GetLastEED(overlappedNormalRates);
		}

		public IEnumerable<SaleRate> GetOverlappedNormalRates(DateTime inheritedRateBED, DateTime? inheritedRateEED)
		{
			if (NormalRates == null)
				return null;
			return NormalRates.FindAllRecords(x => IsOverlapped(x, inheritedRateBED, inheritedRateEED));
		}

		#endregion

		#region Other Rate Methods

		public DateTime? GetLastOverlappedOtherRateEED(int rateTypeId, DateTime inheritedRateBED, DateTime? inheritedRateEED)
		{
			IEnumerable<SaleRate> overlappedOtherRates = GetOverlappedOtherRates(rateTypeId, inheritedRateBED, inheritedRateEED);
			return GetLastEED(overlappedOtherRates);
		}

		public IEnumerable<SaleRate> GetOverlappedOtherRates(int rateTypeId, DateTime inheritedRateBED, DateTime? inheritedRateEED)
		{
			if (OtherRatesByType == null)
				return null;
			List<SaleRate> otherRatesByType;
			if (!OtherRatesByType.TryGetValue(rateTypeId, out otherRatesByType))
				return null;
			return otherRatesByType.FindAllRecords(x => IsOverlapped(x, inheritedRateBED, inheritedRateEED));
		}

		#endregion

		#region Common Methods

		private DateTime? GetLastEED(IEnumerable<SaleRate> overlappedRates)
		{
			if (overlappedRates == null || overlappedRates.Count() == 0)
				return null;
			return overlappedRates.OrderByDescending(x => x.BED).FirstOrDefault().EED.Value;
		}

		private bool IsOverlapped(SaleRate saleRate, DateTime beginEffectiveDate, DateTime? endEffectiveDate)
		{
			return (endEffectiveDate.VRGreaterThan(saleRate.BED) && saleRate.EED > beginEffectiveDate); // Overlapped rates always have an EED
		}

		#endregion
	}
}

