using System;
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

        public BaseQueue<long> StrategyExecutionItemIds { get; set; }
        public BaseQueue<int> AccountCaseIds { get; set; }
    }

    #endregion

    public sealed class ProcessStrategyExecutionCancellation : BaseAsyncActivity<ProcessStrategyExecutionCancellationInput>
    {
        #region Arguments

        [RequiredArgument]
        public InArgument<long> StrategyExecutionId { get; set; }


        [RequiredArgument]
        public InOutArgument<BaseQueue<long>> StrategyExecutionItemIds { get; set; }

        [RequiredArgument]
        public InOutArgument<BaseQueue<int>> AccountCaseIds { get; set; }

        #endregion

        protected override void OnBeforeExecute(AsyncCodeActivityContext context, AsyncActivityHandle handle)
        {
            if (this.StrategyExecutionItemIds.Get(context) == null)
                this.StrategyExecutionItemIds.Set(context, new MemoryQueue<StrategyExecutionItemSummaryBatch>());

            if (this.AccountCaseIds.Get(context) == null)
                this.AccountCaseIds.Set(context, new MemoryQueue<StrategyExecutionItemSummaryBatch>());

            base.OnBeforeExecute(context, handle);
        }


        protected override void DoWork(ProcessStrategyExecutionCancellationInput inputArgument, AsyncActivityHandle handle)
        {
            IStrategyExecutionItemDataManager dataManager = FraudDataManagerFactory.GetDataManager<IStrategyExecutionItemDataManager>();


            List<StrategyExecutionItem> items = new List<StrategyExecutionItem>();
            List<AccountCase> cases = new List<AccountCase>();
            List<StrategyExecutionItem> itemsofCases = new List<StrategyExecutionItem>();

            dataManager.GetStrategyExecutionbyExecutionId(inputArgument.StrategyExecutionId, out items, out cases, out itemsofCases);


            List<int> IgnoredCaseIds = cases.Where(x => x.StatusID == CaseStatus.ClosedFraud || x.StatusID == CaseStatus.ClosedWhiteList).Select(x => x.CaseID).ToList();
            cases = cases.Where(x => !IgnoredCaseIds.Contains(x.CaseID)).ToList();
            items = items.Where(x => !IgnoredCaseIds.Contains(x.CaseID.Value)).ToList();


            List<long> IgnoredItemIds = itemsofCases.Where(x => x.StrategyExecutionID != inputArgument.StrategyExecutionId && x.SuspicionOccuranceStatus != SuspicionOccuranceStatus.Deleted).Select(x => x.ID).ToList();
            items = items.Where(x => !IgnoredItemIds.Contains(x.ID)).ToList();
                    

        }


        protected override ProcessStrategyExecutionCancellationInput GetInputArgument(AsyncCodeActivityContext context)
        {
            return new ProcessStrategyExecutionCancellationInput
            {
                StrategyExecutionItemIds = this.StrategyExecutionItemIds.Get(context),
                AccountCaseIds = this.AccountCaseIds.Get(context)
            };
        }
    }
}
