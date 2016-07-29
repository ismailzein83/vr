using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Vanrise.BusinessProcess.Entities;

namespace TOne.WhS.RouteSync.BP.Arguments
{
    public class RouteSyncProcessInput : BaseProcessInputArgument
    {
        public override string GetTitle()
        {
            throw new NotImplementedException();
        }

        public int RouteSyncDefinitionId { get; set; }
    }
}
