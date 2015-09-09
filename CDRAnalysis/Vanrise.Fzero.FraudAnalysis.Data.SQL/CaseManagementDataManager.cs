using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using Vanrise.Data.SQL;
using Vanrise.Entities;
using Vanrise.Fzero.FraudAnalysis.Entities;

namespace Vanrise.Fzero.FraudAnalysis.Data.SQL
{
    public class CaseManagementDataManager : BaseSQLDataManager, ICaseManagementDataManager
    {
        private static Dictionary<string, string> _columnMapper = new Dictionary<string, string>();

        public CaseManagementDataManager()
            : base("CDRDBConnectionString")
        {

        }

            static CaseManagementDataManager()
        {
            _columnMapper.Add("SuspicionLevelDescription", "SuspicionLevelID");
            _columnMapper.Add("AccountStatusDescription", "AccountStatusID");
        }

        public BigResult<AccountSuspicionSummary> GetFilteredAccountSuspicionSummaries(Vanrise.Entities.DataRetrievalInput<AccountSuspicionSummaryQuery> input)
        {
            Dictionary<string, string> mapper = new Dictionary<string, string>();
            mapper.Add("SuspicionLevelDescription", "SuspicionLevelID");
            mapper.Add("AccountStatusDescription", "AccountStatusID");

            return RetrieveData(input, (tempTableName) =>
            {
                string selectedStrategyIDs = null;
                string selectedSuspicionLevelIDs = null;
                string selectedCaseStatusIDs = null;

                if (input.Query.SelectedStrategyIDs != null && input.Query.SelectedStrategyIDs.Count() > 0)
                    selectedStrategyIDs = string.Join<int>(",", input.Query.SelectedStrategyIDs);

                if (input.Query.SelectedSuspicionLevelIDs != null && input.Query.SelectedSuspicionLevelIDs.Count() > 0)
                    selectedSuspicionLevelIDs = string.Join(",", input.Query.SelectedSuspicionLevelIDs.Select(n => ((int)n).ToString()).ToArray());

                if (input.Query.SelectedCaseStatusIDs != null && input.Query.SelectedCaseStatusIDs.Count() > 0)
                    selectedCaseStatusIDs = string.Join(",", input.Query.SelectedCaseStatusIDs.Select(n => ((int)n).ToString()).ToArray());

                ExecuteNonQuerySP("FraudAnalysis.sp_StrategyExecutionDetails_CreateTempByAccountNumberForSummaries", tempTableName, input.Query.AccountNumber, input.Query.From, input.Query.To, selectedStrategyIDs, selectedSuspicionLevelIDs, selectedCaseStatusIDs);

            }, (reader) => AccountSuspicionSummaryMapper(reader), mapper);
        }

        public BigResult<AccountSuspicionDetail> GetFilteredAccountSuspicionDetails(Vanrise.Entities.DataRetrievalInput<AccountSuspicionDetailQuery> input)
        {
            Dictionary<string, string> mapper = new Dictionary<string, string>();
            mapper.Add("SuspicionLevelDescription", "SuspicionLevelID");
            mapper.Add("SuspicionOccuranceStatusDescription", "SuspicionOccuranceStatus");

            Action<string> createTempTableAction = (tempTableName) =>
            {
                ExecuteNonQuerySP("FraudAnalysis.sp_StrategyExecutionDetails_GetByAccountNumber", tempTableName, input.Query.AccountNumber, input.Query.From, input.Query.To);
            };

            return RetrieveData(input, createTempTableAction, AccountSuspicionDetailMapper, mapper);
        }

        public BigResult<AccountCase> GetFilteredCasesByAccountNumber(Vanrise.Entities.DataRetrievalInput<AccountCaseQuery> input)
        {
            Dictionary<string, string> mapper = new Dictionary<string, string>();
            mapper.Add("CaseStatusDescription", "StatusID");

            Action<string> createTempTableAction = (tempTableName) =>
            {
                ExecuteNonQuerySP("FraudAnalysis.sp_AccountCase_CreateTempByAccountNumber", tempTableName, input.Query.AccountNumber, input.Query.From, input.Query.To);
            };

            return RetrieveData(input, createTempTableAction, AccountCaseMapper, mapper);
        }

        public BigResult<AccountSuspicionDetail> GetFilteredDetailsByCaseID(Vanrise.Entities.DataRetrievalInput<CaseDetailQuery> input)
        {
            Dictionary<string, string> mapper = new Dictionary<string, string>();
            mapper.Add("SuspicionLevelDescription", "SuspicionLevelID");
            mapper.Add("SuspicionOccuranceStatusDescription", "SuspicionOccuranceStatus");

            Action<string> createTempTableAction = (tempTableName) =>
            {
                ExecuteNonQuerySP("FraudAnalysis.sp_StrategyExecutionDetails_CreateTempByCaseID", tempTableName, input.Query.AccountNumber, input.Query.CaseID, input.Query.FromDate, input.Query.ToDate);
            };

            return RetrieveData(input, createTempTableAction, AccountSuspicionDetailMapper, mapper);
        }

