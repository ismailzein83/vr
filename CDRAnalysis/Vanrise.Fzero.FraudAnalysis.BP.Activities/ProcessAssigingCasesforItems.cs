using System;
using System.Activities;
using System.Collections.Generic;
using System.Linq;
using Vanrise.BusinessProcess;
using Vanrise.Common;
using Vanrise.Fzero.FraudAnalysis.Data;
using Vanrise.Fzero.FraudAnalysis.Entities;
using Vanrise.Queueing;

namespace Vanrise.Fzero.FraudAnalysis.BP.Activities
{
    #region Arguments Classes

    public class ProcessAssigingCasesforItemsInput
    {
        public long StrategyExecutionId { get; set; }

        public int UserId { get; set; }

        public BaseQueue<AssignCasesforItemsBatch> OutputQueue { get; set; }
    }

    #endregion

    public sealed class ProcessAssigingCasesforItems : BaseAsyncActivity<ProcessAssigingCasesforItemsInput>
    {
        #region Arguments

        [RequiredArgument]
        public InOutArgument<BaseQueue<AssignCasesforItemsBatch>> OutputQueue { get; set; }


        #endregion

        protected override void OnBeforeExecute(AsyncCodeActivityContext context, AsyncActivityHandle handle)
        {
            if (this.OutputQueue.Get(context) == null)
                this.OutputQueue.Set(context, new MemoryQueue<CancellingStrategyExecutionBatch>());

            base.OnBeforeExecute(context, handle);
        }


        protected override void DoWork(ProcessAssigingCasesforItemsInput inputArgument, AsyncActivityHandle handle)
        {
            handle.SharedInstanceData.WriteTrackingMessage(LogEntryType.Information, "Started Processing Assign Cases for Items");

            IStrategyExecutionItemDataManager dataManager = FraudDataManagerFactory.GetDataManager<IStrategyExecutionItemDataManager>();
            List<AccountInfo> itemInfos = new List<AccountInfo>();
            List<AccountCase> cases = new List<AccountCase>();
            List<AccountInfo> infos = new List<AccountInfo>();

            dataManager.GetStrategyExecutionItemsbyNULLCaseId(out itemInfos, out cases, out infos);

            List<string> accountNumbers = new List<string>();
            Dictionary<string, int> accountNumberCaseIds = new Dictionary<string, int>();
            List<AccountInfo> tobeInsertedAccountInfos = new List<AccountInfo>();
            List<AccountInfo> tobeUpdatedAccountInfos = new List<AccountInfo>();

            AccountCase lastCase = new AccountCase();
            AccountInfo currentAccountInfo = new AccountInfo();

            int index = 0;
            int totalIndex = 0;


            foreach (var item in itemInfos)
            {
                index++;
                totalIndex++;

                if (index == 1000)
                {
                    BuildandEnqueueBatch(inputArgument, handle, ref accountNumbers, ref accountNumberCaseIds, ref tobeInsertedAccountInfos, ref tobeUpdatedAccountInfos, ref index, totalIndex);
                }
                lastCase = cases.Where(x => x.AccountNumber == item.AccountNumber).OrderByDescending(x => x.CaseID).FirstOrDefault();
                if (lastCase == null || (lastCase.StatusID == CaseStatus.ClosedFraud) || (lastCase.StatusID == CaseStatus.ClosedWhiteList))
                {
                    accountNumbers.Add(item.AccountNumber);
                    currentAccountInfo = infos.Where(x => x.AccountNumber == item.AccountNumber).FirstOrDefault();
                    if (currentAccountInfo == null)
                        tobeInsertedAccountInfos.Add(new AccountInfo { AccountNumber = item.AccountNumber, InfoDetail = new InfoDetail { IMEIs = item.InfoDetail.IMEIs } });
                    else if (currentAccountInfo.InfoDetail.IMEIs.Except(item.InfoDetail.IMEIs).Count() > 0)//If IMEIs changed
                        tobeUpdatedAccountInfos.Add(new AccountInfo { AccountNumber = item.AccountNumber, InfoDetail = new InfoDetail { IMEIs = item.InfoDetail.IMEIs } });
                }
                else
                {
                    if (!accountNumberCaseIds.ContainsKey(item.AccountNumber))
                        accountNumberCaseIds.Add(item.AccountNumber, lastCase.CaseID);
                }

            }

            if (accountNumbers.Count() > 0 || accountNumberCaseIds.Count() > 0 || tobeInsertedAccountInfos.Count() > 0 || tobeUpdatedAccountInfos.Count() > 0)
            {
                BuildandEnqueueBatch(inputArgument, handle, ref accountNumbers, ref accountNumberCaseIds, ref tobeInsertedAccountInfos, ref tobeUpdatedAccountInfos, ref index, totalIndex);
            }


            handle.SharedInstanceData.WriteTrackingMessage(LogEntryType.Information, "Finished Processing Assign Cases for Items");

        }

        private static void BuildandEnqueueBatch(ProcessAssigingCasesforItemsInput inputArgument, AsyncActivityHandle handle, ref List<string> accountNumbers, ref   Dictionary<string, int> accountNumberCaseIds, ref   List<AccountInfo> tobeInsertedAccountInfos, ref      List<AccountInfo> tobeUpdatedAccountInfos, ref int index, int totalIndex)
        {
            inputArgument.OutputQueue.Enqueue(new AssignCasesforItemsBatch() { AccountNumberCaseIds = accountNumberCaseIds, AccountNumbers = accountNumbers, TobeInsertedAccountInfos = tobeInsertedAccountInfos, TobeUpdatedAccountInfos = tobeUpdatedAccountInfos });
            Console.WriteLine("{0} Items Processed", totalIndex);
            handle.SharedInstanceData.WriteTrackingMessage(LogEntryType.Information, "{0} Items Processed", totalIndex);
            index = 0;
            accountNumbers = new List<string>();
            accountNumberCaseIds = new Dictionary<string, int>();
            tobeInsertedAccountInfos = new List<AccountInfo>();
            tobeUpdatedAccountInfos = new List<AccountInfo>();
        }


        protected override ProcessAssigingCasesforItemsInput GetInputArgument(AsyncCodeActivityContext context)
        {
            return new ProcessAssigingCasesforItemsInput
            {
                OutputQueue = this.OutputQueue.Get(context)
            };
        }
    }
}
