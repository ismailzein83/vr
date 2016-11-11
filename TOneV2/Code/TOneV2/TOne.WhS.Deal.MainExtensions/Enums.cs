using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TOne.WhS.Deal.MainExtensions
{
	public enum CarrierFilterType
	{
		All = 0,
		Specific = 1,
		AllExcept = 2
	}

	public enum ZoneRateEvaluationType
	{
		AverageRate = 0,
		MaximumRate = 1,
		BasedOnTraffic = 2
	}
}
