using PartnerPortal.CustomerAccess.Business;
using PartnerPortal.CustomerAccess.Entities;
using Retail.BusinessEntity.APIEntities;
using Retail.BusinessEntity.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Http;
using Vanrise.Entities;
using Vanrise.GenericData.Entities;
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
        [HttpGet]
        [Route("GetRemoteGenericFieldDefinitionsInfo")]
        public IEnumerable<GenericFieldDefinitionInfo> GetRemoteGenericFieldDefinitionsInfo(Guid vrConnectionId, Guid accountBEDefinitionId)
        {
            return _manager.GetRemoteGenericFieldDefinitionsInfo(vrConnectionId, accountBEDefinitionId);
        }
        [HttpPost]
        [Route("GetSubAccountsGridColumnAttributes")]
        public List<DataRecordGridColumnAttribute> GetSubAccountsGridColumnAttributes(AccountGridFieldInput input)
        {
            return _manager.GetSubAccountsGridColumnAttributes(input);

        }
        [HttpPost]
        [Route("GetFilteredSubAccounts")]
        public object GetFilteredSubAccounts(DataRetrievalInput<AccountAppQuery> input)
        {
            return GetWebResponse(input, _manager.GetFilteredSubAccounts(input));
        }
    }
}