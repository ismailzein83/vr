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
        [HttpPost]
        [Route("GetFilteredRecurringChargeDefinitions")]
        public object GetFilteredRecurringChargeDefinitions(Vanrise.Entities.DataRetrievalInput<RecurringChargeDefinitionQuery> input)
        {
            RecurringChargeDefinitionManager manager = new RecurringChargeDefinitionManager();
            return GetWebResponse(input, manager.GetFilteredRecurringChargeDefinitions(input));
        }

        [HttpGet]
        [Route("GetRecurringChargeDefinitionsInfo")]
        public IEnumerable<RecurringChargeDefinitionInfo> GetRecurringChargeDefinitionsInfo(string serializedFilter = null)
        {
            RecurringChargeDefinitionInfoFilter deserializedFilter = (serializedFilter != null) ? Vanrise.Common.Serializer.Deserialize<RecurringChargeDefinitionInfoFilter>(serializedFilter) : null;
            return _manager.GetRecurringChargeDefinitionsInfo(deserializedFilter);
        }

        [HttpGet]
        [Route("GetRecurringChargeDefinition")]
        public RecurringChargeDefinition GetRecurringChargeDefinition(Guid recurringChargeDefinitionId)
        {
            RecurringChargeDefinitionManager manager = new RecurringChargeDefinitionManager();
            return manager.GetRecurringChargeDefinition(recurringChargeDefinitionId);
        }

        [HttpPost]
        [Route("AddRecurringChargeDefinition")]
        public Vanrise.Entities.InsertOperationOutput<RecurringChargeDefinitionDetail> AddRecurringChargeDefinition(RecurringChargeDefinition recurringChargeDefinition)
        {

            RecurringChargeDefinitionManager manager = new RecurringChargeDefinitionManager();
            return manager.AddRecurringChargeDefinition(recurringChargeDefinition);
        }

        [HttpPost]
        [Route("UpdateRecurringChargeDefinition")]
        public Vanrise.Entities.UpdateOperationOutput<RecurringChargeDefinitionDetail> UpdateRecurringChargeDefinition(RecurringChargeDefinition recurringChargeDefinition)
        {
            RecurringChargeDefinitionManager manager = new RecurringChargeDefinitionManager();
            return manager.UpdateRecurringChargeDefinition(recurringChargeDefinition);
        }
       
    }
}