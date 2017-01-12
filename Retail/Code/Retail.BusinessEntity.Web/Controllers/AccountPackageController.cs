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
            return GetWebResponse(input, _manager.GetFilteredAccountPackages(input));
        }

        [HttpGet]
        [Route("GetAccountPackage")]
        public AccountPackage GetAccountPackage(int accountPackageId)
        {
            return _manager.GetAccountPackage(accountPackageId);
        }

        [HttpPost]
        [Route("AddAccountPackage")]
        public Vanrise.Entities.InsertOperationOutput<AccountPackageDetail> AddAccountPackage(AccountPackageToAdd accountPackageToAdd)
        {
            return _manager.AddAccountPackage(accountPackageToAdd);
        }
    }
}