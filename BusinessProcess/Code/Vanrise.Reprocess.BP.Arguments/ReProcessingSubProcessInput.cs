using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Vanrise.Reprocess.Entities;

namespace Vanrise.Reprocess.BP.Arguments
{
    public class ReProcessingSubProcessInput : Vanrise.BusinessProcess.Entities.BaseProcessInputArgument
    {
        public Guid ReprocessDefinitionId { get; set; }

        public DateTime FromTime { get; set; }

        public DateTime ToTime { get; set; }
        
        public ReprocessFilter ReprocessFilter { get; set; }

        public Dictionary<string, object> InitializationOutputByStage { get; set; }

        public override string GetTitle()
        {
            return String.Format("#BPDefinitionTitle# From {0} To {1}", FromTime.ToString("yyyy-MM-dd HH:mm:ss"), ToTime.ToString("yyyy-MM-dd HH:mm:ss"));
        }
    }
}
