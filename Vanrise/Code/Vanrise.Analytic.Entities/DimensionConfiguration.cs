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
}
