using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Vanrise.Analytic.Entities
{
    public class MeasureConfiguration
    {
        public string ColumnName { get; set; }
        public MeasureExpression Expression { get; set; }
    }
}
