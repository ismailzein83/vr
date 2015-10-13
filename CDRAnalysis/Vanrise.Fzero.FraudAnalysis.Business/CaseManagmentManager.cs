using System;
using System.Collections.Generic;
using Vanrise.Fzero.FraudAnalysis.Data;
using Vanrise.Fzero.FraudAnalysis.Entities;
using Vanrise.Fzero.FraudAnalysis.Entities.ResultQuery;

namespace Vanrise.Fzero.FraudAnalysis.Business
{
    public class CaseManagmentManager
    {
        public Vanrise.Entities.IDataRetrievalResult<AccountSuspicionSummary> GetFilteredAccountSuspicionSummaries(Vanrise.Entities.DataRetrievalInput<AccountSuspicionSummaryQuery> input)
        {
            ICaseManagementDataManager dataManager = FraudDataManagerFactory.GetDataManager<ICaseManagementDataManager>();
            return Vanrise.Common.DataRetrievalManager.Instance.ProcessResult(input, dataManager.GetFilteredAccountSuspicionSummaries(input));
        }

        public Vanrise.Entities.IDataRetrievalResult<AccountSuspicionDetail> GetFilteredAccountSuspicionDetails(Vanrise.Entities.DataRetrievalInput<AccountSuspicionDetailQuery> input)
        {
            ICaseManagementDataManager dataManager = FraudDataManagerFactory.GetDataManager<ICaseManagementDataManager>();
            return Vanrise.Common.DataRetrievalManager.Instance.ProcessResult(input, dataManager.GetFilteredAccountSuspicionDetails(input));
        }

        public Vanrise.Entities.IDataRetrievalResult<AccountCase> GetFilteredCasesByAccountNumber(Vanrise.Entities.DataRetrievalInput<AccountCaseResultQuery> input)
        {
            ICaseManagementDataManager dataManager = FraudDataManagerFactory.GetDataManager<ICaseManagementDataManager>();
            return Vanrise.Common.DataRetrievalManager.Instance.ProcessResult(input, dataManager.GetFilteredCasesByAccountNumber(input));
        }

        public Vanrise.Entities.IDataRetrievalResult<AccountCase> GetFilteredCasesByFilters(Vanrise.Entities.DataRetrievalInput<CancelAccountCasesResultQuery> input)
        {
            ICaseManagementDataManager dataManager = FraudDataManagerFactory.GetDataManager<ICaseManagementDataManager>();
            return Vanrise.Common.DataRetrievalManager.Instance.ProcessResult(input, dataManager.GetFilteredCasesByFilters(input));
        }

        public Vanrise.Entities.IDataRetrievalResult<AccountSuspicionDetail> GetFilteredDetailsByCaseID(Vanrise.Entities.DataRetrievalInput<CaseDetailQuery> input)
        {
            ICaseManagementDataManager dataManager = FraudDataManagerFactory.GetDataManager<ICaseManagementDataManager>();
            return Vanrise.Common.DataRetrievalManager.Instance.ProcessResult(input, dataManager.GetFilteredDetailsByCaseID(input));
        }

        public List<RelatedNumber> GetRelatedNumbersByAccountNumber(string accountNumber)
        {
            ICaseManagementDataManager dataManager = FraudDataManagerFactory.GetDataManager<ICaseManagementDataManager>();
            return dataManager.GetRelatedNumbersByAccountNumber(accountNumber);
        }

        public CaseStatus? GetAccountStatus(string accountNumber)
        {
            ICaseManagementDataManager dataManager = FraudDataManagerFactory.GetDataManager<ICaseManagementDataManager>();
            return dataManager.GetAccountStatus(accountNumber);
        }

        public Vanrise.Entities.IDataRetrievalResult<AccountCaseLog> GetFilteredAccountCaseLogsByCaseID(Vanrise.Entities.DataRetrievalInput<AccountCaseLogResultQuery> input)
        {
            ICaseManagementDataManager dataManager = FraudDataManagerFactory.GetDataManager<ICaseManagementDataManager>();
            return Vanrise.Common.DataRetrievalManager.Instance.ProcessResult(input, dataManager.GetFilteredAccountCaseLogsByCaseID(input));
        }

        public Vanrise.Entities.UpdateOperationOutput<AccountSuspicionSummary> UpdateAccountCase(AccountCaseUpdateResultQuery input)
        {
            Vanrise.Entities.UpdateOperationOutput<AccountSuspicionSummary> updateOperationOutput = new Vanrise.Entities.UpdateOperationOutput<AccountSuspicionSummary>();

            updateOperationOutput.Result = Vanrise.Entities.UpdateOperationResult.Failed;
            updateOperationOutput.UpdatedObject = null;

            CaseManagmentManager manager = new CaseManagmentManager();
            bool updated = manager.UpdateAccountCase(input.AccountNumber, input.CaseStatus, input.ValidTill, input.Reason);

            if (updated)
            {
                updateOperationOutput.Result = Vanrise.Entities.UpdateOperationResult.Succeeded;

                ICaseManagementDataManager dataManager = FraudDataManagerFactory.GetDataManager<ICaseManagementDataManager>();

                updateOperationOutput.UpdatedObject = dataManager.GetAccountSuspicionSummaryByAccountNumber(input.AccountNumber, input.FromDate, input.ToDate);
            }

            return updateOperationOutput;
        }

