﻿using CDRComparison.Data;
using CDRComparison.Entities;
using System;
using System.Activities;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Vanrise.BusinessProcess;
using Vanrise.Queueing;

namespace CDRComparison.BP.Activities
{
    #region Argument Classes
    public class PrepareMissingCDRsInput
    {
        public BaseQueue<MissingCDRBatch> InputQueue { get; set; }

        public BaseQueue<Object> OutputQueue { get; set; }
    }

    #endregion
    public sealed class PrepareMissingCDRs : DependentAsyncActivity<PrepareMissingCDRsInput>
    {
        [RequiredArgument]
        public InArgument<BaseQueue<MissingCDRBatch>> InputQueue { get; set; }

        [RequiredArgument]
        public InOutArgument<BaseQueue<Object>> OutputQueue { get; set; }
        protected override void DoWork(PrepareMissingCDRsInput inputArgument, AsyncActivityStatus previousActivityStatus, AsyncActivityHandle handle)
        {
            IMissingCDRDataManager dataManager = CDRComparisonDataManagerFactory.GetDataManager<IMissingCDRDataManager>();
            PrepareDataForDBApply(previousActivityStatus, handle, dataManager, inputArgument.InputQueue, inputArgument.OutputQueue, missingCDRBatch => missingCDRBatch.MissingCDRs);
        }

        protected override PrepareMissingCDRsInput GetInputArgument2(AsyncCodeActivityContext context)
        {
            return new PrepareMissingCDRsInput
            {
                InputQueue = this.InputQueue.Get(context),
                OutputQueue = this.OutputQueue.Get(context)
            };
        }

        protected override void OnBeforeExecute(AsyncCodeActivityContext context, AsyncActivityHandle handle)
        {
            if (this.OutputQueue.Get(context) == null)
                this.OutputQueue.Set(context, new MemoryQueue<object>());
            base.OnBeforeExecute(context, handle);
        }
    }
}
