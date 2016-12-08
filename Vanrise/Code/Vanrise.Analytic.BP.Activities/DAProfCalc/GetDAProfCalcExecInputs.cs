using System;
using System.Collections.Generic;
using System.Activities;
using Vanrise.Analytic.Entities;
using Vanrise.BusinessProcess;

namespace Vanrise.Analytic.BP.Activities.DAProfCalc
{
    public sealed class GetDAProfCalcExecInputs : CodeActivity
    {
        [RequiredArgument]
        public InArgument<List<Guid>> DataAnalysisDefinitionItemIds { get; set; }

        [RequiredArgument]
        public OutArgument<List<DAProfCalcExecInput>> DAProfCalcExecInputs { get; set; }

        protected override void Execute(CodeActivityContext context)
        {
            List<Guid> dataAnalysisDefinitionItemIds = this.DataAnalysisDefinitionItemIds.Get(context);

            List<DAProfCalcExecInput> daProfCalcExecInputs = new List<DAProfCalcExecInput>();
            foreach (Guid dataAnalysisDefinitionItemId in dataAnalysisDefinitionItemIds)
            {
                DAProfCalcExecInput recordProfiligOutput = new DAProfCalcExecInput() { OutputItemDefinitionId = dataAnalysisDefinitionItemId };
                daProfCalcExecInputs.Add(recordProfiligOutput);
            }
            DAProfCalcExecInputs.Set(context, daProfCalcExecInputs);
            context.GetSharedInstanceData().WriteTrackingMessage(Vanrise.Entities.LogEntryType.Information, "Data Analysis Profiling and Calculation Execution Inputs loaded.", null);
        }
    }
}