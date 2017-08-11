using System;
using System.Collections.Generic;
using System.Linq;
using TOne.WhS.BusinessEntity.Business;
using TOne.WhS.BusinessEntity.Entities;
using TOne.WhS.Routing.Business;
using TOne.WhS.Routing.Entities;
using TOne.WhS.Sales.Entities;
using Vanrise.Analytic.Business;
using Vanrise.Analytic.Entities;
using Vanrise.Common;
using Vanrise.Common.Business;
using Vanrise.Entities;

namespace TOne.WhS.Sales.Business
{
    public class SupplierTargetMatchManager
    {

        public IDataRetrievalResult<SupplierTargetMatchDetail> GetFilteredSupplierTargetMatches(Vanrise.Entities.DataRetrievalInput<SupplierTargetMatchQuery> input)
        {
            return BigDataManager.Instance.RetrieveData(input, new SupplierTargetMatchRequestHandler());
        }

        public IEnumerable<SupplierTargetMatchMethodConfig> GetTargetMatchMethodConfigs()
        {
            var extensionConfigManager = new ExtensionConfigurationManager();
            return extensionConfigManager.GetExtensionConfigurations<SupplierTargetMatchMethodConfig>(SupplierTargetMatchMethodConfig.EXTENSION_TYPE).OrderBy(x => x.Title);
        }

        class SupplierTargetMatchRequestHandler : BigDataRequestHandler<SupplierTargetMatchQuery, SupplierTargetMatch, SupplierTargetMatchDetail>
        {
            public override SupplierTargetMatchDetail EntityDetailMapper(SupplierTargetMatch supplierTargetMatch)
            {
                return new SupplierTargetMatchDetail
                {
                    Entity = supplierTargetMatch
                };
            }

            public override IEnumerable<SupplierTargetMatch> RetrieveAllData(DataRetrievalInput<SupplierTargetMatchQuery> input)
            {
                RPRouteManager rpRouteManager = new RPRouteManager();
                List<RPZone> rpZones = GetRPZones(input);
                var rpRouteDetails = rpRouteManager.GetRPRoutes(input.Query.Filter.RoutingDataBaseId, input.Query.Filter.PolicyId, input.Query.Filter.NumberOfOptions, rpZones);

                List<SupplierTargetMatch> result = new List<SupplierTargetMatch>();

                ZoneAnalyticDetail zoneAnalyticDetails = GetAnalyticZoneDetails(input, rpZones);

                foreach (var rpRouteDetail in rpRouteDetails)
                {
                    SupplierAnalyticDetail supplierAnalyticDetail;
                    SupplierTargetMatch targetMatch = new SupplierTargetMatch
                    {
                        SaleZone = rpRouteDetail.SaleZoneName,
                        Options = rpRouteDetail.RouteOptionsDetails,
                        SaleZoneId = rpRouteDetail.SaleZoneId
                    };

                    if (rpRouteDetail.RouteOptionsDetails != null)
                    {
                        if (zoneAnalyticDetails.TryGetValue(rpRouteDetail.SaleZoneId, out supplierAnalyticDetail))
                        {
                            TargetMatchCalculationMethodContext context = new TargetMatchCalculationMethodContext(supplierAnalyticDetail)
                            {
                                RPRouteDetail = rpRouteDetail,
                                MarginType = input.Query.Settings.MarginType,
                                MarginValue = input.Query.Settings.MarginValue

                            };

                            input.Query.Settings.CalculationMethod.Evaluate(context);

                            targetMatch.TargetOptions = context.Options;

                            foreach (var supplierOption in rpRouteDetail.RouteOptionsDetails)
                            {
                                SupplierTargetMatchAnalyticOption supplierAnalyticInfo;

                                if (supplierAnalyticDetail.TryGetValue(supplierOption.Entity.SupplierId, out supplierAnalyticInfo))
                                {
                                    supplierOption.ASR = supplierAnalyticInfo.ASR;
                                    supplierOption.ACD = supplierAnalyticInfo.ACD;
                                    supplierOption.Duration = supplierAnalyticInfo.Duration;

                                    targetMatch.Volume += supplierAnalyticInfo.Duration;
                                }
                            }
                            targetMatch.TargetVolume = input.Query.Settings.VolumeMultiplier * targetMatch.Volume;
                        }
                    }
                    if (targetMatch.TargetVolume < input.Query.Settings.DefaultVolume)
                        targetMatch.TargetVolume = input.Query.Settings.DefaultVolume;
                    result.Add(targetMatch);
                }

                return result;
            }

