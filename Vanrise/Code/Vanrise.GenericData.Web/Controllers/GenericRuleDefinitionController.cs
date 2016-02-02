﻿using System;
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
    [RoutePrefix(Constants.ROUTE_PREFIX + "GenericRuleDefinition")]
    public class GenericRuleDefinitionController : BaseAPIController
    {
        GenericRuleDefinitionManager _manager = new GenericRuleDefinitionManager();

        [HttpPost]
        [Route("GetFilteredGenericRuleDefinitions")]
        public object GetFilteredGenericRuleDefinitions(Vanrise.Entities.DataRetrievalInput<GenericRuleDefinitionQuery> input)
        {
            return GetWebResponse(input, _manager.GetFilteredGenericRuleDefinitions(input));
        }

        [HttpGet]
        [Route("GetGenericRuleDefinition")]
        public GenericRuleDefinition GetGenericRuleDefinition(int genericRuleDefinitionId)
        {
            return _manager.GetGenericRuleDefinition(genericRuleDefinitionId);
        }

        [HttpPost]
        [Route("AddGenericRuleDefinition")]
        public Vanrise.Entities.InsertOperationOutput<GenericRuleDefinition> AddGenericRuleDefinition(GenericRuleDefinition genericRuleDefinition)
        {
            return _manager.AddGenericRuleDefinition(genericRuleDefinition);
        }

        [HttpPost]
        [Route("UpdateGenericRuleDefinition")]
        public Vanrise.Entities.UpdateOperationOutput<GenericRuleDefinition> UpdateGenericRuleDefinition(GenericRuleDefinition genericRuleDefinition)
        {
            return _manager.UpdateGenericRuleDefinition(genericRuleDefinition);
        }

        [HttpGet]
        [Route("DeleteGenericRuleDefinition")]
        public Vanrise.Entities.DeleteOperationOutput<object> DeleteGenericRuleDefinition(int genericRuleDefinitionId)
        {
            return _manager.DeleteGenericRuleDefinition(genericRuleDefinitionId);
        }
    }
}