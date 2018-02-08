using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TOne.WhS.BusinessEntity.Business;
using TOne.WhS.Routing.Entities;
using TOne.WhS.Sales.Entities;
using Vanrise.Analytic.Business;
using Vanrise.Analytic.Entities;
using Vanrise.Entities;
using Vanrise.Common;
using TOne.WhS.BusinessEntity.Entities;

namespace TOne.WhS.Sales.Business
{
    public class TQIManager
    {
        private CarrierAccountManager _carrierAccountManager = new CarrierAccountManager();

        private Dictionary<int, decimal> _ratesBySupplier = new Dictionary<int, decimal>();
        public TQIEvaluatedRate Evaluate(TQIMethod tqiMethod, RPRouteDetail rpRouteDetail)
        {
            var context = new TQIMethodContext()
            {
                Route = rpRouteDetail,
                LongPrecision = new Vanrise.Common.Business.GeneralSettingsManager().GetLongPrecisionValue()
            };
            tqiMethod.CalculateRate(context);
            return new TQIEvaluatedRate() { EvaluatedRate = context.Rate };
        }

        public TQISupplierInfoWithSummary GetTQISuppliersInfo(TQISupplierInfoQuery input)
        {
            if (input.RPRouteDetail == null)
                return null;

            StructureRatesBySupplier(input.RPRouteDetail);
            AnalyticManager analyticManager = new AnalyticManager();
            List<string> listMeasures = new List<string> { "DurationInMinutes", "ACD", "ASR", "NER" };
            List<string> listDimensions = new List<string> { "Supplier", "SaleZone" };
            string supplierDimensionFilterName = "Supplier";
            string saleZoneDimensionFilterName = "SaleZone";

            DateTime fromDate = DateTime.MinValue;

            switch (input.PeriodType)
            {
                case PeriodTypes.Days:
                    fromDate = DateTime.Today.AddDays(-(double)input.PeriodValue);
                    break;
                case PeriodTypes.Hours:
                    fromDate = DateTime.Today.AddHours(-(double)input.PeriodValue);
                    break;
                case PeriodTypes.Minutes:
                    fromDate = DateTime.Today.AddMinutes(-(double)input.PeriodValue);
                    break;
                default:
                    throw new DataIntegrityValidationException(string.Format("Period Type must be set"));
            }

            DateTime toDate = DateTime.Today;

            IEnumerable<int> supplierIds = input.RPRouteDetail.RouteOptionsDetails.Select(item => item.SupplierId);

            var analyticResult = GetFilteredRecords(listDimensions, listMeasures, supplierDimensionFilterName, supplierIds, saleZoneDimensionFilterName, input.RPRouteDetail.SaleZoneId, fromDate, toDate);
            if (analyticResult == null || analyticResult.Data == null)
                return null;

            IEnumerable<TQISuppplierInfo> suppliersInfo = analyticResult.Data.MapRecords(TQISupplierInfoMapper);

            TQISupplierInfoWithSummary suppliersInfoWithSummary = new TQISupplierInfoWithSummary();
            suppliersInfoWithSummary.SuppliersInfo = suppliersInfo;
            suppliersInfoWithSummary.TotalDurationInMinutesSummary = GetSuppliersInfoSummary(analyticResult.Summary);

            return suppliersInfoWithSummary;
        }

        private decimal? GetSuppliersInfoSummary(AnalyticRecord analyticRecord)
        {
            if (analyticRecord != null)
            {
                MeasureValue durationInMinutesMeasure = GetMeasureValue(analyticRecord, "DurationInMinutes");
                return Convert.ToDecimal(durationInMinutesMeasure.Value);
            }

            return null;
        }

        private void StructureRatesBySupplier(RPRouteDetail rpRouteDetail)
        {
            if (rpRouteDetail != null && rpRouteDetail.RouteOptionsDetails != null)
            {
                foreach (RPRouteOptionDetail route in rpRouteDetail.RouteOptionsDetails)
                {
                    if (!_ratesBySupplier.ContainsKey(route.SupplierId))
                        _ratesBySupplier.Add(route.SupplierId, route.ConvertedSupplierRate);
                }
            }
        }


        #region Private Methods

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
                    TableId = Guid.Parse("58DD0497-498D-40F2-8687-08F8356C63CC"),
                    FromTime = fromDate,
                    ToTime = toDate,
                    ParentDimensions = new List<string>(),
                    Filters = new List<DimensionFilter>(),
                    WithSummary = true
                },
                SortByColumnName = "DimensionValues[0].Name"
            };

            List<object> firstDimensionFilterValues = new List<object>();
            foreach (int supplierId in firstDimensionFilterValue as IEnumerable<int>)
            {
                firstDimensionFilterValues.Add(supplierId);
            }
            DimensionFilter dimensionFilter = new DimensionFilter()
            {
                Dimension = firstDimensionFilterName,
                FilterValues = firstDimensionFilterValues

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


        TQISuppplierInfo TQISupplierInfoMapper(AnalyticRecord analyticRecord)
        {
            DimensionValue supplierDimension = analyticRecord.DimensionValues.ElementAtOrDefault(0);
            int supplierId = Convert.ToInt16(supplierDimension.Value);

            MeasureValue durationInMinutesMeasure = GetMeasureValue(analyticRecord, "DurationInMinutes");
            decimal durationInMinutes = Convert.ToDecimal(durationInMinutesMeasure.Value ?? 0.0);

            MeasureValue acdMeasure = GetMeasureValue(analyticRecord, "ACD");
            decimal acd = Convert.ToDecimal(acdMeasure.Value ?? 0.0);

            MeasureValue asrMeasure = GetMeasureValue(analyticRecord, "ASR");
            decimal asr = Convert.ToDecimal(asrMeasure.Value ?? 0.0);

            MeasureValue nerMeasure = GetMeasureValue(analyticRecord, "NER");
            decimal ner = Convert.ToDecimal(nerMeasure.Value ?? 0.0);

            decimal supplierRate = _ratesBySupplier[supplierId];

            return new TQISuppplierInfo()
            {
                SupplierId = supplierId,
                SupplierName = _carrierAccountManager.GetCarrierAccountName(supplierId),
                ACD = acd,
                ASR = asr,
                NER = ner,
                TQI = acd * asr,
                Duration = durationInMinutes,
                Rate = supplierRate
            };
        }

        #endregion
    }
}
