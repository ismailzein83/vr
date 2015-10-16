using System.Web.Http;
using Vanrise.Fzero.FraudAnalysis.Business;
using Vanrise.Fzero.FraudAnalysis.Entities;
using Vanrise.Web.Base;

namespace  Vanrise.Fzero.FraudAnalysis.Web.Controllers
{

    public class ReportingController : BaseAPIController
    {
        [HttpPost]
        public object GetFilteredCasesProductivity(Vanrise.Entities.DataRetrievalInput<CaseProductivityQuery> input)
        {

            ReportingManager manager = new ReportingManager();
                                   
            return GetWebResponse(input, manager.GetFilteredCasesProductivity(input));
        }



        [HttpPost]
        public object GetFilteredBlockedLines(Vanrise.Entities.DataRetrievalInput<BlockedLinesQuery> input)
        {

            ReportingManager manager = new ReportingManager();

            return GetWebResponse(input, manager.GetFilteredBlockedLines(input));
        }


        [HttpPost]
        public object GetFilteredLinesDetected(Vanrise.Entities.DataRetrievalInput<LinesDetectedQuery> input)
        {

            ReportingManager manager = new ReportingManager();

            return GetWebResponse(input, manager.GetFilteredLinesDetected(input));
        }



    }
}