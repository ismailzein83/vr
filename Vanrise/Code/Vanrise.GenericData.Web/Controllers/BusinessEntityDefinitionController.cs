using System;
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
    [RoutePrefix(Constants.ROUTE_PREFIX + "BusinessEntityDefinition")]
    public class BusinessEntityDefinitionController : BaseAPIController
    {
        BusinessEntityDefinitionManager _manager = new BusinessEntityDefinitionManager();

        [HttpGet]
        [Route("GetBusinessEntityDefinition")]
        public BusinessEntityDefinition GetBusinessEntityDefinition(Guid businessEntityDefinitionId)
        {
            return _manager.GetBusinessEntityDefinition(businessEntityDefinitionId);
        }

        [HttpGet]
        [Route("GetBusinessEntityDefinitionsInfo")]
        public IEnumerable<BusinessEntityDefinitionInfo> GetBusinessEntityDefinitionsInfo(string filter = null)
        {
            BusinessEntityDefinitionInfoFilter deserializedFilter = (filter != null) ? Vanrise.Common.Serializer.Deserialize<BusinessEntityDefinitionInfoFilter>(filter) : null;
            return _manager.GetBusinessEntityDefinitionsInfo(deserializedFilter);
        }

        [HttpPost]
        [Route("GetFilteredBusinessEntityDefinitions")]
        public object GetFilteredDataRecordStorages(Vanrise.Entities.DataRetrievalInput<BusinessEntityDefinitionQuery> input)
        {
            return GetWebResponse(input, _manager.GetFilteredBusinessEntityDefinitions(input));
        }

        [Route("UpdateBusinessEntityDefinition")]
        public Vanrise.Entities.UpdateOperationOutput<BusinessEntityDefinitionDetail> UpdateBusinessEntityDefinition(BusinessEntityDefinition businessEntityDefinition)
        {
            return _manager.UpdateBusinessEntityDefinition(businessEntityDefinition);
        }

        [HttpPost]
        [Route("AddBusinessEntityDefinition")]
        public Vanrise.Entities.InsertOperationOutput<BusinessEntityDefinitionDetail> AddBusinessEntityDefinition(BusinessEntityDefinition businessEntityDefinition)
        {
            return _manager.AddBusinessEntityDefinition(businessEntityDefinition);
        }

        [HttpGet]
        [Route("GetGenericBEDefinitionView")]
        public Vanrise.Security.Entities.View GetGenericBEDefinitionView(Guid businessEntityDefinitionId)
        {
            return _manager.GetGenericBEDefinitionView(businessEntityDefinitionId);
        }

        [HttpGet]
        [Route("GetBEDataRecordTypeIdIfGeneric")]
        public Guid? GetBEDataRecordTypeIdIfGeneric(Guid businessEntityDefinitionId)
        {
            return _manager.GetBEDataRecordTypeIdIfGeneric(businessEntityDefinitionId);
        }

        [HttpGet]
        [Route("GetBEDefinitionSettingConfigs")]
        public IEnumerable<BusinessEntityDefinitionSettingsConfig> GetBEDefinitionSettingConfigs()
        {
            return _manager.GetBEDefinitionSettingConfigs();
        }
    }
}