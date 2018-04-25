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
    [RoutePrefix(Constants.ROUTE_PREFIX + "AccountPackage")]
    public class AccountPackageController : BaseAPIController
    {
        AccountPackageManager _manager = new AccountPackageManager();

        [HttpPost]
        [Route("GetFilteredAccountPackages")]
        public object GetFilteredAccountPackages(Vanrise.Entities.DataRetrievalInput<AccountPackageQuery> input)
        {
            if (!_manager.DoesUserHaveViewAccountPackageAccess(input.Query.AccountBEDefinitionId))
                return GetUnauthorizedResponse();
            return GetWebResponse(input, _manager.GetFilteredAccountPackages(input));
        }
        [HttpGet]
        [Route("DoesUserHaveAddAccess")]
        public bool DoesUserHaveAddAccess(Guid accountBEDefinitionId)
        {
            return _manager.DoesUserHaveAddAccountPackageAccess(accountBEDefinitionId);
        }

        [HttpGet]
        [Route("GetAccountPackage")]
        public AccountPackage GetAccountPackage(long accountPackageId)
        {
            return _manager.GetAccountPackage(accountPackageId);
        }

        [HttpPost]
        [Route("AddAccountPackage")]
        public object AddAccountPackage(AccountPackageToAdd accountPackageToAdd)
        {
            if (!DoesUserHaveAddAccess(accountPackageToAdd.AccountBEDefinitionId))
                return GetUnauthorizedResponse();
            return _manager.AddAccountPackage(accountPackageToAdd);
        }

        [HttpPost]
        [Route("UpdateAccountPackage")]
        public object UpdateAccountPackage(AccountPackageToEdit accountPackageToEdit)
        {

            if (!_manager.DoesUserHaveEditAccountPackageAccess(accountPackageToEdit.AccountPackageId))
                return GetUnauthorizedResponse();
            return _manager.UpdateAccountPackage(accountPackageToEdit);
        }

        [HttpPost]
        [Route("ExportRates")]
        public object ExportRates(AccountDefinition accountDefinition)
        {
            return GetExcelResponse(_manager.ExportRates(accountDefinition.AccountBEDefinitionId, accountDefinition.AccountId, DateTime.Now, true));
        }
    }
}