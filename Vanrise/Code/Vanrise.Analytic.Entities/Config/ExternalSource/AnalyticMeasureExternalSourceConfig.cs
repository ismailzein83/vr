using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Vanrise.Entities;

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

    public abstract class AnalyticMeasureExternalSourceResult
    {
        public List<AnalyticMeasureExternalSourceRecord> Records { get; set; }

        public Dictionary<string, Object> SummaryMeasureValues { get; set; }

        public abstract dynamic GetMatchRecordMeasureValue(IAnalyticMeasureExternalSourceResultGetMatchRecordMesureValueContext context);
    }

    public interface IAnalyticMeasureExternalSourceResultGetMatchRecordMesureValueContext
    {
        AnalyticQuery Query { get; }

        AnalyticRecord Record { get; }

        bool IsSummaryRecord { get; }

        string MeasureName { get; }
    }

    public class AnalyticMeasureExternalSourceRecord
    {
        public DateTime? RecordTime { get; set; }

        public Dictionary<string, Object> DimensionValues { get; set; }

        public Dictionary<string, Object> MeasureValues { get; set; }
    }
    public class AnalyticMeasureExternalSourceExtendedSettingsConfig: ExtensionConfiguration
    {
        public const string EXTENSION_TYPE = "VR_Analytic_AnalyticMeasureExternalSourceExtendedSettings";

        public string Editor { get; set; }
    }
}
