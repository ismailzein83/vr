using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Vanrise.Fzero.FraudAnalysis.Data;
using Vanrise.Fzero.FraudAnalysis.Entities;

namespace Vanrise.Fzero.FraudAnalysis.Business
{
    public class AccountStatusManager
    {
        public List<string> GetAccountNumbersByNumberPrefixAndStatuses(List<CaseStatusEnum> caseStatuses, List<string> numberPrefixes)
        {
            IAccountStatusDataManager dataManager = FraudDataManagerFactory.GetDataManager<IAccountStatusDataManager>();
            return dataManager.GetAccountNumbersByNumberPrefixAndStatuses(caseStatuses, numberPrefixes);
        }
        public bool InsertOrUpdateAccountStatus(string accountNumber, CaseStatusEnum caseStatus, DateTime? validTill)
        {
            IAccountStatusDataManager dataManager = FraudDataManagerFactory.GetDataManager<IAccountStatusDataManager>();
            return dataManager.InsertOrUpdateAccountStatus(accountNumber, caseStatus, validTill);
        }
    }
}
