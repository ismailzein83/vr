using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Activities;
using Vanrise.BEBridge.Entities;

namespace Vanrise.BEBridge.BP.Activities
{

    public sealed class SetTargetBEs : CodeActivity
    {
        [RequiredArgument]
        public InArgument<TargetBESynchronizer> TargetBESynchronizer { get; set; }
        [RequiredArgument]
        public InArgument<List<ITargetBE>> TargetBEs { get; set; }
        [RequiredArgument]
        public OutArgument<List<ITargetBE>> TargetsToInsert { get; set; }
        [RequiredArgument]
        public OutArgument<List<ITargetBE>> TargetsToUpdate { get; set; }

        protected override void Execute(CodeActivityContext context)
        {
            List<ITargetBE> targetsToInsert = new List<ITargetBE>();
            List<ITargetBE> targetsToUpdate = new List<ITargetBE>();
            List<ITargetBE> targetBEs = TargetBEs.Get(context);
            TargetBESynchronizer targetBESynchronizer = this.TargetBESynchronizer.Get(context);
            foreach (ITargetBE targetBE in targetBEs)
            {
                TargetBETryGetExistingContext targetContext = new TargetBETryGetExistingContext();
                targetContext.SourceBEId = targetBE.SourceBEId;
                targetContext.TargetBE = targetBE;
                if (targetBESynchronizer.TryGetExistingBE(targetContext))
                    targetsToUpdate.Add(targetBE);
                else
                    targetsToInsert.Add(targetBE);
            }

            TargetsToInsert.Set(context, targetsToInsert);
            TargetsToUpdate.Set(context, targetsToUpdate);
        }

        private class TargetBETryGetExistingContext : ITargetBESynchronizerTryGetExistingBEContext
        {

            public object SourceBEId
            {
                get;
                set;
            }

            public ITargetBE TargetBE
            {
                set;
                get;
            }
        }
    }
}
