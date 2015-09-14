using System;
using System.Collections.Generic;
using Vanrise.Entities;
using Vanrise.Fzero.FraudAnalysis.Entities;
using Vanrise.Fzero.FraudAnalysis.Entities.ResultQuery;

namespace Vanrise.Fzero.FraudAnalysis.Data
{
    public interface ICaseManagementDataManager : IDataManager 
    {
        List<StrategyCases> GetStrategyCases(DateTime fromDate, DateTime toDate);

        BigResult<BTSCases> GetBTSCases(Vanrise.Entities.DataRetrievalInput<DashboardResultQuery> input);

        BigResult<BTSHighValueCases> GetTop10BTSHighValue(Vanrise.Entities.DataRetrievalInput<DashboardResultQuery> input);

        AccountCase GetLastAccountCaseByAccountNumber(string accountNumber);

        bool InsertAccountCase(out int insertedID, string accountNumber, int? userID, CaseStatus caseStatus, DateTime? validTill, string reason);

        bool UpdateAccountCase(int caseID, int userID, CaseStatus statusID, DateTime? validTill, string reason);

        bool InsertAccountCaseHistory(int caseID, int? userID, CaseStatus caseStatus, string reason);

        bool InsertOrUpdateAccountStatus(string accountNumber, CaseStatus caseStatus);

        bool InsertOrUpdateAccountStatus(string accountNumber, CaseStatus caseStatus, AccountInfo accountInfo);

        bool LinkDetailToCase(string accountNumber, int caseID, CaseStatus caseStatus);

        BigResult<AccountSuspicionSummary> GetFilteredAccountSuspicionSummaries(Vanrise.Entities.DataRetrievalInput<AccountSuspicionSummaryQuery> input);

        AccountSuspicionSummary GetAccountSuspicionSummaryByAccountNumber(string accountNumber, DateTime from, DateTime to);

        BigResult<AccountSuspicionDetail> GetFilteredAccountSuspicionDetails(Vanrise.Entities.DataRetrievalInput<AccountSuspicionDetailQuery> input);

        BigResult<AccountSuspicionDetail> GetFilteredDetailsByCaseID(Vanrise.Entities.DataRetrievalInput<CaseDetailQuery> input);

        List<RelatedNumber> GetRelatedNumbersByAccountNumber(string accountNumber);

        CaseStatus? GetAccountStatus(string accountNumber);

        BigResult<AccountCaseLog> GetFilteredAccountCaseLogsByCaseID(Vanrise.Entities.DataRetrievalInput<AccountCaseLogResultQuery> input);

        BigResult<CasesSummary> GetCasesSummary(Vanrise.Entities.DataRetrievalInput<DashboardResultQuery> input);

        BigResult<AccountCase> GetFilteredCasesByAccountNumber(Vanrise.Entities.DataRetrievalInput<AccountCaseResultQuery> input);

        BigResult<DailyVolumeLoose> GetDailyVolumeLooses(Vanrise.Entities.DataRetrievalInput<DashboardResultQuery> input);

        bool CancelAccountCases(int strategyID, string accountNumber, DateTime from, DateTime to);

    }
}
