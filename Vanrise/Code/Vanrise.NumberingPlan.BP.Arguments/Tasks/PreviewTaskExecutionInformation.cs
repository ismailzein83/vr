using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Vanrise.BusinessProcess.Entities;

namespace Vanrise.NumberingPlan.BP.Arguments
{
    public class PreviewTaskExecutionInformation : BPTaskExecutionInformation
    {
       public bool Decision { get; set; }
    }
}
