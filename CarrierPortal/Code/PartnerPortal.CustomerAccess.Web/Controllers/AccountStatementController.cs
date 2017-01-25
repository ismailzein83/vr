using PartnerPortal.CustomerAccess.Business;
using PartnerPortal.CustomerAccess.Entities;
using System.Collections.Generic;
using System.Web.Http;
using Vanrise.Web.Base;

namespace PartnerPortal.CustomerAccess.Web.Controllers
{
    [RoutePrefix(Constants.ROUTE_PREFIX + "AccountStatement")]
    [JSONWithTypeAttribute]
    public class AccountStatementController : BaseAPIController
    {
        [HttpPost]
        [Route("GetFilteredAccountStatments")]
        public object GetFilteredAccountStatments(Vanrise.Entities.DataRetrievalInput<AccountStatementAppQuery> input)
        {
            AccountStatementManager manager = new AccountStatementManager();
            return GetWebResponse(input, manager.GetFilteredAccountStatments(input));
        }

        [HttpGet]
        [Route("GetAccountStatementContextHandlerTemplates")]
        public IEnumerable<AccountStatementContextHandlerTemplate> GetAccountStatementContextHandlerTemplates()
        {
            AccountStatementManager manager = new AccountStatementManager();
            return manager.GetAccountStatementContextHandlerTemplates();
        }
    }
}