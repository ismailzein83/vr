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
        public AccountCaseDataManager()
            : base("CDRDBConnectionString")
        {

        }

        public bool UpdateAccountCase(string accountNumber, CaseStatus caseStatus, DateTime? validTill)
        {
            int userID = Vanrise.Security.Business.SecurityContext.Current.GetLoggedInUserId();

            IAccountCaseDataManager dataManager = FraudDataManagerFactory.GetDataManager<IAccountCaseDataManager>();
            AccountCase1 accountCase = dataManager.GetLastAccountCaseByAccountNumber(accountNumber);

            int caseID;
            bool operationSucceeded;

            if (accountCase == null || (accountCase.StatusID == CaseStatus.ClosedFraud) || (accountCase.StatusID == CaseStatus.ClosedWhiteList))
                operationSucceeded = dataManager.InsertAccountCase(out caseID, accountNumber, userID, validTill);
            else
            {
                caseID = accountCase.CaseID;
                operationSucceeded = dataManager.UpdateAccountCaseStatus(accountCase.CaseID, caseStatus, validTill);
            }

            if (!operationSucceeded) return false;

            operationSucceeded = dataManager.InsertAccountCaseHistory(caseID, userID, caseStatus);

            if (!operationSucceeded) return false;

            operationSucceeded = dataManager.InsertOrUpdateAccountStatus(accountNumber, caseStatus);

            if (!operationSucceeded) return false;

            return dataManager.UpdateFraudResultCase(accountNumber, caseID, caseStatus);
        }

        AccountCase1 IAccountCaseDataManager.GetLastAccountCaseByAccountNumber(string accountNumber)
        {
            return GetItemSP("FraudAnalysis.sp_AccountCase1_GetLastByAccountNumber", AccountCase1Mapper, accountNumber);
        }

        bool IAccountCaseDataManager.InsertAccountCase(out int insertedID, string accountNumber, int userID, DateTime? validTill)
        {
            object accountCaseID;

            int recordsAffected = ExecuteNonQuerySP("FraudAnalysis.sp_AccountCase1_Insert", out accountCaseID, accountNumber, userID, validTill);

            insertedID = (recordsAffected > 0) ? (int)accountCaseID : -1;

            return (recordsAffected > 0);
        }

        bool IAccountCaseDataManager.UpdateAccountCaseStatus(int caseID, CaseStatus statusID, DateTime? validTill)
        {
            int recordsAffected = ExecuteNonQuerySP("FraudAnalysis.sp_AccountCase1_Update", caseID, statusID, validTill);
            return (recordsAffected > 0);
        }

        bool IAccountCaseDataManager.InsertAccountCaseHistory(int caseID, int userID, CaseStatus caseStatus)
        {
            int recordsAffected = ExecuteNonQuerySP("FraudAnalysis.sp_AccountCaseHistory_Insert", caseID, userID, caseStatus);
            return (recordsAffected > 0);
        }

        bool IAccountCaseDataManager.InsertOrUpdateAccountStatus(string accountNumber, CaseStatus caseStatus)
        {
            int recordsAffected = ExecuteNonQuerySP("FraudAnalysis.sp_AccountStatus_InsertOrUpdate", accountNumber, caseStatus);
            return (recordsAffected > 0);
        }

        bool IAccountCaseDataManager.UpdateFraudResultCase(string accountNumber, int caseID, CaseStatus caseStatus)
        {
            SuspicionOccuranceStatus occuranceStatus = (caseStatus.CompareTo(CaseStatus.Open) == 0 || caseStatus.CompareTo(CaseStatus.Pending) == 0) ? SuspicionOccuranceStatus.Open : SuspicionOccuranceStatus.Closed;

            int recordsAffected = ExecuteNonQuerySP("FraudAnalysis.sp_FraudResult_SetStatusToCaseStatus", accountNumber, caseID, occuranceStatus);
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
