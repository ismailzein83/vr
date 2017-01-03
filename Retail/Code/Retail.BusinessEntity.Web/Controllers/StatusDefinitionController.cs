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
    public class RetailStatusDefinitionController : BaseAPIController
    {
        StatusDefinitionManager _manager = new StatusDefinitionManager();

        [HttpPost]
        [Route("GetFilteredStatusDefinitions")]
        public object GetFilteredStatusDefinitions(Vanrise.Entities.DataRetrievalInput<Retail.BusinessEntity.Entities.StatusDefinitionQuery> input)
        {
            return GetWebResponse(input, _manager.GetFilteredStatusDefinitions(input));
        }

        [HttpPost]
        [Route("AddStatusDefinition")]
        public Vanrise.Entities.InsertOperationOutput<Retail.BusinessEntity.Entities.StatusDefinitionDetail> AddStatusDefinition(Retail.BusinessEntity.Entities.StatusDefinition statusDefinitionItem)
        {
            return _manager.AddStatusDefinition(statusDefinitionItem);
        }

        [HttpPost]
        [Route("UpdateStatusDefinition")]
        public Vanrise.Entities.UpdateOperationOutput<Retail.BusinessEntity.Entities.StatusDefinitionDetail> UpdateStatusDefinition(Retail.BusinessEntity.Entities.StatusDefinition statusDefinitionItem)
        {
            return _manager.UpdateStatusDefinition(statusDefinitionItem);
        }

        [HttpGet]
        [Route("GetStatusDefinition")]
        public Retail.BusinessEntity.Entities.StatusDefinition GetStatusDefinition(Guid statusDefinitionId)
        {
            return _manager.GetStatusDefinition(statusDefinitionId);
        }

        [HttpGet]
        [Route("GetStatusDefinitionsInfo")]
        public IEnumerable<Retail.BusinessEntity.Entities.StatusDefinitionInfo> GetStatusDefinitionsInfo(string filter = null)
        {
            Retail.BusinessEntity.Entities.StatusDefinitionFilter deserializedFilter = (filter != null) ? Vanrise.Common.Serializer.Deserialize<Retail.BusinessEntity.Entities.StatusDefinitionFilter>(filter) : null;
            return _manager.GetStatusDefinitionsInfo(deserializedFilter);
        }
    }
}