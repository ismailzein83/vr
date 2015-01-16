﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Activities;
using Vanrise.BusinessProcess;

namespace Vanrise.BusinessProcess.WFActivities
{

    public sealed class SendProcessCompleteToParent : CodeActivity
    {
        // Define an activity input argument of type string
        [RequiredArgument]
        public InArgument<string> BookmarkName { get; set; }

        public InArgument<object> EventPayload { get; set; }

        // If your activity returns a value, derive from CodeActivity<TResult>
        // and return the value from the Execute method.
        protected override void Execute(CodeActivityContext context)
        {            
            BPSharedInstanceData sharedInstanceData = context.GetExtension<BPSharedInstanceData>();
            if (sharedInstanceData == null)
                throw new ArgumentNullException("BPSharedInstanceData");

            BusinessProcessRuntime.Current.TriggerProcessEvent(sharedInstanceData.ParentProcessID.Value, this.BookmarkName.Get(context), this.EventPayload.Get(context));
        }
    }
}
