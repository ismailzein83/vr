using Retail.BusinessEntity.Entities;
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
        public IEnumerable<TelesEnterpriseInfo> GetEnterprisesInfo(Guid vrConnectionId, string serializedFilter = null)
        {
            TelesEnterpriseFilter filter = Vanrise.Common.Serializer.Deserialize<TelesEnterpriseFilter>(serializedFilter);
            return _manager.GetEnterprisesInfo(vrConnectionId, filter);
        }
        [HttpPost]
        [Route("MapEnterpriseToAccount")]
        public object MapEnterpriseToAccount(MapEnterpriseToAccountInput input)
        {
            if (!_manager.DosesuserHaveExecutePermission(input.AccountBEDefinitionId))
                return GetUnauthorizedResponse();
            return _manager.MapEnterpriseToAccount(input);
        }
    }
}