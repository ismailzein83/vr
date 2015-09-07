using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Vanrise.Data.MySQL;

namespace Vanrise.Fzero.FraudAnalysis.Data.MySQL
{
    public class AccountCaseDataManager : BaseMySQLDataManager, IAccountCaseDataManager
    {
        public AccountCaseDataManager()
            : base("CDRDBConnectionStringMySQL")
        {

        }

        public bool UpdateAccountCase(string accountNumber, Entities.CaseStatus caseStatus, DateTime? validTill)
        {
            throw new NotImplementedException();
        }

        public Entities.AccountCase GetLastAccountCaseByAccountNumber(string accountNumber)
        {
            throw new NotImplementedException();
        }

        public bool InsertAccountCase(out int insertedID, string accountNumber, int userID, DateTime? validTill)
        {
            throw new NotImplementedException();
        }

        public bool UpdateAccountCaseStatus(int caseID, Entities.CaseStatus statusID, DateTime? validTill)
        {
            throw new NotImplementedException();
        }

        public bool InsertAccountCaseHistory(int caseID, int userID, Entities.CaseStatus caseStatus)
        {
            throw new NotImplementedException();
        }

        public bool InsertOrUpdateAccountStatus(string accountNumber, Entities.CaseStatus caseStatus)
        {
            throw new NotImplementedException();
        }

        public bool LinkDetailToCase(string accountNumber, int caseID, Entities.CaseStatus caseStatus)
        {
            throw new NotImplementedException();
        }
    }
}
