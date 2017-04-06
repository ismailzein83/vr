using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Vanrise.BusinessProcess.Entities
{
    public interface IBPDefinitionBPExecutionCompletedContext
    {
        BPInstance BPInstance { get; }
    }

    public class BPDefinitionBPExecutionCompletedContext : IBPDefinitionBPExecutionCompletedContext
    {
        public BPInstance BPInstance { get; set; }
    }
}
