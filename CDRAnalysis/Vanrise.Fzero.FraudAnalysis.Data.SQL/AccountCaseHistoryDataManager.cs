using System;
using System.Collections.Generic;
using System.Data;
using Vanrise.Data.SQL;
using Vanrise.Entities;
using Vanrise.Fzero.FraudAnalysis.Entities;
using Vanrise.Fzero.FraudAnalysis.Entities.ResultQuery;

namespace Vanrise.Fzero.FraudAnalysis.Data.SQL
{
    public class AccountCaseHistoryDataManager : BaseSQLDataManager, IAccountCaseHistoryDataManager
    {

        #region ctor
        private static Dictionary<string, string> _columnMapper = new Dictionary<string, string>();
        public AccountCaseHistoryDataManager()
            : base("CDRDBConnectionString")
        {

        }

        static AccountCaseHistoryDataManager()
        {
            _columnMapper.Add("UserName", "UserID");
            _columnMapper.Add("AccountCaseStatusDescription", "AccountCaseStatusID");
        }
        #endregion

        #region Public Methods

        public void SavetoDB(List<AccountCaseHistory> records)
        {
            string[] s_Columns = new string[] {
            "ID"
            ,"CaseID"
            ,"Reason"
            ,"Status"
            ,"StatusTime"
            ,"UserID"
        };


            StreamForBulkInsert stream = InitializeStreamForBulkInsert();

            foreach (AccountCaseHistory record in records)
            {
                stream.WriteRecord("0^{0}^{1}^{2}^{3}^{4}",
                                record.CaseID,
                                record.Reason,
                                (int)record.Status,
                                record.StatusTime,
                                record.UserID.Value
                                );
            }

            stream.Close();

            InsertBulkToTable(
                new StreamBulkInsertInfo
                {
                    TableName = "[FraudAnalysis].[AccountCaseHistory]",
                    Stream = stream,
                    TabLock = true,
                    ColumnNames = s_Columns,
                    KeepIdentity = false,
                    FieldSeparator = '^'
                });
        }

        public bool InsertAccountCaseHistory(int caseID, int? userID, CaseStatus caseStatus, string reason)
        {
            int recordsAffected = ExecuteNonQuerySP("FraudAnalysis.sp_AccountCaseHistory_Insert", caseID, userID, caseStatus, reason);
            return (recordsAffected > 0);
        }
        public BigResult<AccountCaseLog> GetFilteredAccountCaseHistoryByCaseID(Vanrise.Entities.DataRetrievalInput<AccountCaseLogQuery> input)
        {


            Action<string> createTempTableAction = (tempTableName) =>
            {
                ExecuteNonQuerySP("FraudAnalysis.sp_AccountCaseHistory_CreateTempByCaseID", tempTableName, input.Query.CaseID);
            };

            return RetrieveData(input, createTempTableAction, AccountCaseLogMapper, _columnMapper);
        }
        #endregion

        #region Mappers
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