        public AccountSuspicionSummary GetAccountSuspicionSummaryByAccountNumber(string accountNumber, DateTime from, DateTime to)
        {
            return GetItemSP("FraudAnalysis.sp_StrategyExecutionDetails_GetSummaryByAccountNumber", AccountSuspicionSummaryMapper, accountNumber, from, to);
        }

        public bool UpdateAccountCase(string accountNumber, CaseStatus caseStatus, DateTime? validTill)
        {
            int userID = Vanrise.Security.Business.SecurityContext.Current.GetLoggedInUserId();
            AccountCase accountCase = GetLastAccountCaseByAccountNumber(accountNumber);
            int caseID;
            bool succeeded;

            if (accountCase == null || (accountCase.StatusID == CaseStatus.ClosedFraud) || (accountCase.StatusID == CaseStatus.ClosedWhiteList))
                succeeded = InsertAccountCase(out caseID, accountNumber, userID, caseStatus, validTill);
            else
            {
                caseID = accountCase.CaseID;
                succeeded = UpdateAccountCaseStatus(accountCase.CaseID, caseStatus, validTill);
            }

            if (!succeeded) return false;

            succeeded = InsertAccountCaseHistory(caseID, userID, caseStatus);

            if (!succeeded) return false;

            succeeded = InsertOrUpdateAccountStatus(accountNumber, caseStatus);

            if (!succeeded) return false;

            return LinkDetailToCase(accountNumber, caseID, caseStatus);
        }

        public AccountCase GetLastAccountCaseByAccountNumber(string accountNumber)
        {
            return GetItemSP("FraudAnalysis.sp_AccountCase_GetLastByAccountNumber", AccountCaseMapper, accountNumber);
        }

        public bool InsertAccountCase(out int insertedID, string accountNumber, int userID, CaseStatus caseStatus, DateTime? validTill)
        {
            object accountCaseID;

            int recordsAffected = ExecuteNonQuerySP("FraudAnalysis.sp_AccountCase_Insert", out accountCaseID, accountNumber, userID, caseStatus, validTill);

            insertedID = (recordsAffected > 0) ? (int)accountCaseID : -1;

            return (recordsAffected > 0);
        }

        public bool UpdateAccountCaseStatus(int caseID, CaseStatus statusID, DateTime? validTill)
        {
            int recordsAffected = ExecuteNonQuerySP("FraudAnalysis.sp_AccountCase_Update", caseID, statusID, validTill);
            return (recordsAffected > 0);
        }

        public bool InsertAccountCaseHistory(int caseID, int userID, CaseStatus caseStatus)
        {
            int recordsAffected = ExecuteNonQuerySP("FraudAnalysis.sp_AccountCaseHistory_Insert", caseID, userID, caseStatus);
            return (recordsAffected > 0);
        }

        public bool InsertOrUpdateAccountStatus(string accountNumber, CaseStatus caseStatus)
        {
            int recordsAffected = ExecuteNonQuerySP("FraudAnalysis.sp_AccountStatus_InsertOrUpdate", accountNumber, caseStatus);
            return (recordsAffected > 0);
        }

        public bool LinkDetailToCase(string accountNumber, int caseID, CaseStatus caseStatus)
        {
            SuspicionOccuranceStatus occuranceStatus = (caseStatus.CompareTo(CaseStatus.Open) == 0 || caseStatus.CompareTo(CaseStatus.Pending) == 0) ? SuspicionOccuranceStatus.Open : SuspicionOccuranceStatus.Closed;

            int recordsAffected = ExecuteNonQuerySP("FraudAnalysis.sp_StrategyExecutionDetails_SetStatusToCaseStatus", accountNumber, caseID, occuranceStatus);
            return (recordsAffected > 0);
        }

        public BigResult<CasesSummary> GetCasesSummary(Vanrise.Entities.DataRetrievalInput<DashboardResultQuery> input)
        {
            Action<string> createTempTableAction = (tempTableName) =>
            {
                ExecuteNonQuerySP("FraudAnalysis.sp_AccountCase_CreateTempForCasesSummary", tempTableName, input.Query.FromDate, input.Query.ToDate);
            };
            return RetrieveData(input, createTempTableAction, CasesSummaryMapper);
        }

        public BigResult<DailyVolumeLoose> GetDailyVolumeLooses(DataRetrievalInput<DashboardResultQuery> input)
        {
            Action<string> createTempTableAction = (tempTableName) =>
            {
                ExecuteNonQuerySP("FraudAnalysis.sp_AccountCase_CreateTempForDailyVolumeLooses", tempTableName, input.Query.FromDate, input.Query.ToDate);
            };
            return RetrieveData(input, createTempTableAction, DailyVolumeLoosesMapper);
        }

