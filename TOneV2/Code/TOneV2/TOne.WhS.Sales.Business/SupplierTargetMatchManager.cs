using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
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
                    ACD = supplierTargetMatch.ACD,
                    ASR = supplierTargetMatch.ASR,
                    Options = supplierTargetMatch.Options,
                    SaleZone = supplierTargetMatch.SaleZone,
                    TargetRates = supplierTargetMatch.TargetRates,
                    TargetVolume = supplierTargetMatch.TargetVolume,
                    SaleZoneId = supplierTargetMatch.SaleZoneId,
                    Volume = supplierTargetMatch.Volume
                };
            }

            public override IEnumerable<SupplierTargetMatch> RetrieveAllData(DataRetrievalInput<SupplierTargetMatchQuery> input)
            {
                RPRouteManager rpRouteManager = new RPRouteManager();
                List<RPZone> rpZones = GetRPZones(input);
                var rpRouteDetails = rpRouteManager.GetRPRoutes(input.Query.RoutingDataBaseId, input.Query.PolicyId, input.Query.NumberOfOptions, rpZones, true, false);

                List<SupplierTargetMatch> result = new List<SupplierTargetMatch>();

                ZoneAnalyticDetail zoneAnalyticDetails = GetAnalyticZoneDetails(input, rpZones);
                if (rpRouteDetails != null)
                {
                    foreach (var rpRouteDetail in rpRouteDetails)
                    {
                        SupplierAnalyticDetail supplierAnalyticDetail;
                        SupplierTargetMatch targetMatch = new SupplierTargetMatch
                        {
                            SaleZone = rpRouteDetail.SaleZoneName,
                            Options = rpRouteDetail.RouteOptionsDetails,
                            SaleZoneId = rpRouteDetail.SaleZoneId,
                        };

                        if (rpRouteDetail.RouteOptionsDetails != null)
                        {
                            if (zoneAnalyticDetails.TryGetValue(rpRouteDetail.SaleZoneId, out supplierAnalyticDetail))
                            {
                                TargetMatchCalculationMethodContext context = new TargetMatchCalculationMethodContext(input.Query.MarginValue, input.Query.MarginType)
                                {
                                    RPRouteDetail = rpRouteDetail,
                                    SupplierAnalyticDetail = supplierAnalyticDetail
                                };
                                List<SupplierTargetMatchAnalyticItem> supplierTargetMatchAnalyticItems = new List<SupplierTargetMatchAnalyticItem>();
								foreach (var rpDetail in rpRouteDetail.RouteOptionsDetails)
								{ SupplierTargetMatchAnalyticItem linkedrpRouteToZoneAnalytic;
									if (supplierAnalyticDetail.TryGetValue(rpDetail.SupplierId, out linkedrpRouteToZoneAnalytic))
									{
										//for (var i = 0; i < supplierAnalyticDetail.Count && i < input.Query.NumberOfOptions; i++)
										//{
										//	supplierTargetMatchAnalyticItems.Add(supplierAnalyticDetail.ElementAt(i).Value);
										//}
										//break;
										supplierTargetMatchAnalyticItems.Add(linkedrpRouteToZoneAnalytic);
									}
								}

                                if (supplierTargetMatchAnalyticItems.Count > 0)
                                {
                                    targetMatch.Volume = supplierTargetMatchAnalyticItems.Max(x => x.Duration);
                                    targetMatch.ACD = supplierTargetMatchAnalyticItems.Max(x => x.ACD);
                                    targetMatch.ASR = supplierTargetMatchAnalyticItems.Max(x => x.ASR);
                                    targetMatch.TargetVolume = supplierTargetMatchAnalyticItems.Max(x => x.Duration) * input.Query.VolumeMultiplier;
                                }
                                input.Query.CalculationMethod.Evaluate(context);
                                targetMatch.TargetRates = context.TargetRates;
                                result.Add(targetMatch);
                            }
                        }
                    }
                }
                return result;
            }

            protected override ResultProcessingHandler<SupplierTargetMatchDetail> GetResultProcessingHandler(DataRetrievalInput<SupplierTargetMatchQuery> input, BigResult<SupplierTargetMatchDetail> bigResult)
            {
                return new ResultProcessingHandler<SupplierTargetMatchDetail>
                {
                    ExportExcelHandler = new SupplierTargetMatchExportExcelHandler
                    {
                    }
                };
            }
            ZoneAnalyticDetail GetAnalyticZoneDetails(DataRetrievalInput<SupplierTargetMatchQuery> input, List<RPZone> rpZones)
            {
                var analyticResult = GetFilteredRecords(rpZones, input.Query.From, input.Query.To);
                ZoneAnalyticDetail zoneAnalyticDetails = new ZoneAnalyticDetail();
                if (analyticResult != null)
                {
                    foreach (var analyticRecord in analyticResult)
                    {
                        DimensionValue supplierDimension = analyticRecord.DimensionValues.ElementAt(0);
                        DimensionValue zoneDimension = analyticRecord.DimensionValues.ElementAt(1);

                        long zoneId = (long)zoneDimension.Value;
                        int? supplierId = supplierDimension.Value != null ? (int?)supplierDimension.Value : null;
                        var supplierDetails = zoneAnalyticDetails.GetOrCreateItem(zoneId);
                        if (supplierId.HasValue)
                        {
                            supplierDetails.GetOrCreateItem(supplierId.Value, () => new SupplierTargetMatchAnalyticItem
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

            List<AnalyticRecord> GetFilteredRecords(List<RPZone> rpZones, DateTime fromDate, DateTime? toDate)
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
                        TableId = Guid.Parse("58DD0497-498D-40F2-8687-08F8356C63CC"),
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
                return analyticManager.GetAllFilteredRecords(analyticQuery.Query);
            }

            List<RPZone> GetRPZones(DataRetrievalInput<SupplierTargetMatchQuery> input)
            {
                List<RPZone> rpZones = new List<RPZone>();
                SaleZoneManager saleZoneManager = new SaleZoneManager();
                List<SaleZone> saleZones = new List<SaleZone>();

                if (input.Query.CountryIds == null)
                {
                    saleZones.AddRange(saleZoneManager.GetSaleZonesEffectiveAfter(input.Query.SellingNumberPlanId, DateTime.Now));
                }
                else
                {
                    foreach (var countryId in input.Query.CountryIds)
                    {
                        saleZones.AddRange(saleZoneManager.GetSaleZonesByCountryId(input.Query.SellingNumberPlanId, countryId, DateTime.Now));
                    }
                }
                if (saleZones != null)
                {
                    foreach (var saleZone in saleZones)
                    {
                        RPZone rpZone = new RPZone
                        {
                            RoutingProductId = input.Query.RoutingProductId,
                            SaleZoneId = saleZone.SaleZoneId
                        };
                        rpZones.Add(rpZone);
                    }
                }
                return rpZones;
            }
        }

        class SupplierTargetMatchExportExcelHandler : ExcelExportHandler<SupplierTargetMatchDetail>
        {
            public override void ConvertResultToExcelData(IConvertResultToExcelDataContext<SupplierTargetMatchDetail> context)
            {
                DataRetrievalInput<SupplierTargetMatchQuery> input = context.Input as DataRetrievalInput<SupplierTargetMatchQuery>;
                input.ThrowIfNull("input");

                var sheet = new ExportExcelSheet()
                {
                    SheetName = "Supplier Target Generation",
                    Header = new ExportExcelHeader() { Cells = new List<ExportExcelHeaderCell>() }
                };

                sheet.Header.Cells.Add(new ExportExcelHeaderCell() { Title = "Sale Zone", Width = 40 });
                sheet.Header.Cells.Add(new ExportExcelHeaderCell() { Title = "Route Option", Width = 45 });
                sheet.Header.Cells.Add(new ExportExcelHeaderCell() { Title = "Target Rates", Width = 45 });
                sheet.Header.Cells.Add(new ExportExcelHeaderCell() { Title = "Volume" });
                sheet.Header.Cells.Add(new ExportExcelHeaderCell() { Title = "Target Volume" });
                if (input.Query.IncludeACD_ASR)
                {
                    sheet.Header.Cells.Add(new ExportExcelHeaderCell() { Title = "ASR" });
                    sheet.Header.Cells.Add(new ExportExcelHeaderCell() { Title = "ACD" });
                }
                sheet.Rows = new List<ExportExcelRow>();
                if (context.BigResult != null && context.BigResult.Data != null)
                {
                    foreach (var record in context.BigResult.Data)
                    {
                        if (record != null)
                        {
                            var row = new ExportExcelRow() { Cells = new List<ExportExcelCell>() };
                            row.Cells.Add(new ExportExcelCell() { Value = record.SaleZone });
                            StringBuilder routeOption = new StringBuilder();
                            StringBuilder targetRates = new StringBuilder();

                            if (record.Options != null)
                            {
                                for (var f = 0; f < record.Options.Count(); f++)
                                {
                                    routeOption.Append(string.Format("{0:0.00000000}", record.Options.ElementAt(f).SupplierRate) + " ");
                                }
                            }
                            if (record.TargetRates != null)
                            {
                                for (var f = 0; f < record.TargetRates.Count(); f++)
                                {
                                    targetRates.Append(string.Format("{0:0.00000000}", record.TargetRates.ElementAt(f)) + " ");
                                }
                            }
                            row.Cells.Add(new ExportExcelCell() { Value = routeOption });
                            row.Cells.Add(new ExportExcelCell() { Value = targetRates });
                            row.Cells.Add(new ExportExcelCell() { Value = record.Volume });
                            row.Cells.Add(new ExportExcelCell() { Value = (record.TargetVolume > input.Query.DefaultVolume) ? record.TargetVolume : input.Query.DefaultVolume });

                            if (input.Query.IncludeACD_ASR)
                            {
                                row.Cells.Add(new ExportExcelCell() { Value = record.ACD > input.Query.DefaultACD ? record.ACD : input.Query.DefaultACD });
                                row.Cells.Add(new ExportExcelCell() { Value = record.ASR > input.Query.DefaultASR ? record.ASR : input.Query.DefaultASR });
                            }
                            sheet.Rows.Add(row);
                        }
                    }
                }
                context.MainSheet = sheet;
            }
        }
    }

    public class TargetMatchCalculationMethodContext : ITargetMatchCalculationMethodContext
    {
        decimal _MarginValue;
        MarginType _MarginType;
        public TargetMatchCalculationMethodContext(decimal marginValue, MarginType marginType)
        {
            _MarginValue = marginValue;
            _MarginType = marginType;
        }
        public RPRouteDetailByZone RPRouteDetail { get; set; }

        public List<decimal> _targetRates = new List<decimal>();

        public List<decimal> TargetRates { get { return this._targetRates; } }

        public SupplierAnalyticDetail SupplierAnalyticDetail { get; set; }

        public decimal EvaluateRate(decimal originalRate)
        {
            decimal value = 0;
            switch (_MarginType)
            {
                case MarginType.Percentage:
                    value = originalRate * (100 - _MarginValue) / 100;
                    break;
                case MarginType.Fixed:
                    value = originalRate - _MarginValue;
                    break;
            }
            return value;
        }
    }
}
