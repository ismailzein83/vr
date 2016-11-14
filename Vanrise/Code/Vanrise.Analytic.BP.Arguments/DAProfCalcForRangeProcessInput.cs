using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Vanrise.Analytic.Entities;
using Vanrise.BusinessProcess.Entities;

namespace Vanrise.Analytic.BP.Arguments
{
    public class DAProfCalcForRangeProcessInput : BaseProcessInputArgument
    {
        public override string GetTitle()
        {
            throw new NotImplementedException();
        }

        public List<Guid> RecordStorageIds { get; set; }

        public DateTime FromTime { get; set; }

        public DateTime ToTime { get; set; }

        public Guid DAProfCalcDefinitionId { get; set; }

        public List<DAProfCalcExecInput> DAProfCalcExecInputs { get; set; }

        public IDAProfCalcOutputRecordProcessor OutputRecordProcessor { get; set; }
    }
}
