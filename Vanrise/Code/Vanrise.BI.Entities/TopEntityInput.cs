using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Vanrise.BI.Entities
{
    public class TopEntityInput:BaseBIInput
    {
        public List<string> EntityTypeName { get; set; }
        public string TopByMeasureTypeName { get; set; }
        public int TopCount { get; set; }
        public List<DimensionFilter> Filter { get; set; }
    }
}
