using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Vanrise.Analytic.Entities
{
    public class AnalyticMeasureExternalSourceConfig
    {
        public AnalyticMeasureExternalSourceExtendedSettings ExtendedSettings { get; set; }
    }

    public abstract class AnalyticMeasureExternalSourceExtendedSettings
    {
        public abstract Guid ConfigId { get; }

        public abstract AnalyticMeasureExternalSourceResult Execute(IAnalyticMeasureExternalSourceContext context);
    }

    public interface IAnalyticMeasureExternalSourceContext
    {
        AnalyticQuery AnalyticQuery { get; }
    }

    public class AnalyticMeasureExternalSourceResult
    {
        public List<AnalyticMeasureExternalSourceRecord> Records { get; set; }

        public Dictionary<string, Object> SummaryMeasureValues { get; set; }
    }

    public class AnalyticMeasureExternalSourceRecord
    {
        public DateTime? RecordTime { get; set; }

        public Dictionary<string, Object> DimensionValues { get; set; }

        public Dictionary<string, Object> MeasureValues { get; set; }
    }
}
