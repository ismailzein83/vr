using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Vanrise.BI.Entities
{
    public class EntityMeasureValueInput:BaseBIInput
    {
        public List<string> EntityType { get; set; }
        public  string EntityId { get; set; }
        public TimeDimensionType TimeDimensionType { get; set; }
        public List<DimensionValueFilter> Filter { get; set; }
    }
}
