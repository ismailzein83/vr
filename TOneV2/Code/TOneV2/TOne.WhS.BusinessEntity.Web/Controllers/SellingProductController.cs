using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Http;
using TOne.WhS.BusinessEntity.Business;
using TOne.WhS.BusinessEntity.Entities;
using Vanrise.Web.Base;

namespace TOne.WhS.BusinessEntity.Web.Controllers
{
     [JSONWithTypeAttribute]
    public class SellingProductController : BaseAPIController
    {
         [HttpPost]
         public object GetFilteredSellingProducts(Vanrise.Entities.DataRetrievalInput<SellingProductQuery> input)
         {
             SellingProductManager manager = new SellingProductManager();
             return GetWebResponse(input, manager.GetFilteredSellingProducts(input));
         }
          [HttpGet]
         public IEnumerable<SellingProductInfo> GetAllSellingProduct()
         {
             SellingProductManager manager = new SellingProductManager();
             return manager.GetAllSellingProduct();
         }
         
         [HttpGet]
         public SellingProduct GetSellingProduct(int sellingProductId)
         {
             SellingProductManager manager = new SellingProductManager();
             return manager.GetSellingProduct(sellingProductId);
         }

         [HttpPost]
         public TOne.Entities.InsertOperationOutput<SellingProductDetail> AddSellingProduct(SellingProduct sellingProduct)
         {
             SellingProductManager manager = new SellingProductManager();
             return manager.AddSellingProduct(sellingProduct);
         }

         [HttpPost]
         public TOne.Entities.UpdateOperationOutput<SellingProductDetail> UpdateSellingProduct(SellingProduct sellingProduct)
         {
             SellingProductManager manager = new SellingProductManager();
             return manager.UpdateSellingProduct(sellingProduct);
         }

         [HttpGet]
         public TOne.Entities.DeleteOperationOutput<object> DeleteSellingProduct(int sellingProductId)
         {
             SellingProductManager manager = new SellingProductManager();
             return manager.DeleteSellingProduct(sellingProductId);
         } 
    }
}