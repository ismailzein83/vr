using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Vanrise.BI.Entities
{
    public class MeasureValueInput : BaseBIInput
    {
        public TimeDimensionType TimeDimensionType { get; set; }
    }
}
