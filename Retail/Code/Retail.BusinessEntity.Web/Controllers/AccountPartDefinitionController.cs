using Retail.BusinessEntity.Business;
using Retail.BusinessEntity.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Http;
using Vanrise.Web.Base;
using Vanrise.Common;

namespace Retail.BusinessEntity.Web.Controllers
{
    [JSONWithTypeAttribute]
    [RoutePrefix(Constants.ROUTE_PREFIX + "AccountPartDefinition")]
    public class AccountPartDefinitionController:BaseAPIController
    {
        AccountPartDefinitionManager _manager = new AccountPartDefinitionManager();

        [HttpPost]
        [Route("GetFilteredAccountPartDefinitions")]
        public object GetFilteredAccountPartDefinitions(Vanrise.Entities.DataRetrievalInput<AccountPartDefinitionQuery> input)
        {
            return GetWebResponse(input, _manager.GetFilteredAccountPartDefinitions(input));
        }

        [HttpGet]
        [Route("GetAccountPartDefinition")]
        public AccountPartDefinition GetAccountPartDefinition(Guid accountPartDefinitionId)
        {
            return _manager.GetAccountPartDefinition(accountPartDefinitionId);
        }

        [HttpGet]
        [Route("GetAccountPartDefinitionsInfo")]
        public IEnumerable<AccountPartDefinitionInfo> GetAccountPartDefinitionsInfo(string filter = null)
        {
            AccountPartDefinitionFilter deserializedFilter = (filter != null) ? Serializer.Deserialize<AccountPartDefinitionFilter>(filter) : null;
            return _manager.GetAccountPartDefinitionsInfo(deserializedFilter);
        }

        [HttpGet]
        [Route("GetAccountPartDefinitionExtensionConfigs")]
        public IEnumerable<AccountPartDefinitionConfig> GetAccountPartDefinitionExtensionConfigs()
        {
            return _manager.GetAccountPartDefinitionExtensionConfigs();
        }

        [HttpPost]
        [Route("AddAccountPartDefinition")]
        public Vanrise.Entities.InsertOperationOutput<AccountPartDefinitionDetail> AddAccountPartDefinition(AccountPartDefinition accountPartDefinition)
        {
            return _manager.AddAccountPartDefinition(accountPartDefinition);
        }

        [HttpPost]
        [Route("UpdateAccountPartDefinition")]
        public Vanrise.Entities.UpdateOperationOutput<AccountPartDefinitionDetail> UpdateAccountPartDefinition(AccountPartDefinitionToEdit accountPartDefinition)
        {
            return _manager.UpdateAccountPartDefinition(accountPartDefinition);
        }
    }
}