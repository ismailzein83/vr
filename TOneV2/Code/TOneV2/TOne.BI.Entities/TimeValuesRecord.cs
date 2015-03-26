using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TOne.BI.Entities
{
    public class TimeValuesRecord : BaseTimeDimensionRecord
    {
        public Decimal[] Values { get; set; }
    }
}
