using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Activities;
using Vanrise.BEBridge.Entities;

namespace Vanrise.BEBridge.BP.Activities
{
    public sealed class InitializeSynchronizer : CodeActivity
    {
        [RequiredArgument]
        public InArgument<TargetBESynchronizer> TargetBeSynchronizer { get; set; }
        [RequiredArgument]
        public OutArgument<TargetBESynchronizerInitializeContext> SynchronizerInitializeContext { get; set; }
        protected override void Execute(CodeActivityContext context)
        {
            TargetBESynchronizerInitializeContext synchronizerContext = new TargetBESynchronizerInitializeContext();
            TargetBeSynchronizer.Get(context).Initialize(synchronizerContext);
            SynchronizerInitializeContext.Set(context, synchronizerContext);
        }
    }
}
