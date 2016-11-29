using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TOne.WhS.BusinessEntity.Entities;

namespace TOne.WhS.Sales.Entities
{
	public class ExistingCustomerCountry : Vanrise.Entities.IDateEffectiveSettings
	{
		public CustomerCountry2 CustomerCountryEntity { get; set; }

		public ChangedCustomerCountry ChangedCustomerCountry { get; set; }

		public DateTime BED
		{
			get { return CustomerCountryEntity.BED; }
		}

		public DateTime? EED
		{
			get { return ChangedCustomerCountry != null ? ChangedCustomerCountry.EED : CustomerCountryEntity.EED; }
		}
	}
}
