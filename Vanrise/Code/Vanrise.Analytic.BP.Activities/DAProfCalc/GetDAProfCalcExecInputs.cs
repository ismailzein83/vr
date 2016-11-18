using System;
using System.Collections.Generic;
using System.Activities;
using Vanrise.Analytic.Entities;

namespace Vanrise.Analytic.BP.Activities.DAProfCalc
{
    public sealed class GetDAProfCalcExecInputs : CodeActivity
    {
        [RequiredArgument]
        public OutArgument<List<DAProfCalcExecInput>> DAProfCalcExecInputs { get; set; }

        protected override void Execute(CodeActivityContext context)
        {
            List<DAProfCalcExecInput> daProfCalcExecInputs = new List<DAProfCalcExecInput>();

            DAProfCalcExecInput recordProfiligOutput = new DAProfCalcExecInput() { OutputItemDefinitionId = new Guid("39E04643-3C5C-4D11-9D3C-41611C34F7B3") };
            daProfCalcExecInputs.Add(recordProfiligOutput);

            DAProfCalcExecInputs.Set(context, daProfCalcExecInputs);
        }
    }
}