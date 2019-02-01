using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TOne.WhS.Deal.Entities
{
    public class DealEvaluatorProcessState
    {
        public object DealDetailedProgressMaxTimestamp { get; set; }

		public object DealDefinitionMaxTimestamp { get; set; }

		public long? LastBPInstanceId { get; set; }
    }
}
