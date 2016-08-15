using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Activities;
using Vanrise.BEBridge.Entities;

namespace Vanrise.BEBridge.BP.Activities
{

    public sealed class UpdateExistingBEs : CodeActivity
    {
        [RequiredArgument]
        public InArgument<TargetBESynchronizer> TargetBESynchronizer { get; set; }

        protected override void Execute(CodeActivityContext context)
        {
            TargetSynchronizerUpdateContext targetSynchronizerContext = new TargetSynchronizerUpdateContext();
            this.TargetBESynchronizer.Get(context).UpdateBEs(targetSynchronizerContext);
        }

        private class TargetSynchronizerUpdateContext : ITargetBESynchronizerUpdateBEsContext
        {

            public List<ITargetBE> TargetBE
            {
                get;
                set;
            }
        }
    }
}
