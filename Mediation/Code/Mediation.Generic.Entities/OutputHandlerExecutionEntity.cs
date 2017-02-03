using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Vanrise.Queueing;

namespace Mediation.Generic.Entities
{
    public class OutputHandlerExecutionEntity
    {
        public MediationOutputHandlerDefinition OutputHandler { get; set; }
        public BaseQueue<PreparedCdrBatch> InputQueue { get; set; }
    }
}
