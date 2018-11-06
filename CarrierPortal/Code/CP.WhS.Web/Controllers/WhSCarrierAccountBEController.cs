using CP.WhS.Business;
using CP.WhS.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Http;
using TOne.WhS.BusinessEntity.APIEntities;
using TOne.WhS.BusinessEntity.Entities;
using Vanrise.Common;
using Vanrise.Entities;
using Vanrise.Web.Base;

namespace CP.WhS.Web.Controllers
{
    [RoutePrefix(Constants.ROUTE_PREFIX + "WhSCarrierAccountBE")]
    [JSONWithTypeAttribute]
    public class WhSCarrierAccountBEController : BaseAPIController
    {
        WhSCarrierAccountBEManager _whSCarrierAccountBEManager = new WhSCarrierAccountBEManager();
        [HttpGet]
        [Route("GetRemoteCarrierAccountsInfo")]
        public IEnumerable<ClientAccountInfo> GetRemoteCarrierAccountsInfo(string serializedFilter)
        {
            return _whSCarrierAccountBEManager.GetRemoteCarrierAccountsInfo(Serializer.Deserialize<ClientAccountInfoFilter>(serializedFilter));
        }
    }
}