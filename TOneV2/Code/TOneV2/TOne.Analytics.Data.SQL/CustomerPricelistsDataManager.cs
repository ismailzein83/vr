using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TOne.Analytics.Entities;
using TOne.Data.SQL;

namespace TOne.Analytics.Data.SQL
{
    public class CustomerPricelistsDataManager : BaseTOneDataManager, ICustomerPricelistsDataManager
    {
       public Vanrise.Entities.BigResult<CustomerPriceLists> GetCustomerPriceLists(Vanrise.Entities.DataRetrievalInput<string> input)
        {
            return new Vanrise.Entities.BigResult<CustomerPriceLists>();
        }
    }

}
