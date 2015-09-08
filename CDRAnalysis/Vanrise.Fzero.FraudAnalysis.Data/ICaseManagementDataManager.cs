using System;
using System.Collections.Generic;
using Vanrise.Entities;
using Vanrise.Fzero.FraudAnalysis.Entities;

namespace Vanrise.Fzero.FraudAnalysis.Data
{
    public interface ICaseManagementDataManager : IDataManager 
    {
        bool SaveAccountCase(AccountCase accountCaseObject);
        BigResult<AccountCase> GetFilteredAccountCases(Vanrise.Entities.DataRetrievalInput<AccountCaseResultQuery> input);

        bool UpdateAccountCase(string accountNumber, CaseStatus caseStatus, DateTime? validTill);
        AccountCase GetLastAccountCaseByAccountNumber(string accountNumber);
        bool InsertAccountCase(out int insertedID, string accountNumber, int userID, DateTime? validTill);
        bool UpdateAccountCaseStatus(int caseID, CaseStatus statusID, DateTime? validTill);
        bool InsertAccountCaseHistory(int caseID, int userID, CaseStatus caseStatus);
        bool InsertOrUpdateAccountStatus(string accountNumber, CaseStatus caseStatus);
        bool LinkDetailToCase(string accountNumber, int caseID, CaseStatus caseStatus);
        void UpdateSusbcriberCases(List<AccountCaseType> cases);

        BigResult<FraudResult> GetFilteredSuspiciousNumbers(Vanrise.Entities.DataRetrievalInput<FraudResultQuery> input);

        BigResult<AccountSuspicionSummary> GetFilteredAccountSuspicionSummaries(Vanrise.Entities.DataRetrievalInput<AccountSuspicionSummaryQuery> input);

        AccountSuspicionSummary GetAccountSuspicionSummaryByAccountNumber(string accountNumber, DateTime from, DateTime to);

        BigResult<AccountSuspicionDetail> GetFilteredAccountSuspicionDetails(Vanrise.Entities.DataRetrievalInput<AccountSuspicionDetailQuery> input);

        FraudResult GetFraudResult(DateTime fromDate, DateTime toDate, List<int> strategiesList, List<int> suspicionLevelsList, string accountNumber);

        void ApplySuspiciousNumbersToDB(object preparedSuspiciousNumbers);

    }
}
