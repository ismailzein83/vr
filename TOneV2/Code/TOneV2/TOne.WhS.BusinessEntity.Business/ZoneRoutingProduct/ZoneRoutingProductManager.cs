using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TOne.WhS.BusinessEntity.Entities;
using Vanrise.Common;
using Vanrise.Common.Business;
using Vanrise.Entities;

namespace TOne.WhS.BusinessEntity.Business
{
    public class ZoneRoutingProductManager
    {
        #region public Methods
        public IDataRetrievalResult<ZoneRoutingProductDetail> GetFilteredZoneRoutingProducts(DataRetrievalInput<ZoneRoutingProductQuery> input)
        {
            VRActionLogger.Current.LogGetFilteredAction(ZoneRoutingProductLoggableEntity.Instance, input);
            return BigDataManager.Instance.RetrieveData(input, new ZoneRoutingProductHandler());
        }
        #endregion

        #region private Methods
        private class ZoneRoutingProductHandler : BigDataRequestHandler<ZoneRoutingProductQuery, ZoneRoutingProduct, ZoneRoutingProductDetail>
        {
            RoutingProductManager _routingProductManager = new RoutingProductManager();
            SaleZoneManager _saleZoneManager = new SaleZoneManager();

            #region mapper
            private ZoneRoutingProduct ZoneRoutingProductMapper(SaleEntityZoneRoutingProduct saleEntityZoneRoutingProduct, long zoneId, DateTime zoneBED, int countryId, IEnumerable<int> serviceIds, bool IsInherited)
            {
                return new ZoneRoutingProduct
                {
                    BED = saleEntityZoneRoutingProduct.BED > zoneBED ? saleEntityZoneRoutingProduct.BED : zoneBED,
                    EED = saleEntityZoneRoutingProduct.EED,
                    ZoneRoutingProductId = saleEntityZoneRoutingProduct.RoutingProductId,
                    ZoneId = zoneId,
                    CountryId = countryId,
                    ServiceIds = serviceIds.ToList(),
                    IsInherited = IsInherited
                };
            }
            public override ZoneRoutingProductDetail EntityDetailMapper(ZoneRoutingProduct entity)
            {
                return new ZoneRoutingProductDetail
                {
                    Entity = entity,
                    RoutingProductName = _routingProductManager.GetRoutingProductName(entity.ZoneRoutingProductId),
                    ZoneName = _saleZoneManager.GetSaleZoneName(entity.ZoneId)
                };
            }

            #endregion

            #region public Methods
            public override IEnumerable<ZoneRoutingProduct> RetrieveAllData(DataRetrievalInput<ZoneRoutingProductQuery> input)
            {
                List<ZoneRoutingProduct> zoneRoutingProducts = new List<ZoneRoutingProduct>();
                IEnumerable<SaleZone> saleZones = (input.Query.SellingNumberPlanId.HasValue) ?
                    _saleZoneManager.GetSaleZonesByOwner(input.Query.OwnerType, input.Query.OwnerId, input.Query.SellingNumberPlanId.Value, input.Query.EffectiveOn, false) :
                    _saleZoneManager.GetSaleZonesByOwner(input.Query.OwnerType, input.Query.OwnerId, input.Query.EffectiveOn, false);
                var routingProductLocator = new SaleEntityZoneRoutingProductLocator(new SaleEntityRoutingProductReadWithCache(input.Query.EffectiveOn));
                if (saleZones == null) return zoneRoutingProducts;
                var filteredSaleZone = saleZones.FindAllRecords(sz => input.Query.ZonesIds == null || input.Query.ZonesIds.Contains(sz.SaleZoneId));
                if (input.Query.OwnerType == SalePriceListOwnerType.SellingProduct)
                {
                    foreach (SaleZone saleZone in filteredSaleZone)
                    {
                        SaleEntityZoneRoutingProduct saleEntityZoneRoutingProduct =
                            routingProductLocator.GetSellingProductZoneRoutingProduct(input.Query.OwnerId,
                                saleZone.SaleZoneId);
                        if (saleEntityZoneRoutingProduct != null)
                        {
                            var serviceIds = _routingProductManager.GetZoneServiceIds(
                                saleEntityZoneRoutingProduct.RoutingProductId, saleZone.SaleZoneId);
                            ZoneRoutingProduct routingProduct = ZoneRoutingProductMapper(saleEntityZoneRoutingProduct, saleZone.SaleZoneId, saleZone.BED, saleZone.CountryId, serviceIds, false);
                            zoneRoutingProducts.Add(routingProduct);
                        }
                    }
                }
                else
                {
                    var customerSellingProductManager = new CustomerSellingProductManager();
                    var sellingProductId =
                        customerSellingProductManager.GetEffectiveSellingProductId(input.Query.OwnerId,
                            input.Query.EffectiveOn, false);
                    if (!sellingProductId.HasValue)
                        return zoneRoutingProducts;

                    foreach (SaleZone saleZone in filteredSaleZone)
                    {
                        SaleEntityZoneRoutingProduct zoneRoutingProduct =
                            routingProductLocator.GetCustomerZoneRoutingProduct(input.Query.OwnerId,
                                sellingProductId.Value, saleZone.SaleZoneId);
                        if (zoneRoutingProduct != null)
                        {
                            var serviceIds = _routingProductManager.GetZoneServiceIds(
                                zoneRoutingProduct.RoutingProductId, saleZone.SaleZoneId);
                            ZoneRoutingProduct routingProduct = ZoneRoutingProductMapper(zoneRoutingProduct,
                                saleZone.SaleZoneId, saleZone.BED, saleZone.CountryId, serviceIds,
                                zoneRoutingProduct.Source != SaleEntityZoneRoutingProductSource.CustomerZone);
                            zoneRoutingProducts.Add(routingProduct);
                        }
                    }
                }
                return zoneRoutingProducts;
            }

