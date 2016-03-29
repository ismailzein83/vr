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

    public class PrepareCDRsForDBApplyInput
    {
        public BaseQueue<CDRBatch> InputQueue { get; set; }

        public BaseQueue<Object> OutputQueue { get; set; }
    }

    #endregion

    public sealed class PrepareCDRsForDBApply : DependentAsyncActivity<PrepareCDRsForDBApplyInput>
    {
        #region Arguments

        [RequiredArgument]
        public InArgument<BaseQueue<CDRBatch>> InputQueue { get; set; }

        [RequiredArgument]
        public OutArgument<BaseQueue<Object>> OutputQueue { get; set; }
        
        #endregion

        protected override void DoWork(PrepareCDRsForDBApplyInput inputArgument, AsyncActivityStatus previousActivityStatus, AsyncActivityHandle handle)
        {
            ICDRDataManager dataManager = CDRComparisonDataManagerFactory.GetDataManager<ICDRDataManager>();
            PrepareDataForDBApply(previousActivityStatus, handle, dataManager, inputArgument.InputQueue, inputArgument.OutputQueue, cdrBatch => cdrBatch.CDRs);
        }

        protected override void OnBeforeExecute(AsyncCodeActivityContext context, AsyncActivityHandle handle)
        {
            if (this.OutputQueue.Get(context) == null)
                this.OutputQueue.Set(context, new MemoryQueue<object>());
            base.OnBeforeExecute(context, handle);
        }

        protected override PrepareCDRsForDBApplyInput GetInputArgument2(AsyncCodeActivityContext context)
        {
            return new PrepareCDRsForDBApplyInput
            {
                InputQueue = this.InputQueue.Get(context),
                OutputQueue = this.OutputQueue.Get(context)
            };
        }
    }
}
