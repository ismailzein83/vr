using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Vanrise.BusinessProcess.Entities;

namespace Vanrise.BEBridge.BP.Arguments
{
    public class SourceBESyncProcessInput : BaseProcessInputArgument
    {
        public override string GetTitle()
        {
            return "Source BE Sync Process";
        }

        public List<Guid> BEReceiveDefinitionIds { get; set; }
    }
}
