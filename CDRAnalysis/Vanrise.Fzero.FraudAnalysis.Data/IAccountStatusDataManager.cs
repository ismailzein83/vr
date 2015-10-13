using System;
using System.Collections.Generic;
using Vanrise.Entities;
using Vanrise.Fzero.CDRImport.Entities;
using Vanrise.Fzero.FraudAnalysis.Entities;

namespace Vanrise.Fzero.FraudAnalysis.Data
{
    public interface IAccountStatusDataManager : IDataManager 
    {

        void LoadAccountStatus(Action<AccountStatus> onBatchReady, List<CaseStatus> caseStatuses);
        List<string> GetAccountStatusByFilters(List<CaseStatus> caseStatuses, string fromAccountNumber, int nbOfRecords);
    }
}
