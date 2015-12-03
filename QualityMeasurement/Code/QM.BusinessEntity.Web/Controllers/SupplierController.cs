using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;

namespace QM.BusinessEntity.Web.Controllers
{
    [RoutePrefix(Constants.ROUTE_PREFIX + "Supplier")]
    public class SupplierController : Vanrise.Web.Base.BaseAPIController
    {
        [HttpGet]
        [Route("GetSupplier")]
        public string GetSupplier()
        {
            return "this is the supplier";
        }
    }
}