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
    [RoutePrefix(Constants.ROUTE_PREFIX + "RetailAccountInfo")]
    [JSONWithTypeAttribute]
    public class RetailAccountInfoController : BaseAPIController
    {
        RetailAccountInfoManager _manager = new RetailAccountInfoManager();
        [HttpGet]
        [Route("GetClientProfileAccountInfo")]
        public ClientRetailProfileAccountInfo GetClientProfileAccountInfo(Guid vrConnectionId)
        {
            return _manager.GetClientProfileAccountInfo(vrConnectionId);
        }
    }
}