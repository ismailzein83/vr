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
    [RoutePrefix(Constants.ROUTE_PREFIX + "DealProgress")]
    public class DealProgressController : Vanrise.Web.Base.BaseAPIController
    {
        DealProgressManager _manager = new DealProgressManager();

        [HttpPost]
        [Route("GetFilteredDealsProgress")]
        public object GetFilteredDealsProgress(Vanrise.Entities.DataRetrievalInput<DealProgressQuery> input)
        {
            return GetWebResponse(input, _manager.GetFilteredDealsProgress(input));
        }
    }
}