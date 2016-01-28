using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Vanrise.Fzero.FraudAnalysis.Data;
using Vanrise.Fzero.FraudAnalysis.Entities;

namespace Vanrise.Fzero.FraudAnalysis.Business
{
    public class AccountCaseManager
    {
        IAccountCaseDataManager _dataManager;
        public AccountCaseManager()
        {
            _dataManager = FraudDataManagerFactory.GetDataManager<IAccountCaseDataManager>();
        }
        public AccountCase GetLastAccountCase(string accountNumber)
        {
            return _dataManager.GetLastAccountCaseByAccountNumber(accountNumber);
        }
        public Vanrise.Entities.IDataRetrievalResult<AccountSuspicionSummary> GetFilteredAccountSuspicionSummaries(Vanrise.Entities.DataRetrievalInput<AccountSuspicionSummaryQuery> input)
        {
            return Vanrise.Common.DataRetrievalManager.Instance.ProcessResult(input, _dataManager.GetFilteredAccountSuspicionSummaries(input));
        }
        public Vanrise.Entities.IDataRetrievalResult<AccountCase> GetFilteredCasesByAccountNumber(Vanrise.Entities.DataRetrievalInput<AccountCaseQuery> input)
        {
            return Vanrise.Common.DataRetrievalManager.Instance.ProcessResult(input, _dataManager.GetFilteredCasesByAccountNumber(input));
        }
        public Vanrise.Entities.IDataRetrievalResult<AccountCase> GetFilteredCasesByFilters(Vanrise.Entities.DataRetrievalInput<CancelAccountCasesQuery> input)
        {
            return Vanrise.Common.DataRetrievalManager.Instance.ProcessResult(input, _dataManager.GetFilteredCasesByFilters(input));
        }
        public AccountCase GetAccountCase(int caseID)
        {
            return _dataManager.GetAccountCase(caseID);
        }
        public Vanrise.Entities.UpdateOperationOutput<AccountCase> CancelAccountCases(CancelAccountCasesQuery input)
        {
            Vanrise.Entities.UpdateOperationOutput<AccountCase> updateOperationOutput = new Vanrise.Entities.UpdateOperationOutput<AccountCase>();

            updateOperationOutput.Result = Vanrise.Entities.UpdateOperationResult.Failed;
            updateOperationOutput.UpdatedObject = null;

            IStrategyExecutionDataManager strategyExecutionDataManager = FraudDataManagerFactory.GetDataManager<IStrategyExecutionDataManager>();

            List<int> CaseIDs = strategyExecutionDataManager.GetCasesIDsofStrategyExecutionItem(input.AccountNumber, input.From, input.To, input.StrategyIDs);

            if (CaseIDs != null && CaseIDs.Count > 0)
            {
                strategyExecutionDataManager.DeleteStrategyExecutionItem_ByFilters(input.AccountNumber, input.From, input.To, input.StrategyIDs);

                _dataManager.DeleteAccountCases_ByCaseIDs(CaseIDs);
            }


            updateOperationOutput.Result = Vanrise.Entities.UpdateOperationResult.Succeeded;
            return updateOperationOutput;
        }

        public Vanrise.Entities.UpdateOperationOutput<AccountSuspicionSummary> UpdateAccountCase(AccountCaseUpdateQuery input)
        {
            Vanrise.Entities.UpdateOperationOutput<AccountSuspicionSummary> updateOperationOutput = new Vanrise.Entities.UpdateOperationOutput<AccountSuspicionSummary>();

            int userID = Vanrise.Security.Business.SecurityContext.Current.GetLoggedInUserId();
            int caseID;

            AccountCase accountCase = _dataManager.GetLastAccountCaseByAccountNumber(input.AccountNumber);

            if (accountCase == null || (accountCase.StatusID == CaseStatus.ClosedFraud) || (accountCase.StatusID == CaseStatus.ClosedWhiteList))
                _dataManager.InsertAccountCase(out caseID, input.AccountNumber, userID, input.CaseStatus, input.ValidTill, input.Reason);
            else
            {
                caseID = accountCase.CaseID;
                _dataManager.UpdateAccountCase(caseID, userID, input.CaseStatus, input.ValidTill, input.Reason);
            }
                AccountCaseHistoryManager accountCaseHistoryManager = new Business.AccountCaseHistoryManager();
                accountCaseHistoryManager.InsertAccountCaseHistory(caseID, userID, input.CaseStatus, input.Reason);
                AccountStatusManager accountStatusManager = new AccountStatusManager();
            if (input.CaseStatus == CaseStatus.ClosedFraud || input.CaseStatus == CaseStatus.ClosedWhiteList)
                accountStatusManager.InsertOrUpdateAccountStatus(input.AccountNumber, input.CaseStatus, input.ValidTill);

            StrategyExecutionItemManager strategyExecutionItemManager = new StrategyExecutionItemManager();
            strategyExecutionItemManager.LinkDetailToCase(input.AccountNumber, caseID, input.CaseStatus);

            updateOperationOutput.Result = Vanrise.Entities.UpdateOperationResult.Succeeded;

            updateOperationOutput.UpdatedObject = _dataManager.GetAccountSuspicionSummaryByCaseId(caseID);

            return updateOperationOutput;
        }
        public Vanrise.Entities.UpdateOperationOutput<AccountCase> CancelSelectedAccountCases(List<int> CaseIDs)
        {
            Vanrise.Entities.UpdateOperationOutput<AccountCase> updateOperationOutput = new Vanrise.Entities.UpdateOperationOutput<AccountCase>();

            updateOperationOutput.Result = Vanrise.Entities.UpdateOperationResult.Failed;
            updateOperationOutput.UpdatedObject = null;
            IStrategyExecutionDataManager strategyExecutionDataManager = FraudDataManagerFactory.GetDataManager<IStrategyExecutionDataManager>();
            strategyExecutionDataManager.DeleteStrategyExecutionItem_ByCaseIDs(CaseIDs);
            _dataManager.DeleteAccountCases_ByCaseIDs(CaseIDs);

            updateOperationOutput.Result = Vanrise.Entities.UpdateOperationResult.Succeeded;
            return updateOperationOutput;
        }

        public bool AssignAccountCase(string accountNumber, HashSet<string> imeis)
        {
            AccountCaseHistoryManager accountCaseHistoryManager = new AccountCaseHistoryManager();
            AccountInfoManager accountInfoManager = new AccountInfoManager();
            StrategyExecutionItemManager strategyExecutionItemManager = new StrategyExecutionItemManager();
            AccountCase accountCase = _dataManager.GetLastAccountCaseByAccountNumber(accountNumber);
            int caseID;

            if (accountCase == null || (accountCase.StatusID == CaseStatus.ClosedFraud) || (accountCase.StatusID == CaseStatus.ClosedWhiteList))
            {
                _dataManager.InsertAccountCase(out caseID, accountNumber, null, CaseStatus.Open, null, null);
                accountCaseHistoryManager.InsertAccountCaseHistory(caseID, null, CaseStatus.Open, null);
                accountInfoManager.InsertOrUpdateAccountInfo(accountNumber, new InfoDetail() { IMEIs = imeis });
            }
            else
            {
                caseID = accountCase.CaseID;
            }

            return strategyExecutionItemManager.LinkDetailToCase(accountNumber, caseID, CaseStatus.Open);
        }
    }
}
