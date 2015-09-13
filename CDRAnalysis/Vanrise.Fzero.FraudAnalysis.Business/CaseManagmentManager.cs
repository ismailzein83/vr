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
            bool updated = manager.UpdateAccountCase(input.AccountNumber, input.CaseStatus, input.ValidTill);

            if (updated)
            {
                updateOperationOutput.Result = Vanrise.Entities.UpdateOperationResult.Succeeded;

                ICaseManagementDataManager dataManager = FraudDataManagerFactory.GetDataManager<ICaseManagementDataManager>();
                updateOperationOutput.UpdatedObject = dataManager.GetAccountSuspicionSummaryByAccountNumber(input.AccountNumber, input.FromDate, input.ToDate);
            }

            return updateOperationOutput;
        }

        public Vanrise.Entities.IDataRetrievalResult<DailyVolumeLoose> GetDailyVolumeLooses(Vanrise.Entities.DataRetrievalInput<DashboardResultQuery> input)
        {
            ICaseManagementDataManager manager = FraudDataManagerFactory.GetDataManager<ICaseManagementDataManager>();
            return Vanrise.Common.DataRetrievalManager.Instance.ProcessResult(input, manager.GetDailyVolumeLooses(input));
        }

        public Vanrise.Entities.IDataRetrievalResult<CasesSummary> GetCasesSummary(Vanrise.Entities.DataRetrievalInput<DashboardResultQuery> input)
        {
            ICaseManagementDataManager manager = FraudDataManagerFactory.GetDataManager<ICaseManagementDataManager>();
            return Vanrise.Common.DataRetrievalManager.Instance.ProcessResult(input, manager.GetCasesSummary(input));
        }

        public IEnumerable<StrategyCases> GetStrategyCases(DateTime fromDate, DateTime toDate)
        {
            ICaseManagementDataManager manager = FraudDataManagerFactory.GetDataManager<ICaseManagementDataManager>();
            return manager.GetStrategyCases(fromDate, toDate);
        }

        public Vanrise.Entities.IDataRetrievalResult<BTSCases> GetBTSCases(Vanrise.Entities.DataRetrievalInput<DashboardResultQuery> input)
        {
            ICaseManagementDataManager manager = FraudDataManagerFactory.GetDataManager<ICaseManagementDataManager>();
            return Vanrise.Common.DataRetrievalManager.Instance.ProcessResult(input, manager.GetBTSCases(input));
        }

        public Vanrise.Entities.IDataRetrievalResult<BTSHighValueCases> GetTop10BTSHighValue(Vanrise.Entities.DataRetrievalInput<DashboardResultQuery> input)
        {
            ICaseManagementDataManager manager = FraudDataManagerFactory.GetDataManager<ICaseManagementDataManager>();
            return Vanrise.Common.DataRetrievalManager.Instance.ProcessResult(input, manager.GetTop10BTSHighValue(input));
        }


        public Vanrise.Entities.UpdateOperationOutput<AccountCase> CancelAccountCases(CancelAccountCasesResultQuery input)
        {
            Vanrise.Entities.UpdateOperationOutput<AccountCase> updateOperationOutput = new Vanrise.Entities.UpdateOperationOutput<AccountCase>();

            updateOperationOutput.Result = Vanrise.Entities.UpdateOperationResult.Failed;
            updateOperationOutput.UpdatedObject = null;

            ICaseManagementDataManager dataManager = FraudDataManagerFactory.GetDataManager<ICaseManagementDataManager>();
            bool updated = dataManager.CancelAccountCases(input.StrategyID, input.AccountNumber, input.From, input.To);

            if (updated)
            {
                updateOperationOutput.Result = Vanrise.Entities.UpdateOperationResult.Succeeded;
            }

            return updateOperationOutput;
        }

        public bool UpdateAccountCase(string accountNumber, CaseStatus caseStatus, DateTime? validTill)
        {
            ICaseManagementDataManager dataManager = FraudDataManagerFactory.GetDataManager<ICaseManagementDataManager>();

            int userID = Vanrise.Security.Business.SecurityContext.Current.GetLoggedInUserId();
            int caseID;
            bool succeeded;

            AccountCase accountCase = dataManager.GetLastAccountCaseByAccountNumber(accountNumber);

            if (accountCase == null || (accountCase.StatusID == CaseStatus.ClosedFraud) || (accountCase.StatusID == CaseStatus.ClosedWhiteList))
                succeeded = dataManager.InsertAccountCase(out caseID, accountNumber, userID, caseStatus, validTill);
            else
            {
                caseID = accountCase.CaseID;
                succeeded = dataManager.UpdateAccountCaseStatus(accountCase.CaseID, userID, caseStatus, validTill);
            }

            if (!succeeded) return false;

            succeeded = dataManager.InsertAccountCaseHistory(caseID, userID, caseStatus);

            if (!succeeded) return false;

            succeeded = dataManager.InsertOrUpdateAccountStatus(accountNumber, caseStatus);

            if (!succeeded) return false;

            return dataManager.LinkDetailToCase(accountNumber, caseID, caseStatus);
        }
        
        public bool AssignAccountCase(string accountNumber , HashSet<string> imeis )
        {
            ICaseManagementDataManager dataManager = FraudDataManagerFactory.GetDataManager<ICaseManagementDataManager>();

            AccountCase accountCase = dataManager.GetLastAccountCaseByAccountNumber(accountNumber);
            int caseID;
            bool succeeded = false;

            if (accountCase == null || (accountCase.StatusID == CaseStatus.ClosedFraud) || (accountCase.StatusID == CaseStatus.ClosedWhiteList))
            {
                succeeded = dataManager.InsertAccountCase(out caseID, accountNumber, null, CaseStatus.Open, null);

                if (!succeeded) return false;

                succeeded = dataManager.InsertAccountCaseHistory(caseID, null, CaseStatus.Open);

                if (!succeeded) return false;

                succeeded = dataManager.InsertOrUpdateAccountStatus(accountNumber, CaseStatus.Open, new AccountInfo() { IMEIs = imeis });
            }
            else
            {
                caseID = accountCase.CaseID;
                succeeded = true;
            }

            if (!succeeded) return false;

            return dataManager.LinkDetailToCase(accountNumber, caseID, CaseStatus.Open);
        }
    }
}
