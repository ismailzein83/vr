using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Http;
using TOne.WhS.BusinessEntity.Business;
using TOne.WhS.BusinessEntity.Entities;
using Vanrise.Web.Base;

namespace TOne.WhS.BusinessEntity.Web.Controllers
{
    [JSONWithTypeAttribute]
    [RoutePrefix(Constants.ROUTE_PREFIX + "FinancialAccount")]
    public class WHSFinancialAccountController : BaseAPIController
    {
        WHSFinancialAccountManager _manager = new WHSFinancialAccountManager();

        [HttpGet]
        [Route("GetAccountCurrencyName")]
        public string GetAccountCurrencyName(int? carrierProfileId = null, int? carrierAccountId = null)
        {
            return _manager.GetAccountCurrencyName(carrierProfileId, carrierAccountId);
        }

        [HttpPost]
        [Route("GetFilteredFinancialAccounts")]
        public object GetFilteredFinancialAccounts(Vanrise.Entities.DataRetrievalInput<WHSFinancialAccountQuery> input)
        {
            return GetWebResponse(input, _manager.GetFilteredFinancialAccounts(input));
        }

        [HttpPost]
        [Route("AddFinancialAccount")]
        public Vanrise.Entities.InsertOperationOutput<WHSFinancialAccountDetail> AddFinancialAccount(WHSFinancialAccount financialAccount)
        {
            return _manager.AddFinancialAccount(financialAccount);
        }

        [HttpPost]
        [Route("UpdateFinancialAccount")]
        public Vanrise.Entities.UpdateOperationOutput<WHSFinancialAccountDetail> UpdateFinancialAccount(WHSFinancialAccountToEdit financialAccountToEdit)
        {
            return _manager.UpdateFinancialAccount(financialAccountToEdit);
        }

        [HttpGet]
        [Route("CanAddFinancialAccountToCarrier")]
        public bool CanAddFinancialAccountToCarrier(int? carrierProfileId = null, int? carrierAccountId = null)
        {
            return _manager.CanAddFinancialAccountToCarrier(carrierProfileId, carrierAccountId);
        }

        [HttpGet]
        [Route("GetFinancialAccountsInfo")]
        public IEnumerable<WHSFinancialAccountInfo> GetFinancialAccountsInfo(string filter = null)
        {
            WHSFinancialAccountInfoFilter deserializedFilter = filter != null ? Vanrise.Common.Serializer.Deserialize<WHSFinancialAccountInfoFilter>(filter) : null;
            return _manager.GetFinancialAccountsInfo(deserializedFilter);
        }
      
        [HttpGet]
        [Route("GetFinancialAccount")]
        public WHSFinancialAccount GetFinancialAccount(int financialAccountId)
        {
            return _manager.GetFinancialAccount(financialAccountId);
        }
       
    }
}