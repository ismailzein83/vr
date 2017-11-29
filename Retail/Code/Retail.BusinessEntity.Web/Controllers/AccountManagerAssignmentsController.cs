using Retail.BusinessEntity.Business;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Http;
using Vanrise.Web.Base;
using Vanrise.AccountManager.Entities;
using Vanrise.Entities;
using Retail.BusinessEntity.Entities;
using Vanrise.AccountManager.Business;
using Vanrise.Common;

namespace Retail.BusinessEntity.Web.Controllers
{
    [RoutePrefix(Constants.ROUTE_PREFIX + "AccountManagerAssignment")]
    [JSONWithTypeAttribute]
    public class AccountManagerAssignmentsController : BaseAPIController
    {
        Retail.BusinessEntity.Business.RetailAccountManagerAssignmentManager _manager = new Retail.BusinessEntity.Business.RetailAccountManagerAssignmentManager();

        [HttpPost]
        [Route("GetAccountManagerAssignments")]
        public object GetAccountManagerAssignments(Vanrise.Entities.DataRetrievalInput<AccountManagerAssignmentQuery> input)
        {
            return GetWebResponse(input, _manager.GetFilteredAccountManagerAssignments(input));
        }
        [HttpPost]
        [Route("AddAccountManagerAssignment")]
        public InsertOperationOutput<AccountManagerAssignmentDetail> AddAccountManagerAssignment(AssignAccountManagerToAccountsInput accountManagerAssignment)
        {
            return _manager.AddAccountManagerAssignment(accountManagerAssignment);

        }
        [HttpPost]
        [Route("UpdateAccountManagerAssignment")]
        public UpdateOperationOutput<AccountManagerAssignmentDetail> UpdateAccountManagerAssignment(UpdateAccountManagerAssignmentInput accountManagerAssignment)
        {
            return _manager.UpdateAccountManagerAssignment(accountManagerAssignment);

        }
        [HttpPost]
        [Route("GetAccountManagerAssignmentRuntimeEditor")]
        public AccountManagerAssignmentRuntimeEditor GetAccountManagerAssignmentRuntimeEditor(AccountManagerAssignmentRuntimeInput accountManagerAssignmentInput)
        {
            return _manager.GetAccountManagerAssignmentRuntimeEditor(accountManagerAssignmentInput);
        }
        [HttpGet]
        [Route("GetAccountManagerDefInfo")]
        public AccountManagerDefInfo GetAccountManagerDefInfo(Guid accountBeDefinitionId)
        {
            return _manager.GetAccountManagerDefInfoByAccountBeDefinitionId(accountBeDefinitionId);
        }
        [HttpGet]
        [Route("IsAccountAssignedToAccountManager")]
        public bool IsAccountAssignedToAccountManager(string accountId, Guid accountBeDefinitionId)
        {
            return _manager.IsAccountAssignedToAccountManager(accountId, accountBeDefinitionId);
        }
    }
}