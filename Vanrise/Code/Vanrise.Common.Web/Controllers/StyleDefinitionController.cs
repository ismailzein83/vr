﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Http;
using Vanrise.Common.Business;
using Vanrise.Entities;
using Vanrise.Web.Base;

namespace Vanrise.Common.Web.Controllers
{
    [RoutePrefix(Constants.ROUTE_PREFIX + "StyleDefinition")]
    [JSONWithTypeAttribute]
    public class StyleDefinitionController : BaseAPIController
    {
        StyleDefinitionManager _manager = new StyleDefinitionManager();

        [HttpPost]
        [Route("GetFilteredStyleDefinitions")]
        public object GetFilteredStyleDefinitions(Vanrise.Entities.DataRetrievalInput<StyleDefinitionQuery> input)
        {
            return GetWebResponse(input, _manager.GetFilteredStyleDefinitions(input));
        }

        [HttpGet]
        [Route("GetStyleDefinition")]
        public StyleDefinition GetStyleDefinition(Guid StyleDefinitionId)
        {
            return _manager.GetStyleDefinition(StyleDefinitionId);
        }

        [HttpPost]
        [Route("AddStyleDefinition")]
        public Vanrise.Entities.InsertOperationOutput<StyleDefinitionDetail> AddStyleDefinition(StyleDefinition StyleDefinitionItem)
        {
            return _manager.AddStyleDefinition(StyleDefinitionItem);
        }

        [HttpPost]
        [Route("UpdateStyleDefinition")]
        public Vanrise.Entities.UpdateOperationOutput<StyleDefinitionDetail> UpdateStyleDefinition(StyleDefinition StyleDefinitionItem)
        {
            return _manager.UpdateStyleDefinition(StyleDefinitionItem);
        }

        [HttpGet]
        [Route("GetStyleFormatingExtensionConfigs")]
        public IEnumerable<StyleFormatingConfig> GetStyleFormatingExtensionConfigs()
        {
            return _manager.GetStyleFormatingExtensionConfigs();
        }

        [HttpGet]
        [Route("GetStyleDefinitionsInfo")]
        public IEnumerable<StyleDefinitionInfo> GetStyleDefinitionsInfo(string filter = null)
        {
            StyleDefinitionFilter deserializedFilter = (filter != null) ? Vanrise.Common.Serializer.Deserialize<StyleDefinitionFilter>(filter) : null;
            return _manager.GetStyleDefinitionsInfo(deserializedFilter);
        }
    }
}