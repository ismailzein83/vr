using Retail.BusinessEntity.Business;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Http;
using Vanrise.Web.Base;

namespace Retail.BusinessEntity.Web.Controllers
{
    [JSONWithTypeAttribute]
    [RoutePrefix(Constants.ROUTE_PREFIX + "VRAccountBalance")]
    public class VRAccountBalanceController : BaseAPIController
    {
        [HttpGet]
        [Route("GetAccountBEDefinitionIdByAccountTypeId")]
        public Guid GetAccountBEDefinitionIdByAccountTypeId(Guid accountTypeId)
        {
            VRAccountBalanceManager manager = new VRAccountBalanceManager();
            return manager.GetAccountBEDefinitionIdByAccountTypeId(accountTypeId);
        }
    }
}