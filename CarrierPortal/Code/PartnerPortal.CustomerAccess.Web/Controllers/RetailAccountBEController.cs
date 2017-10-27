using PartnerPortal.CustomerAccess.Business;
using Retail.BusinessEntity.APIEntities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Http;
using Vanrise.Web.Base;

namespace PartnerPortal.CustomerAccess.Web.Controllers
{
    [RoutePrefix(Constants.ROUTE_PREFIX + "RetailAccountBE")]
    [JSONWithTypeAttribute]
    public class RetailAccountBEController : BaseAPIController
    {
        [HttpGet]
        [Route("GetClientChildAccountsInfo")]
        public IEnumerable<ClientAccountInfo> GetClientChildAccountsInfo(Guid businessEntityDefinitionId)
        {
            RetailAccountBEManager _manager = new RetailAccountBEManager();
            return _manager.GetClientChildAccountsInfo(businessEntityDefinitionId);
        }
    }
}