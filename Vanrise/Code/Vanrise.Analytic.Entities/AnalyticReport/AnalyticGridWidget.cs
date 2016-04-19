using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Vanrise.Analytic.Entities
{
    public class AnalyticGridWidget : AnalyticReportWidget
    {
        public bool RootDimensionsFromSearchSection { get; set; }

        public List<AnalyticGridWidgetDimension> Dimensions { get; set; }

        public List<AnalyticGridWidgetMeasure> Measures { get; set; }
    }

    public class AnalyticGridWidgetDimension
    {
        public string DimensionName { get; set; }

        public bool IsRootDimension { get; set; }

        public string Title { get; set; }
    }

    public class AnalyticGridWidgetMeasure
    {
        public string MeasureName { get; set; }

        public string Title { get; set; }
    }
}
