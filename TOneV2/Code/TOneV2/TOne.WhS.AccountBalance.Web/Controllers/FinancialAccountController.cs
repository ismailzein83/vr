using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Http;
using TOne.WhS.AccountBalance.Business;
using TOne.WhS.AccountBalance.Entities;
using Vanrise.Web.Base;

namespace TOne.WhS.AccountBalance.Web.Controllers
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
        public Vanrise.Entities.InsertOperationOutput<FinancialAccountDetail> AddFinancialAccount(FinancialAccount financialAccount)
        {
            return _manager.AddFinancialAccount(financialAccount);
        }

        [HttpPost]
        [Route("UpdateFinancialAccount")]
        public Vanrise.Entities.UpdateOperationOutput<FinancialAccountDetail> UpdateFinancialAccount(FinancialAccount financialAccount)
        {
            return _manager.UpdateFinancialAccount(financialAccount);
        }

        [HttpGet]
        [Route("GetFinancialAccount")]
        public FinancialAccount GetFinancialAccount(int financialAccountId)
        {
            return _manager.GetFinancialAccount(financialAccountId);
        }
        [HttpGet]
        [Route("GetFinancialAccountEditorRuntime")]
        public FinancialAccountEditorRuntime GetFinancialAccountEditorRuntime(int financialAccountId)
        {
            return _manager.GetFinancialAccountEditorRuntime(financialAccountId);
        }

        [HttpGet]
        [Route("CheckCarrierAllowAddFinancialAccounts")]
        public bool CheckCarrierAllowAddFinancialAccounts(int? carrierProfileId = null, int? carrierAccountId = null)
        {
            return _manager.CheckCarrierAllowAddFinancialAccounts(carrierProfileId, carrierAccountId);
        }

        [HttpGet]
        [Route("GetAccountCurrencyName")]
        public string GetAccountCurrencyName(int? carrierProfileId = null, int? carrierAccountId = null)
        {
            return _manager.GetAccountCurrencyName(carrierProfileId, carrierAccountId);
        }

        [HttpGet]
        [Route("GetFinancialAccountsInfo")]
        public IEnumerable<FinancialAccountInfo> GetFinancialAccountsInfo(string filter = null)
        {
            FinancialAccountInfoFilter deserializedFilter = filter != null ? Vanrise.Common.Serializer.Deserialize<FinancialAccountInfoFilter>(filter) : null;
            return _manager.GetFinancialAccountsInfo(deserializedFilter);
        }
    }
}