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
    [RoutePrefix(Constants.ROUTE_PREFIX + "FinancialAccount")]
    public class FinancialAccountController : BaseAPIController
    {
        FinancialAccountManager _manager = new FinancialAccountManager();
        [HttpPost]
        [Route("GetFilteredFinancialAccounts")]
        public object GetFilteredFinancialAccounts(Vanrise.Entities.DataRetrievalInput<FinancialAccountQuery> input)
        {
            return GetWebResponse(input, _manager.GetFilteredFinancialAccounts(input));
        }
        [HttpPost]
        [Route("AddFinancialAccount")]
        public object AddFinancialAccount(FinancialAccountToInsert financialAccountToInsert)
        {
            return _manager.AddFinancialAccount(financialAccountToInsert);
        }

        [HttpPost]
        [Route("UpdateFinancialAccount")]
        public object UpdateFinancialAccount(FinancialAccountToEdit financialAccountToEdit)
        {
            return _manager.UpdateFinancialAccount(financialAccountToEdit);
        }
        [HttpGet]
        [Route("GetFinancialAccountEditorRuntime")]
        public FinancialAccountRuntimeEditor GetFinancialAccountEditorRuntime(Guid accountBEDefinitionId, long accountId, int sequenceNumber)
        {
            return _manager.GetFinancialAccountEditorRuntime(accountBEDefinitionId, accountId, sequenceNumber);
        }
    }
}