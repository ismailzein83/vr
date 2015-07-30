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


        public void UpdateSusbcriberCases(List<string> suspiciousNumbers)
        {
            throw new NotImplementedException();
        }

        public IEnumerable<FraudResult> GetFilteredSuspiciousNumbers(string tempTableKey, int fromRow, int toRow, DateTime fromDate, DateTime toDate, List<int> strategiesList, List<int> suspicionLevelsList, List<int> caseStatusesList)
        {
            throw new NotImplementedException();
        }

        public FraudResult GetFraudResult(DateTime fromDate, DateTime toDate, List<int> strategiesList, List<int> suspicionLevelsList, string subscriberNumber)
        {
            throw new NotImplementedException();
        }

        public void ApplySuspiciousNumbersToDB(object preparedSuspiciousNumbers)
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

        public List<SubscriberThreshold> GetSubscriberThresholds(int fromRow, int toRow, DateTime fromDate, DateTime toDate, string msisdn)
        {
            throw new NotImplementedException();
        }
    }
}
