using TOne.WhS.Sales.Data;
using TOne.WhS.Sales.Entities;
using System.Collections.Generic;

namespace TOne.WhS.Sales.Business
{
    public class NewCustomerCountryManager : INewCustomerCountryDataManager
    {
        public long ProcessInstanceId { get; set; }

        public void ApplyNewCustomerCountriesToDB(IEnumerable<NewCustomerCountry> newCustomerCountries)
        {
            INewCustomerCountryDataManager dataManager = SalesDataManagerFactory.GetDataManager<INewCustomerCountryDataManager>();
            dataManager.ApplyNewCustomerCountriesToDB(newCustomerCountries);
        }

        public IEnumerable<int> GetAffectedCustomerIds(long processInstanceId)
        {
            INewCustomerCountryDataManager dataManager = SalesDataManagerFactory.GetDataManager<INewCustomerCountryDataManager>();
            return dataManager.GetAffectedCustomerIds(processInstanceId);
        }

        #region Bulk

        public object InitialiazeStreamForDBApply()
        {
            INewCustomerCountryDataManager dataManager = SalesDataManagerFactory.GetDataManager<INewCustomerCountryDataManager>();
            return dataManager.InitialiazeStreamForDBApply();
        }

        public void WriteRecordToStream(NewCustomerCountry record, object dbApplyStream)
        {
            INewCustomerCountryDataManager dataManager = SalesDataManagerFactory.GetDataManager<INewCustomerCountryDataManager>();
            dataManager.WriteRecordToStream(record, dbApplyStream);
        }

        public object FinishDBApplyStream(object dbApplyStream)
        {
            INewCustomerCountryDataManager dataManager = SalesDataManagerFactory.GetDataManager<INewCustomerCountryDataManager>();
            return dataManager.FinishDBApplyStream(dbApplyStream);
        }

        #endregion
    }
}
