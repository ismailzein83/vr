using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TOne.WhS.Sales.Entities;

namespace TOne.WhS.Sales.Data
{
	public interface INewCustomerCountryDataManager : IDataManager, Vanrise.Data.IBulkApplyDataManager<NewCustomerCountry>
	{
		long ProcessInstanceId { set; }

		void ApplyNewCustomerCountriesToDB(IEnumerable<NewCustomerCountry> newCustomerCountries);
	}
}
