using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Vanrise.Analytic.Entities;
using Vanrise.Entities;
using Vanrise.GenericData.Entities;

namespace Vanrise.Analytic.MainExtensions.AutomatedReport.Queries
{
    public class AnalyticTableQuerySettings : VRAutomatedReportQuerySettings
    {
        public VRTimePeriod TimePeriod { get; set; }

        public List<AnalyticTableQueryDimension> Dimensions { get; set; }

        public List<AnalyticTableQueryMeasure> Measures { get; set; }

        public List<DimensionFilter> Filters { get; set; }

        public RecordFilterGroup FilterGroup { get; set; }

        public int? CurrencyId { get; set; }

        public bool WithSummary { get; set; }

        public int? TopRecords { get; set; }

        public AnalyticQueryOrderType? OrderType { get; set; }

        public AnalyticQueryAdvancedOrderOptionsBase AdvancedOrderOptions { get; set; }

        public override VRAutomatedReportDataResult Execute(IVRAutomatedReportQueryExecuteContext context)
        {
            throw new NotImplementedException();
        }

        public override VRAutomatedReportDataSchema GetSchema(IVRAutomatedReportQueryGetSchemaContext context)
        {
            throw new NotImplementedException();
        }
    }

    public class AnalyticTableQueryDimension
    {
        public string DimensionName { get; set; }
    }

    public class AnalyticTableQueryMeasure
    {
        public string MeasureName { get; set; }
    }
}
