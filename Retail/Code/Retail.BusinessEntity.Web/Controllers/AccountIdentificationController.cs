using Retail.BusinessEntity.Business;
using Retail.BusinessEntity.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Http;
using Vanrise.Web.Base;

namespace Retail.BusinessEntity.Web.Controllers
{
    [JSONWithTypeAttribute]
    [RoutePrefix(Constants.ROUTE_PREFIX + "AccountIdentification")]
    public class AccountIdentificationController : BaseAPIController
    {
        AccountIdentificationManager _manager = new AccountIdentificationManager();

        [HttpPost]
        [Route("GetFilteredAccountIdentificationRules")]
        public object GetFilteredAccountIdentificationRules(Vanrise.Entities.DataRetrievalInput<AccountIdentificationQuery> input)
        {
            return _manager.GetFilteredAccountIdentificationRules(input);
        }

       
    }
}