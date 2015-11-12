using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TOne.WhS.BusinessEntity.Entities;

namespace TOne.WhS.BusinessEntity.Data
{
   public interface ICustomerSellingProductDataManager:IDataManager
    {
       bool Insert(List<CustomerSellingProduct> customerSellingProduct, out List<CustomerSellingProduct> insertedObjects);

       bool Update(CustomerSellingProduct customerSellingProduct);
       //bool Delete(int customerSellingProductId);
       bool AreCustomerSellingProductsUpdated(ref object updateHandle);
       List<CustomerSellingProduct> GetCustomerSellingProducts();
    }
}