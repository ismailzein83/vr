using System.Collections.Generic;
using System.Web.Http;
using Vanrise.Fzero.FraudAnalysis.Business;
using Vanrise.Fzero.FraudAnalysis.Data;
using Vanrise.Fzero.FraudAnalysis.Entities;
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
        public IEnumerable<Strategy> GetFilteredStrategies(int fromRow, int toRow,string name, string description)
        {
            IStrategyDataManager manager = FraudDataManagerFactory.GetDataManager<IStrategyDataManager>();
            return ((IEnumerable<Strategy>)(manager.GetFilteredStrategies(fromRow, toRow, name, description)));
        }

        [HttpGet]
        public Strategy GetStrategy(int StrategyId)
        {
            StrategyManager manager = new StrategyManager();
            return ((Strategy)(manager.GetStrategy(StrategyId)));
        }
    }
}