        public Vanrise.Entities.UpdateOperationOutput<AccountCase> CancelAccountCases(CancelAccountCasesResultQuery input)
        {
            Vanrise.Entities.UpdateOperationOutput<AccountCase> updateOperationOutput = new Vanrise.Entities.UpdateOperationOutput<AccountCase>();

            updateOperationOutput.Result = Vanrise.Entities.UpdateOperationResult.Failed;
            updateOperationOutput.UpdatedObject = null;

            IStrategyExecutionDataManager strategyExecutionDataManager = FraudDataManagerFactory.GetDataManager<IStrategyExecutionDataManager>();
            ICaseManagementDataManager caseManagementDataManager = FraudDataManagerFactory.GetDataManager<ICaseManagementDataManager>();

            List<int> CaseIDs = strategyExecutionDataManager.GetCasesIDsofStrategyExecutionDetails(input.AccountNumber, input.From, input.To, input.StrategyIDs);

            if (CaseIDs != null && CaseIDs.Count > 0)
            {
                strategyExecutionDataManager.DeleteStrategyExecutionDetails_ByFilters(input.AccountNumber, input.From, input.To, input.StrategyIDs);

                caseManagementDataManager.DeleteAccountCases_ByCaseIDs(CaseIDs);
            }


            updateOperationOutput.Result = Vanrise.Entities.UpdateOperationResult.Succeeded;
            return updateOperationOutput;
        }


        public Vanrise.Entities.UpdateOperationOutput<AccountCase> CancelSelectedAccountCases(List<int> CaseIDs)
        {
            Vanrise.Entities.UpdateOperationOutput<AccountCase> updateOperationOutput = new Vanrise.Entities.UpdateOperationOutput<AccountCase>();

            updateOperationOutput.Result = Vanrise.Entities.UpdateOperationResult.Failed;
            updateOperationOutput.UpdatedObject = null;

            IStrategyExecutionDataManager strategyExecutionDataManager = FraudDataManagerFactory.GetDataManager<IStrategyExecutionDataManager>();
            ICaseManagementDataManager caseManagementDataManager = FraudDataManagerFactory.GetDataManager<ICaseManagementDataManager>();
            strategyExecutionDataManager.DeleteStrategyExecutionDetails_ByCaseIDs(CaseIDs);
            caseManagementDataManager.DeleteAccountCases_ByCaseIDs(CaseIDs);

            updateOperationOutput.Result = Vanrise.Entities.UpdateOperationResult.Succeeded;
            return updateOperationOutput;
        }

        public bool UpdateAccountCase(string accountNumber, CaseStatus caseStatus, DateTime? validTill, string reason)
        {
            ICaseManagementDataManager dataManager = FraudDataManagerFactory.GetDataManager<ICaseManagementDataManager>();

            int userID = Vanrise.Security.Business.SecurityContext.Current.GetLoggedInUserId();
            int caseID;

            AccountCase accountCase = dataManager.GetLastAccountCaseByAccountNumber(accountNumber);

            if (accountCase == null || (accountCase.StatusID == CaseStatus.ClosedFraud) || (accountCase.StatusID == CaseStatus.ClosedWhiteList))
                dataManager.InsertAccountCase(out caseID, accountNumber, userID, caseStatus, validTill, reason);
            else
            {
                caseID = accountCase.CaseID;
                dataManager.UpdateAccountCase(accountCase.CaseID, userID, caseStatus, validTill, reason);
            }

            dataManager.InsertAccountCaseHistory(caseID, userID, caseStatus, reason);

            dataManager.InsertOrUpdateAccountStatus(accountNumber, caseStatus);

            dataManager.LinkDetailToCase(accountNumber, caseID, caseStatus);

            return true;
        }

        public bool AssignAccountCase(string accountNumber, HashSet<string> imeis)
        {
            ICaseManagementDataManager dataManager = FraudDataManagerFactory.GetDataManager<ICaseManagementDataManager>();

            AccountCase accountCase = dataManager.GetLastAccountCaseByAccountNumber(accountNumber);
            int caseID;

            if (accountCase == null || (accountCase.StatusID == CaseStatus.ClosedFraud) || (accountCase.StatusID == CaseStatus.ClosedWhiteList))
            {
                dataManager.InsertAccountCase(out caseID, accountNumber, null, CaseStatus.Open, null, null);
                dataManager.InsertAccountCaseHistory(caseID, null, CaseStatus.Open, null);
                dataManager.InsertOrUpdateAccountStatus(accountNumber, CaseStatus.Open, new AccountInfo() { IMEIs = imeis });
            }
            else
            {
                caseID = accountCase.CaseID;
            }

            return dataManager.LinkDetailToCase(accountNumber, caseID, CaseStatus.Open);
        }
    }
}
