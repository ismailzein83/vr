﻿using System.Activities;
using Vanrise.BusinessProcess;
using Vanrise.Analytic.Entities;
using Vanrise.Common.Business;

namespace Vanrise.Analytic.BP.Activities.DAProfCalc
{
    #region Arguments

    public class FinaliseRecordProfilingInput
    {
        public DAProfCalcExecInputDetail DAProfCalcExecInputDetail { get; set; }

        public IDAProfCalcOutputRecordProcessor OutputRecordProcessor { get; set; }
    }

    public class FinaliseRecordProfilingOutput
    {

    }

    #endregion

    public sealed class FinaliseRecordProfiling : DependentAsyncActivity<FinaliseRecordProfilingInput, FinaliseRecordProfilingOutput>
    {
        [RequiredArgument]
        public InArgument<DAProfCalcExecInputDetail> DAProfCalcExecInputDetail { get; set; }

        [RequiredArgument]
        public InArgument<IDAProfCalcOutputRecordProcessor> OutputRecordProcessor { get; set; }

        protected override FinaliseRecordProfilingOutput DoWorkWithResult(FinaliseRecordProfilingInput inputArgument, AsyncActivityStatus previousActivityStatus, AsyncActivityHandle handle)
        {
            FinaliseRecordProfilingOutput output = new FinaliseRecordProfilingOutput();

            DistributedDataGrouper distributedDataGrouper = new DistributedDataGrouper(
                inputArgument.DAProfCalcExecInputDetail.DataAnalysisUniqueName,
                new ProfilingDGHandler
                {
                    OutputItemDefinitionId = inputArgument.DAProfCalcExecInputDetail.DAProfCalcExecInput.OutputItemDefinitionId,
                    OutputRecordProcessor = inputArgument.OutputRecordProcessor
                });

            distributedDataGrouper.StartGettingFinalResults(null);

            return output;
        }

        protected override FinaliseRecordProfilingInput GetInputArgument2(AsyncCodeActivityContext context)
        {
            return new FinaliseRecordProfilingInput()
            {
                DAProfCalcExecInputDetail = DAProfCalcExecInputDetail.Get(context),
                OutputRecordProcessor = OutputRecordProcessor.Get(context)
            };
        }

        protected override void OnWorkComplete(AsyncCodeActivityContext context, FinaliseRecordProfilingOutput result)
        {
        }
    }
}