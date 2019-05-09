using System;
using System.Collections.Generic;
using System.Linq;
using TOne.WhS.Routing.Entities;
using TOne.WhS.Sales.Entities;
using Vanrise.Analytic.Business;
using Vanrise.Analytic.Entities;
using Vanrise.Common;
using Vanrise.Entities;

namespace TOne.WhS.Sales.MainExtensions.CostCalculation
{
    public class WeightedAvgCostCalculation : CostCalculationMethod
    {
        public override Guid ConfigId { get { return new Guid("D71F6102-29B3-4781-8167-5C08282CCB5B"); } }
        public decimal PeriodValue { get; set; }
        public PeriodTypes PeriodType { get; set; }

        public override void CalculateCost(ICostCalculationMethodContext context)
        {
            if (context.Route == null)
                return;

            if (context.Route.RouteOptionsDetails == null || context.Route.RouteOptionsDetails.Count() == 0)
                return;

            var filteredRouteOptionDetails = context.Route.RouteOptionsDetails.FindAllRecords(itm => itm.SupplierRate.HasValue);

            if (filteredRouteOptionDetails == null || filteredRouteOptionDetails.Count() == 0)
                return;

            if (context.CustomObject == null)
                context.CustomObject = this.GetDurationByZone(context.ZoneIds);

            DurationByZone durationByZone = context.CustomObject as DurationByZone;

            decimal sumOfDuration = 0;
            decimal sumOfRatesMultipliedByDuration = 0;

            IEnumerable<RPRouteOptionDetail> routeOptionsDetails = new List<RPRouteOptionDetail>();

            if (context.NumberOfOptions.HasValue && filteredRouteOptionDetails.Count() >= context.NumberOfOptions.Value)
                routeOptionsDetails = filteredRouteOptionDetails.Take(context.NumberOfOptions.Value);
            else
                routeOptionsDetails = filteredRouteOptionDetails;

            foreach (RPRouteOptionDetail option in routeOptionsDetails)
            {
                DurationBySupplier durationBySupplier = null;
                if (durationByZone.TryGetValue(option.SaleZoneId, out durationBySupplier))
                {
                    decimal durationInMinutes = 0;
                    if (durationBySupplier.TryGetValue(option.SupplierId, out durationInMinutes))
                    {
                        sumOfRatesMultipliedByDuration += durationInMinutes * option.ConvertedSupplierRate.Value;
                        sumOfDuration += durationInMinutes;
                    }
                }
            }

            decimal? weightedAverageCost = null;

            if (sumOfDuration != 0)
                weightedAverageCost = sumOfRatesMultipliedByDuration / sumOfDuration;

            if (weightedAverageCost.HasValue && weightedAverageCost.Value > 0)
                context.Cost = weightedAverageCost.Value;
            else
                new AvgCostCalculation().CalculateCost(context);
        }

        #region Private Methods
        private DurationByZone GetDurationByZone(IEnumerable<long> zoneIds)
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

            var analyticResult = GetFilteredRecords(listDimensions, listMeasures, zoneIds, fromDate, toDate);

            DurationByZone durationByZone = new DurationByZone();

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

            return durationByZone;
        }

        private AnalyticSummaryBigResult<AnalyticRecord> GetFilteredRecords(List<string> listDimensions, List<string> listMeasures, IEnumerable<long> saleZoneIds, DateTime fromDate, DateTime toDate)
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
                FilterValues = saleZoneIds.Cast<object>().ToList()

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
    }

    class DurationBySupplier : Dictionary<int, decimal>
    {

    }

    class DurationByZone : Dictionary<long, DurationBySupplier>
    {

    }
}