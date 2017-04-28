using Retail.BusinessEntity.Business;
using Retail.BusinessEntity.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Http;
using Vanrise.Web.Base;

namespace Retail.BusinessEntity.Web.Controllers
{
    [JSONWithTypeAttribute]
    [RoutePrefix(Constants.ROUTE_PREFIX + "FinancialAccountDefinition")]
    public class FinancialAccountDefinitionController : BaseAPIController
    {
        FinancialAccountDefinitionManager _manager = new FinancialAccountDefinitionManager();
        [HttpGet]
        [Route("GetFinancialAccountDefinitionsInfo")]
        public IEnumerable<FinancialAccountDefinitionInfo> GetFinancialAccountDefinitionsInfo(string filter)
        {
            FinancialAccountDefinitionInfoFilter deserializedFlter = Vanrise.Common.Serializer.Deserialize<FinancialAccountDefinitionInfoFilter>(filter);
            return _manager.GetFinancialAccountDefinitionsInfo(deserializedFlter);
        }
    }
}