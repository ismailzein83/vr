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

        public bool UpdateAccountCaseStatus(caseID, acco)
        {
            int recordsAffected = ExecuteNonQuerySP("FraudAnalysis.sp_AccountCase1_Insert", accountCaseObject.CaseID, accountCaseObject.StatusID, accountCaseObject.ValidTill);

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
