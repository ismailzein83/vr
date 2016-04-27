using System.Collections.Generic;
using System.Web.Http;
using TOne.WhS.DBSync.Business;
using Vanrise.Entities;
using Vanrise.Web.Base;

namespace TOne.WhS.BusinessEntity.Web.Controllers
{
   
    [RoutePrefix(Constants.ROUTE_PREFIX + "Supplier")]

    [JSONWithTypeAttribute]
    public class QMBE_SupplierController : BaseAPIController
    {

        [HttpGet]
        [Route("GetSupplierSourceTemplates")]
        public List<TemplateConfig> GetSupplierSourceTemplates()
        {
            SupplierManager manager = new SupplierManager();
            return manager.GetSupplierSourceTemplates();
        }

    }
}