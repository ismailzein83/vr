using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using Vanrise.Data.SQL;
using Vanrise.Entities;
using Vanrise.Fzero.CDRImport.Entities;
using Vanrise.Fzero.FraudAnalysis.Entities;
using Vanrise.Fzero.CDRImport.Entities;

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

        #region Old Methods

        public bool SaveAccountCase(AccountCase accountCaseObject)
        {
            throw new NotImplementedException();
        }

        public BigResult<AccountCase> GetFilteredAccountCases(DataRetrievalInput<AccountCaseResultQuery> input)
        {
            throw new NotImplementedException();
        }

        public void UpdateSusbcriberCases(List<AccountCaseType> cases)
        {
            DataTable dataTable = new DataTable("[FraudAnalysis].[AccountCaseType]");
            //we create column names as per the type in DB 
            dataTable.Columns.Add("AccountNumber", typeof(string));
            dataTable.Columns.Add("StrategyId", typeof(int));
            dataTable.Columns.Add("SuspicionLevelID", typeof(int));
            foreach (var i in cases)
            {
                dataTable.Rows.Add(i.AccountNumber, i.StrategyId, i.SuspicionLevelID);
            }

            ExecuteNonQuerySPCmd("[FraudAnalysis].[sp_FraudResult_UpdateAccountCases]",
                    (cmd) =>
                    {

                        SqlParameter parameter = new SqlParameter();
                        parameter.ParameterName = "@AccountCase";
                        parameter.SqlDbType = System.Data.SqlDbType.Structured;
                        parameter.Value = dataTable;
                        parameter.TypeName = "[FraudAnalysis].[AccountCaseType]";
                        cmd.Parameters.Add(parameter);
                    });
        }

        public BigResult<FraudResult> GetFilteredSuspiciousNumbers(Vanrise.Entities.DataRetrievalInput<FraudResultQuery> input)
        {
            Action<string> createTempTableAction = (tempTableName) =>
            {
                ExecuteNonQuerySP("[FraudAnalysis].[sp_FraudResult_CreateTempForFilteredSuspiciousNumbers]", tempTableName, input.Query.FromDate, input.Query.ToDate, input.Query.StrategiesList, input.Query.SuspicionLevelsList, input.Query.CaseStatusesList, input.Query.AccountNumber);
            };
            return RetrieveData(input, createTempTableAction, FraudResultMapper);
        }

        public FraudResult GetFraudResult(DateTime fromDate, DateTime toDate, List<int> strategiesList, List<int> suspicionLevelsList, string accountNumber)
        {
            return GetItemsSP("FraudAnalysis.sp_FraudResult_Get", FraudResultMapper, fromDate, toDate, string.Join(",", strategiesList), string.Join(",", suspicionLevelsList), accountNumber).FirstOrDefault();
        }

        public object FinishDBApplyStream(object dbApplyStream)
        {
            StreamForBulkInsert streamForBulkInsert = dbApplyStream as StreamForBulkInsert;
            streamForBulkInsert.Close();
            return new StreamBulkInsertInfo
            {
                TableName = "[FraudAnalysis].[AccountThreshold]",
                Stream = streamForBulkInsert,
                TabLock = false,
                KeepIdentity = false,
                FieldSeparator = '^'
            };
        }

        public object InitialiazeStreamForDBApply()
        {
            return base.InitializeStreamForBulkInsert();
        }

        public void WriteRecordToStream(SuspiciousNumber record, object dbApplyStream)
        {
            StreamForBulkInsert streamForBulkInsert = dbApplyStream as StreamForBulkInsert;
            streamForBulkInsert.WriteRecord("0^{0}^{1}^{2}^{3}^{4}",
                                 record.DateDay.Value,
                                 record.Number,
                                 record.SuspicionLevel,
                                 record.StrategyId,
                                 Vanrise.Common.Serializer.Serialize(record.CriteriaValues, true));
        }

        public void ApplySuspiciousNumbersToDB(object preparedSuspiciousNumbers)
        {
            InsertBulkToTable(preparedSuspiciousNumbers as BaseBulkInsertInfo);
        }

        #endregion

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

                ExecuteNonQuerySP("FraudAnalysis.sp_StrategyExecutionDetails_CreateTempByFiltered", tempTableName, input.Query.AccountNumber, input.Query.From, input.Query.To, selectedStrategyIDs, selectedSuspicionLevelIDs, selectedCaseStatusIDs);

            }, (reader) => AccountSuspicionSummaryMapper(reader), mapper);
        }

        public BigResult<AccountSuspicionDetail> GetFilteredAccountSuspicionDetails(Vanrise.Entities.DataRetrievalInput<AccountSuspicionDetailQuery> input)
        {
            Dictionary<string, string> mapper = new Dictionary<string, string>();
            mapper.Add("SuspicionLevelDescription", "SuspicionLevelID");
            mapper.Add("AccountStatusDescription", "AccountStatusID");

            Action<string> createTempTableAction = (tempTableName) =>
            {
                ExecuteNonQuerySP("FraudAnalysis.sp_StrategyExecutionDetails_GetByAccountNumber", tempTableName, input.Query.AccountNumber, input.Query.From, input.Query.To);
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

        #region Old Mappers

        private FraudResult FraudResultMapper(IDataReader reader)
        {
            var fraudResult = new FraudResult();
            fraudResult.LastOccurance = (DateTime)reader["LastOccurance"];
            fraudResult.AccountNumber = reader["AccountNumber"] as string;
            fraudResult.SuspicionLevelName = ((SuspicionLevelEnum)Enum.ToObject(typeof(SuspicionLevelEnum), GetReaderValue<int>(reader, "SuspicionLevelId"))).ToString();
            fraudResult.StrategyName = reader["StrategyName"] as string;
            fraudResult.NumberofOccurances = reader["NumberofOccurances"] as string;
            fraudResult.CaseStatus = reader["CaseStatus"] as string;
            fraudResult.StatusId = GetReaderValue<int?>(reader, "StatusId");
            fraudResult.ValidTill = GetReaderValue<DateTime?>(reader, "ValidTill");
            return fraudResult;
        }

        private AccountThreshold AccountThresholdMapper(IDataReader reader)
        {
            var accountThreshold = new AccountThreshold();

            accountThreshold.DateDay = GetReaderValue<DateTime>(reader, "DateDay");
            accountThreshold.SuspicionLevelName = reader["SuspicionLevelName"] as string;
            accountThreshold.StrategyName = reader["StrategyName"] as string;
            accountThreshold.CriteriaValues = Vanrise.Common.Serializer.Deserialize<Dictionary<int, decimal>>(GetReaderValue<string>(reader, "CriteriaValues"));

            return accountThreshold;
        }

        #endregion

        #region New Mappers

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
            detail.AccountNumber = reader["AccountNumber"] as string;
            detail.SuspicionLevelID = (SuspicionLevel)reader["SuspicionLevelID"];
            detail.StrategyName = reader["StrategyName"] as string;
            detail.AccountStatusID = GetReaderValue<CaseStatus>(reader, "AccountStatusID");
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
                StatusID = (CaseStatus)reader["StatusID"],
                StatusUpdatedTime = (DateTime)reader["StatusUpdatedTime"],
                ValidTill = GetReaderValue<DateTime?>(reader, "ValidTill"),
                CreatedTime = GetReaderValue<DateTime?>(reader, "CreatedTime")
            };
        }


        #endregion

        #region Junk Code

        /*
        public bool SaveAccountCase(AccountCase accountCaseObject)
        {
            int recordesEffected = ExecuteNonQuerySP("FraudAnalysis.sp_AccountCase_Insert", accountCaseObject.AccountNumber, accountCaseObject.StatusID, accountCaseObject.ValidTill, accountCaseObject.UserId, accountCaseObject.StrategyId, accountCaseObject.SuspicionLevelID);
            if (recordesEffected > 0)
                return true;
            return false;
        }
        
        public BigResult<AccountCase> GetFilteredAccountCases(Vanrise.Entities.DataRetrievalInput<AccountCaseResultQuery> input)
        {
            Action<string> createTempTableAction = (tempTableName) =>
            {
                ExecuteNonQuerySP("FraudAnalysis.sp_FraudResult_CreateTempForFilteredAccountCases", tempTableName, input.Query.AccountNumber);
            };

            return RetrieveData(input, createTempTableAction, AccountCaseMapper);
        }

        private AccountCase AccountCaseMapper(IDataReader reader)
        {
            AccountCase accountCase = new AccountCase();
            accountCase.AccountNumber = reader["AccountNumber"] as string;
            accountCase.LogDate = (DateTime) reader["LogDate"] ;
            accountCase.StatusID = (int)reader["StatusId"];
            accountCase.StatusName = reader["StatusName"] as string;
            accountCase.StrategyId =  GetReaderValue<int?>(reader,"StrategyId");
            accountCase.SuspicionLevelID = GetReaderValue<int?>(reader, "SuspicionLevelID");
            accountCase.StrategyName = reader["StrategyName"] as string;
            accountCase.SuspicionLevelName = reader["SuspicionLevelName"] as string;
            accountCase.UserId = GetReaderValue<int?>(reader, "UserId");
            accountCase.ValidTill = GetReaderValue<DateTime?>(reader, "ValidTill");

            return accountCase;
        }
        */

        #endregion
    }
}
