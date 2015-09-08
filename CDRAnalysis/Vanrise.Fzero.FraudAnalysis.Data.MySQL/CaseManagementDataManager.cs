using System;
using Vanrise.Data.MySQL;
using Vanrise.Fzero.CDRImport.Entities;
using Vanrise.Fzero.FraudAnalysis.Entities;

namespace Vanrise.Fzero.FraudAnalysis.Data.MySQL
{
    public class CaseManagementDataManager : BaseMySQLDataManager, ICaseManagementDataManager
    {
        public CaseManagementDataManager()
            : base("CDRDBConnectionStringMySQL")
        {

        }

        public bool SaveAccountCase(AccountCase accountCaseObject)
        {
            throw new NotImplementedException();
        }

        public Vanrise.Entities.BigResult<AccountCase> GetFilteredAccountCases(Vanrise.Entities.DataRetrievalInput<AccountCaseResultQuery> input)
        {
            throw new NotImplementedException();
        }

        public bool UpdateAccountCase(string accountNumber, CaseStatus caseStatus, DateTime? validTill)
        {
            throw new NotImplementedException();
        }

        public AccountCase GetLastAccountCaseByAccountNumber(string accountNumber)
        {
            throw new NotImplementedException();
        }

        public bool InsertAccountCase(out int insertedID, string accountNumber, int userID, CaseStatus caseStatus, DateTime? validTill)
        {
            throw new NotImplementedException();
        }

        public bool UpdateAccountCaseStatus(int caseID, CaseStatus statusID, DateTime? validTill)
        {
            throw new NotImplementedException();
        }

        public bool InsertAccountCaseHistory(int caseID, int userID, CaseStatus caseStatus)
        {
            throw new NotImplementedException();
        }

        public bool InsertOrUpdateAccountStatus(string accountNumber, CaseStatus caseStatus)
        {
            throw new NotImplementedException();
        }

        public bool LinkDetailToCase(string accountNumber, int caseID, CaseStatus caseStatus)
        {
            throw new NotImplementedException();
        }

        public void UpdateSusbcriberCases(System.Collections.Generic.List<AccountCaseType> cases)
        {
            throw new NotImplementedException();
        }

        public Vanrise.Entities.BigResult<FraudResult> GetFilteredSuspiciousNumbers(Vanrise.Entities.DataRetrievalInput<FraudResultQuery> input)
        {
            throw new NotImplementedException();
        }

        public Vanrise.Entities.BigResult<AccountSuspicionSummary> GetFilteredAccountSuspicionSummaries(Vanrise.Entities.DataRetrievalInput<AccountSuspicionSummaryQuery> input)
        {
            throw new NotImplementedException();
        }

        public AccountSuspicionSummary GetAccountSuspicionSummaryByAccountNumber(string accountNumber, DateTime from, DateTime to)
        {
            throw new NotImplementedException();
        }

        public Vanrise.Entities.BigResult<AccountSuspicionDetail> GetFilteredAccountSuspicionDetails(Vanrise.Entities.DataRetrievalInput<AccountSuspicionDetailQuery> input)
        {
            throw new NotImplementedException();
        }

        public FraudResult GetFraudResult(DateTime fromDate, DateTime toDate, System.Collections.Generic.List<int> strategiesList, System.Collections.Generic.List<int> suspicionLevelsList, string accountNumber)
        {
            throw new NotImplementedException();
        }

        public void ApplySuspiciousNumbersToDB(object preparedSuspiciousNumbers)
        {
            throw new NotImplementedException();
        }


      
    }
}
