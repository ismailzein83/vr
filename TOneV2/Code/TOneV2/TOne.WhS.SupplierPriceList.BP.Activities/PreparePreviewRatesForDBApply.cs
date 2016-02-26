﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Activities;
using Vanrise.Queueing;
using TOne.WhS.SupplierPriceList.Entities.SPL;
using Vanrise.BusinessProcess;
using TOne.WhS.SupplierPriceList.Data;
using TOne.WhS.SupplierPriceList.Entities;

namespace TOne.WhS.SupplierPriceList.BP.Activities
{

    public class PreparePreviewRatesForApplyInput
    {
        public BaseQueue<IEnumerable<RatePreview>> InputQueue { get; set; }

        public BaseQueue<Object> OutputQueue { get; set; }
    }

    public sealed class PreparePreviewRatesForDBApply : DependentAsyncActivity<PreparePreviewRatesForApplyInput>
    {

        [RequiredArgument]
        public InArgument<BaseQueue<IEnumerable<RatePreview>>> InputQueue { get; set; }

        [RequiredArgument]
        public InOutArgument<BaseQueue<Object>> OutputQueue { get; set; }


        protected override void DoWork(PreparePreviewRatesForApplyInput inputArgument, AsyncActivityStatus previousActivityStatus, AsyncActivityHandle handle)
        {
            ISupplierRatePreviewDataManager dataManager = SupPLDataManagerFactory.GetDataManager<ISupplierRatePreviewDataManager>();
            dataManager.ProcessInstanceId = handle.SharedInstanceData.InstanceInfo.ProcessInstanceID;
            PrepareDataForDBApply(previousActivityStatus, handle, dataManager, inputArgument.InputQueue, inputArgument.OutputQueue, RatePreviewList => RatePreviewList);
        }

        protected override PreparePreviewRatesForApplyInput GetInputArgument2(AsyncCodeActivityContext context)
        {
            return new PreparePreviewRatesForApplyInput()
            {
                InputQueue = this.InputQueue.Get(context),
                OutputQueue = this.OutputQueue.Get(context),
            };
        }

        protected override void OnBeforeExecute(AsyncCodeActivityContext context, AsyncActivityHandle handle)
        {
            if (this.OutputQueue.Get(context) == null)
                this.OutputQueue.Set(context, new MemoryQueue<Object>());
            base.OnBeforeExecute(context, handle);
        }
    }
}
