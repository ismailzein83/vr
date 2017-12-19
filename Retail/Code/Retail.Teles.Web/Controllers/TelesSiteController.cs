using Retail.Teles.Business;
using Retail.Teles.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Http;
using Vanrise.Web.Base;

namespace Retail.Teles.Web.Controllers
{
    [JSONWithTypeAttribute]
    [RoutePrefix(Constants.ROUTE_PREFIX + "TelesSite")]
    public class TelesSiteController : BaseAPIController
    {
        TelesSiteManager _manager = new TelesSiteManager();

        [HttpGet]
        [Route("GetEnterpriseSitesInfo")]
        public IEnumerable<TelesEnterpriseSiteInfo> GetEnterpriseSitesInfo(Guid vrConnectionId, string enterpriseId, string serializedFilter = null)
        {
            TelesEnterpriseSiteFilter filter = Vanrise.Common.Serializer.Deserialize<TelesEnterpriseSiteFilter>(serializedFilter);
            return _manager.GetEnterpriseSitesInfo(vrConnectionId, enterpriseId, filter);
        }
        [HttpPost]
        [Route("MapSiteToAccount")]
        public object MapSiteToAccount(MapSiteToAccountInput input)
        {
            if (!_manager.DoesUserHaveExecutePermission(input.AccountBEDefinitionId))
                return GetUnauthorizedResponse();
            return _manager.MapSiteToAccount(input);
        }
        [HttpPost]
        [Route("AddTelesSite")]
        public Vanrise.Entities.InsertOperationOutput<TelesEnterpriseSiteInfo> AddTelesSite(TelesSiteInput input)
        {
            return _manager.AddTelesSite(input);
        }
        [HttpGet]
        [Route("GetSiteRoutingGroupsInfo")]
        public IEnumerable<TelesSiteRoutingGroupInfo> GetSiteRoutingGroupsInfo(Guid vrConnectionId, string siteId, string serializedFilter = null)
        {
            TelesSiteRoutingGroupFilter filter = Vanrise.Common.Serializer.Deserialize<TelesSiteRoutingGroupFilter>(serializedFilter);
            return _manager.GetSiteRoutingGroupsInfo(vrConnectionId, siteId, filter);
        }

    }
}