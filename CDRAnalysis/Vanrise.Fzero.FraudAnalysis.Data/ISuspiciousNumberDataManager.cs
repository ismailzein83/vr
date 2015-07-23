using System;
using System.Collections.Generic;
using Vanrise.Fzero.FraudAnalysis.Entities;

namespace Vanrise.Fzero.FraudAnalysis.Data
{
    public interface ISuspiciousNumberDataManager : IDataManager 
    {
        void SaveSuspiciousNumbers(List<SuspiciousNumber> suspiciousNumbers);
        void SaveNumberProfiles(List<NumberProfile> numberProfiles);

        IEnumerable<FraudResult> GetFilteredSuspiciousNumbers(string tempTableKey, int fromRow, int toRow, DateTime fromDate, DateTime toDate, List<int> strategiesList, List<int> suspicionLevelsList);

        FraudResult GetFraudResult(DateTime fromDate, DateTime toDate, List<int> strategiesList, List<int> suspicionLevelsList, string subscriberNumber);
    }
}
