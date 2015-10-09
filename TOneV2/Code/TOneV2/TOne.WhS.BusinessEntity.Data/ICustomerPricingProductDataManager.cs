using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TOne.WhS.BusinessEntity.Entities;

namespace TOne.WhS.BusinessEntity.Data
{
   public interface ICustomerPricingProductDataManager:IDataManager
    {
       Vanrise.Entities.BigResult<CustomerPricingProductDetail> GetFilteredCustomerPricingProducts(Vanrise.Entities.DataRetrievalInput<CustomerPricingProductQuery> input);

       CustomerPricingProductDetail GetCustomerPricingProduct(int customerPricingProductId);
       bool Insert(List<CustomerPricingProduct> customerPricingProduct);
       List<CustomerPricingProduct> GetCustomerPricingProductByCustomerID(int customerId);
       bool Delete(int customerPricingProductId);
    }
}
