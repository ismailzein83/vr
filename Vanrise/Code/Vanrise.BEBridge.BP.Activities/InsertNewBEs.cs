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

        protected override void Execute(CodeActivityContext context)
        {
            TargetSynchronizerInsertContext targetSynchronizerContext = new TargetSynchronizerInsertContext();
            this.TargetBESynchronizer.Get(context).InsertBEs(targetSynchronizerContext);
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
