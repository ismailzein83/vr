﻿using System;
using System.Activities;
using System.Collections.Generic;
using System.Configuration;
using Vanrise.BusinessProcess;
using Vanrise.Common;
using Vanrise.Fzero.CDRImport.Entities;
using Vanrise.Fzero.FraudAnalysis.Data;
using Vanrise.Fzero.FraudAnalysis.Entities;
using Vanrise.Queueing;
using System.Linq;

namespace Vanrise.Fzero.FraudAnalysis.BP.Activities
{
    #region Arguments Classes

    public class ProcessStrategyExecutionCancellationInput
    {
        public long StrategyExecutionId { get; set; }

        public BaseQueue<CancellingStrategyExecutionBatch> OutputQueue { get; set; }
    }

    #endregion

    public sealed class ProcessStrategyExecutionCancellation : BaseAsyncActivity<ProcessStrategyExecutionCancellationInput>
    {
        #region Arguments

        [RequiredArgument]
        public InArgument<long> StrategyExecutionId { get; set; }


        [RequiredArgument]
        public InOutArgument<BaseQueue<CancellingStrategyExecutionBatch>> OutputQueue { get; set; }


        #endregion

        protected override void OnBeforeExecute(AsyncCodeActivityContext context, AsyncActivityHandle handle)
        {
            if (this.OutputQueue.Get(context) == null)
                this.OutputQueue.Set(context, new MemoryQueue<StrategyExecutionItemSummaryBatch>());

            base.OnBeforeExecute(context, handle);
        }


        protected override void DoWork(ProcessStrategyExecutionCancellationInput inputArgument, AsyncActivityHandle handle)
        {
            handle.SharedInstanceData.WriteTrackingMessage(LogEntryType.Information, "Started Processing Strategy Execution Cancellation");

            IStrategyExecutionItemDataManager dataManager = FraudDataManagerFactory.GetDataManager<IStrategyExecutionItemDataManager>();
            List<StrategyExecutionItem> items = new List<StrategyExecutionItem>();
            List<AccountCase> cases = new List<AccountCase>();
            List<StrategyExecutionItem> itemsofCases = new List<StrategyExecutionItem>();

            dataManager.GetStrategyExecutionbyExecutionId(inputArgument.StrategyExecutionId, out items, out cases, out itemsofCases);

            List<long> tobeDeletedItems = new List<long>();
            List<int> tobeDeletedCases = new List<int>();
            AccountCase relatedCase = new AccountCase();

            int index = 0;
            int totalIndex = 0;

            foreach (StrategyExecutionItem item in items)
            {
                index++;
                totalIndex++;

                if (index == 1000)
                {
                    inputArgument.OutputQueue.Enqueue(new CancellingStrategyExecutionBatch() { AccountCaseIds = tobeDeletedCases, StrategyExecutionItemIds = tobeDeletedItems });
                    Console.WriteLine("{0} Items Processed", totalIndex);
                    handle.SharedInstanceData.WriteTrackingMessage(LogEntryType.Information, "{0} Items Processed", totalIndex);
                    index = 0;
                    tobeDeletedItems = new List<long>();
                    tobeDeletedCases = new List<int>();
                }

                if (item.CaseID != null)
                {
                    relatedCase = cases.Where(x => x.CaseID == item.CaseID).First();
                    if (relatedCase.StatusID == CaseStatus.Pending || relatedCase.StatusID == CaseStatus.Open)
                        if (!itemsofCases.Exists(x => x.CaseID == relatedCase.CaseID && x.StrategyExecutionID != inputArgument.StrategyExecutionId))
                        {
                            tobeDeletedItems.Add(item.ID);
                            tobeDeletedCases.Add(relatedCase.CaseID);
                        }

                }
                else
                    tobeDeletedItems.Add(item.ID);
            }

            if (tobeDeletedItems.Count > 0 || tobeDeletedCases.Count() > 0)
            {
                inputArgument.OutputQueue.Enqueue(new CancellingStrategyExecutionBatch() { AccountCaseIds = tobeDeletedCases, StrategyExecutionItemIds = tobeDeletedItems });
                tobeDeletedItems = new List<long>();
                tobeDeletedCases = new List<int>();
            }

            handle.SharedInstanceData.WriteTrackingMessage(LogEntryType.Information, "Finished Processing Strategy Execution Cancellation");

        }


        protected override ProcessStrategyExecutionCancellationInput GetInputArgument(AsyncCodeActivityContext context)
        {
            return new ProcessStrategyExecutionCancellationInput
            {
                OutputQueue = this.OutputQueue.Get(context)
            };
        }
    }
}
