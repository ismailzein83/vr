using System.Collections.Generic;
using System.Linq;
using Vanrise.Entities;
using Vanrise.Fzero.FraudAnalysis.Data;
using Vanrise.Fzero.FraudAnalysis.Entities;

namespace Vanrise.Fzero.FraudAnalysis.Business
{
    public class ReportingManager
    {

        public Vanrise.Entities.IDataRetrievalResult<CaseProductivity> GetFilteredCasesProductivity(Vanrise.Entities.DataRetrievalInput<CaseProductivityResultQuery> input)
        {
            IReportingDataManager manager = FraudDataManagerFactory.GetDataManager<IReportingDataManager>();

            BigResult<CaseProductivity> casesProductivity = manager.GetFilteredCasesProductivity(input);

            return Vanrise.Common.DataRetrievalManager.Instance.ProcessResult(input, casesProductivity);
        }

        public Vanrise.Entities.IDataRetrievalResult<BlockedLines> GetFilteredBlockedLines(Vanrise.Entities.DataRetrievalInput<BlockedLinesResultQuery> input)
        {
            IReportingDataManager manager = FraudDataManagerFactory.GetDataManager<IReportingDataManager>();

            BigResult<BlockedLines> blockedLines = manager.GetFilteredBlockedLines(input);

            return Vanrise.Common.DataRetrievalManager.Instance.ProcessResult(input, blockedLines);
        }



        public Vanrise.Entities.IDataRetrievalResult<LinesDetected> GetFilteredLinesDetected(Vanrise.Entities.DataRetrievalInput<LinesDetectedResultQuery> input)
        {
            IReportingDataManager manager = FraudDataManagerFactory.GetDataManager<IReportingDataManager>();

            BigResult<LinesDetected> linesDetected = manager.GetFilteredLinesDetected(input);

            return Vanrise.Common.DataRetrievalManager.Instance.ProcessResult(input, linesDetected);
        }


    }
}
