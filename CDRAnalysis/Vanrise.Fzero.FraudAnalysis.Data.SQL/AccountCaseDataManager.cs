using System;
using System.Collections.Generic;
using System.Data;
using System.Text;
using Vanrise.Data.SQL;
using Vanrise.Entities;
using Vanrise.Fzero.FraudAnalysis.Entities;

namespace Vanrise.Fzero.FraudAnalysis.Data.SQL
{
    public class AccountCaseDataManager : BaseSQLDataManager, IAccountCaseDataManager
    {

        #region ctor
        private static Dictionary<string, string> _columnMapper = new Dictionary<string, string>();
        public AccountCaseDataManager()
            : base("CDRDBConnectionString")
        {

        }
        static AccountCaseDataManager()
        {
            _columnMapper.Add("UserName", "UserID");
            _columnMapper.Add("CaseStatusDescription", "StatusID");
            _columnMapper.Add("SuspicionLevelDescription", "SuspicionLevelID");
            _columnMapper.Add("AccountStatusDescription", "Status");
        }
        #endregion

        #region Public Methods
        public BigResult<AccountSuspicionSummary> GetFilteredAccountSuspicionSummaries(Vanrise.Entities.DataRetrievalInput<AccountSuspicionSummaryQuery> input)
        {
            Action<string> createTempTableAction = (tempTableName) =>
            {
                ExecuteNonQueryText(CreateTempTableIfNotExists(tempTableName, input.Query.AccountNumber, input.Query.StrategyIDs, input.Query.Statuses, input.Query.SuspicionLevelIDs, input.Query.FromDate, input.Query.ToDate), (cmd) => { });
            };

            return RetrieveData(input, createTempTableAction, AccountSuspicionSummaryMapper, _columnMapper);
        }

        public BigResult<AccountCase> GetFilteredCasesByAccountNumber(Vanrise.Entities.DataRetrievalInput<AccountCaseQuery> input)
        {

            Action<string> createTempTableAction = (tempTableName) =>
            {
                ExecuteNonQuerySP("FraudAnalysis.sp_AccountCase_CreateTempByAccountNumber", tempTableName, input.Query.AccountNumber);
            };

            return RetrieveData(input, createTempTableAction, AccountCaseMapper, _columnMapper);
        }

        public AccountSuspicionSummary GetAccountSuspicionSummaryByCaseId(int caseID)
        {
            return GetItemSP("FraudAnalysis.sp_AccountCase_GetDetailsByID", AccountSuspicionSummaryMapper, caseID);
        }

        public AccountCase GetAccountCase(int caseID)
        {
            return GetItemSP("FraudAnalysis.sp_AccountCase_GetByCaseID", AccountCaseMapper, caseID);
        }

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

        DataTable GetAccountCaseTable()
        {
            DataTable dt = new DataTable("FraudAnalysis.AccountCase");
            dt.Columns.Add("ID", typeof(int));
            dt.Columns.Add("UserID", typeof(int));
            dt.Columns.Add("Status", typeof(int));
            return dt;
        }

        public bool UpdateAccountCaseBatch(List<int> CaseIds, int userId, CaseStatus status)
        {
            DataTable dtAccountCasesToUpdate = GetAccountCaseTable();
            DataRow dr;

            foreach (var item in CaseIds)
            {
                dr = dtAccountCasesToUpdate.NewRow();
                dr["ID"] = item;
                dr["UserID"] = userId;
                dr["Status"] = (int)status;
                dtAccountCasesToUpdate.Rows.Add(dr);
            }

            int recordsAffected = 0;
            if (dtAccountCasesToUpdate.Rows.Count > 0)
            {
                recordsAffected = ExecuteNonQuerySPCmd("[FraudAnalysis].[sp_AccountCase_BulkUpdate]",
                       (cmd) =>
                       {
                           var dtPrm = new System.Data.SqlClient.SqlParameter("@AccountCase", SqlDbType.Structured);
                           dtPrm.Value = dtAccountCasesToUpdate;
                           cmd.Parameters.Add(dtPrm);
                       });
            }

            return (recordsAffected > 0);


        }

