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
    [RoutePrefix(Constants.ROUTE_PREFIX + "TelesEnterprise")]
    public class TelesEnterpriseController:BaseAPIController
    {
        TelesEnterpriseManager _manager = new TelesEnterpriseManager();
        [HttpGet]
        [Route("GetEnterprisesInfo")]
        public IEnumerable<TelesEnterpriseInfo> GetEnterprisesInfo(int switchId, int domainId, string serializedFilter = null)
        {
            TelesEnterpriseFilter filter = Vanrise.Common.Serializer.Deserialize<TelesEnterpriseFilter>(serializedFilter);
            return _manager.GetEnterprisesInfo(switchId, domainId, filter);
        }
        [HttpPost]
        [Route("MapEnterpriseToAccount")]
        public bool MapEnterpriseToAccount(MapEnterpriseToAccountInput input)
        {
            return _manager.MapEnterpriseToAccount(input);
        }
    }
}