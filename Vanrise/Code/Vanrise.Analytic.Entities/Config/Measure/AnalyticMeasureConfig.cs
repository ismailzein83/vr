using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Vanrise.Security.Entities;

namespace Vanrise.Analytic.Entities
{
    public enum AnalyticSummaryFunction { Count = 0, Sum = 1, Avg = 2, Max = 3, Min = 4 }
    public enum AnalyticMeasureUnit { Fixed = 1, QueryCurrency = 2}
    public class AnalyticMeasureConfig
    {
        public string GetValueMethod { get; set; }

        public List<string> JoinConfigNames { get; set; }

        public List<string> DependentAggregateNames { get; set; }

        public List<string> DependentDimensions { get; set; }

        public GenericData.Entities.DataRecordFieldType FieldType { get; set; }

        public RequiredPermissionSettings RequiredPermission { get; set; }

        public string Description { get; set; }

        public bool HideDescriptionInLegend { get; set; }
        public AnalyticMeasureUnit MeasureUnit { get; set; }
        public string FixedMeasureUnit { get; set; }

    }
}
