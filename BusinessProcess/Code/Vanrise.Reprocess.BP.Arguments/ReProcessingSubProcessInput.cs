using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Vanrise.Reprocess.BP.Arguments
{
    public class ReProcessingSubProcessInput : Vanrise.BusinessProcess.Entities.BaseProcessInputArgument
    {
        public Guid ReprocessDefinitionId { get; set; }

        public DateTime FromTime { get; set; }

        public DateTime ToTime { get; set; }

        public override string GetTitle()
        {
            return String.Format("From {0} To {1}", FromTime, ToTime);
        }
    }
}