            ZoneAnalyticDetail GetAnalyticZoneDetails(DataRetrievalInput<SupplierTargetMatchQuery> input, List<RPZone> rpZones)
            {
                var analyticResult = GetFilteredRecords(rpZones, input.Query.Filter.From, input.Query.Filter.To);
                ZoneAnalyticDetail zoneAnalyticDetails = new ZoneAnalyticDetail();
                if (analyticResult != null)
                {

                    foreach (var analyticRecord in analyticResult.Data)
                    {
                        DimensionValue supplierDimension = analyticRecord.DimensionValues.ElementAt(0);
                        DimensionValue zoneDimension = analyticRecord.DimensionValues.ElementAt(1);

                        long zoneId = (long)zoneDimension.Value;
                        int? supplierId = supplierDimension.Value != null ? (int?)supplierDimension.Value : null;
                        var supplierDetails = zoneAnalyticDetails.GetOrCreateItem(zoneId);
                        if (supplierId.HasValue)
                        {
                            supplierDetails.GetOrCreateItem(supplierId.Value, () => new SupplierTargetMatchAnalyticOption
                            {
                                Duration = GetDecimalMeasureValue(analyticRecord, "DurationInMinutes"),
                                ACD = GetDecimalMeasureValue(analyticRecord, "ACD"),
                                ASR = GetDecimalMeasureValue(analyticRecord, "ASR")
                            });
                        }
                    }
                }
                return zoneAnalyticDetails;
            }

            decimal GetDecimalMeasureValue(AnalyticRecord analyticRecord, string measureName)
            {
                MeasureValue measureValue;
                analyticRecord.MeasureValues.TryGetValue(measureName, out measureValue);
                return Convert.ToDecimal(measureValue.Value ?? 0.0);
            }

            AnalyticSummaryBigResult<AnalyticRecord> GetFilteredRecords(List<RPZone> rpZones, DateTime fromDate, DateTime? toDate)
            {
                List<string> listMeasures = new List<string> { "DurationInMinutes", "ASR", "ACD" };
                List<string> listDimensions = new List<string> { "Supplier", "SaleZone" };

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
                    Dimension = "SaleZone",
                    FilterValues = rpZones.Select(z => z.SaleZoneId).Cast<object>().ToList()

                };

                analyticQuery.Query.Filters.Add(dimensionFilter);
                return analyticManager.GetFilteredRecords(analyticQuery) as Vanrise.Analytic.Entities.AnalyticSummaryBigResult<AnalyticRecord>;
            }

            List<RPZone> GetRPZones(DataRetrievalInput<SupplierTargetMatchQuery> input)
            {
                List<RPZone> rpZones = new List<RPZone>();
                SaleZoneManager saleZoneManager = new SaleZoneManager();
                List<SaleZone> saleZones = new List<SaleZone>();

                if (input.Query.Filter.CountryIds == null)
                {
                    saleZones.AddRange(saleZoneManager.GetSaleZonesEffectiveAfter(input.Query.Filter.SellingNumberPlanId, DateTime.Now));
                }
                else
                {
                    foreach (var countryId in input.Query.Filter.CountryIds)
                    {
                        saleZones.AddRange(saleZoneManager.GetSaleZonesByCountryId(input.Query.Filter.SellingNumberPlanId, countryId, DateTime.Now));
                    }
                }

                foreach (var saleZone in saleZones)
                {
                    RPZone rpZone = new RPZone
                    {
                        RoutingProductId = input.Query.Filter.RoutingProductId,
                        SaleZoneId = saleZone.SaleZoneId
                    };
                    rpZones.Add(rpZone);
                }
                return rpZones;
            }
        }
    }

    public class TargetMatchCalculationMethodContext : ITargetMatchCalculationMethodContext
    {
        Dictionary<int, SupplierTargetMatchAnalyticOption> _SupplierAnalyticOptions;
        public TargetMatchCalculationMethodContext(Dictionary<int, SupplierTargetMatchAnalyticOption> supplierAnalyticOptions)
        {
            _SupplierAnalyticOptions = supplierAnalyticOptions;
        }
        public RPRouteDetail RPRouteDetail
        {
            get;
            set;
        }

        public decimal MarginValue
        {
            get;
            set;
        }

        public MarginType MarginType
        {
            get;
            set;
        }

        public List<SupplierTargetMatchAnalyticOption> Options
        {
            get;
            set;
        }

        public SupplierTargetMatchAnalyticOption GetSupplierAnalyticInfo(int supplierId)
        {
            SupplierTargetMatchAnalyticOption supplierTargetMatchAnalyticOption;
            _SupplierAnalyticOptions.TryGetValue(supplierId, out supplierTargetMatchAnalyticOption);
            return supplierTargetMatchAnalyticOption;
        }


        public decimal EvaluateRate(decimal originalRate)
        {
            decimal value = 0;
            switch (MarginType)
            {
                case MarginType.Percentage:
                    value = originalRate * (100 - MarginValue) / 100;
                    break;
                case MarginType.Fixed:
                    value = MarginValue;
                    break;
            }
            return value;
        }
    }
}
