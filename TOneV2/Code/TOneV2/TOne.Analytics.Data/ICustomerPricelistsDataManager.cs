using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TOne.Analytics.Entities;

namespace TOne.Analytics.Data
{
    public interface ICustomerPricelistsDataManager : IDataManager
    {
        Vanrise.Entities.BigResult<CustomerPriceLists> GetCustomerPriceLists(Vanrise.Entities.DataRetrievalInput<string> input);
        Vanrise.Entities.BigResult<CustomerPriceListDetail> GetCustomerPriceListDetails(Vanrise.Entities.DataRetrievalInput<int> input);
    }
}
