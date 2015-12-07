using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TOne.WhS.Analytics.Data.SQL
{
    public class AnalyticDimensionConfig
    {
        public string IdColumn { get; set; }

        public string NameColumn { get; set; }

        public List<string> JoinStatements { get; set; }

        public List<string> GroupByStatements { get; set; }

        public string ExpressionSummary { get; set; }

    }
}
