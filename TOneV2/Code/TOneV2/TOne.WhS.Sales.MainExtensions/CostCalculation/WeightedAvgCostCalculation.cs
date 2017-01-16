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

namespace TOne.WhS.Sales.MainExtensions.CostCalculation
{
    public class WeightedAvgCostCalculation : CostCalculationMethod
    {
        public override Guid ConfigId { get { return new Guid("D71F6102-29B3-4781-8167-5C08282CCB5B"); } }
        public double PeriodValue { get; set; }
        public PeriodTypes PeriodType { get; set; }
        
        public override void CalculateCost(ICostCalculationMethodContext context)
        {
            List<string> listMeasures = new List<string> { "DurationInMinutes" };
            List<string> listDimensions = new List<string> { "Supplier" };
            string dimensionName = "Supplier";
            
            DateTime fromDate = DateTime.MinValue;
           
            switch (this.PeriodType)
            {
                case PeriodTypes.Days:
                    fromDate = DateTime.Today.AddDays(-this.PeriodValue);
                    break;
                case PeriodTypes.Hours:
                    fromDate = DateTime.Today.AddHours(-this.PeriodValue);
                    break;
                case PeriodTypes.Minutes:
                    fromDate = DateTime.Today.AddMinutes(-this.PeriodValue);
                    break;
            }
            
            DateTime toDate = DateTime.Today;

            Dictionary<int, decimal> ratesBySuppliers = new Dictionary<int, decimal>();

            if (context.Route == null || context.Route.RouteOptionsDetails == null)
                return;

            decimal sumOfDuration = 0;
            Dictionary<int, decimal> suppliersByDuration = new Dictionary<int, decimal>();

            foreach (RPRouteOptionDetail option in context.Route.RouteOptionsDetails)
            {
                if (!ratesBySuppliers.ContainsKey(option.Entity.SupplierId))
                    ratesBySuppliers.Add(option.Entity.SupplierId, option.ConvertedSupplierRate);

                var analyticResult = GetFilteredRecords(listDimensions, listMeasures, dimensionName, option.Entity.SupplierId, fromDate, toDate);
                if (analyticResult == null || analyticResult.Data == null)
                    continue;

                foreach (var analyticRecord in analyticResult.Data)
                {
                    MeasureValue durationInMinutes = GetMeasureValue(analyticRecord, "DurationInMinutes");
                    decimal durationInMinutesValue = Convert.ToDecimal(durationInMinutes.Value ?? 0.0);
                    if (durationInMinutesValue == 0)
                        continue;
                    
                    suppliersByDuration.Add(option.Entity.SupplierId, durationInMinutesValue);
                    sumOfDuration += durationInMinutesValue;
                }
            }

            Dictionary<int, decimal> suppliersByDurationPercentage = new Dictionary<int, decimal>();
            foreach (KeyValuePair<int, decimal> item in suppliersByDuration)
            {
                decimal supplierDurationPercentage = (item.Value * 100) / sumOfDuration;
                suppliersByDurationPercentage.Add(item.Key, supplierDurationPercentage);
            }

            decimal cost = 0;
            foreach (KeyValuePair<int, decimal> item in suppliersByDuration)
            {
                decimal supplierRate = ratesBySuppliers[item.Key];
                cost += (supplierRate * item.Value) / 100;
            }

            if (cost != 0)
                context.Cost = cost;
        }


        private AnalyticSummaryBigResult<AnalyticRecord> GetFilteredRecords(List<string> listDimensions, List<string> listMeasures, string dimensionFilterName, object dimensionFilterValue, DateTime fromDate, DateTime toDate)
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
                Dimension = dimensionFilterName,
                FilterValues = new List<object> { dimensionFilterValue }
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
    }
}
