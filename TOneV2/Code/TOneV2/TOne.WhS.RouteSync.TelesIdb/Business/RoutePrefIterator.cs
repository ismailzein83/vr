using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Vanrise.Common;

namespace TOne.WhS.RouteSync.TelesIdb.Business
{
	public class RoutePrefIterator
	{
		public RouteIterator ExactIterator { get; set; }

		public RouteIterator LongestMatchIterator { get; set; }

		public string GetMatched(string pref)
		{
			string result = null;
			result = (ExactIterator != null) ? ExactIterator.GetExactMatch(pref) : null;
			if (string.IsNullOrEmpty(result))
				result = (LongestMatchIterator != null) ? LongestMatchIterator.GetLongestMatch(pref) : null;
			return result;
		}
	}

	public class RouteIterator : VRCodeIterator<string>
	{
		public RouteIterator(IEnumerable<string> routePrefs)
			: base(routePrefs)
		{
		}

		protected override List<string> GetCodes(string routePref)
		{
			if (!String.IsNullOrEmpty(routePref))
				return new List<string> { routePref };
			else
				return null;
		}
	}
}
