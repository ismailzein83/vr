using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Http;
using Vanrise.Fzero.FraudAnalysis.Business;
using Vanrise.Fzero.FraudAnalysis.Entities;
using Vanrise.Web.Base;

namespace Vanrise.Fzero.FraudAnalysis.Web.Controllers
{
    [RoutePrefix(Constants.ROUTE_PREFIX + "RelatedNumber")]
    public class RelatedNumberController : BaseAPIController
    {
        [HttpGet]
        [Route("GetRelatedNumbersByAccountNumber")]
        public List<RelatedNumber> GetRelatedNumbersByAccountNumber(string accountNumber)
        {
            RelatedNumberManager manager = new RelatedNumberManager();
            return manager.GetRelatedNumbersByAccountNumber(accountNumber);
        }
    }
}