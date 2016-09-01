using System.Collections.Generic;
using Vanrise.Fzero.FraudAnalysis.Data;
using Vanrise.Fzero.FraudAnalysis.Entities;

namespace Vanrise.Fzero.FraudAnalysis.Business
{
    public class AccountCaseManager
    {
        IAccountCaseDataManager accountCaseDataManager;
        IAccountSuspicionSummaryDataManager accountSuspicionSummaryDataManager;

        public AccountCaseManager()
        {
            accountCaseDataManager = FraudDataManagerFactory.GetDataManager<IAccountCaseDataManager>();
            accountSuspicionSummaryDataManager = FraudDataManagerFactory.GetDataManager<IAccountSuspicionSummaryDataManager>();
        }
        public AccountCase GetLastAccountCase(string accountNumber)
        {
            return accountCaseDataManager.GetLastAccountCaseByAccountNumber(accountNumber);
        }
        public Vanrise.Entities.IDataRetrievalResult<AccountSuspicionSummary> GetFilteredAccountSuspicionSummaries(Vanrise.Entities.DataRetrievalInput<AccountSuspicionSummaryQuery> input)
        {
            return Vanrise.Common.DataRetrievalManager.Instance.ProcessResult(input, accountSuspicionSummaryDataManager.GetFilteredAccountSuspicionSummaries(input));
        }
        public Vanrise.Entities.IDataRetrievalResult<AccountCase> GetFilteredCasesByAccountNumber(Vanrise.Entities.DataRetrievalInput<AccountCaseQuery> input)
        {
            return Vanrise.Common.DataRetrievalManager.Instance.ProcessResult(input, accountCaseDataManager.GetFilteredCasesByAccountNumber(input));
        }

        public AccountCase GetAccountCase(int caseID)
        {
            return accountCaseDataManager.GetAccountCase(caseID);
        }

        public Vanrise.Entities.UpdateOperationOutput<AccountSuspicionSummary> UpdateAccountCase(AccountCaseUpdateQuery input)
        {
            Vanrise.Entities.UpdateOperationOutput<AccountSuspicionSummary> updateOperationOutput = new Vanrise.Entities.UpdateOperationOutput<AccountSuspicionSummary>();

            int userID = Vanrise.Security.Business.SecurityContext.Current.GetLoggedInUserId();
            int caseID;

            AccountCase accountCase = accountCaseDataManager.GetLastAccountCaseByAccountNumber(input.AccountNumber);

            if (accountCase == null || (accountCase.StatusID == CaseStatus.ClosedFraud) || (accountCase.StatusID == CaseStatus.ClosedWhiteList))
            {
                long newCaseId;
                Vanrise.Common.Business.IDManager.Instance.ReserveIDRange(typeof(AccountCase), 1, out newCaseId);
                caseID = (int)newCaseId;
                accountCaseDataManager.InsertAccountCase(new AccountCase() { CaseID = caseID, AccountNumber = input.AccountNumber, UserID = userID, StatusID = input.CaseStatus, ValidTill = input.ValidTill, Reason = input.Reason });
            }
            else
            {
                caseID = accountCase.CaseID;
                accountCaseDataManager.UpdateAccountCase(caseID, userID, input.CaseStatus, input.ValidTill, input.Reason);
            }
            AccountCaseHistoryManager accountCaseHistoryManager = new Business.AccountCaseHistoryManager();
            accountCaseHistoryManager.InsertAccountCaseHistory(caseID, userID, input.CaseStatus, input.Reason);
            AccountStatusManager accountStatusManager = new AccountStatusManager();
            if (input.CaseStatus == CaseStatus.ClosedFraud || input.CaseStatus == CaseStatus.ClosedWhiteList)
                accountStatusManager.InsertOrUpdateAccountStatus(input.AccountNumber, input.CaseStatus, input.ValidTill, input.Reason, userID);

            StrategyExecutionItemManager strategyExecutionItemManager = new StrategyExecutionItemManager();
            strategyExecutionItemManager.LinkItemToCase(input.AccountNumber, caseID, input.CaseStatus);

            updateOperationOutput.Result = Vanrise.Entities.UpdateOperationResult.Succeeded;

            updateOperationOutput.UpdatedObject = accountSuspicionSummaryDataManager.GetAccountSuspicionSummaryByCaseId(caseID);

            return updateOperationOutput;
        }

    }
}
