using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TOne.Analytics.Entities;

namespace TOne.Analytics.Data.SQL
{
    public class AnalyticMeasureFieldConfig
    {
        public Func<AnalyticQuery, string> GetFieldExpression { get; set; }

    }
}
