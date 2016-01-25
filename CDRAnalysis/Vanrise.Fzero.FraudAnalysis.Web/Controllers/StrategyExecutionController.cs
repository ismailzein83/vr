using System.Web.Http;
using Vanrise.Fzero.FraudAnalysis.Business;
using Vanrise.Fzero.FraudAnalysis.Entities;
using Vanrise.Web.Base;

namespace  Vanrise.Fzero.FraudAnalysis.Web.Controllers
{

    public class StrategyExecutionController : BaseAPIController
    {

        [HttpPost]
        public object GetFilteredStrategyExecutions(Vanrise.Entities.DataRetrievalInput<StrategyExecutionQuery> input)
        {
            StrategyExecutionManager manager = new StrategyExecutionManager();
            return GetWebResponse(input, manager.GetFilteredStrategyExecutions(input));
        }
    }
}