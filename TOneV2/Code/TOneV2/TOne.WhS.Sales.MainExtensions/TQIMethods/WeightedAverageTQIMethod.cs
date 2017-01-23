using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TOne.WhS.Routing.Entities;
using TOne.WhS.Sales.Entities;
using Vanrise.Analytic.Business;
using Vanrise.Analytic.Entities;
using Vanrise.Entities;

namespace TOne.WhS.Sales.MainExtensions
{
    public class WeightedAverageTQIMethod : TQIMethod
    {
        public override Guid ConfigId { get { return new Guid("C75EA417-7D46-48C2-93DE-1B575373AAD2"); } }
        public decimal PeriodValue { get; set; }
        public PeriodTypes PeriodType { get; set; }
        public override void CalculateRate(ITQIMethodContext context)
        {
            List<string> listMeasures = new List<string> { "DurationInMinutes" };
            List<string> listDimensions = new List<string> { "Supplier", "SaleZone" };
            string supplierDimensionFilterName = "Supplier";
            string saleZoneDimensionFilterName = "SaleZone";

            DateTime fromDate = DateTime.MinValue;

            switch (this.PeriodType)
            {
                case PeriodTypes.Days:
                    fromDate = DateTime.Today.AddDays(-(double)this.PeriodValue);
                    break;
                case PeriodTypes.Hours:
                    fromDate = DateTime.Today.AddHours(-(double)this.PeriodValue);
                    break;
                case PeriodTypes.Minutes:
                    fromDate = DateTime.Today.AddMinutes(-(double)this.PeriodValue);
                    break;
                default:
                    throw new DataIntegrityValidationException(string.Format("Period Type must be set"));
            }

            DateTime toDate = DateTime.Today;

            if (context.Route == null || context.Route.RouteOptionsDetails == null)
                return;

            decimal sumOfDuration = 0;
            decimal sumOfRatesMultipliedByDuration = 0;
            foreach (RPRouteOptionDetail option in context.Route.RouteOptionsDetails)
            {
                var analyticResult = GetFilteredRecords(listDimensions, listMeasures, supplierDimensionFilterName, option.Entity.SupplierId, saleZoneDimensionFilterName, context.Route.SaleZoneId, fromDate, toDate);
                if (analyticResult == null || analyticResult.Data == null)
                    continue;

                foreach (var analyticRecord in analyticResult.Data)
                {
                    MeasureValue durationInMinutes = GetMeasureValue(analyticRecord, "DurationInMinutes");
                    decimal durationInMinutesValue = Convert.ToDecimal(durationInMinutes.Value ?? 0.0);
                    if (durationInMinutesValue == 0)
                        continue;

                    sumOfRatesMultipliedByDuration += durationInMinutesValue * option.ConvertedSupplierRate;
                    sumOfDuration += durationInMinutesValue;
                }
            }

            if (sumOfDuration != 0)
                context.Rate = sumOfRatesMultipliedByDuration / sumOfDuration;
        }

        private AnalyticSummaryBigResult<AnalyticRecord> GetFilteredRecords(List<string> listDimensions, List<string> listMeasures, string firstDimensionFilterName, object firstDimensionFilterValue,
         string secondDimensionFilterName, object secondDimensionFilterValue, DateTime fromDate, DateTime toDate)
        {
            AnalyticManager analyticManager = new AnalyticManager();
            Vanrise.Entities.DataRetrievalInput<AnalyticQuery> analyticQuery = new DataRetrievalInput<AnalyticQuery>()
            {
                Query = new AnalyticQuery()
                {
                    DimensionFields = listDimensions,
                    MeasureFields = listMeasures,
                    TableId = 4,
                    FromTime = fromDate,
                    ToTime = toDate,
                    ParentDimensions = new List<string>(),
                    Filters = new List<DimensionFilter>()
                },
                SortByColumnName = "DimensionValues[0].Name"
            };
            DimensionFilter dimensionFilter = new DimensionFilter()
            {
                Dimension = firstDimensionFilterName,
                FilterValues = new List<object> { firstDimensionFilterValue }

            };
            DimensionFilter secondDimensionFilter = new DimensionFilter()
            {
                Dimension = secondDimensionFilterName,
                FilterValues = new List<object> { secondDimensionFilterValue }
            };
            analyticQuery.Query.Filters.Add(dimensionFilter);
            analyticQuery.Query.Filters.Add(secondDimensionFilter);

            return analyticManager.GetFilteredRecords(analyticQuery) as Vanrise.Analytic.Entities.AnalyticSummaryBigResult<AnalyticRecord>;
        }

        private MeasureValue GetMeasureValue(AnalyticRecord analyticRecord, string measureName)
        {
            MeasureValue measureValue;
            analyticRecord.MeasureValues.TryGetValue(measureName, out measureValue);
            return measureValue;
        }
    }
}
