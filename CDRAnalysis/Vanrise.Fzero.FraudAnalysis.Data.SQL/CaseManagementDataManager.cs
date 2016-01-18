﻿using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Text;
using Vanrise.Data.SQL;
using Vanrise.Entities;
using Vanrise.Fzero.FraudAnalysis.Entities;
using Vanrise.Fzero.FraudAnalysis.Entities.ResultQuery;

namespace Vanrise.Fzero.FraudAnalysis.Data.SQL
{
    public partial class CaseManagementDataManager : BaseSQLDataManager, ICaseManagementDataManager
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
            _columnMapper.Add("UserName", "UserID");
            _columnMapper.Add("CaseStatusDescription", "StatusID");
            _columnMapper.Add("SuspicionOccuranceStatusDescription", "SuspicionOccuranceStatus");
            _columnMapper.Add("AccountCaseStatusDescription", "AccountCaseStatusID");
        }

        #region Account Suspicion Summaries

        public BigResult<AccountSuspicionSummary> GetFilteredAccountSuspicionSummaries(Vanrise.Entities.DataRetrievalInput<AccountSuspicionSummaryQuery> input)
        {
            Action<string> createTempTableAction = (tempTableName) =>
            {
                ExecuteNonQueryText(CreateTempTableIfNotExists(tempTableName, input.Query.AccountNumber, input.Query.StrategyIDs, input.Query.AccountStatusIDs, input.Query.SuspicionLevelIDs), (cmd) =>
                {
                    cmd.Parameters.Add(new SqlParameter("@FromDate", input.Query.FromDate));
                    cmd.Parameters.Add(new SqlParameter("@ToDate", input.Query.ToDate));
                });
            };

            return RetrieveData(input, createTempTableAction, AccountSuspicionSummaryMapper, _columnMapper);
        }

        private string CreateTempTableIfNotExists(string tempTableName, string accountNumber, List<int> strategyIDs, List<CaseStatus> accountStatusIDs, List<SuspicionLevel> suspicionLevelIDs)
        {
            StringBuilder query = new StringBuilder(@"
                IF NOT OBJECT_ID('#TEMP_TABLE_NAME#', N'U') IS NOT NULL
                BEGIN
                

                SELECT        MAX(sed.SuspicionLevelID) AS SuspicionLevelID, CASE WHEN accStatus.[Status] IN (1, 2) 
                         THEN SUM(CASE WHEN sed.SuspicionOccuranceStatus = 1 THEN 1 ELSE 0 END) ELSE COUNT(*) END AS NumberOfOccurances, MAX(se.ExecutionDate) 
                         AS LastOccurance, { fn IFNULL(accStatus.Status, 1) } AS AccountStatusID, sed.AccountNumber
                INTO #TEMP_TABLE_NAME#                
                FROM            FraudAnalysis.StrategyExecution AS se INNER JOIN
                         FraudAnalysis.StrategyExecutionDetails AS sed ON se.ID = sed.StrategyExecutionID LEFT OUTER JOIN
                         FraudAnalysis.AccountStatus AS accStatus ON sed.AccountNumber = accStatus.AccountNumber
                #WHERE_CLAUSE#                
                GROUP BY accStatus.Status, sed.AccountNumber
                END
            ");

            query.Replace("#TEMP_TABLE_NAME#", tempTableName);
            query.Replace("#WHERE_CLAUSE#", GetWhereClause(accountNumber, strategyIDs, accountStatusIDs, suspicionLevelIDs));

            return query.ToString();
        }

        private string GetWhereClause(string accountNumber, List<int> strategyIDs, List<CaseStatus> accountStatusIDs, List<SuspicionLevel> suspicionLevelIDs)
        {
            StringBuilder whereClause = new StringBuilder();

            whereClause.Append("WHERE (se.ExecutionDate IS NULL OR (se.ExecutionDate >= @FromDate AND se.ExecutionDate <= @ToDate))");

            if (accountNumber != null)
                whereClause.Append(" AND sed.AccountNumber = '" + accountNumber + "'");

            if (strategyIDs != null)
                whereClause.Append(" AND se.StrategyID IN (" + string.Join(",", strategyIDs) + ")");

            if (accountStatusIDs != null)
                whereClause.Append(" AND (accStatus.[Status] is NULL  or accStatus.[Status] IN (" + string.Join(",", GetCaseStatusListAsIntList(accountStatusIDs)) + "))");

            if (suspicionLevelIDs != null)
                whereClause.Append(" AND sed.SuspicionLevelID IN (" + string.Join(",", GetSuspicionLevelListAsIntList(suspicionLevelIDs)) + ")");

            return whereClause.ToString();
        }


        private List<int> GetCaseStatusListAsIntList(List<CaseStatus> items)
        {
            List<int> list = new List<int>();

            foreach (CaseStatus item in items)
                list.Add((int)item);

            return list;
        }


        private List<int> GetSuspicionLevelListAsIntList(List<SuspicionLevel> items)
        {
            List<int> list = new List<int>();

            foreach (SuspicionLevel item in items)
                list.Add((int)item);

            return list;
        }

        #endregion

        public BigResult<AccountSuspicionDetail> GetFilteredAccountSuspicionDetails(Vanrise.Entities.DataRetrievalInput<AccountSuspicionDetailQuery> input)
        {

            Action<string> createTempTableAction = (tempTableName) =>
            {
                ExecuteNonQuerySP("FraudAnalysis.sp_StrategyExecutionDetails_CreateTempByAccountNumber", tempTableName, input.Query.AccountNumber, input.Query.FromDate, input.Query.ToDate);
            };

            return RetrieveData(input, createTempTableAction, AccountSuspicionDetailMapper, _columnMapper);
        }

        public BigResult<AccountCase> GetFilteredCasesByAccountNumber(Vanrise.Entities.DataRetrievalInput<AccountCaseQuery> input)
        {

            Action<string> createTempTableAction = (tempTableName) =>
            {
                ExecuteNonQuerySP("FraudAnalysis.sp_AccountCase_CreateTempByAccountNumber", tempTableName, input.Query.AccountNumber);
            };

            return RetrieveData(input, createTempTableAction, AccountCaseMapper, _columnMapper);
        }

        public BigResult<AccountCase> GetFilteredCasesByFilters(Vanrise.Entities.DataRetrievalInput<CancelAccountCasesQuery> input)
        {



            string strategyIDsString = null;
            if (input.Query.StrategyIDs != null)
                strategyIDsString = string.Join(",", input.Query.StrategyIDs);

            Action<string> createTempTableAction = (tempTableName) =>
            {



                ExecuteNonQuerySP("FraudAnalysis.sp_AccountCase_CreateTempByFilters", tempTableName, input.Query.AccountNumber, input.Query.From, input.Query.To, strategyIDsString);
            };

            return RetrieveData(input, createTempTableAction, AccountCaseMapper, _columnMapper);
        }

        public BigResult<AccountSuspicionDetail> GetFilteredDetailsByCaseID(Vanrise.Entities.DataRetrievalInput<CaseDetailQuery> input)
        {


            Action<string> createTempTableAction = (tempTableName) =>
            {
                ExecuteNonQuerySP("FraudAnalysis.sp_StrategyExecutionDetails_CreateTempByCaseID", tempTableName, input.Query.AccountNumber, input.Query.CaseID);
            };

            return RetrieveData(input, createTempTableAction, AccountSuspicionDetailMapper, _columnMapper);
        }

        public AccountSuspicionSummary GetAccountSuspicionSummaryByAccountNumber(string accountNumber, DateTime from, DateTime to)
        {
            return GetItemSP("FraudAnalysis.sp_AccountStatus_GetSummaryByAccountNumber", AccountSuspicionSummaryMapper, accountNumber, from, to);
        }

        public List<RelatedNumber> GetRelatedNumbersByAccountNumber(string accountNumber)
        {
            string result = ExecuteScalarSP("FraudAnalysis.sp_RelatedNumbers_GetRelatedNumbersByAccountNumber", accountNumber) as string;

            List<RelatedNumber> list = new List<RelatedNumber>();

            if (result != null)
            {
                List<string> relatedNumbers = result.ToString().Split(',').ToList();

                foreach (string number in relatedNumbers)
                {
                    list.Add(new RelatedNumber() { AccountNumber = number });
                }
            }

            return list;

        }

        public CaseStatus? GetAccountStatus(string accountNumber)
        {
            int? result = (int?)ExecuteScalarSP("FraudAnalysis.sp_AccountStatus_GetStatusByAccountNumber", accountNumber);
            return (CaseStatus?)result;
        }

        public BigResult<AccountCaseLog> GetFilteredAccountCaseLogsByCaseID(Vanrise.Entities.DataRetrievalInput<AccountCaseLogQuery> input)
        {


            Action<string> createTempTableAction = (tempTableName) =>
            {
                ExecuteNonQuerySP("FraudAnalysis.sp_AccountCaseHistory_CreateTempByCaseID", tempTableName, input.Query.CaseID);
            };

            return RetrieveData(input, createTempTableAction, AccountCaseLogMapper, _columnMapper);
        }

        #region Methods that update an account case

        public AccountCase GetLastAccountCaseByAccountNumber(string accountNumber)
        {
            return GetItemSP("FraudAnalysis.sp_AccountCase_GetLastByAccountNumber", AccountCaseMapper, accountNumber);
        }

        public bool InsertAccountCase(out int insertedID, string accountNumber, int? userID, CaseStatus caseStatus, DateTime? validTill, string reason)
        {
            object accountCaseID;

            int recordsAffected = ExecuteNonQuerySP("FraudAnalysis.sp_AccountCase_Insert", out accountCaseID, accountNumber, userID, caseStatus, validTill, reason);

            insertedID = (recordsAffected > 0) ? (int)accountCaseID : -1;

            return (recordsAffected > 0);
        }

        public bool UpdateAccountCase(int caseID, int userID, CaseStatus statusID, DateTime? validTill, string reason)
        {
            int recordsAffected = ExecuteNonQuerySP("FraudAnalysis.sp_AccountCase_Update", caseID, userID, statusID, validTill, reason);
            return (recordsAffected > 0);
        }

        public bool InsertAccountCaseHistory(int caseID, int? userID, CaseStatus caseStatus, string reason)
        {
            int recordsAffected = ExecuteNonQuerySP("FraudAnalysis.sp_AccountCaseHistory_Insert", caseID, userID, caseStatus, reason);
            return (recordsAffected > 0);
        }

        public bool InsertOrUpdateAccountStatus(string accountNumber, CaseStatus caseStatus, DateTime? validTill)
        {
            int recordsAffected = ExecuteNonQuerySP("FraudAnalysis.sp_AccountStatus_InsertOrUpdate", accountNumber, caseStatus, validTill);
            return (recordsAffected > 0);
        }


        public bool InsertOrUpdateAccountInfo(string accountNumber, InfoDetail infoDetail)
        {
            int recordsAffected = ExecuteNonQuerySP("FraudAnalysis.sp_AccountInfo_InsertOrUpdate", accountNumber, Vanrise.Common.Serializer.Serialize(infoDetail));
            return (recordsAffected > 0);
        }

        public bool LinkDetailToCase(string accountNumber, int caseID, CaseStatus caseStatus)
        {
            SuspicionOccuranceStatus occuranceStatus = (caseStatus.CompareTo(CaseStatus.Open) == 0 || caseStatus.CompareTo(CaseStatus.Pending) == 0) ? SuspicionOccuranceStatus.Open : SuspicionOccuranceStatus.Closed;

            int recordsAffected = ExecuteNonQuerySP("FraudAnalysis.sp_StrategyExecutionDetails_UpdateStatus", accountNumber, caseID, occuranceStatus);
            return (recordsAffected > 0);
        }

        #endregion

        public List<int> DeleteAccountCases_ByCaseIDs(List<int> caseIDs)
        {
            return GetItemsSP("FraudAnalysis.sp_AccountCase_DeleteByCaseIDs", CaseMapper, string.Join(",", caseIDs));
        }

        #region Private Members

        private int CaseMapper(IDataReader reader)
        {
            return (int)reader["CaseID"];
        }

        private AccountSuspicionSummary AccountSuspicionSummaryMapper(IDataReader reader)
        {
            var summary = new AccountSuspicionSummary();

            summary.AccountNumber = reader["AccountNumber"] as string;
            summary.SuspicionLevelID = GetReaderValue<SuspicionLevel>(reader, "SuspicionLevelID");
            summary.NumberOfOccurances = (int)reader["NumberOfOccurances"];
            summary.LastOccurance = GetReaderValue<DateTime?>(reader, "LastOccurance");
            summary.AccountStatusID = GetReaderValue<CaseStatus>(reader, "AccountStatusID");

            return summary;
        }

        private AccountSuspicionDetail AccountSuspicionDetailMapper(IDataReader reader)
        {
            var detail = new AccountSuspicionDetail(); // a detail is a fraud result instance

            detail.DetailID = (long)reader["DetailID"];
            detail.SuspicionLevelID = (SuspicionLevel)reader["SuspicionLevelID"];
            detail.StrategyName = reader["StrategyName"] as string;
            detail.SuspicionOccuranceStatus = (SuspicionOccuranceStatus)reader["SuspicionOccuranceStatus"];
            detail.FromDate = (DateTime)reader["FromDate"];
            detail.ToDate = (DateTime)reader["ToDate"];
            detail.ExecutionDate = (DateTime)reader["ExecutionDate"];
            detail.AggregateValues = Vanrise.Common.Serializer.Deserialize<Dictionary<string, decimal>>(GetReaderValue<string>(reader, "AggregateValues"));

            return detail;
        }

        private AccountCase AccountCaseMapper(IDataReader reader)
        {
            AccountCase accountCase = new AccountCase();

            accountCase.CaseID = (int)reader["CaseID"];
            accountCase.AccountNumber = reader["AccountNumber"] as string;
            accountCase.UserID = GetReaderValue<int>(reader, "UserID");
            accountCase.StatusID = (CaseStatus)reader["StatusID"];
            accountCase.StatusUpdatedTime = (DateTime)reader["StatusUpdatedTime"];
            accountCase.ValidTill = GetReaderValue<DateTime?>(reader, "ValidTill");
            accountCase.CreatedTime = GetReaderValue<DateTime?>(reader, "CreatedTime");

            return accountCase;
        }

        private AccountCaseLog AccountCaseLogMapper(IDataReader reader)
        {
            AccountCaseLog log = new AccountCaseLog();

            log.LogID = (int)reader["LogID"];
            log.UserID = GetReaderValue<int?>(reader, "UserID");
            log.AccountCaseStatusID = (CaseStatus)reader["AccountCaseStatusID"];
            log.StatusTime = (DateTime)reader["StatusTime"];
            log.Reason = GetReaderValue<string>(reader, "Reason");

            return log;
        }


        #endregion
    }
}
