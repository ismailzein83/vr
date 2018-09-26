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
   [RoutePrefix(Constants.ROUTE_PREFIX + "SupplierOtherRate")]
    public class SupplierOtherRateController : BaseAPIController
    {
       [HttpPost]
       [Route("GetFilteredSupplierOtherRates")]
       public object GetFilteredSupplierOtherRates(Vanrise.Entities.DataRetrievalInput<SupplierOtherRateQuery> input)
       {
           SupplierOtherRateManager manager = new SupplierOtherRateManager();
           return GetWebResponse(input, manager.GetFilteredSupplierOtherRates(input), "Supplier Other Rates");
       }
    }
  
}