using Retail.BusinessEntity.APIEntities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web.Http;
using Vanrise.Web.Base;

namespace Retail.BusinessEntity.MainExtensions
{
    [JSONWithTypeAttribute]
    [RoutePrefix(Constants.ROUTE_PREFIX + "RetailClientAccount")]
    public class RetailClientAccountController:BaseAPIController
    {
        [HttpGet]
        [Route("GetClientProfileAccountInfo")]
        public ClientRetailProfileAccountInfo GetClientProfileAccountInfo(Guid accountBEDefinitionId, long accountId)
        {
            RetailClientAccountManager manager = new RetailClientAccountManager();
            return manager.GetClientProfileAccountInfo(accountBEDefinitionId, accountId);
        }
    }
}
