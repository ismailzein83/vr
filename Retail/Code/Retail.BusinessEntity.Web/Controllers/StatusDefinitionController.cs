using Retail.BusinessEntity.Business;
using Retail.BusinessEntity.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Http;
using Vanrise.Entities;
using Vanrise.Web.Base;

namespace Retail.BusinessEntity.Web.Controllers
{
    [RoutePrefix(Constants.ROUTE_PREFIX + "StatusDefinition")]
    [JSONWithTypeAttribute]
    public class StatusDefinitionController : BaseAPIController
    {
        StatusDefinitionManager _manager = new StatusDefinitionManager();

        [HttpPost]
        [Route("GetFilteredStatusDefinition")]
        public object GetFilteredStatusDefinition(Vanrise.Entities.DataRetrievalInput<StatusDefinitionDetail> input)
        {
            return GetWebResponse(input, _manager.GetFilteredStatusDefinition(input));
        }
    }
}