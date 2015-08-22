using Vanrise.Entities;
using Vanrise.Fzero.FraudAnalysis.Entities;

namespace Vanrise.Fzero.FraudAnalysis.Data
{
    public interface IReportingDataManager : IDataManager 
    {
        BigResult<CaseProductivity> GetFilteredCasesProductivity(Vanrise.Entities.DataRetrievalInput<CaseProductivityResultQuery> input);

        BigResult<BlockedLines> GetFilteredBlockedLines(Vanrise.Entities.DataRetrievalInput<BlockedLinesResultQuery> input);

        BigResult<LinesDetected> GetFilteredLinesDetected(Vanrise.Entities.DataRetrievalInput<LinesDetectedResultQuery> input);
        
    }
}
