using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Vanrise.BI.Entities
{
    public class SummaryDirectiveSetting:BIWidgetSetting
    {
        public List<string> MeasureTypes { get; set; }


        public override List<string> GetMeasures()
        {
            return MeasureTypes;
        }
    }
}
