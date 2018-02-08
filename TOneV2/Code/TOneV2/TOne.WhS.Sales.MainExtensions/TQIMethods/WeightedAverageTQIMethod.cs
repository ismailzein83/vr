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
            if (context.Route == null || context.Route.RouteOptionsDetails == null)
                return;

            DurationByZone durationByZone = this.GetDurationByZone(context.Route.SaleZoneId);

            decimal sumOfDuration = 0;
            decimal sumOfRatesMultipliedByDuration = 0;
            foreach (RPRouteOptionDetail option in context.Route.RouteOptionsDetails)
            {
                DurationBySupplier durationBySupplier = null;
                if (durationByZone.TryGetValue(option.SaleZoneId, out durationBySupplier))
                {
                    decimal durationInMinutes = 0;
                    if (durationBySupplier.TryGetValue(option.SupplierId, out durationInMinutes))
                    {
                        sumOfRatesMultipliedByDuration += durationInMinutes * option.ConvertedSupplierRate;
                        sumOfDuration += durationInMinutes;
                    }
                }
            }

            if (sumOfDuration != 0)
            {
                context.Rate = decimal.Round(sumOfRatesMultipliedByDuration / sumOfDuration, context.LongPrecision);
            }
        }

        #region Private Methods

        private DurationByZone GetDurationByZone(long saleZoneId)
        {
            List<string> listMeasures = new List<string> { "DurationInMinutes" };
            List<string> listDimensions = new List<string> { "Supplier", "SaleZone" };

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

            var analyticResult = GetFilteredRecords(listDimensions, listMeasures, saleZoneId, fromDate, toDate);

            DurationByZone durationByZone = new DurationByZone();

            if (analyticResult != null)
            {
                foreach (var analyticRecord in analyticResult.Data)
                {
                    DimensionValue supplierDimension = analyticRecord.DimensionValues.ElementAt(0);
                    DimensionValue zoneDimension = analyticRecord.DimensionValues.ElementAt(1);

                    long zoneId = (long)zoneDimension.Value;
                    int? supplierId = supplierDimension.Value != null ? (int?)supplierDimension.Value : null;

                    DurationBySupplier durationBySupplier = null;
                    if (!durationByZone.TryGetValue(zoneId, out durationBySupplier))
                    {
                        durationBySupplier = new DurationBySupplier();
                        durationByZone.Add(zoneId, durationBySupplier);
                    }

                    MeasureValue durationInMinutes = GetMeasureValue(analyticRecord, "DurationInMinutes");
                    decimal durationInMinutesValue = Convert.ToDecimal(durationInMinutes.Value ?? 0.0);

                    if (supplierId.HasValue)
                        durationBySupplier.Add(supplierId.Value, durationInMinutesValue);
                }
            }

            return durationByZone;
        }

        private AnalyticSummaryBigResult<AnalyticRecord> GetFilteredRecords(List<string> listDimensions, List<string> listMeasures, long saleZoneId, DateTime fromDate, DateTime toDate)
        {
            AnalyticManager analyticManager = new AnalyticManager();
            Vanrise.Entities.DataRetrievalInput<AnalyticQuery> analyticQuery = new DataRetrievalInput<AnalyticQuery>()
            {
                Query = new AnalyticQuery()
                {
                    DimensionFields = listDimensions,
                    MeasureFields = listMeasures,
                    TableId = Guid.Parse("58dd0497-498d-40f2-8687-08f8356c63cc"),
                    FromTime = fromDate,
                    ToTime = toDate,
                    ParentDimensions = new List<string>(),
                    Filters = new List<DimensionFilter>()
                },
                SortByColumnName = "DimensionValues[0].Name"
            };

            DimensionFilter dimensionFilter = new DimensionFilter()
            {
                Dimension = "SaleZone",
                FilterValues = new List<object>() { saleZoneId }
            };

            analyticQuery.Query.Filters.Add(dimensionFilter);
            return analyticManager.GetFilteredRecords(analyticQuery) as Vanrise.Analytic.Entities.AnalyticSummaryBigResult<AnalyticRecord>;
        }

        private MeasureValue GetMeasureValue(AnalyticRecord analyticRecord, string measureName)
        {
            MeasureValue measureValue;
            analyticRecord.MeasureValues.TryGetValue(measureName, out measureValue);
            return measureValue;
        }

        #endregion

        #region Private Classes

        class DurationBySupplier : Dictionary<int, decimal>
        {

        }

        class DurationByZone : Dictionary<long, DurationBySupplier>
        {

        }

        #endregion
    }
}