            protected override ResultProcessingHandler<ZoneRoutingProductDetail> GetResultProcessingHandler(DataRetrievalInput<ZoneRoutingProductQuery> input, BigResult<ZoneRoutingProductDetail> bigResult)
            {
                return new ResultProcessingHandler<ZoneRoutingProductDetail>
                {
                    ExportExcelHandler = new ZoneRoutingProductExcelExportHandler()
                };
            }
            #endregion
        }

        private class ZoneRoutingProductExcelExportHandler : ExcelExportHandler<ZoneRoutingProductDetail>
        {
            public override void ConvertResultToExcelData(IConvertResultToExcelDataContext<ZoneRoutingProductDetail> context)
            {
                ZoneServiceConfigManager zoneServiceConfigManager = new ZoneServiceConfigManager();

                ExportExcelSheet sheet = new ExportExcelSheet()
                {
                    SheetName = "Zone Routing Products",
                    Header = new ExportExcelHeader { Cells = new List<ExportExcelHeaderCell>() }
                };

                sheet.Header.Cells.Add(new ExportExcelHeaderCell { Title = "ID" });
                sheet.Header.Cells.Add(new ExportExcelHeaderCell { Title = "Zone", Width = 30 });
                sheet.Header.Cells.Add(new ExportExcelHeaderCell { Title = "Routing Product" });
                sheet.Header.Cells.Add(new ExportExcelHeaderCell { Title = "Services" });
                sheet.Header.Cells.Add(new ExportExcelHeaderCell { Title = "BED", CellType = ExcelCellType.DateTime, DateTimeType = DateTimeType.Date });
                sheet.Header.Cells.Add(new ExportExcelHeaderCell { Title = "EED", CellType = ExcelCellType.DateTime, DateTimeType = DateTimeType.Date });

                sheet.Rows = new List<ExportExcelRow>();
                if (context.BigResult != null && context.BigResult.Data != null)
                {
                    foreach (var record in context.BigResult.Data)
                    {
                        if (record.Entity != null)
                        {
                            var row = new ExportExcelRow { Cells = new List<ExportExcelCell>() };
                            sheet.Rows.Add(row);
                            row.Cells.Add(new ExportExcelCell { Value = record.Entity.ZoneRoutingProductId });
                            row.Cells.Add(new ExportExcelCell { Value = record.ZoneName });
                            row.Cells.Add(new ExportExcelCell { Value = record.RoutingProductName });
                            row.Cells.Add(new ExportExcelCell { Value = record.Entity.ServiceIds == null ? "" : zoneServiceConfigManager.GetZoneServicesNames(record.Entity.ServiceIds) });
                            row.Cells.Add(new ExportExcelCell { Value = record.Entity.BED });
                            row.Cells.Add(new ExportExcelCell { Value = record.Entity.EED });
                        }
                    }
                }
                context.MainSheet = sheet;
            }
        }
        #endregion

        private class ZoneRoutingProductLoggableEntity : VRLoggableEntityBase
        {
            public static ZoneRoutingProductLoggableEntity Instance = new ZoneRoutingProductLoggableEntity();

            private ZoneRoutingProductLoggableEntity()
            {

            }

            static ZoneRoutingProductManager s_zoneRoutingProductManager = new ZoneRoutingProductManager();

            public override string EntityUniqueName
            {
                get { return "WhS_BusinessEntity_ZoneRoutingProduct"; }
            }

            public override string ModuleName
            {
                get { return "Business Entity"; }
            }

            public override string EntityDisplayName
            {
                get { return "Zone Routing Product"; }
            }

            public override string ViewHistoryItemClientActionName
            {
                get { return "WhS_BusinessEntity_ZoneRoutingProduct_ViewHistoryItem"; }
            }

            public override object GetObjectId(IVRLoggableEntityGetObjectIdContext context)
            {
                ZoneRoutingProduct zoneRoutingProduct = context.Object.CastWithValidate<ZoneRoutingProduct>("context.Object");
                return zoneRoutingProduct.ZoneRoutingProductId;
            }

            public override string GetObjectName(IVRLoggableEntityGetObjectNameContext context)
            {
                return null;
            }
        }
    }
}
