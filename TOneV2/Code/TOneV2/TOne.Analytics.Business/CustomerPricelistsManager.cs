using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TOne.Analytics.Data;
using TOne.Analytics.Entities;

namespace TOne.Analytics.Business
{
   public class CustomerPricelistsManager
    {
       public Vanrise.Entities.IDataRetrievalResult<CustomerPriceLists> GetCustomerPriceLists(Vanrise.Entities.DataRetrievalInput<string> input)
       {

           ICustomerPricelistsDataManager dataManager = AnalyticsDataManagerFactory.GetDataManager<ICustomerPricelistsDataManager>();
           return Vanrise.Common.DataRetrievalManager.Instance.ProcessResult(input, dataManager.GetCustomerPriceLists(input));
       }

       public Vanrise.Entities.IDataRetrievalResult<CustomerPriceListDetail> GetCustomerPriceListDetails(Vanrise.Entities.DataRetrievalInput<int> input)
       {

           ICustomerPricelistsDataManager dataManager = AnalyticsDataManagerFactory.GetDataManager<ICustomerPricelistsDataManager>();
           return Vanrise.Common.DataRetrievalManager.Instance.ProcessResult(input, dataManager.GetCustomerPriceListDetails(input));
       }
    }
}
