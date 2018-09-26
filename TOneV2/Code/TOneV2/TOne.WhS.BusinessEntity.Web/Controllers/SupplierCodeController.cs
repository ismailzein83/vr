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
   [RoutePrefix(Constants.ROUTE_PREFIX + "SupplierCode")]
    public class SupplierCodeController : BaseAPIController
    {
       [HttpPost]
       [Route("GetFilteredSupplierCodes")]
       public object GetFilteredSupplierCodes(Vanrise.Entities.DataRetrievalInput<SupplierCodeQuery> input)
       {
           SupplierCodeManager manager = new SupplierCodeManager();
           return GetWebResponse(input, manager.GetFilteredSupplierCodes(input), "Supplier Codes");
       }
    }
  
}