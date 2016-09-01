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

        public BigResult<AccountCase> GetFilteredCasesByAccountNumber(Vanrise.Entities.DataRetrievalInput<AccountCaseQuery> input)
        {

            Action<string> createTempTableAction = (tempTableName) =>
            {
                ExecuteNonQuerySP("FraudAnalysis.sp_AccountCase_CreateTempByAccountNumber", tempTableName, input.Query.AccountNumber);
            };

            return RetrieveData(input, createTempTableAction, AccountCaseMapper, _columnMapper);
        }

        public AccountCase GetAccountCase(int caseID)
        {
            return GetItemSP("FraudAnalysis.sp_AccountCase_GetByCaseID", AccountCaseMapper, caseID);
        }

        public AccountCase GetLastAccountCaseByAccountNumber(string accountNumber)
        {
            return GetItemSP("FraudAnalysis.sp_AccountCase_GetLastByAccountNumber", AccountCaseMapper, accountNumber);
        }

        public bool InsertAccountCase(AccountCase accountCaseObject)
        {
           int recordsAffected = ExecuteNonQuerySP("FraudAnalysis.sp_AccountCase_Insert", accountCaseObject.CaseID, accountCaseObject.AccountNumber, accountCaseObject.UserID, accountCaseObject.StatusID, accountCaseObject.ValidTill, accountCaseObject.Reason);

            return (recordsAffected > 0);
        }

        public bool UpdateAccountCase(int caseID, int userID, CaseStatus statusID, DateTime? validTill, string reason)
        {
            int recordsAffected = ExecuteNonQuerySP("FraudAnalysis.sp_AccountCase_Update", caseID, userID, statusID, validTill, reason);
            return (recordsAffected > 0);
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

        public void SavetoDB(List<AccountCase> records)
        {
            string[] s_Columns = new string[] {
                "ID",
                "AccountNumber"	,
                "UserID"	,
                "Status"	,
                "StatusUpdatedTime"	,
                "ValidTill"	,
                "CreatedTime"	,
                "Reason"
        };


            StreamForBulkInsert stream = InitializeStreamForBulkInsert();

            foreach (AccountCase record in records)
            {
                stream.WriteRecord("{0}^{1}^{2}^{3}^{4}^{5}^{6}^{7}",record.CaseID, record.AccountNumber, record.UserID, (int)record.StatusID, record.StatusUpdatedTime, record.ValidTill, record.CreatedTime, record.Reason);
            }

            stream.Close();

            InsertBulkToTable(
                new StreamBulkInsertInfo
                {
                    TableName = "[FraudAnalysis].[AccountCase]",
                    Stream = stream,
                    TabLock = true,
                    ColumnNames = s_Columns,
                    KeepIdentity = false ,
                    FieldSeparator = '^'
                });
        }

        #endregion

        #region Private Methods
        private DataTable GetAccountCaseTable()
        {
            DataTable dt = new DataTable("FraudAnalysis.AccountCase");
            dt.Columns.Add("ID", typeof(int));
            dt.Columns.Add("UserID", typeof(int));
            dt.Columns.Add("Status", typeof(int));
            return dt;
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

        #endregion

    }
}
