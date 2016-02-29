using System;
using System.Activities;
using System.Collections.Generic;
using Vanrise.BusinessProcess;
using Vanrise.Common;
using Vanrise.Fzero.FraudAnalysis.Business;
using Vanrise.Fzero.FraudAnalysis.Data;
using Vanrise.Fzero.FraudAnalysis.Entities;
using Vanrise.Queueing;

namespace Vanrise.Fzero.FraudAnalysis.BP.Activities
{

    #region Arguments Classes

    public class ApplyAssigningCasesforItemsInput
    {
        public BaseQueue<AssignCasesforItemsBatch> InputQueue { get; set; }

        public int UserId { get; set; }
    }

    #endregion

    public class ApplyAssigningCasesforItems : DependentAsyncActivity<ApplyAssigningCasesforItemsInput>
    {

        #region Arguments

        [RequiredArgument]
        public InArgument<BaseQueue<AssignCasesforItemsBatch>> InputQueue { get; set; }

        public InArgument<int> UserId { get; set; }

        #endregion

        protected override void DoWork(ApplyAssigningCasesforItemsInput inputArgument, AsyncActivityStatus previousActivityStatus, AsyncActivityHandle handle)
        {

            handle.SharedInstanceData.WriteTrackingMessage(LogEntryType.Information, "Started Applying Assign Cases for Items");

            StrategyExecutionItemManager manager = new StrategyExecutionItemManager();

            DoWhilePreviousRunning(previousActivityStatus, handle, () =>
            {
                int index = 0;
                int totalIndex = 0;

                bool hasItem = false;
                do
                {

                    hasItem = inputArgument.InputQueue.TryDequeue(
                        (batch) =>
                        {

                            index++;
                            totalIndex++;
                            if (index == 1000)
                            {
                                Console.WriteLine("{0} Items Dequeued", totalIndex);
                                index = 0;
                            }

                            //Bulk Insert: Account Info
                            List<AccountInfo> accountInfos = new List<AccountInfo>();
                            foreach (var accountInfo in batch.TobeInsertedAccountInfos)
                                accountInfos.Add(new AccountInfo() { AccountNumber = accountInfo.AccountNumber, InfoDetail = accountInfo.InfoDetail });
                            IAccountInfoDataManager accountInfoDataManager = FraudDataManagerFactory.GetDataManager<IAccountInfoDataManager>();
                            accountInfoDataManager.SavetoDB(accountInfos);

                            //Bulk Update: Account Info
                            accountInfoDataManager.UpdateAccountInfoBatch(batch.TobeUpdatedAccountInfos);


                            //Bulk Insert: Account Case
                            long startingId;
                            Vanrise.Common.Business.IDManager.Instance.ReserveIDRange(typeof(AccountCase), batch.AccountNumbers.Count, out startingId);
                            List<AccountCase> accountCases = new List<AccountCase>();
                            foreach (var accountNumber in batch.AccountNumbers)
                                accountCases.Add(new AccountCase() { CaseID = (int)startingId++, AccountNumber = accountNumber, CreatedTime = DateTime.Now, StatusID = CaseStatus.Open, StatusUpdatedTime = DateTime.Now, UserID = inputArgument.UserId });
                            IAccountCaseDataManager accountCaseDataManager = FraudDataManagerFactory.GetDataManager<IAccountCaseDataManager>();
                            accountCaseDataManager.SavetoDB(accountCases);


                            //Bulk Insert: Account Case History
                            List<AccountCaseHistory> accountCaseHistories = new List<AccountCaseHistory>();
                            foreach (var accountCase in accountCases)
                                accountCaseHistories.Add(new AccountCaseHistory() { CaseId = accountCase.CaseID, Status = accountCase.StatusID, StatusTime = accountCase.StatusUpdatedTime, UserId = accountCase.UserID });
                            IAccountCaseHistoryDataManager accountCaseHistoryDataManager = FraudDataManagerFactory.GetDataManager<IAccountCaseHistoryDataManager>();
                            accountCaseHistoryDataManager.SavetoDB(accountCaseHistories);


                            foreach (var accountCase in accountCases)
                                if (!batch.AccountNumberCaseIds.ContainsKey(accountCase.AccountNumber))
                                    batch.AccountNumberCaseIds.Add(accountCase.AccountNumber, accountCase.CaseID);


                            //Bulk Update: Strategy Execution Item
                            IStrategyExecutionItemDataManager itemDataManager = FraudDataManagerFactory.GetDataManager<IStrategyExecutionItemDataManager>();
                            itemDataManager.UpdateStrategyExecutionItemBatch(batch.AccountNumberCaseIds);

                        });
                }
                while (!ShouldStop(handle) && hasItem);
            });

            handle.SharedInstanceData.WriteTrackingMessage(LogEntryType.Information, "Finished Applying Assign Cases for Items");
        }


        protected override ApplyAssigningCasesforItemsInput GetInputArgument2(System.Activities.AsyncCodeActivityContext context)
        {
            return new ApplyAssigningCasesforItemsInput
            {
                InputQueue = this.InputQueue.Get(context),
                UserId = this.UserId.Get(context)
            };
        }

    }
}
