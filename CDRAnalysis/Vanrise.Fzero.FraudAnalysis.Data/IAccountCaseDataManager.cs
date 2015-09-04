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
        AccountCase1 GetLastAccountCaseByAccountNumber(string accountNumber);
        bool InsertAccountCase(out int insertedID, string accountNumber, int userID, DateTime? validTill);
        bool UpdateAccountCaseStatus(int caseID, CaseStatus statusID, DateTime? validTill);
        bool InsertAccountCaseHistory(int caseID, int userID, CaseStatus CaseStatus);
        bool InsertOrUpdateAccountStatus(string AccountNumber, CaseStatus CaseStatus);
        bool SetStatusToCaseStatus(string accountNumber, CaseStatus caseStatus);
    }
}