        #endregion

        #region Private Methods
        private string CreateTempTableIfNotExists(string tempTableName, string accountNumber, List<int> strategyIDs, List<CaseStatus> accountStatusIDs, List<SuspicionLevel> suspicionLevelIDs, DateTime fromDate, DateTime? toDate)
        {
            StringBuilder query = new StringBuilder(@"
                IF NOT OBJECT_ID('#TEMP_TABLE_NAME#', N'U') IS NOT NULL
                BEGIN
                
                    SELECT ac.ID CaseID, ac.AccountNumber, ac.[Status], 
	                       COUNT(sed.ID) AS NumberOfOccurances,
	                       MAX(sed.SuspicionLevelID) AS SuspicionLevelID, 
	                       MAX(se.ExecutionDate) AS LastOccurance
                    INTO #TEMP_TABLE_NAME#                
                    FROM FraudAnalysis.AccountCase ac  WITH (NOLOCK)
                    LEFT JOIN FraudAnalysis.StrategyExecutionItem AS sed WITH (NOLOCK) ON sed.CaseID = ac.ID
                    LEFT JOIN FraudAnalysis.StrategyExecution AS se WITH (NOLOCK) ON se.ID = sed.StrategyExecutionID
                    #WHERE_CLAUSE#                
                    GROUP BY ac.ID, ac.AccountNumber, ac.Status
              
                END
            ");

            query.Replace("#TEMP_TABLE_NAME#", tempTableName);
            query.Replace("#WHERE_CLAUSE#", GetWhereClause(accountNumber, strategyIDs, accountStatusIDs, suspicionLevelIDs, fromDate, toDate));

            return query.ToString();
        }
        private string GetWhereClause(string accountNumber, List<int> strategyIDs, List<CaseStatus> accountStatusIDs, List<SuspicionLevel> suspicionLevelIDs, DateTime fromDate, DateTime? toDate)
        {
            StringBuilder whereClause = new StringBuilder();

            if (toDate.HasValue)
                whereClause.Append("WHERE ac.CreatedTime >= '" + fromDate + "' AND ac.CreatedTime < '" + toDate + "'");
            else
                whereClause.Append("WHERE ac.CreatedTime >= '" + fromDate + "' ");

            if (accountNumber != null)
                whereClause.Append(" AND ac.AccountNumber = '" + accountNumber + "'");

            if (strategyIDs != null && strategyIDs.Count > 0)
                whereClause.Append(" AND se.StrategyID IN (" + string.Join(",", strategyIDs) + ")");

            if (accountStatusIDs != null && accountStatusIDs.Count > 0)
                whereClause.Append(" AND ac.[Status] IN (" + string.Join(",", GetCaseStatusListAsIntList(accountStatusIDs)) + ")");

            if (suspicionLevelIDs != null && suspicionLevelIDs.Count > 0)
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

        #region Mappers
        internal AccountCase AccountCaseMapper(IDataReader reader)
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
        private AccountSuspicionSummary AccountSuspicionSummaryMapper(IDataReader reader)
        {
            var summary = new AccountSuspicionSummary();

            summary.AccountNumber = reader["AccountNumber"] as string;
            summary.SuspicionLevelID = GetReaderValue<SuspicionLevel>(reader, "SuspicionLevelID");
            summary.NumberOfOccurances = (int)reader["NumberOfOccurances"];
            summary.LastOccurance = GetReaderValue<DateTime?>(reader, "LastOccurance");
            summary.Status = GetReaderValue<CaseStatus>(reader, "Status");
            summary.CaseID = GetReaderValue<int>(reader, "CaseID");
            return summary;
        }

        #endregion

    }
}
