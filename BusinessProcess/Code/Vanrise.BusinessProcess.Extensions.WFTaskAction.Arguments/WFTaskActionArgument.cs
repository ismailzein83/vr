using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Vanrise.BusinessProcess.Entities;
using Vanrise.Runtime.Entities;

namespace Vanrise.BusinessProcess.Extensions.WFTaskAction.Arguments
{
    public class WFTaskActionArgument : BaseTaskActionArgument
    {
        public int BPDefinitionID { get; set; }

        public BaseProcessInputArgument ProcessInputArguments { get; set; }
    }
}
