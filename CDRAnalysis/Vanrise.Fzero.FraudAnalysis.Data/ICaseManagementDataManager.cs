using System;
using Vanrise.Entities;
using Vanrise.Fzero.FraudAnalysis.Entities;

namespace Vanrise.Fzero.FraudAnalysis.Data
{
    public interface ICaseManagementDataManager : IDataManager 
    {
        bool UpdateAccountCase(string accountNumber, CaseStatus caseStatus, DateTime? validTill);

        AccountCase GetLastAccountCaseByAccountNumber(string accountNumber);

        bool InsertAccountCase(out int insertedID, string accountNumber, int userID, CaseStatus caseStatus, DateTime? validTill);

        bool UpdateAccountCaseStatus(int caseID, CaseStatus statusID, DateTime? validTill);

        bool InsertAccountCaseHistory(int caseID, int userID, CaseStatus caseStatus);

        bool InsertOrUpdateAccountStatus(string accountNumber, CaseStatus caseStatus);

        bool LinkDetailToCase(string accountNumber, int caseID, CaseStatus caseStatus);

        BigResult<AccountSuspicionSummary> GetFilteredAccountSuspicionSummaries(Vanrise.Entities.DataRetrievalInput<AccountSuspicionSummaryQuery> input);

        AccountSuspicionSummary GetAccountSuspicionSummaryByAccountNumber(string accountNumber, DateTime from, DateTime to);

        BigResult<AccountSuspicionDetail> GetFilteredAccountSuspicionDetails(Vanrise.Entities.DataRetrievalInput<AccountSuspicionDetailQuery> input);

        BigResult<AccountCase> GetFilteredCasesByAccountNumber(Vanrise.Entities.DataRetrievalInput<AccountCaseQuery> input);
    }
}
