using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TOne.WhS.Sales.Entities
{
	public class CustomerCountryToAdd : Vanrise.BusinessProcess.Entities.IRuleTarget, Vanrise.Entities.IDateEffectiveSettings
	{
		private List<ExistingCustomerCountry> _changedExistingCustomerCountries = new List<ExistingCustomerCountry>();

		public int CustomerId { get; set; }

		public int CountryId { get; set; }

		public DateTime BED { get; set; }

		public DateTime? EED { get; set; }

		public NewCustomerCountry NewCustomerCountry { get; set; }

		public List<ExistingCustomerCountry> ChangedExistingCustomerCountries
		{
			get
			{
				return _changedExistingCustomerCountries;
			}
		}

		#region IRuleTarget Properties

		public object Key
		{
			get
			{
				return CountryId;
			}
		}

		public string TargetType
		{
			get
			{
				return "CustomerCountryToAdd";
			}
		}

		#endregion
	}
}
