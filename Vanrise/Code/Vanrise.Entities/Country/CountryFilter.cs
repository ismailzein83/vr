using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Vanrise.Entities
{
	public class CountryFilter
	{
		public IEnumerable<int> ExcludedCountryIds { get; set; }

		public IEnumerable<ICountryFilter> Filters { get; set; }
	}

	public interface ICountryFilter
	{
		bool IsExcluded(ICountryFilterContext context);
	}

	public interface ICountryFilterContext
	{
		Country Country { get; }
	}

	public class CountryFilterContext : ICountryFilterContext
	{
		public Country Country { get; set; }
	}
}
