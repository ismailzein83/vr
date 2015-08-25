using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TOne.BusinessEntity.Entities;

namespace TOne.BusinessEntity.Data
{
    public interface ICustomerPricelistsDataManager : IDataManager
    {
        Vanrise.Entities.BigResult<PriceLists> GetCustomerPriceLists(Vanrise.Entities.DataRetrievalInput<string> input);
        Vanrise.Entities.BigResult<CustomerPriceListDetail> GetCustomerPriceListDetails(Vanrise.Entities.DataRetrievalInput<int> input);
    }
}
