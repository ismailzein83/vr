using System;
using System.Activities;
using System.Collections.Generic;
using Vanrise.BusinessProcess;
using Vanrise.Common;
using Vanrise.Entities;
using Vanrise.Fzero.FraudAnalysis.Data;
using Vanrise.Fzero.FraudAnalysis.Entities;

namespace Vanrise.Fzero.FraudAnalysis.BP.Activities
{

    public class GetStrategyExecutionItemswithCases : CodeActivity
    {
        #region Arguments

        [RequiredArgument]
        public InArgument<long> StrategyExecutionId { get; set; }

        [RequiredArgument]
        public InOutArgument<List<StrategyExecutionItem>> Items { get; set; }

        [RequiredArgument]
        public InOutArgument<List<AccountCase>> Cases { get; set; }

        [RequiredArgument]
        public InOutArgument<List<StrategyExecutionItem>> ItemsofCases { get; set; }

        #endregion


        protected override void Execute(CodeActivityContext context)
        {
            ContextExtensions.WriteTrackingMessage(context, LogEntryType.Information, "Started getting strategy execution items with related cases");

            IStrategyExecutionItemDataManager dataManager = FraudDataManagerFactory.GetDataManager<IStrategyExecutionItemDataManager>();
            List<StrategyExecutionItem> items = new List<StrategyExecutionItem>();
            List<AccountCase> cases = new List<AccountCase>();
            List<StrategyExecutionItem> itemsofCases = new List<StrategyExecutionItem>();

            dataManager.GetStrategyExecutionItemswithCasesbyExecutionId(StrategyExecutionId.Get(context), out items, out cases, out itemsofCases);

            context.SetValue(Items, items);
            context.SetValue(Cases, cases);
            context.SetValue(ItemsofCases, itemsofCases);

            ContextExtensions.WriteTrackingMessage(context, LogEntryType.Information, "Finshed getting strategy execution items with related cases");
        }

    }
}
