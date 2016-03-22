using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Vanrise.Analytic.Entities
{
    public class DimensionConfiguration
    {
        public string ColumnId { get; set; }
        public string ColumnName { get; set; }
        public string GroupByStatement { get; set; }
        public string JoinStatement { get; set; }
    }

    public class DimensionFilter
    {
        public string Dimension { get; set; }

        public List<Object> FilterValues { get; set; }
    }
}
