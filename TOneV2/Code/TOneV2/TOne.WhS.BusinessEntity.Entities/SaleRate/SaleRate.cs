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

	public class BaseRatesByZone : Dictionary<long, BaseRates>
	{
		public void AddZoneBaseRate(long zoneId, IBaseRates entity, int countryId, int? rateTypeId, DateTime rateBED, DateTime? rateEED)
		{
			BaseRates baseRates;
			if (!base.TryGetValue(zoneId, out baseRates))
			{
				baseRates = new BaseRates()
				{
					ZoneId = zoneId,
					Entity = entity,
					CountryId = countryId
				};
				base.Add(zoneId, baseRates);
			}
			if (rateTypeId.HasValue)
			{
				if (!baseRates.BaseOtherRates.ContainsKey(rateTypeId.Value))
				{
					baseRates.BaseOtherRates.Add(rateTypeId.Value, new BaseRate()
					{
						RateTypeId = rateTypeId.Value,
						BED = rateBED,
						EED = rateEED
					});
				}
			}
			else
			{
				baseRates.BaseNormalRate = new BaseRate()
				{
					RateTypeId = rateTypeId,
					BED = rateBED,
					EED = rateEED
				};
			}
		}

		public DateTime GetMinimumBED()
		{
			var dates = new List<DateTime>();
			foreach (BaseRates zoneBaseRates in base.Values)
			{
				dates.Add(zoneBaseRates.GetMinimumBED());
			}
			return dates.Min();
		}
	}

	public class BaseRates
	{
		public BaseRates()
		{
			BaseOtherRates = new Dictionary<int, BaseRate>();
		}

		public long ZoneId { get; set; }

		public int CountryId { get; set; }

		public IBaseRates Entity { get; set; }

		public BaseRate BaseNormalRate { get; set; }

		public Dictionary<int, BaseRate> BaseOtherRates { get; set; }

		public DateTime GetMinimumBED()
		{
			var dates = new List<DateTime>();
			if (BaseNormalRate != null)
				dates.Add(BaseNormalRate.BED);
			if (BaseOtherRates != null)
			{
				foreach (BaseRate baseOtherRate in BaseOtherRates.Values)
					dates.Add(baseOtherRate.BED);
			}
			return dates.Min();
		}
	}

	public class BaseRate
	{
		public int? RateTypeId { get; set; }

		public DateTime BED { get; set; }

		public DateTime? EED { get; set; }
	}

	public interface IBaseRates
	{
		void SetNormalRateBED(DateTime beginEffectiveDate);

		void SetOtherRateBED(int rateTypeId, DateTime beginEffectiveDate);
	}
}
