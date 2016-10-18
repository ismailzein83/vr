using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Vanrise.Reprocess.Entities;

namespace Vanrise.Reprocess.BP.Arguments
{
    public class ReProcessingProcessInput : Vanrise.BusinessProcess.Entities.BaseProcessInputArgument
    {
        public Guid ReprocessDefinitionId { get; set; }

        public DateTime FromTime { get; set; }

        public DateTime ToTime { get; set; }

        public ChunkTime ChunkTime { get; set; }

        public override string GetTitle()
        {
            return String.Format("Reprocess from {0} to {1}", FromTime, ToTime);
        }
    }
}
