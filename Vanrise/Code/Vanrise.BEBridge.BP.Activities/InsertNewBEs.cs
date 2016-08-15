using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Activities;
using Vanrise.BEBridge.Entities;

namespace Vanrise.BEBridge.BP.Activities
{

    public sealed class InsertNewBEs : CodeActivity
    {
        [RequiredArgument]
        public InArgument<TargetBESynchronizer> TargetBESynchronizer { get; set; }

        public InArgument<List<ITargetBE>> TargetBEs { get; set; }

        protected override void Execute(CodeActivityContext context)
        {
            TargetBESynchronizer targetSynchronizer = this.TargetBESynchronizer.Get(context);
            if (targetSynchronizer == null)
                throw new NullReferenceException("targetSynchronizer");
            TargetSynchronizerInsertContext targetSynchronizerContext = new TargetSynchronizerInsertContext();
            targetSynchronizerContext.TargetBE = TargetBEs.Get(context);
            targetSynchronizer.InsertBEs(targetSynchronizerContext);
        }

        private class TargetSynchronizerInsertContext : ITargetBESynchronizerInsertBEsContext
        {
            public List<ITargetBE> TargetBE
            {
                get;
                set;
            }
        }
    }
}
