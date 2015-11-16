﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Activities;
using TOne.WhS.Routing.Entities;
using Vanrise.Queueing;
using Vanrise.BusinessProcess;
using TOne.WhS.Routing.Data;

namespace TOne.WhS.Routing.BP.Activities
{
    public class PrepareCustomerZoneDetailForDBApplyInput
    {
        public BaseQueue<CustomerZoneDetailBatch> InputQueue { get; set; }
        public BaseQueue<Object> OutputQueue { get; set; }
    }
    public sealed class PrepareCustomerZoneDetailForDBApply : DependentAsyncActivity<PrepareCustomerZoneDetailForDBApplyInput>
    {
        [RequiredArgument]
        public InArgument<BaseQueue<CustomerZoneDetailBatch>> InputQueue { get; set; }
        [RequiredArgument]
        public InOutArgument<BaseQueue<Object>> OutputQueue { get; set; }

        protected override void DoWork(PrepareCustomerZoneDetailForDBApplyInput inputArgument, AsyncActivityStatus previousActivityStatus, AsyncActivityHandle handle)
        {
            ICustomerZoneDetailsDataManager dataManager = RoutingDataManagerFactory.GetDataManager<ICustomerZoneDetailsDataManager>();
            PrepareDataForDBApply(previousActivityStatus, handle, dataManager, inputArgument.InputQueue, inputArgument.OutputQueue, CustomerZoneDetailsBatch => CustomerZoneDetailsBatch.CustomerZoneDetails);
        }

        protected override PrepareCustomerZoneDetailForDBApplyInput GetInputArgument2(AsyncCodeActivityContext context)
        {
            return new PrepareCustomerZoneDetailForDBApplyInput()
            {
                InputQueue = this.InputQueue.Get(context),
                OutputQueue = this.OutputQueue.Get(context)
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
