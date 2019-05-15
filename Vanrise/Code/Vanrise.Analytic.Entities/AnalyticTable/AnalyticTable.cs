using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Vanrise.Analytic.Entities
{
    public class AnalyticTable
    {
        public Guid AnalyticTableId { get; set; }

        public Guid? DevProjectId { get; set; }
        public string Name { get; set; }

        public AnalyticTableSettings Settings { get; set; }
        public AnalyticTableMeasureStyles MeasureStyles { get; set; }
        public AnalyticTablePermanentFilter PermanentFilter { get; set; }
    }
    public class AnalyticTableMeasureStyles
    {
        public List<MeasureStyleRule> MeasureStyleRules { get; set; }
    }
    public class AnalyticTablePermanentFilter
    {
        public AnalyticTablePermanentFilterSettings Settings { get; set; }
    }
   
}
