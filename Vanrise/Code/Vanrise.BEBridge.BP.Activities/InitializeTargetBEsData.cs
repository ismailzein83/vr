using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Activities;
using Vanrise.BEBridge.Entities;

namespace Vanrise.BEBridge.BP.Activities
{
    public sealed class InitializeTargetBEsData : CodeActivity
    {
        [RequiredArgument]
        public InArgument<TargetBESynchronizer> TargetBeSynchronizer { get; set; }
        [RequiredArgument]
        public OutArgument<TargetBESynchronizerInitializeContext> SynchronizerInitializeContext { get; set; }
        [RequiredArgument]
        public InArgument<TargetBEConvertor> TargetBeConvertor { get; set; }
        [RequiredArgument]
        public OutArgument<TargetBEConvertorInitializeContext> ConvertorInitializeContext { get; set; }
        protected override void Execute(CodeActivityContext context)
        {
            TargetBESynchronizerInitializeContext synchronizerContext = new TargetBESynchronizerInitializeContext();
            TargetBeSynchronizer.Get(context).Initialize(synchronizerContext);
            SynchronizerInitializeContext.Set(context, synchronizerContext);

            TargetBEConvertorInitializeContext convertorContext = new TargetBEConvertorInitializeContext();
            TargetBeConvertor.Get(context).Initialize(convertorContext);
            ConvertorInitializeContext.Set(context, convertorContext);
        }
    }
}
