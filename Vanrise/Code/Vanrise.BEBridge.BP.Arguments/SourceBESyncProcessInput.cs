using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Vanrise.BusinessProcess.Entities;

namespace Vanrise.BEBridge.BP.Arguments
{
    public  class SourceBESyncProcessInput : BaseProcessInputArgument
    {
        public override string GetTitle()
        {
            throw new NotImplementedException();
        }

        public Guid BEReceiveDefinitionId { get; set; }
    }
}
