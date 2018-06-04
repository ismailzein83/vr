using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Vanrise.Entities;

namespace TOne.WhS.Sales.Business
{
	public class RPCountrySoldToCustomerFilter : ICountryFilter
	{
		private Dictionary<int, DateTime> _datesByCountry;

		public int CustomerId { get; set; }

		public DateTime? EffectiveOn { get; set; }

		public bool IsEffectiveInFuture { get; set; }

		public bool IsExcluded(ICountryFilterContext context)
		{
			if (_datesByCountry == null)
			{
				_datesByCountry = UtilitiesManager.GetDatesByCountry(CustomerId, EffectiveOn, IsEffectiveInFuture);
			}
			return (!_datesByCountry.ContainsKey(context.Country.CountryId));
		}
	}
}
