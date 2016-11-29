using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TOne.WhS.Sales.Entities
{
	public class CustomerCountryToChange : Vanrise.BusinessProcess.Entities.IRuleTarget
	{
		private List<ExistingCustomerCountry> _changedExistingCustomerCountries = new List<ExistingCustomerCountry>();

		public int CountryId { get; set; }

		public DateTime CloseEffectiveDate { get; set; }

		public List<ExistingCustomerCountry> ChangedExistingCustomerCountries
		{
			get
			{
				return _changedExistingCustomerCountries;
			}
		}

		#region IRuleTarget Implementation

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
				return "CustomerCountryToChange";
			}
		}

		#endregion
	}
}
