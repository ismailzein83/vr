using System;
using System.Collections.Generic;
using System.Data;
using Vanrise.Fzero.FraudAnalysis.Entities;

namespace Vanrise.Fzero.FraudAnalysis.Data
{
    public interface IAccountStatusDataManager : IDataManager
    {
        bool ApplyAccountStatuses(DataTable accountStatusDataTables, DateTime validTill);
        IEnumerable<AccountStatus> GetAccountStatusesData(Vanrise.Entities.DataRetrievalInput<AccountStatusQuery> input);
        bool Update(AccountStatus accountStatus);
        bool Insert(AccountStatus accountStatus);
        List<string> GetAccountNumbersByNumberPrefixAndStatuses(List<CaseStatus> caseStatuses, List<string> numberPrefixes);
        bool InsertOrUpdateAccountStatus(string accountNumber, CaseStatus caseStatus, DateTime? validTill);
        AccountStatus GetAccountStatus(string accountNumber);
    }
}
