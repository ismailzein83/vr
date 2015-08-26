using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TOne.BusinessEntity.Data;
using TOne.BusinessEntity.Entities;


namespace TOne.BusinessEntity.Business
{
    public class CustomerPricelistsManager : BasePricelistManager<CustomerPriceListDetail>
    {
        public Vanrise.Entities.IDataRetrievalResult<PriceLists> GetCustomerPriceLists(Vanrise.Entities.DataRetrievalInput<string> input)
       {

           ICustomerPricelistsDataManager dataManager = BEDataManagerFactory.GetDataManager<ICustomerPricelistsDataManager>();
           return Vanrise.Common.DataRetrievalManager.Instance.ProcessResult(input, dataManager.GetCustomerPriceLists(input));
       }
    }
}
