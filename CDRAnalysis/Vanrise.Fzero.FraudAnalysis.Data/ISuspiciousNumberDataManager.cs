using System;
using System.Collections.Generic;
using Vanrise.Data;
using Vanrise.Entities;
using Vanrise.Fzero.FraudAnalysis.Entities;

namespace Vanrise.Fzero.FraudAnalysis.Data
{
    public interface ISuspiciousNumberDataManager : IDataManager, IBulkApplyDataManager<SuspiciousNumber>
    {
        BigResult<FraudResult> GetFilteredSuspiciousNumbers(Vanrise.Entities.DataRetrievalInput<FraudResultQuery> input);

        FraudResult GetFraudResult(DateTime fromDate, DateTime toDate, List<int> strategiesList, List<int> suspicionLevelsList, string accountNumber);

        void ApplySuspiciousNumbersToDB(object preparedSuspiciousNumbers);

        BigResult<AccountThreshold> GetAccountThresholds(Vanrise.Entities.DataRetrievalInput<AccountThresholdResultQuery> input);
    }
}
