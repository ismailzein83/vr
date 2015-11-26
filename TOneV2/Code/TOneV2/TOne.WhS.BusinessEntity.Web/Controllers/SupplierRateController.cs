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
   [RoutePrefix(Constants.ROUTE_PREFIX + "SupplierRate")]
    public class SupplierRateController : BaseAPIController
    {
       [HttpPost]
       [Route("GetFilteredSupplierRates")]
       public object GetFilteredSupplierRates(Vanrise.Entities.DataRetrievalInput<SupplierRateQuery> input)
       {
           SupplierRateManager manager = new SupplierRateManager();
           return GetWebResponse(input, manager.GetFilteredSupplierRates(input));
       }
    }
  
}