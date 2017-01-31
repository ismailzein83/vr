﻿using Retail.BusinessEntity.Business;
using Retail.BusinessEntity.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Http;
using Vanrise.GenericData.Entities;
using Vanrise.Web.Base;

namespace Retail.BusinessEntity.Web.Controllers
{
    [JSONWithTypeAttribute]
    [RoutePrefix(Constants.ROUTE_PREFIX + "AccountBEDefinition")]
    public class AccountBEDefinitionController : BaseAPIController
    {
        AccountBEDefinitionManager _manager = new AccountBEDefinitionManager();

        [HttpGet]
        [Route("GetAccountBEDefinitionSettings")]
        public AccountBEDefinitionSettings GetAccountBEDefinitionSettings(Guid accountBEDefinitionId)
        {
            return _manager.GetAccountBEDefinitionSettings(accountBEDefinitionId);
        }

        [HttpGet]
        [Route("GetAccountViewDefinitionSettingsConfigs")]
        public IEnumerable<AccountViewDefinitionConfig> GetAccountViewDefinitionSettingsConfigs()
        {
            return _manager.GetAccountViewDefinitionSettingsConfigs();
        }

        [HttpGet]
        [Route("GetAccountActionDefinitionSettingsConfigs")]
        public IEnumerable<AccountActionDefinitionConfig> GetAccountActionDefinitionSettingsConfigs()
        {
            return _manager.GetAccountActionDefinitionSettingsConfigs();
        }

        [HttpGet]
        [Route("GetAccountGridColumnAttributes")]
        public List<DataRecordGridColumnAttribute> GetAccountGridColumnAttributes(Guid accountBEDefinitionId, long? parentAccountId = null)
        {
            return _manager.GetAccountGridColumnAttributes(accountBEDefinitionId, parentAccountId);
        }

        [HttpGet]
        [Route("GetAccountViewDefinitions")]
        public List<AccountViewDefinition> GetAccountViewDefinitions(Guid accountBEDefinitionId)
        {
            return _manager.GetAccountViewDefinitions(accountBEDefinitionId);
        }

        [HttpGet]
        [Route("GetAccountViewDefinitionsByAccountId")]
        public List<AccountViewDefinition> GetAccountViewDefinitionsByAccountId(Guid accountBEDefinitionId, long accountId)
        {
            return _manager.GetAccountViewDefinitionsByAccountId(accountBEDefinitionId, accountId);
        }

        [HttpGet]
        [Route("GetAccountActionDefinitions")]
        public List<AccountActionDefinition> GetAccountActionDefinitions(Guid accountBEDefinitionId)
        {
            return _manager.GetAccountActionDefinitions(accountBEDefinitionId);
        }
        [HttpGet]
        [Route("GetAccountActionDefinitionsInfo")]
        public IEnumerable<AccountActionDefinitionInfo> GetAccountActionDefinitionsInfo(Guid accountBEDefinitionId, string serializedFilter = null)
        {
            AccountActionDefinitionInfoFilter filter = null;
            if (serializedFilter != null)
                filter = Vanrise.Common.Serializer.Deserialize<AccountActionDefinitionInfoFilter>(serializedFilter);
            return _manager.GetAccountActionDefinitionsInfo(accountBEDefinitionId, filter);
        }
    }
}