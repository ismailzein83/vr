using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Text;
using Vanrise.Data.SQL;
using Vanrise.Entities;
using Vanrise.Fzero.FraudAnalysis.Entities;

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
        }

        #region Account Suspicion Summaries

        public BigResult<AccountSuspicionSummary> GetFilteredAccountSuspicionSummaries(Vanrise.Entities.DataRetrievalInput<AccountSuspicionSummaryQuery> input)
        {
            Dictionary<string, string> mapper = new Dictionary<string, string>();
            mapper.Add("SuspicionLevelDescription", "SuspicionLevelID");
            mapper.Add("AccountStatusDescription", "AccountStatusID");

            //Action<string> createTempTableAction = (tempTableName) =>
            //{
            //    ExecuteNonQueryText(CreateTempTableIfNotExists(tempTableName,
            //        input.Query.SelectedStrategyIDs,
            //        input.Query.SelectedSuspicionLevelIDs,
            //        input.Query.SelectedCaseStatusIDs),
            //        (cmd) =>
            //        {
            //            cmd.Parameters.Add(new SqlParameter("@From", input.Query.From));
            //            cmd.Parameters.Add(new SqlParameter("@To", input.Query.To));
            //        });
            //};

            //return RetrieveData(input, createTempTableAction, AccountSuspicionSummaryMapper, mapper);

            return RetrieveData(input, (tempTableName) =>
            {
                string strategyIDs = null;
                string suspicionLevelIDs = null;
                string accountStatusIDs = null;

                if (input.Query.StrategyIDs != null && input.Query.StrategyIDs.Count() > 0)
                    strategyIDs = string.Join<int>(",", input.Query.StrategyIDs);

                if (input.Query.SuspicionLevelIDs != null && input.Query.SuspicionLevelIDs.Count() > 0)
                    suspicionLevelIDs = string.Join(",", input.Query.SuspicionLevelIDs.Select(n => ((int)n).ToString()).ToArray());

                if (input.Query.AccountStatusIDs != null && input.Query.AccountStatusIDs.Count() > 0)
                    accountStatusIDs = string.Join(",", input.Query.AccountStatusIDs.Select(n => ((int)n).ToString()).ToArray());

                ExecuteNonQuerySP("FraudAnalysis.sp_StrategyExecutionDetails_CreateTempByAccountNumberForSummaries", tempTableName, input.Query.AccountNumber, input.Query.FromDate, input.Query.ToDate, strategyIDs, accountStatusIDs, suspicionLevelIDs);

            }, (reader) => AccountSuspicionSummaryMapper(reader), mapper);
        }

        private string CreateTempTableIfNotExists(string tempTableName, List<int> SelectedStrategyIDs, List<SuspicionLevel> SelectedSuspicionLevelIDs, List<CaseStatus> SelectedCaseStatusIDs)
        {
            StringBuilder query = new StringBuilder();

            if (SelectedCaseStatusIDs != null && !SelectedCaseStatusIDs.Contains(CaseStatus.Open) && !SelectedCaseStatusIDs.Contains(CaseStatus.Pending))
            {
                query.Append(@"
                    IF NOT OBJECT_ID('#TEMP_TABLE_NAME#', N'U') IS NOT NULL
                        BEGIN
			                SELECT
				                sed.AccountNumber,
				                MAX(sed.SuspicionLevelID) AS SuspicionLevelID,
				                COUNT(*) AS NumberOfOccurances,
				                MAX(se.ExecutionDate) AS LastOccurance,
				                accStatus.[Status] AS AccountStatusID

                            INTO #RESULT

			                FROM FraudAnalysis.AccountStatus accStatus 
			                LEFT JOIN FraudAnalysis.StrategyExecutionDetails sed ON accStatus.AccountNumber = sed.AccountNumber
			                LEFT JOIN FraudAnalysis.StrategyExecution se ON se.ID = sed.StrategyExecutionID
			                LEFT JOIN OpenAccounts openAcc ON openAcc.AccountNumber = sed.AccountNumber

			                WHERE openAcc.AccountNumber IS NULL AND accStatus.Status IN (3, 4) --3: Fraud, 4: WhiteList
				                AND se.FromDate >= @From
				                AND se.FromDate <= @To
				                #COMMON_WHERE_CLAUSE_CONDITIONS#

			                GROUP BY sed.AccountNumber, accStatus.[Status]

                            DECLARE @sql VARCHAR(1000)
		                    SET @sql = 'SELECT * INTO #TEMP_TABLE_NAME# FROM #RESULT';
		                    EXEC(@sql)
                        END
                ");
            }
            else if (SelectedCaseStatusIDs != null && !SelectedCaseStatusIDs.Contains(CaseStatus.ClosedFraud) && !SelectedCaseStatusIDs.Contains(CaseStatus.ClosedWhiteList))
            {
                query.Append(@"
                    IF NOT OBJECT_ID('#TEMP_TABLE_NAME#', N'U') IS NOT NULL
                        BEGIN
			                SELECT
				                sed.AccountNumber,
				                MAX(sed.SuspicionLevelID) AS SuspicionLevelID,
				                COUNT(*) AS NumberOfOccurances,
				                MAX(se.ExecutionDate) AS LastOccurance,
				                CASE WHEN ISNULL(accStatus.[Status], 0) IN (0, 2) THEN ISNULL(accStatus.[Status], 0) ELSE 0 END AS AccountStatusID

                            INTO #RESULT

			                FROM FraudAnalysis.StrategyExecutionDetails sed
			                INNER JOIN FraudAnalysis.StrategyExecution se ON se.ID = sed.StrategyExecutionID
			                LEFT JOIN FraudAnalysis.AccountStatus accStatus ON accStatus.AccountNumber = sed.AccountNumber

			                WHERE sed.SuspicionOccuranceStatus = 0
				                AND se.FromDate >= @From
				                AND se.FromDate <= @To
                                #COMMON_WHERE_CLAUSE_CONDITIONS#

			                GROUP BY sed.AccountNumber, accStatus.[Status]

                            DECLARE @sql VARCHAR(1000)
		                    SET @sql = 'SELECT * INTO #TEMP_TABLE_NAME# FROM #RESULT';
		                    EXEC(@sql)
                        END
                ");
            }
            else
            {
                query.Append(@"
                    IF NOT OBJECT_ID('#TEMP_TABLE_NAME#', N'U') IS NOT NULL
                        BEGIN
                            WITH OpenAccounts AS
		                    (
			                    SELECT
				                    sed.AccountNumber,
				                    MAX(sed.SuspicionLevelID) AS SuspicionLevelID,
				                    COUNT(*) AS NumberOfOccurances,
				                    MAX(se.ExecutionDate) AS LastOccurance,
				                    CASE WHEN ISNULL(accStatus.[Status], 0) IN (0, 2) THEN ISNULL(accStatus.[Status], 0) ELSE 0 END AS AccountStatusID
			  
			                    FROM FraudAnalysis.StrategyExecutionDetails sed
			                    INNER JOIN FraudAnalysis.StrategyExecution se ON se.ID = sed.StrategyExecutionID
			                    LEFT JOIN FraudAnalysis.AccountStatus accStatus ON accStatus.AccountNumber = sed.AccountNumber

			                    WHERE sed.SuspicionOccuranceStatus = 0
				                    AND se.FromDate >= @From
				                    AND se.FromDate <= @To
				                    #COMMON_WHERE_CLAUSE_CONDITIONS#

			                    GROUP BY sed.AccountNumber, accStatus.[Status]
		                    ),
		      
		                    ClosedAccounts AS
		                    (
			                    SELECT
				                    sed.AccountNumber,
				                    MAX(sed.SuspicionLevelID) AS SuspicionLevelID,
				                    COUNT(*) AS NumberOfOccurances,
				                    MAX(se.ExecutionDate) AS LastOccurance,
				                    accStatus.[Status] AS AccountStatusID

			                    FROM FraudAnalysis.AccountStatus accStatus 
			                    LEFT JOIN FraudAnalysis.StrategyExecutionDetails sed ON accStatus.AccountNumber = sed.AccountNumber
			                    LEFT JOIN FraudAnalysis.StrategyExecution se ON se.ID = sed.StrategyExecutionID
			                    LEFT JOIN OpenAccounts openAcc ON openAcc.AccountNumber = sed.AccountNumber 

			                    WHERE openAcc.AccountNumber IS NULL AND accStatus.Status IN (3, 4) --3: Fraud, 4: WhiteList
				                    AND se.FromDate >= @From
				                    AND se.FromDate <= @To
				                    #COMMON_WHERE_CLAUSE_CONDITIONS#

			                    GROUP BY sed.AccountNumber, accStatus.[Status]
		                    )

		                    SELECT * INTO #RESULT
		                    FROM (SELECT * FROM OpenAccounts UNION SELECT * FROM ClosedAccounts) AllAccounts
		                    WHERE AllAccounts.AccountStatusID = 0

                            DECLARE @sql VARCHAR(1000)
		                    SET @sql = 'SELECT * INTO #TEMP_TABLE_NAME# FROM #RESULT';
		                    EXEC(@sql)
                        END
                ");
            }

            query.Replace("#TEMP_TABLE_NAME#", tempTableName);
            query.Replace("#COMMON_WHERE_CLAUSE_CONDITIONS#", GetCommonWhereClauseConditions(SelectedSuspicionLevelIDs));

            return query.ToString();
        }

        private string GetCommonWhereClauseConditions(List<SuspicionLevel> SelectedSuspicionLevelIDs)
        {
            StringBuilder conditions = new StringBuilder();

            if (SelectedSuspicionLevelIDs != null && SelectedSuspicionLevelIDs.Count > 0) {

                string selectedSuspicionLevelIDs = string.Join(",", SelectedSuspicionLevelIDs.Select(n => ((int)n).ToString()).ToArray());
                conditions.Append("AND sed.SuspicionLevelID IN (" + selectedSuspicionLevelIDs + ")");
            }

            return conditions.ToString();
        }

        #endregion

        public BigResult<AccountSuspicionDetail> GetFilteredAccountSuspicionDetails(Vanrise.Entities.DataRetrievalInput<AccountSuspicionDetailQuery> input)
        {
            Dictionary<string, string> mapper = new Dictionary<string, string>();
            mapper.Add("SuspicionLevelDescription", "SuspicionLevelID");

            Action<string> createTempTableAction = (tempTableName) =>
            {
                ExecuteNonQuerySP("FraudAnalysis.sp_StrategyExecutionDetails_CreateTempByAccountNumber", tempTableName, input.Query.AccountNumber, input.Query.FromDate, input.Query.ToDate);
            };

            return RetrieveData(input, createTempTableAction, AccountSuspicionDetailMapper, mapper);
        }

        public BigResult<AccountCase> GetFilteredCasesByAccountNumber(Vanrise.Entities.DataRetrievalInput<AccountCaseResultQuery> input)
        {
            Dictionary<string, string> mapper = new Dictionary<string, string>();
            mapper.Add("CaseStatusDescription", "StatusID");

            Action<string> createTempTableAction = (tempTableName) =>
            {
                ExecuteNonQuerySP("FraudAnalysis.sp_AccountCase_CreateTempByAccountNumber", tempTableName, input.Query.AccountNumber);
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
                ExecuteNonQuerySP("FraudAnalysis.sp_StrategyExecutionDetails_CreateTempByCaseID", tempTableName, input.Query.AccountNumber, input.Query.CaseID);
            };

            return RetrieveData(input, createTempTableAction, AccountSuspicionDetailMapper, mapper);
        }

        public AccountSuspicionSummary GetAccountSuspicionSummaryByAccountNumber(string accountNumber, DateTime from, DateTime to)
        {
            return GetItemSP("FraudAnalysis.sp_StrategyExecutionDetails_GetSummaryByAccountNumber", AccountSuspicionSummaryMapper, accountNumber, from, to);
        }

        public List<RelatedNumber> GetRelatedNumbersByAccountNumber(string accountNumber)
        {
            string result = ExecuteScalarSP("FraudAnalysis.sp_RelatedNumbers_GetByAccountNumber", accountNumber) as string;
            
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


        #region Methods that update an account case

            public AccountCase GetLastAccountCaseByAccountNumber(string accountNumber)
            {
                return GetItemSP("FraudAnalysis.sp_AccountCase_GetLastByAccountNumber", AccountCaseMapper, accountNumber);
            }

            public bool InsertAccountCase(out int insertedID, string accountNumber, int? userID, CaseStatus caseStatus, DateTime? validTill)
            {
                object accountCaseID;

                int recordsAffected = ExecuteNonQuerySP("FraudAnalysis.sp_AccountCase_Insert", out accountCaseID, accountNumber, userID, caseStatus, validTill);

                insertedID = (recordsAffected > 0) ? (int)accountCaseID : -1;

                return (recordsAffected > 0);
            }

            public bool UpdateAccountCaseStatus(int caseID, int userID, CaseStatus statusID, DateTime? validTill)
            {
                int recordsAffected = ExecuteNonQuerySP("FraudAnalysis.sp_AccountCase_Update", caseID, userID, statusID, validTill);
                return (recordsAffected > 0);
            }

            public bool InsertAccountCaseHistory(int caseID, int? userID, CaseStatus caseStatus)
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

        #endregion


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

        public bool CancelAccountCases(int strategyID, string accountNumber, DateTime from, DateTime to)
        {
            int recordsAffected = ExecuteNonQuerySP("FraudAnalysis.sp_AccountCase_Cancel", strategyID, accountNumber, from, to);
            return (recordsAffected > 0);
        }

        #region Private Members

        private AccountSuspicionSummary AccountSuspicionSummaryMapper(IDataReader reader)
        {
            var summary = new AccountSuspicionSummary();

            summary.AccountNumber = reader["AccountNumber"] as string;
            summary.SuspicionLevelID = (SuspicionLevel)reader["SuspicionLevelID"];
            summary.NumberOfOccurances = (int)reader["NumberOfOccurances"];
            summary.LastOccurance = (DateTime)reader["LastOccurance"];
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
