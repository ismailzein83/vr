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

        [HttpPost]
        [Route("GetFilteredBusinessEntityDefinitions")]
        public object GetFilteredDataRecordStorages(Vanrise.Entities.DataRetrievalInput<BusinessEntityDefinitionQuery> input)
        {
            return GetWebResponse(input, _manager.GetFilteredBusinessEntityDefinitions(input));
        }

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

        [HttpGet]
        [Route("GetRemoteBusinessEntityDefinitionsInfo")]
        public IEnumerable<BusinessEntityDefinitionInfo> GetRemoteBusinessEntityDefinitionsInfo(Guid connectionId, string serializedFilter = null)
        {
            return _manager.GetRemoteBusinessEntityDefinitionsInfo(connectionId, serializedFilter);
        }

        [HttpPost]
        [Route("AddBusinessEntityDefinition")]
        public Vanrise.Entities.InsertOperationOutput<BusinessEntityDefinitionDetail> AddBusinessEntityDefinition(BusinessEntityDefinition businessEntityDefinition)
        {
            return _manager.AddBusinessEntityDefinition(businessEntityDefinition);
        }

        [Route("UpdateBusinessEntityDefinition")]
        public Vanrise.Entities.UpdateOperationOutput<BusinessEntityDefinitionDetail> UpdateBusinessEntityDefinition(BusinessEntityDefinition businessEntityDefinition)
        {
            return _manager.UpdateBusinessEntityDefinition(businessEntityDefinition);
        }

        [HttpGet]
        [Route("GetBEDefinitionSettingConfigs")]
        public IEnumerable<BusinessEntityDefinitionSettingsConfig> GetBEDefinitionSettingConfigs()
        {
            return _manager.GetBEDefinitionSettingConfigs();
        }

        [HttpGet]
        [Route("GetBEDataRecordTypeIdIfGeneric")]
        public Guid? GetBEDataRecordTypeIdIfGeneric(Guid businessEntityDefinitionId)
        {
            return _manager.GetBEDataRecordTypeIdIfGeneric(businessEntityDefinitionId);
        }


        [HttpGet]
        [Route("GetBusinessEntityIdType")]
        public string GetBusinessEntityIdType(Guid remoteBEDefinitionId)
        {
            return _manager.GetBusinessEntityIdType(remoteBEDefinitionId);
        }
    }
}