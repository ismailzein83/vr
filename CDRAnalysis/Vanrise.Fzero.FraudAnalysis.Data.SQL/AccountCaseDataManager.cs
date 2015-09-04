using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Vanrise.Data.SQL;
using Vanrise.Fzero.FraudAnalysis.Entities;

namespace Vanrise.Fzero.FraudAnalysis.Data.SQL
{
    public class AccountCaseDataManager : BaseSQLDataManager, IAccountCaseDataManager
    {
        public AccountCase1 GetLastAccountCaseByAccountNumber(string accountNumber)
        {
            return GetItemSP("FraudAnalysis.sp_AccountCase1_GetLastByAccountNumber", AccountCase1Mapper, accountNumber);
        }

        public bool InsertAccountCase(out int insertedID, string accountNumber, int userID, DateTime? validTill)
        {
            object accountCaseID;

            int recordsAffected = ExecuteNonQuerySP("FraudAnalysis.sp_AccountCase1_Insert", out accountCaseID, accountNumber, userID, validTill);

            insertedID = (recordsAffected > 0) ? (int)accountCaseID : -1;

            return (recordsAffected > 0);
        }

        public bool UpdateAccountCaseStatus(int caseID, CaseStatus statusID, DateTime? validTill)
        {
            int recordsAffected = ExecuteNonQuerySP("FraudAnalysis.sp_AccountCase1_Update", caseID, statusID, validTill);
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

        public bool SetStatusToCaseStatus(string accountNumber, CaseStatus caseStatus)
        {
            int recordsAffected = ExecuteNonQuerySP("FraudAnalysis.sp_FraudResult_SetStatusToCaseStatus", accountNumber, caseStatus);
            return (recordsAffected > 0);
        }

        private AccountCase1 AccountCase1Mapper(IDataReader reader)
        {
            return new AccountCase1
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
    }
}
