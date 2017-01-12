using Retail.BusinessEntity.Business;
using Retail.BusinessEntity.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Http;
using Vanrise.Web.Base;

namespace Retail.BusinessEntity.Web.Controllers
{
    [JSONWithTypeAttribute]
    [RoutePrefix(Constants.ROUTE_PREFIX + "AccountType")]
    public class AccountTypeController : BaseAPIController
    {
        AccountTypeManager _manager = new AccountTypeManager();

        [HttpPost]
        [Route("GetFilteredAccountTypes")]
        public object GetFilteredAccountTypes(Vanrise.Entities.DataRetrievalInput<AccountTypeQuery> input)
        {
            return GetWebResponse(input, _manager.GetFilteredAccountTypes(input));
        }

        [HttpGet]
        [Route("GetAccountType")]
        public AccountType GetAccountType(Guid accountTypeId)
        {
            return _manager.GetAccountType(accountTypeId);
        }

        [HttpGet]
        [Route("GetAccountTypesInfo")]
        public IEnumerable<AccountTypeInfo> GetAccountTypesInfo(string filter = null)
        {
            AccountTypeFilter deserializedFilter = (filter != null) ? Vanrise.Common.Serializer.Deserialize<AccountTypeFilter>(filter) : null;
            return _manager.GetAccountTypesInfo(deserializedFilter);
        }

        [HttpGet]
        [Route("GetAccountTypePartDefinitionExtensionConfigs")]
        public IEnumerable<AccountPartDefinitionConfig> GetAccountTypePartDefinitionExtensionConfigs()
        {
            return _manager.GetAccountTypePartDefinitionExtensionConfigs();
        }

        [HttpPost]
        [Route("AddAccountType")]
        public Vanrise.Entities.InsertOperationOutput<AccountTypeDetail> AddAccountType(AccountType accountType)
        {
            return _manager.AddAccountType(accountType);
        }

        [HttpPost]
        [Route("UpdateAccountType")]
        public Vanrise.Entities.UpdateOperationOutput<AccountTypeDetail> UpdateAccountType(AccountTypeToEdit accountType)
        {
            return _manager.UpdateAccountType(accountType);
        }

        [HttpGet]
        [Route("GetGenericFieldDefinitionsInfo")]
        public IEnumerable<GenericFieldDefinitionInfo> GetGenericFieldDefinitionsInfo(Guid? accountBEDefinitionId = null)
        {
            return _manager.GetGenericFieldDefinitionsInfo(accountBEDefinitionId);
        }
    }
}