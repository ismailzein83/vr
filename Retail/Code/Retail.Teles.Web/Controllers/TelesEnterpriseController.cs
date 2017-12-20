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
            if (!_manager.DoesUserHaveExecutePermission(input.AccountBEDefinitionId))
                return GetUnauthorizedResponse();
            return _manager.MapEnterpriseToAccount(input);
        }
        [HttpGet]
        [Route("GetParentAccountEnterpriseId")]
        public string GetParentAccountEnterpriseId(Guid accountBEDefinitionId, long accountId)
        {
            return _manager.GetParentAccountEnterpriseId(accountBEDefinitionId, accountId);
        }

        [HttpPost]
        [Route("GetFilteredEnterpriseDIDs")]
        public object GetFilteredEnterpriseDIDs(Vanrise.Entities.DataRetrievalInput<EnterpriseDIDsQuery> input)
        {

            return GetWebResponse(input, _manager.GetFilteredEnterpriseDIDs(input));
        }

        [HttpPost]
        [Route("GetFilteredEnterpriseBusinessTrunks")]
        public object GetFilteredEnterpriseBusinessTrunks(Vanrise.Entities.DataRetrievalInput<EnterpriseBusinessTrunksQuery> input)
        {

            return GetWebResponse(input, _manager.GetFilteredEnterpriseBusinessTrunks(input));
        }
        [HttpPost]
        [Route("GetFilteredAccountEnterprisesDIDs")]
        public object GetFilteredAccountEnterprisesDIDs(Vanrise.Entities.DataRetrievalInput<AccountEnterpriseDIDQuery> input)
        {

            return GetWebResponse(input, _manager.GetFilteredAccountEnterprisesDIDs(input));
        }
        [HttpPost]
        [Route("SaveAccountEnterprisesDIDs")]
        public void SaveAccountEnterprisesDIDs()
        {
            _manager.SaveAccountEnterprisesDIDs();
        }
    }
}