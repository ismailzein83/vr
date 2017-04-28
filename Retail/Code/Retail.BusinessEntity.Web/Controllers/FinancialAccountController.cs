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
    [RoutePrefix(Constants.ROUTE_PREFIX + "FinancialAccount")]
    public class FinancialAccountController : BaseAPIController
    {
        FinancialAccountManager _manager = new FinancialAccountManager();
        [HttpGet]
        [Route("GetFinancialAccountDefinitionsInfo")]
        public object GetFilteredFinancialAccounts(Vanrise.Entities.DataRetrievalInput<FinancialAccountQuery> input)
        {
            return GetWebResponse(input, _manager.GetFilteredFinancialAccounts(input));
        }
    }
}