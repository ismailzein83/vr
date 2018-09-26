using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Web;
using System.Web.Http;
using Vanrise.Entities;
using Vanrise.NumberingPlan.Business;
using Vanrise.Web.Base;
using Vanrise.NumberingPlan.Entities;

namespace Vanrise.NumberingPlan.Web.Controllers
{
    [JSONWithTypeAttribute]
    [RoutePrefix(Vanrise.NumberingPlan.Web.Constants.ROUTE_PREFIX + "SaleCode")]
    public class NP_SaleCodeController : BaseAPIController
    {
        [HttpPost]
        [Route("GetFilteredSaleCodes")]
        public object GetFilteredSaleCodes(Vanrise.Entities.DataRetrievalInput<BaseSaleCodeQueryHandler> input)
        {
            SaleCodeManager manager = new SaleCodeManager();
            return GetWebResponse(input, manager.GetFilteredSaleCodes(input), "Sale Codes");
        }
    }

}