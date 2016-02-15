using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Http;
using Vanrise.Entities;
using Vanrise.Fzero.FraudAnalysis.Business;
using Vanrise.Fzero.FraudAnalysis.Entities.ResultQuery;
using Vanrise.Web.Base;

namespace Vanrise.Fzero.FraudAnalysis.Web.Controllers
{
    [RoutePrefix(Constants.ROUTE_PREFIX + "AccountCaseHistory")]
    public class AccountCaseHistoryController : BaseAPIController
    {
        [HttpPost]
        [Route("GetFilteredAccountCaseHistoryByCaseID")]
        public object GetFilteredAccountCaseHistoryByCaseID(DataRetrievalInput<AccountCaseHistoryQuery> input)
        {
            AccountCaseHistoryManager manager = new AccountCaseHistoryManager();
            return GetWebResponse(input, manager.GetFilteredAccountCaseHistoryByCaseID(input));
        }
    }
}