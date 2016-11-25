using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Activities;
using Vanrise.BEBridge.Entities;
using Vanrise.Queueing;

namespace Vanrise.BEBridge.BP.Activities
{

    public sealed class PrepareBatchProcessingContext : CodeActivity
    {
        [RequiredArgument]
        public InArgument<BEReceiveDefinition> BEReceiveDefinition { get; set; }

        [RequiredArgument]
        public OutArgument<List<BaseQueue<BatchProcessingContext>>> BatchProcessingContexts { get; set; }

        protected override void Execute(CodeActivityContext context)
        {
            BatchProcessingContexts.Set(context, new List<BaseQueue<BatchProcessingContext>>(BEReceiveDefinition.Get(context).Settings.EntitySyncDefinitions.Select(def => new MemoryQueue<BatchProcessingContext>())));
        }
    }
}
