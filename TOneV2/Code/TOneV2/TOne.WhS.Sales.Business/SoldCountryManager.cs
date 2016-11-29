using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TOne.WhS.BusinessEntity.Business;
using TOne.WhS.BusinessEntity.Entities;
using TOne.WhS.Sales.Entities;
using Vanrise.Common.Business;
using Vanrise.Common;

namespace TOne.WhS.Sales.Business
{
	public class SoldCountryManager
	{
		#region Fields

		private CountryManager _countryManager = new Vanrise.Common.Business.CountryManager();

		#endregion

		#region Public Methods

		public Vanrise.Entities.IDataRetrievalResult<SoldCountryDetail> GetFilteredSoldCountries(Vanrise.Entities.DataRetrievalInput<SoldCountryQuery> input)
		{
			var customerCountryManager = new CustomerCountryManager();
			IEnumerable<CustomerCountry2> customerCountries = customerCountryManager.GetSoldCountries(input.Query.CustomerId, input.Query.EffectiveOn);
			if (customerCountries == null)
				customerCountries = new List<CustomerCountry2>();
			return Vanrise.Common.DataRetrievalManager.Instance.ProcessResult(input, customerCountries.ToBigResult(input, null, SoldCountryDetailMapper));
		}

		#endregion

		#region Mappers

		private SoldCountryDetail SoldCountryDetailMapper(CustomerCountry2 customerCountry)
		{
			return new SoldCountryDetail()
			{
				Entity = new SoldCountry()
				{
					CountryId = customerCountry.CountryId,
					Name = _countryManager.GetCountryName(customerCountry.CountryId),
					BED = customerCountry.BED,
					EED = customerCountry.EED
				},
				IsSoldInFuture = (customerCountry.BED > DateTime.Now.Date)
			};
		}

		#endregion
	}
}
