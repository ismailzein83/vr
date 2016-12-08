using System;
using System.Collections.Generic;
using System.Activities;
using Vanrise.Analytic.Entities;

namespace Vanrise.Analytic.BP.Activities.DAProfCalc
{
    public sealed class GenerateDAProfCalcExecInputDetails : CodeActivity
    {
        [RequiredArgument]
        public InArgument<List<DAProfCalcExecInput>> DAProfCalcExecInputs { get; set; }

        [RequiredArgument]
        public OutArgument<List<DAProfCalcExecInputDetail>> DAProfCalcExecInputDetails { get; set; }

        protected override void Execute(CodeActivityContext context)
        {
            List<DAProfCalcExecInput> daProfCalcExecInputs = this.DAProfCalcExecInputs.Get(context);

            List<DAProfCalcExecInputDetail> daProfCalcExecInputDetails = new List<DAProfCalcExecInputDetail>();
            foreach (DAProfCalcExecInput daProfCalcExecInput in daProfCalcExecInputs)
            {
                DAProfCalcExecInputDetail daProfCalcExecInputDetail = new DAProfCalcExecInputDetail() { DAProfCalcExecInput = daProfCalcExecInput,DataAnalysisUniqueName = Guid.NewGuid().ToString() };
                daProfCalcExecInputDetails.Add(daProfCalcExecInputDetail);
            }
            DAProfCalcExecInputDetails.Set(context, daProfCalcExecInputDetails);
        }
    }
}