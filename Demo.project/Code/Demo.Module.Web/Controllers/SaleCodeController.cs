using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Http;
using Demo.Module.Business;
using Demo.Module.Entities;
using Vanrise.Entities;
using Vanrise.Web.Base;

namespace  Demo.Module.Web.Controllers
{
    [RoutePrefix(Constants.ROUTE_PREFIX + "SaleCode")]
    public class Demo_SaleCodeController : BaseAPIController
    {
        [HttpPost]
        [Route("GetFilteredSaleCodes")]
        public object GetFilteredSaleCodes(Vanrise.Entities.DataRetrievalInput<SaleCodeQuery> input)
        {
            SaleCodeManager manager = new SaleCodeManager();
            return GetWebResponse(input, manager.GetFilteredSaleCodes(input));
        }
    }

}