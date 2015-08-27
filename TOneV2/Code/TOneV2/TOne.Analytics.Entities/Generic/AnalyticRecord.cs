using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TOne.Analytics.Entities
{
    public class AnalyticRecord
    {
        public AnalyticGroupFieldValue[] GroupFieldValues { get; set; }

        public Object[] MeasureValues { get; set; }
    }
}
