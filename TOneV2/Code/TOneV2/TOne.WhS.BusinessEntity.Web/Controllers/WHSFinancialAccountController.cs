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
        public Vanrise.Entities.InsertOperationOutput<WHSFinancialAccountDetail> AddFinancialAccount(WHSFinancialAccountToAdd financialAccountToAdd)
        {
            return _manager.AddFinancialAccount(financialAccountToAdd);
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
        [HttpGet]
        [Route("GetFinancialAccountCurrencyId")]
        public int GetFinancialAccountCurrencyId(int financialAccountId)
        {
            return _manager.GetFinancialAccountCurrencyId(financialAccountId);
        }

        [HttpGet]
        [Route("GetFinancialAccountRuntimeEditor")]
        public WHSFinancialAccountRuntimeEditor GetFinancialAccountRuntimeEditor(int financialAccountId)
        {
            return _manager.GetFinancialAccountRuntimeEditor(financialAccountId);
        }

        [HttpGet]
        [Route("GetSupplierTimeZoneId")]
        public int GetSupplierTimeZoneId(int financialAccountId)
        {
            return _manager.GetSupplierTimeZoneId(financialAccountId);
        }
        [HttpGet]
        [Route("GetCustomerTimeZoneId")]
        public int GetCustomerTimeZoneId(int financialAccountId)
        {
            return _manager.GetCustomerTimeZoneId(financialAccountId);
        }
    }
}