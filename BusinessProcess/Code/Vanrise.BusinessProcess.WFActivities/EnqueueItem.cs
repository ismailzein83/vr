using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Activities;

namespace Vanrise.BusinessProcess.WFActivities
{

    public sealed class EnqueueItem<T> : CodeActivity
    {
        [RequiredArgument]
        public InArgument<T> Item { get; set; }

        [RequiredArgument]
        public InArgument<Vanrise.Queueing.BaseQueue<T>> Queue { get; set; }

        protected override void Execute(CodeActivityContext context)
        {
            this.Queue.Get(context).Enqueue(this.Item.Get(context));
        }
    }
}
