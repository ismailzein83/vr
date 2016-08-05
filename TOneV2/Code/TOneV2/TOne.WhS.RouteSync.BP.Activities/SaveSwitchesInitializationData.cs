using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Activities;
using TOne.WhS.RouteSync.Entities;

namespace TOne.WhS.RouteSync.BP.Activities
{

    public sealed class SaveSwitchesInitializationData : CodeActivity
    {
        [RequiredArgument]
        public InArgument<Dictionary<string, SwitchRouteSyncInitializationData>> SwitchesInitializationData { get; set; }

        [RequiredArgument]
        public OutArgument<Guid> SwitchesInitializationDataId { get; set; }

        protected override void Execute(CodeActivityContext context)
        {
            //throw new NotImplementedException();
        }
    }
}
