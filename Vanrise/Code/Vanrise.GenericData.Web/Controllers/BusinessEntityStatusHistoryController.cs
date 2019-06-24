using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Http;
using Vanrise.GenericData.Business;
using Vanrise.GenericData.Entities;
using Vanrise.Web.Base;

namespace Vanrise.GenericData.Web.Controllers
{
    [JSONWithTypeAttribute]
    [RoutePrefix(Constants.ROUTE_PREFIX + "BusinessEntityStatusHistory")]
    public class BusinessEntityStatusHistoryController : BaseAPIController
    {
        BusinessEntityStatusHistoryManager _manager = new BusinessEntityStatusHistoryManager();

        [HttpPost]
        [Route("GetFilteredBusinessEntitiesStatusHistory")]
        public object GetFilteredBusinessEntitiesStatusHistory(Vanrise.Entities.DataRetrievalInput<BusinessEntityStatusHistoryQuery> input)
        {
            return GetWebResponse(input, _manager.GetFilteredBusinessEntitiesStatusHistory(input));
        }
    }
}