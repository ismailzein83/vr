using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TOne.WhS.Deal.Entities
{
    public class DealEvaluatorProcessState
    {
        public byte[] DealDetailedProgressMaxTimestamp { get; set; }

		public byte[] DealDefinitionMaxTimestamp { get; set; }

		public long? LastBPInstanceId { get; set; }
    }
}
