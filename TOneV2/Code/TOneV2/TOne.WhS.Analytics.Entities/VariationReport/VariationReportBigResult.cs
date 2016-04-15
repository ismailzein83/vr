using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TOne.WhS.Analytics.Entities
{
    public class VariationReportBigResult : Vanrise.Entities.BigResult<VariationReportRecord>
    {
        public VariationReportRecord Summary { get; set; }

        public List<TimePeriod> TimePeriods { get; set; }
    }
}
