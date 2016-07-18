﻿using Retail.BusinessEntity.Business;
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
        [Route("GetFilteredStatusDefinitions")]
        public object GetFilteredStatusDefinitions(Vanrise.Entities.DataRetrievalInput<StatusDefinitionQuery> input)
        {
            return GetWebResponse(input, _manager.GetFilteredStatusDefinitions(input));
        }

        [HttpPost]
        [Route("AddStatusDefinition")]
        public Vanrise.Entities.InsertOperationOutput<StatusDefinitionDetail> AddStatusDefinition(StatusDefinition statusDefinitionItem)
        {
            return _manager.AddStatusDefinition(statusDefinitionItem);
        }

        [HttpPost]
        [Route("UpdateStatusDefinition")]
        public Vanrise.Entities.UpdateOperationOutput<StatusDefinitionDetail> UpdateStatusDefinition(StatusDefinition statusDefinitionItem)
        {
            return _manager.UpdateStatusDefinition(statusDefinitionItem);
        }

        [HttpGet]
        [Route("GetStatusDefinition")]
        public StatusDefinition GetStatusDefinition(Guid statusDefinitionId)
        {
            return _manager.GetStatusDefinition(statusDefinitionId);
        }

        [HttpGet]
        [Route("GetStatusDefinitionInfo")]
        public IEnumerable<StatusDefinitionInfo> GetStatusDefinitionInfo(string filter = null)
        {
            StatusDefinitionFilter deserializedFilter = (filter != null) ? Vanrise.Common.Serializer.Deserialize<StatusDefinitionFilter>(filter) : null;
            return _manager.GetStatusDefinitionInfo(deserializedFilter);
        }
    }
}