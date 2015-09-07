using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Vanrise.Fzero.FraudAnalysis.Entities;

namespace Vanrise.Fzero.FraudAnalysis.Data
{
    public interface IAccountCaseDataManager : IDataManager
    {
        bool UpdateAccountCase(string accountNumber, CaseStatus caseStatus, DateTime? validTill);
        AccountCase1 GetLastAccountCaseByAccountNumber(string accountNumber);
        bool InsertAccountCase(out int insertedID, string accountNumber, int userID, DateTime? validTill);
        bool UpdateAccountCaseStatus(int caseID, CaseStatus statusID, DateTime? validTill);
        bool InsertAccountCaseHistory(int caseID, int userID, CaseStatus caseStatus);
        bool InsertOrUpdateAccountStatus(string accountNumber, CaseStatus caseStatus);
        bool UpdateFraudResultCase(string accountNumber, int caseID, CaseStatus caseStatus);
    }
}
