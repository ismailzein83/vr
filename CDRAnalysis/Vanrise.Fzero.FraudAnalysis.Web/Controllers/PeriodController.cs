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
    [JSONWithTypeAttribute]
    [RoutePrefix(Constants.ROUTE_PREFIX + "Period")]
    public class PeriodController : BaseAPIController
    {
        PeriodManager _manager;

        public PeriodController()
        {
            _manager = new PeriodManager();
        }

        [HttpGet]
        [Route("GetPeriods")]
        public IEnumerable<Period> GetPeriods()
        {
            return _manager.GetPeriods();
        }
    }
}