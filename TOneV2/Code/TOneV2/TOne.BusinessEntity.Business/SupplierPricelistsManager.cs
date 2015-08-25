using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TOne.BusinessEntity.Data;
using TOne.BusinessEntity.Entities;


namespace TOne.BusinessEntity.Business
{
    public class SupplierPricelistsManager
    {
        public Vanrise.Entities.IDataRetrievalResult<PriceLists> GetSupplierPriceLists(Vanrise.Entities.DataRetrievalInput<string> input)
        {

            ISupplierPricelistsDataManager dataManager = BEDataManagerFactory.GetDataManager<ISupplierPricelistsDataManager>();
            return Vanrise.Common.DataRetrievalManager.Instance.ProcessResult(input, dataManager.GetSupplierPriceLists(input));
        }

        public Vanrise.Entities.IDataRetrievalResult<CustomerPriceListDetail> GetSupplierPriceListDetails(Vanrise.Entities.DataRetrievalInput<int> input)
        {

            ISupplierPricelistsDataManager dataManager = BEDataManagerFactory.GetDataManager<ISupplierPricelistsDataManager>();
            return Vanrise.Common.DataRetrievalManager.Instance.ProcessResult(input, dataManager.GetSupplierPriceListDetails(input));
        }
    }
}
