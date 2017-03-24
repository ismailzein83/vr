using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Http;
using Vanrise.InvToAccBalanceRelation.Business;
using Vanrise.InvToAccBalanceRelation.Entities;
using Vanrise.Web.Base;

namespace Vanrise.InvToAccBalanceRelation.Web.Controllers
{
    [RoutePrefix(Constants.ROUTE_PREFIX + "RelationDefinition")]
    [JSONWithTypeAttribute]
    public class RelationDefinitionController:BaseAPIController
    {
        RelationDefinitionManager _relationDefinitionEManager = new RelationDefinitionManager();
        [HttpGet]
        [Route("GetRelationDefinitionExtendedSettingsConfigs")]
        public IEnumerable<RelationDefinitionExtendedSettingsConfig> GetRelationDefinitionExtendedSettingsConfigs()
        {
            return _relationDefinitionEManager.GetRelationDefinitionExtendedSettingsConfigs();
        }
    }
}