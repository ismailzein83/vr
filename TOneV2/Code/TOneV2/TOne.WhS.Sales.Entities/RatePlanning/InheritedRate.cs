using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TOne.WhS.Sales.Entities
{
	public class InheritedRatesByZone : Vanrise.Common.VRDictionary<long, ZoneInheritedRates>
	{
		public DateTime GetMinimumBED()
		{
			var dates = new List<DateTime>();
			foreach (ZoneInheritedRates zoneInheritedRates in base.Values)
			{
				dates.Add(zoneInheritedRates.GetMinimumBED());
			}
			return dates.Min();
		}
	}

	public class ZoneInheritedRates
	{
		public ZoneItem ZoneItem { get; set; }

		public ZoneInheritedRate NormalRate { get; set; }

		public Vanrise.Common.VRDictionary<int, ZoneInheritedRate> OtherRatesByType { get; set; }

		public DateTime GetMinimumBED()
		{
			var dates = new List<DateTime>();
			if (NormalRate != null)
				dates.Add(NormalRate.BED);
			if (OtherRatesByType != null)
			{
				foreach (ZoneInheritedRate inheritedOtherRate in OtherRatesByType.Values)
					dates.Add(inheritedOtherRate.BED);
			}
			return dates.Min();
		}
	}

	public class ZoneInheritedRate
	{
		public int? RateTypeId { get; set; }

		public DateTime BED { get; set; }

		public DateTime? EED { get; set; }
	}

	public class ZonesSoldOn : Vanrise.Common.VRDictionary<long, DateTime>
	{

	}
}
