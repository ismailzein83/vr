using System;
using System.Collections.Generic;
using Vanrise.Data.MySQL;
using Vanrise.Fzero.FraudAnalysis.Entities;

namespace Vanrise.Fzero.FraudAnalysis.Data.MySQL
{
    public class SuspiciousNumberDataManager : BaseMySQLDataManager, ISuspiciousNumberDataManager
    {
        public SuspiciousNumberDataManager()
            : base("CDRDBConnectionStringMySQL")
        {

        }

        public Vanrise.Entities.BigResult<FraudResult> GetFilteredSuspiciousNumbers(Vanrise.Entities.DataRetrievalInput<FraudResultQuery> input)
        {
            throw new NotImplementedException();
        }

        public FraudResult GetFraudResult(DateTime fromDate, DateTime toDate, List<int> strategiesList, List<int> suspicionLevelsList, string accountNumber)
        {
            throw new NotImplementedException();
        }

        public void ApplySuspiciousNumbersToDB(object preparedSuspiciousNumbers)
        {
            throw new NotImplementedException();
        }

        public Vanrise.Entities.BigResult<AccountThreshold> GetAccountThresholds(Vanrise.Entities.DataRetrievalInput<AccountThresholdResultQuery> input)
        {
            throw new NotImplementedException();
        }

        public object FinishDBApplyStream(object dbApplyStream)
        {
            throw new NotImplementedException();
        }

        public object InitialiazeStreamForDBApply()
        {
            throw new NotImplementedException();
        }

        public void WriteRecordToStream(SuspiciousNumber record, object dbApplyStream)
        {
            throw new NotImplementedException();
        }
    }
}
