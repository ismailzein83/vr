using System;
using System.Collections.Generic;

namespace TOne.WhS.TOneV1Transition.BP.Arguments 
{
    public class MigrateAndRouteBuildInput : Vanrise.BusinessProcess.Entities.BaseProcessInputArgument
    {
        public override string GetTitle()
        {
            return String.Format("Migrate And Route Build Process");
        }
    }
}
