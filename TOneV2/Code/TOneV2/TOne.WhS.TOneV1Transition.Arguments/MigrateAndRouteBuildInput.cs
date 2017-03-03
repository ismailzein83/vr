using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TOne.WhS.TOneV1Transition.Arguments
{
    public class MigrateAndRouteBuildInput : Vanrise.BusinessProcess.Entities.BaseProcessInputArgument
    {
        public override string GetTitle()
        {
            return String.Format("Migrate And RouteBuild Process");
        }
    }
}
