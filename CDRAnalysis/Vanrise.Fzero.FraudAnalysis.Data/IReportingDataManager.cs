using Vanrise.Entities;
using Vanrise.Fzero.FraudAnalysis.Entities;

namespace Vanrise.Fzero.FraudAnalysis.Data
{
    public interface IReportingDataManager : IDataManager 
    {
        BigResult<CaseProductivity> GetFilteredCasesProductivity(Vanrise.Entities.DataRetrievalInput<CaseProductivityQuery> input);

        BigResult<BlockedLines> GetFilteredBlockedLines(Vanrise.Entities.DataRetrievalInput<BlockedLinesQuery> input);

        BigResult<LinesDetected> GetFilteredLinesDetected(Vanrise.Entities.DataRetrievalInput<LinesDetectedQuery> input);
        
    }
}
