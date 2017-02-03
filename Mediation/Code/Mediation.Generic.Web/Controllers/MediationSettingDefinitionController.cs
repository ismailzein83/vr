using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Http;
using Mediation.Generic.Business;
using Mediation.Generic.Entities;
using Vanrise.Entities;
using Vanrise.GenericData.Business;
using Vanrise.GenericData.Entities;
using Vanrise.Web.Base;

namespace Vanrise.GenericData.Web.Controllers
{
    [JSONWithTypeAttribute]
    [RoutePrefix(Mediation.Generic.Web.Constants.ROUTE_PREFIX + "MediationDefinition")]
    public class MediationDefinitionController : BaseAPIController
    {
        [HttpGet]
        [Route("GetMediationDefinition")]
        public MediationDefinition GetMediationDefinition(int mediationDefinitionId)
        {
            MediationDefinitionManager mediationDefinitionManager = new MediationDefinitionManager();
            return mediationDefinitionManager.GetMediationDefinition(mediationDefinitionId);
        }

        [HttpGet]
        [Route("GetMediationDefinitionsInfo")]
        public IEnumerable<MediationDefinitionInfo> GetMediationDefinitionsInfo()
        {
            MediationDefinitionManager mediationDefinitionManager = new MediationDefinitionManager();
            return mediationDefinitionManager.GetMediationDefinitionInfo();
        }
        [HttpPost]
        [Route("GetMediationDefinitionsInfoByIds")]
        public IEnumerable<MediationDefinitionInfo> GetMediationDefinitionsInfoByIds(HashSet<int> accountIds)
        {
            MediationDefinitionManager mediationDefinitionManager = new MediationDefinitionManager();
            return mediationDefinitionManager.GetMediationDefinitionInfoByIds(accountIds);
        }

        [HttpPost]
        [Route("UpdateMediationDefinition")]
        public Vanrise.Entities.UpdateOperationOutput<MediationDefinitionDetail> UpdateMediationDefinition(MediationDefinition mediationDefinition)
        {
            MediationDefinitionManager mediationDefinitionManager = new MediationDefinitionManager();
            return mediationDefinitionManager.UpdateMediationDefinition(mediationDefinition);
        }

        [HttpPost]
        [Route("AddMediationDefinition")]
        public Vanrise.Entities.InsertOperationOutput<MediationDefinitionDetail> AddMediationDefinition(MediationDefinition mediationDefinition)
        {
            MediationDefinitionManager mediationDefinitionManager = new MediationDefinitionManager();
            return mediationDefinitionManager.AddMediationDefinition(mediationDefinition);
        }

        [HttpPost]
        [Route("GetFilteredMediationDefinitions")]
        public object GetFilteredMediationDefinitions(Vanrise.Entities.DataRetrievalInput<MediationDefinitionQuery> input)
        {
            MediationDefinitionManager mediationDefinitionManager = new MediationDefinitionManager();
            return GetWebResponse(input, mediationDefinitionManager.GetFilteredMediationDefinitions(input));
        }

        [HttpGet]
        [Route("GetMediationHandlerConfigTypes")]
        public IEnumerable<MediationOutputHandlerConfig> GetMediationHandlerConfigTypes()
        {
            MediationDefinitionManager mediationDefinitionManager = new MediationDefinitionManager();
            return mediationDefinitionManager.GetMediationHandlerConfigTypes();
        }

    }
}