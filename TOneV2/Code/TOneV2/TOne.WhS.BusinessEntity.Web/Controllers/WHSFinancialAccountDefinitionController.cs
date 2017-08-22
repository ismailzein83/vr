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
    [RoutePrefix(Constants.ROUTE_PREFIX + "FinancialAccountDefinition")]
    public class WHSFinancialAccountDefinitionController : Vanrise.Web.Base.BaseAPIController
    {
        WHSFinancialAccountDefinitionManager _manager = new WHSFinancialAccountDefinitionManager();

        [HttpGet]
        [Route("GetFinancialAccountDefinitionsConfigs")]
        public IEnumerable<WHSFinancialAccountDefinitionConfig> GetFinancialAccountDefinitionsConfigs()
        {
            return _manager.GetFinancialAccountDefinitionsConfigs();
        }
        [HttpGet]
        [Route("GetFinancialAccountDefinitionSettings")]
        public WHSFinancialAccountDefinitionSettings GetFinancialAccountDefinitionSettings(Guid financialAccountDefinitionId)
        {
            return _manager.GetFinancialAccountDefinitionSettings(financialAccountDefinitionId);
        }

        [HttpGet]
        [Route("GetFinancialAccountDefinitionInfo")]
        public IEnumerable<WHSFinancialAccountDefinitionInfo> GetFinancialAccountDefinitionInfo(string filter = null)
        {
            WHSFinancialAccountDefinitionInfoFilter deserializedFilter = filter != null ? Vanrise.Common.Serializer.Deserialize<WHSFinancialAccountDefinitionInfoFilter>(filter) : null;
            return _manager.GetFinancialAccountDefinitionInfo(deserializedFilter);
        }
    }
}