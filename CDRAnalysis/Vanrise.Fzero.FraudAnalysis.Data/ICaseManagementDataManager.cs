using System;
using System.Collections.Generic;
using Vanrise.Entities;
using Vanrise.Fzero.FraudAnalysis.Entities;

namespace Vanrise.Fzero.FraudAnalysis.Data
{
    public interface ICaseManagementDataManager : IDataManager 
    {
        List<StrategyCases> GetStrategyCases(DateTime fromDate, DateTime toDate);

        BigResult<BTSCases> GetBTSCases(Vanrise.Entities.DataRetrievalInput<DashboardResultQuery> input);

        BigResult<BTSHighValueCases> GetTop10BTSHighValue(Vanrise.Entities.DataRetrievalInput<DashboardResultQuery> input);

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

        BigResult<AccountSuspicionDetail> GetFilteredDetailsByCaseID(Vanrise.Entities.DataRetrievalInput<CaseDetailQuery> input);

        BigResult<CasesSummary> GetCasesSummary(Vanrise.Entities.DataRetrievalInput<DashboardResultQuery> input);

        BigResult<AccountCase> GetFilteredCasesByAccountNumber(Vanrise.Entities.DataRetrievalInput<AccountCaseQuery> input);

        BigResult<DailyVolumeLoose> GetDailyVolumeLooses(Vanrise.Entities.DataRetrievalInput<DashboardResultQuery> input);

    }
}