        public BigResult<BTSCases> GetBTSCases(Vanrise.Entities.DataRetrievalInput<DashboardResultQuery> input)
        {
            Action<string> createTempTableAction = (tempTableName) =>
            {
                ExecuteNonQuerySP("FraudAnalysis.sp_AccountCase_CreateTempForTopTenBTS", tempTableName, input.Query.FromDate, input.Query.ToDate);
            };
            return RetrieveData(input, createTempTableAction, BTSCasesMapper);
        }

        public List<StrategyCases> GetStrategyCases(DateTime fromDate, DateTime toDate)
        {
            return GetItemsSP("FraudAnalysis.sp_AccountCase_GetFraudCasesPerStrategy", StrategyCasesMapper, fromDate, toDate);
        }

        public BigResult<BTSHighValueCases> GetTop10BTSHighValue(Vanrise.Entities.DataRetrievalInput<DashboardResultQuery> input)
        {
            Action<string> createTempTableAction = (tempTableName) =>
            {
                ExecuteNonQuerySP("FraudAnalysis.sp_AccountCase_CreateTempForTopTenHighValueBTS", tempTableName, input.Query.FromDate, input.Query.ToDate);
            };
            return RetrieveData(input, createTempTableAction, BTSHighValueCasesMapper);
        }

        #region Private Members

        private AccountSuspicionSummary AccountSuspicionSummaryMapper(IDataReader reader)
        {
            var summary = new AccountSuspicionSummary();

            summary.AccountNumber = reader["AccountNumber"] as string;
            summary.SuspicionLevelID = (SuspicionLevelEnum)reader["SuspicionLevelID"];
            summary.NumberOfOccurances = (int)reader["NumberOfOccurances"];
            summary.LastOccurance = (DateTime)reader["LastOccurance"];
            summary.AccountStatusID = GetReaderValue<CaseStatus>(reader, "AccountStatusID");

            return summary;
        }

        private AccountSuspicionDetail AccountSuspicionDetailMapper(IDataReader reader)
        {
            var detail = new AccountSuspicionDetail(); // a detail is a fraud result instance

            detail.DetailID = (long)reader["DetailID"];
            detail.AccountNumber = reader["AccountNumber"] as string;
            detail.SuspicionLevelID = (SuspicionLevelEnum)reader["SuspicionLevelID"];
            detail.StrategyName = reader["StrategyName"] as string;
            detail.SuspicionOccuranceStatus = GetReaderValue<SuspicionOccuranceStatus>(reader, "SuspicionOccuranceStatus");
            detail.FromDate = (DateTime)reader["FromDate"];
            detail.ToDate = (DateTime)reader["ToDate"];
            
            return detail;
        }

        private AccountCase AccountCaseMapper(IDataReader reader)
        {
            return new AccountCase
            {
                CaseID = (int)reader["CaseID"],
                AccountNumber = reader["AccountNumber"] as string,
                UserID = (int)reader["UserID"],
                UserName = reader["UserName"] as string,
                StatusID = (CaseStatus)reader["StatusID"],
                StatusUpdatedTime = (DateTime)reader["StatusUpdatedTime"],
                ValidTill = GetReaderValue<DateTime?>(reader, "ValidTill"),
                CreatedTime = GetReaderValue<DateTime?>(reader, "CreatedTime")
            };
        }

        private CasesSummary CasesSummaryMapper(IDataReader reader)
        {
            var casesSummary = new CasesSummary();
            casesSummary.CountCases = (int)reader["CountCases"];
            casesSummary.StatusName = reader["StatusName"] as string;
            return casesSummary;
        }

        private DailyVolumeLoose DailyVolumeLoosesMapper(IDataReader reader)
        {
            var dailyVolumeLoose = new DailyVolumeLoose();
            dailyVolumeLoose.DateDay = (DateTime)reader["DateDay"];
            dailyVolumeLoose.Volume = GetReaderValue<decimal>(reader, "Volume");
            return dailyVolumeLoose;
        }

        private BTSCases BTSCasesMapper(IDataReader reader)
        {
            var bTSCases = new BTSCases();
            bTSCases.CountCases = (int)reader["CountCases"];
            bTSCases.BTS_Id = GetReaderValue<int?>(reader, "BTS_Id");
            return bTSCases;
        }

        private StrategyCases StrategyCasesMapper(IDataReader reader)
        {
            var strategyCases = new StrategyCases();
            strategyCases.CountCases = (int)reader["CountCases"];
            strategyCases.StrategyName = reader["StrategyName"] as string;
            return strategyCases;
        }

        private BTSHighValueCases BTSHighValueCasesMapper(IDataReader reader)
        {
            var bTSHighValueCases = new BTSHighValueCases();
            bTSHighValueCases.Volume = (decimal)reader["Volume"];
            bTSHighValueCases.BTS_Id = GetReaderValue<int?>(reader, "BTS_Id");
            return bTSHighValueCases;
        }

        #endregion
    }
}
