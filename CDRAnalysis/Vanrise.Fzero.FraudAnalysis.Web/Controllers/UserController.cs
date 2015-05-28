using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;
using Vanrise.Fzero.FraudAnalysis.Business;
using Vanrise.Fzero.FraudAnalysis.Entities;
using Vanrise.Fzero.FraudAnalysis.Data;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using Vanrise.Web.Base;

namespace  Vanrise.Fzero.FraudAnalysis.Web.Controllers
{
    public class ResetPasswordInput
    {
        public int StrategyId { get; set; }
    }

    public class StrategyController : BaseAPIController
    {
        [HttpGet]
        public IEnumerable<Strategy> GetFilteredStrategys(int fromRow, int toRow, string name, string email)
        {
            IStrategyDataManager manager = FraudDataManagerFactory.GetDataManager<IStrategyDataManager>();
            return ((IEnumerable<Strategy>)(manager.GetAllStrategies()));
        }

        [HttpGet]
        public Strategy GetStrategy(int StrategyId)
        {
            StrategyManager manager = new StrategyManager();
            return ((Strategy)(manager.GetStrategy(StrategyId)));
        }
    }
}