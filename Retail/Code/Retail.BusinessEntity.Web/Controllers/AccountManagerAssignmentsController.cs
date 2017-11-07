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

namespace Retail.BusinessEntity.Web.Controllers
{
    [RoutePrefix(Constants.ROUTE_PREFIX + "AccountManagerAssignment")]
    [JSONWithTypeAttribute]
    public class AccountManagerAssignmentsController : BaseAPIController
    {
        AccountManagerAssignmentManager _manager = new AccountManagerAssignmentManager();

        [HttpPost]
        [Route("GetAccountManagerAssignments")]
        public object GetAccountManagerAssignments(Vanrise.Entities.DataRetrievalInput<AccountManagerAssignmentQuery> input)
        {
            return GetWebResponse(input, _manager.GetFilteredAccountManagerAssignments(input));
        }
        [HttpPost]
        [Route("AddAccountManagerAssignment")]
        public InsertOperationOutput<AccountManagerAssignmentDetail> AddAccountManagerAssignment(AccountManagerAssignment accountManagerAssignment)
        {
            return _manager.AddAccountManagerAssignment(accountManagerAssignment);

        }
        [HttpPost]
        [Route("UpdateAccountManagerAssignment")]
        public UpdateOperationOutput<AccountManagerAssignmentDetail> UpdateAccountManagerAssignment(AccountManagerAssignment accountManagerAssignment)
        {
            return _manager.UpdateAccountManagerAssignment(accountManagerAssignment);

        }
        [HttpPost]
        [Route("GetAccountManagerAssignmentRuntimeEditor")]
        public AccountManagerAssignmentRuntime GetAccountManagerAssignmentRuntimeEditor(AccountManagerAssignmentRuntimeInput accountManagerAssignmentInput)
        {
            return _manager.GetAccountManagerAssignmentRuntimeEditor(accountManagerAssignmentInput);
        }
    }
}