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
       bool Insert(List<CustomerPricingProductDetail> customerPricingProduct,out List<CustomerPricingProductDetail> insertedObjects);
       bool Delete(int customerPricingProductId);
       bool AreCustomerPricingProductsUpdated(ref object updateHandle);
       List<CustomerPricingProductDetail> GetCustomerPricingProducts();
    }
}
