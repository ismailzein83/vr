using System.Collections.Generic;
using System.Web.Http;
using Vanrise.Fzero.FraudAnalysis.Business;
using Vanrise.Fzero.FraudAnalysis.Data;
using Vanrise.Fzero.FraudAnalysis.Entities;
using Vanrise.Web.Base;

namespace  Vanrise.Fzero.FraudAnalysis.Web.Controllers
{

    public class StrategyController : BaseAPIController
    {
        [HttpGet]
        public IEnumerable<Strategy> GetFilteredStrategies(int fromRow, int toRow,string name, string description)
        {
            return new StrategyManager().GetFilteredStrategies(fromRow, toRow, name, description);
        }

        [HttpGet]
        public Strategy GetStrategy(int StrategyId)
        {
            return new StrategyManager().GetStrategy(StrategyId);
        }


        [HttpPost]
        public Vanrise.Fzero.FraudAnalysis.Entities.UpdateOperationOutput<Strategy> UpdateStrategy(Strategy strategyObject)
        {
            StrategyManager manager = new StrategyManager();

            return manager.UpdateStrategy(strategyObject);
        }

        [HttpPost]
        public Vanrise.Fzero.FraudAnalysis.Entities.InsertOperationOutput<Strategy> AddStrategy(Strategy strategyObject)
        {
            StrategyManager manager = new StrategyManager();

            return manager.AddStrategy(strategyObject);
        }




       






    }
}