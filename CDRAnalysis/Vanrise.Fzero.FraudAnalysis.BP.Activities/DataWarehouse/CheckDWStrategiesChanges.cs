using System;
using System.Activities;
using System.Collections.Generic;
using Vanrise.BusinessProcess;
using Vanrise.Common;
using Vanrise.Fzero.CDRImport.Entities;
using Vanrise.Fzero.FraudAnalysis.Business;
using Vanrise.Fzero.FraudAnalysis.Data;
using Vanrise.Fzero.FraudAnalysis.Entities;
using Vanrise.Queueing;
using System.Linq;
using System.Reflection;
using System.ComponentModel;

namespace Vanrise.Fzero.FraudAnalysis.BP.Activities
{

    #region Arguments Classes

    public class CheckDWStrategiesChangesInput
    {
        public Dictionary<int, DWStrategy> DWStrategies { get; set; }

        public List<DWStrategy> ToBeInsertedStrategies { get; set; }

    }

    #endregion

    public class CheckDWStrategiesChanges : DependentAsyncActivity<CheckDWStrategiesChangesInput>
    {

        #region Arguments

        [RequiredArgument]
        public InArgument<Dictionary<int, DWStrategy>> DWStrategies { get; set; }


        [RequiredArgument]
        public InOutArgument<List<DWStrategy>> ToBeInsertedStrategies { get; set; }

        #endregion


        protected override void DoWork(CheckDWStrategiesChangesInput inputArgument, AsyncActivityStatus previousActivityStatus, AsyncActivityHandle handle)
        {
            StrategyManager strategyManager = new StrategyManager();
            IEnumerable<Strategy> listStrategies = strategyManager.GetAll();

            foreach (var i in listStrategies)
                if (!inputArgument.DWStrategies.ContainsKey(i.Id))
                {
                    DWStrategy dwStrategy = new DWStrategy();
                    dwStrategy.Id = i.Id;
                    dwStrategy.Kind = (i.IsDefault ? Vanrise.Common.Utilities.GetEnumDescription(StrategyKindEnum.SystemBuiltIn) : Vanrise.Common.Utilities.GetEnumDescription(StrategyKindEnum.UserDefined));
                    dwStrategy.Name = i.Name;
                    dwStrategy.Type = Vanrise.Common.Utilities.GetEnumDescription(((PeriodEnum)i.PeriodId));

                    inputArgument.ToBeInsertedStrategies.Add(dwStrategy);
                }
        }



        protected override CheckDWStrategiesChangesInput GetInputArgument2(System.Activities.AsyncCodeActivityContext context)
        {
            return new CheckDWStrategiesChangesInput
            {
                DWStrategies = this.DWStrategies.Get(context),
                ToBeInsertedStrategies = this.ToBeInsertedStrategies.Get(context)
            };
        }

    }
}
