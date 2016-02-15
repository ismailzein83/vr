using System.Web.Http;
using Vanrise.Entities;
using Vanrise.Fzero.FraudAnalysis.Business;
using Vanrise.Fzero.FraudAnalysis.Entities;
using Vanrise.Web.Base;

namespace Vanrise.Fzero.FraudAnalysis.Web.Controllers
{
    [RoutePrefix(Constants.ROUTE_PREFIX + "StrategyExecutionItem")]
    [JSONWithTypeAttribute]
    public class StrategyExecutionItemController : BaseAPIController
    {
        [HttpPost]
        [Route("GetFilteredDetailsByCaseID")]
        public object GetFilteredDetailsByCaseID(DataRetrievalInput<CaseDetailQuery> input)
        {
            StrategyExecutionItemManager manager = new StrategyExecutionItemManager();
            return GetWebResponse(input, manager.GetFilteredItemsByCaseId(input));
        }
    }
}