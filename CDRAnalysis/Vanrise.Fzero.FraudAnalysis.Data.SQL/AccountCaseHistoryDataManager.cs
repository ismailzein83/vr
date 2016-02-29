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
            _columnMapper.Add("AccountCaseHistoryId", "ID");
            _columnMapper.Add("UserName", "UserID");
            _columnMapper.Add("StatusDescription", "Status");
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
                                record.CaseId,
                                record.Reason,
                                (int)record.Status,
                                record.StatusTime,
                                record.UserId.Value
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
        public BigResult<AccountCaseHistory> GetFilteredAccountCaseHistoryByCaseID(Vanrise.Entities.DataRetrievalInput<AccountCaseHistoryQuery> input)
        {


            Action<string> createTempTableAction = (tempTableName) =>
            {
                ExecuteNonQuerySP("FraudAnalysis.sp_AccountCaseHistory_CreateTempByCaseID", tempTableName, input.Query.CaseId);
            };

            return RetrieveData(input, createTempTableAction, AccountCaseHistoryMapper, _columnMapper);
        }
        #endregion

        #region Mappers
        private AccountCaseHistory AccountCaseHistoryMapper(IDataReader reader)
        {
            AccountCaseHistory history = new AccountCaseHistory();
            history.AccountCaseHistoryId = (int)reader["ID"];
            history.CaseId = GetReaderValue<int>(reader, "CaseID");
            history.UserId = GetReaderValue<int?>(reader, "UserID");
            history.Status = (CaseStatus)reader["Status"];
            history.StatusTime = (DateTime)reader["StatusTime"];
            history.Reason = GetReaderValue<string>(reader, "Reason");
            return history;
        }
        #endregion

    }
}
