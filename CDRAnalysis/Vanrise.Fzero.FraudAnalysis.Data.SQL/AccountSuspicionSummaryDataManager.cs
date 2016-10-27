using System;
using System.Collections.Generic;
using System.Data;
using System.Text;
using Vanrise.Data.SQL;
using Vanrise.Entities;
using Vanrise.Fzero.FraudAnalysis.Entities;

namespace Vanrise.Fzero.FraudAnalysis.Data.SQL
{
    public class AccountSuspicionSummaryDataManager : BaseSQLDataManager, IAccountSuspicionSummaryDataManager
    {

        #region ctor
        private static Dictionary<string, string> _columnMapper = new Dictionary<string, string>();
        public AccountSuspicionSummaryDataManager()
            : base("CDRDBConnectionString")
        {

        }
        static AccountSuspicionSummaryDataManager()
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
                ExecuteNonQueryText(CreateTempTableIfNotExists(tempTableName, input.Query.AccountNumber, input.Query.StrategyIDs, input.Query.Statuses, input.Query.SuspicionLevelIDs, input.Query.FromDate, input.Query.ToDate, input.Query.StrategyExecutionId), (cmd) => { });
            };

            return RetrieveData(input, createTempTableAction, AccountSuspicionSummaryMapper, _columnMapper);
        }

        public AccountSuspicionSummary GetAccountSuspicionSummaryByCaseId(int caseID)
        {
            return GetItemSP("FraudAnalysis.sp_AccountCase_GetDetailsByID", AccountSuspicionSummaryMapper, caseID);
        }

        #endregion

        #region Private Methods
        private string CreateTempTableIfNotExists(string tempTableName, string accountNumber, List<int> strategyIDs, List<CaseStatus> accountStatusIDs, List<SuspicionLevel> suspicionLevelIDs, DateTime fromDate, DateTime? toDate, long? strategyExecutionId)
        {
            StringBuilder query = new StringBuilder(@"
                IF NOT OBJECT_ID('#TEMP_TABLE_NAME#', N'U') IS NOT NULL
                BEGIN
                
                    SELECT ac.ID CaseID, ac.AccountNumber, ac.[Status], 
	                       COUNT(sed.ID) AS NumberOfOccurances,
	                       MAX(sed.SuspicionLevelID) AS SuspicionLevelID, 
	                       MAX(se.ExecutionDate) AS LastOccurance, MAX(sed.StrategyExecutionID) AS StrategyExecutionID
                    INTO #TEMP_TABLE_NAME#                
                    FROM FraudAnalysis.AccountCase ac  WITH (NOLOCK)
                    LEFT JOIN FraudAnalysis.StrategyExecutionItem AS sed WITH (NOLOCK) ON sed.CaseID = ac.ID
                    LEFT JOIN FraudAnalysis.StrategyExecution AS se WITH (NOLOCK) ON se.ID = sed.StrategyExecutionID
                    #WHERE_CLAUSE#                
                    GROUP BY ac.ID, ac.AccountNumber, ac.Status
              
                END
            ");

            query.Replace("#TEMP_TABLE_NAME#", tempTableName);
            query.Replace("#WHERE_CLAUSE#", GetWhereClause(strategyExecutionId, accountNumber, strategyIDs, accountStatusIDs, suspicionLevelIDs, fromDate, toDate));

            return query.ToString();
        }

        private string GetWhereClause(long? strategyExecutionId, string accountNumber, List<int> strategyIDs, List<CaseStatus> accountStatusIDs, List<SuspicionLevel> suspicionLevelIDs, DateTime fromDate, DateTime? toDate)
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

            if (strategyExecutionId != null)
                whereClause.Append(" AND sed.StrategyExecutionID = " + strategyExecutionId + "");

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
        private AccountSuspicionSummary AccountSuspicionSummaryMapper(IDataReader reader)
        {
            var summary = new AccountSuspicionSummary();
            summary.StrategyExecutionId =Convert.ToInt64(reader["StrategyExecutionID"]);
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
