using TOne.WhS.Sales.Entities;
using System.Collections.Generic;

namespace TOne.WhS.Sales.Data
{
    public interface INewCustomerCountryDataManager : IDataManager, Vanrise.Data.IBulkApplyDataManager<NewCustomerCountry>
    {
        long ProcessInstanceId { set; }
        void ApplyNewCustomerCountriesToDB(IEnumerable<NewCustomerCountry> newCustomerCountries);
        IEnumerable<int> GetAffectedCustomerIds(long processInstanceId);
    }
}
