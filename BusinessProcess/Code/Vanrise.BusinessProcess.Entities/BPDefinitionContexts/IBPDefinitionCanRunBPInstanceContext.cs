using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Vanrise.BusinessProcess.Entities
{
    public interface IBPDefinitionCanRunBPInstanceContext
    {
        BaseProcessInputArgument InputArgument { get; }

        List<BPInstance> GetStartedBPInstances();
    }

    

}
