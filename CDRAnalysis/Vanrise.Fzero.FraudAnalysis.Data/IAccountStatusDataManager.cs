using System;
using System.Collections.Generic;
using System.Data;
using Vanrise.Fzero.FraudAnalysis.Entities;

namespace Vanrise.Fzero.FraudAnalysis.Data
{
    public interface IAccountStatusDataManager : IDataManager
    {
        bool ApplyAccountStatuses(DataTable accountStatusDataTables, DateTime validTill, string reason, int userId);

        List<string> GetAccountNumbersByNumberPrefixAndStatuses(List<CaseStatus> caseStatuses, List<string> numberPrefixes);

        bool InsertOrUpdateAccountStatus(string accountNumber, CaseStatus caseStatus, DateTime? validTill, string reason, int userId);

        IEnumerable<AccountStatus> GetAccountStatusesData(Vanrise.Entities.DataRetrievalInput<AccountStatusQuery> input);

        bool Update(AccountStatus accountStatus);

        bool Insert(AccountStatus accountStatus);

        bool Delete(string number);

        AccountStatus GetAccountStatus(string accountNumber);

    }
}
