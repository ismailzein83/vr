using System.Activities;
using System.Collections.Generic;
using Vanrise.BusinessProcess;
using Vanrise.Common;
using Vanrise.Entities;
using Vanrise.Fzero.FraudAnalysis.Business;
using Vanrise.Fzero.FraudAnalysis.Entities;

namespace Vanrise.Fzero.FraudAnalysis.BP.Activities
{

    #region Arguments Classes

    public class CheckDWStrategiesChangesInput
    {
        public DWStrategyDictionary DWStrategies { get; set; }

        public List<DWStrategy> ToBeInsertedStrategies { get; set; }

    }

    public class CheckDWStrategiesChangesOutput
    {
        public List<DWStrategy> ToBeInsertedStrategies { get; set; }

    }

    #endregion

    public class CheckDWStrategiesChanges : DependentAsyncActivity<CheckDWStrategiesChangesInput, CheckDWStrategiesChangesOutput>
    {

        #region Arguments

        [RequiredArgument]
        public InArgument<DWStrategyDictionary> DWStrategies { get; set; }


        [RequiredArgument]
        public InOutArgument<List<DWStrategy>> ToBeInsertedStrategies { get; set; }

        #endregion

        protected override CheckDWStrategiesChangesInput GetInputArgument2(System.Activities.AsyncCodeActivityContext context)
        {
            return new CheckDWStrategiesChangesInput
            {
                DWStrategies = this.DWStrategies.Get(context)
            };
        }

        protected override CheckDWStrategiesChangesOutput DoWorkWithResult(CheckDWStrategiesChangesInput inputArgument, AsyncActivityStatus previousActivityStatus, AsyncActivityHandle handle)
        {
            handle.SharedInstanceData.WriteTrackingMessage(LogEntryType.Information, "Started comparing strategies");
            List<DWStrategy> ToBeInsertedStrategies = new List<DWStrategy>();
            StrategyManager strategyManager = new StrategyManager();
            IEnumerable<Strategy> listStrategies = strategyManager.GetStrategies();

            foreach (var i in listStrategies)
                if (!inputArgument.DWStrategies.ContainsKey(i.Id))
                {
                    DWStrategy dwStrategy = new DWStrategy();
                    dwStrategy.Id = i.Id;
                    dwStrategy.Kind = (i.Settings.IsDefault ? Vanrise.Common.Utilities.GetEnumDescription(StrategyKind.SystemBuiltIn) : Vanrise.Common.Utilities.GetEnumDescription(StrategyKind.UserDefined));
                    dwStrategy.Name = i.Name;
                    dwStrategy.Type = Vanrise.Common.Utilities.GetEnumDescription(((PeriodEnum)i.Settings.PeriodId));

                    ToBeInsertedStrategies.Add(dwStrategy);
                }
            handle.SharedInstanceData.WriteTrackingMessage(LogEntryType.Information, "Finished comparing strategies");

            return new CheckDWStrategiesChangesOutput()
            {
                ToBeInsertedStrategies = ToBeInsertedStrategies
            };

        }

        protected override void OnWorkComplete(AsyncCodeActivityContext context, CheckDWStrategiesChangesOutput result)
        {
            this.ToBeInsertedStrategies.Set(context, result.ToBeInsertedStrategies);
        }
    }
}
