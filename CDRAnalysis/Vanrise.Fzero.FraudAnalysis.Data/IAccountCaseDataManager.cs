﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Vanrise.Entities;
using Vanrise.Fzero.FraudAnalysis.Entities;

namespace Vanrise.Fzero.FraudAnalysis.Data
{
    public interface IAccountCaseDataManager:IDataManager
    {
        BigResult<AccountSuspicionSummary> GetFilteredAccountSuspicionSummaries(Vanrise.Entities.DataRetrievalInput<AccountSuspicionSummaryQuery> input);
        BigResult<AccountCase> GetFilteredCasesByAccountNumber(Vanrise.Entities.DataRetrievalInput<AccountCaseQuery> input);
        BigResult<AccountCase> GetFilteredCasesByFilters(Vanrise.Entities.DataRetrievalInput<CancelAccountCasesQuery> input);
        AccountSuspicionSummary GetAccountSuspicionSummaryByCaseId(int caseID);
        AccountCase GetAccountCase(int caseID);
        AccountCase GetLastAccountCaseByAccountNumber(string accountNumber);
        bool InsertAccountCase(out int insertedID, string accountNumber, int? userID, CaseStatus caseStatus, DateTime? validTill, string reason);
        bool UpdateAccountCase(int caseID, int userID, CaseStatus statusID, DateTime? validTill, string reason);
        List<int> DeleteAccountCases_ByCaseIDs(List<int> caseIDs);
    }
}
