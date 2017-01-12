using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using Retail.BusinessEntity.Entities;
using System.Web.Http;
using Vanrise.Web.Base;
using Retail.BusinessEntity.Business;

namespace Retail.BusinessEntity.Web.Controllers
{
    [RoutePrefix(Constants.ROUTE_PREFIX + "RecurringChargeDefinition")]
    [JSONWithTypeAttribute]
    public class RecurringChargeDefinitionController : BaseAPIController
    {
        RecurringChargeDefinitionManager _manager = new RecurringChargeDefinitionManager();

        [HttpGet]
        [Route("GetRecurringChargeDefinitionsInfo")]
        public IEnumerable<RecurringChargeDefinitionInfo> GetRecurringChargeDefinitionsInfo(string serializedFilter = null)
        {
            RecurringChargeDefinitionInfoFilter deserializedFilter = (serializedFilter != null) ? Vanrise.Common.Serializer.Deserialize<RecurringChargeDefinitionInfoFilter>(serializedFilter) : null;
            return _manager.GetRecurringChargeDefinitionsInfo(deserializedFilter);
        }
    }
}