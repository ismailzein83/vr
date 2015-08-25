using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TOne.BusinessEntity.Entities;

namespace TOne.BusinessEntity.Data
{
    public interface ISupplierPricelistsDataManager:IDataManager
    {
        Vanrise.Entities.BigResult<PriceLists> GetSupplierPriceLists(Vanrise.Entities.DataRetrievalInput<string> input);
        Vanrise.Entities.BigResult<CustomerPriceListDetail> GetSupplierPriceListDetails(Vanrise.Entities.DataRetrievalInput<int> input);
    }
}
