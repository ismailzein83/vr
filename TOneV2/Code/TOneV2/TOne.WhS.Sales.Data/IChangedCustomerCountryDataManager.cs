using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TOne.WhS.Sales.Entities;

namespace TOne.WhS.Sales.Data
{
	public interface IChangedCustomerCountryDataManager : IDataManager, Vanrise.Data.IBulkApplyDataManager<ChangedCustomerCountry>
	{
		long ProcessInstanceId { set; }

		void ApplyChangedCustomerCountriesToDB(IEnumerable<ChangedCustomerCountry> changedCustomerCountries);
	}
}
