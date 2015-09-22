using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Http;
using TOne.WhS.BusinessEntity.Business;
using TOne.WhS.BusinessEntity.Entities;
using Vanrise.Entities;
using Vanrise.Web.Base;

namespace TOne.WhS.BusinessEntity.Web.Controllers
{
    public class RoutingProductController : BaseAPIController
    {
        [HttpPost]
        public object GetFilteredRoutingProducts(Vanrise.Entities.DataRetrievalInput<RoutingProductQuery> input)
        {
            //UserManager manager = new UserManager();
            //return GetWebResponse(input, manager.GetFilteredUsers(input));
            return null;
        }

        [HttpGet]
        public List<TemplateConfig> GetSaleZoneGroupTemplates()
        {
            RoutingProductManager manager = new RoutingProductManager();
            return manager.GetSaleZoneGroupTemplates();
        }

        [HttpGet]
        public List<TemplateConfig> GetSuppliersGroupTemplates()
        {
            RoutingProductManager manager = new RoutingProductManager();
            return manager.GetSuppliersGroupTemplates();
        }

        [HttpPost]
        public TOne.Entities.InsertOperationOutput<RoutingProduct> AddRoutingProduct(RoutingProduct routingProduct)
        {
            RoutingProductManager manager = new RoutingProductManager();
            return manager.AddRoutingProduct(routingProduct);
        }

        [HttpPost]
        public TOne.Entities.UpdateOperationOutput<RoutingProduct> UpdateRoutingProduct(RoutingProduct routingProduct)
        {
            RoutingProductManager manager = new RoutingProductManager();
            return manager.UpdateRoutingProduct(routingProduct);
        } 
    }
}