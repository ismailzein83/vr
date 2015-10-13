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
    public class PricingProductController : BaseAPIController
    {
         [HttpPost]
         public object GetFilteredPricingProducts(Vanrise.Entities.DataRetrievalInput<PricingProductQuery> input)
         {
             PricingProductManager manager = new PricingProductManager();
             return GetWebResponse(input, manager.GetFilteredPricingProducts(input));
         }
          [HttpGet]
         public IEnumerable<PricingProductInfo> GetAllPricingProduct()
         {
             PricingProductManager manager = new PricingProductManager();
             return manager.GetAllPricingProduct();
         }
         
         [HttpGet]
         public PricingProductDetail GetPricingProduct(int pricingProductId)
         {
             PricingProductManager manager = new PricingProductManager();
             return manager.GetPricingProduct(pricingProductId);
         }

         [HttpPost]
         public TOne.Entities.InsertOperationOutput<PricingProductDetail> AddPricingProduct(PricingProduct pricingProduct)
         {
             PricingProductManager manager = new PricingProductManager();
             return manager.AddPricingProduct(pricingProduct);
         }

         [HttpPost]
         public TOne.Entities.UpdateOperationOutput<PricingProductDetail> UpdatePricingProduct(PricingProduct pricingProduct)
         {
             PricingProductManager manager = new PricingProductManager();
             return manager.UpdatePricingProduct(pricingProduct);
         }

         [HttpGet]
         public TOne.Entities.DeleteOperationOutput<object> DeletePricingProduct(int pricingProductId)
         {
             PricingProductManager manager = new PricingProductManager();
             return manager.DeletePricingProduct(pricingProductId);
         } 
    }
}