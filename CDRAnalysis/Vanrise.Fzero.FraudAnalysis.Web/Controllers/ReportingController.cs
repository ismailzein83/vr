using System.Web.Http;
using Vanrise.Fzero.FraudAnalysis.Business;
using Vanrise.Fzero.FraudAnalysis.Entities;
using Vanrise.Web.Base;

namespace  Vanrise.Fzero.FraudAnalysis.Web.Controllers
{

    public class ReportingController : BaseAPIController
    {
        [HttpPost]
        public object GetFilteredCasesProductivity(Vanrise.Entities.DataRetrievalInput<CaseProductivityResultQuery> input)
        {

            ReportingManager manager = new ReportingManager();
                                   
            return GetWebResponse(input, manager.GetFilteredCasesProductivity(input));
        }
    }
}