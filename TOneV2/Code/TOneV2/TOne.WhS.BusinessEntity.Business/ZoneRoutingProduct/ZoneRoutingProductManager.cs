﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TOne.WhS.BusinessEntity.Data;
using TOne.WhS.BusinessEntity.Entities;
using TOne.WhS.Sales.Entities;
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
        public UpdateOperationOutput<ZoneRoutingProductDetail> UpdateZoneRoutingProduct(ZoneRoutingProductToEdit zoneRoutingProductToEdit)
        {
            UpdateOperationOutput<ZoneRoutingProductDetail> updateOperationOutput = new UpdateOperationOutput<ZoneRoutingProductDetail>
            {
                Result = UpdateOperationResult.Failed,
                UpdatedObject = null,
                ShowExactMessage = true
            };

            var zoneManager = new SaleZoneManager();
            var zoneName = zoneManager.GetSaleZoneName(zoneRoutingProductToEdit.ZoneId);

            string warningMessageOnDate = CheckDateCondition(zoneRoutingProductToEdit.ZoneEED, zoneRoutingProductToEdit.CountryEED, zoneRoutingProductToEdit.ZoneBED, zoneRoutingProductToEdit.CountryBED, zoneRoutingProductToEdit.BED, zoneName);

            if (zoneRoutingProductToEdit != null) return updateOperationOutput;

            var saleZoneRoutingProductToCloseList = new List<ZoneRoutingProductToChange>();

            IEnumerable<SaleZoneRoutingProduct> existingZoneRoutingProducts = GetZoneRoutingProduct(zoneRoutingProductToEdit.OwnerId,
                zoneRoutingProductToEdit.OwnerType, zoneRoutingProductToEdit.ZoneId, zoneRoutingProductToEdit.BED);

            DateTime closeDate;
            foreach (SaleZoneRoutingProduct existingRoutingProduct in existingZoneRoutingProducts)
            {
                closeDate = Utilities.Max(zoneRoutingProductToEdit.BED, existingRoutingProduct.BED);
                saleZoneRoutingProductToCloseList.Add(new ZoneRoutingProductToChange
                {
                    ZoneRoutingProductId = existingRoutingProduct.SaleEntityRoutingProductId,
                    EED = closeDate
                });
            }
            var saleEntityRoutingProductManager = new SaleEntityRoutingProductManager();
            long zoneRoutingProductReservedId = saleEntityRoutingProductManager.ReserveIdRange(1);

            var dataManager = BEDataManagerFactory.GetDataManager<ISaleEntityRoutingProductDataManager>();
            bool updateActionSucc = dataManager.Update(zoneRoutingProductToEdit, zoneRoutingProductReservedId, saleZoneRoutingProductToCloseList);
            if (updateActionSucc)
            {
                var routingProductManager = new RoutingProductManager();
                var routingProductName = routingProductManager.GetRoutingProductName(zoneRoutingProductToEdit.ChangedRoutingProductId);
                var servicesIds = routingProductManager.GetZoneServiceIds(zoneRoutingProductToEdit.ChangedRoutingProductId, zoneRoutingProductToEdit.ZoneId);

                updateOperationOutput.Result = UpdateOperationResult.Succeeded;


                ZoneRoutingProduct zoneRoutingProduct = new ZoneRoutingProduct
                {
                    ZoneId = zoneRoutingProductToEdit.ZoneId,
                    RoutingProductId = zoneRoutingProductToEdit.ChangedRoutingProductId,
                    SaleEntityZoneRoutingProductId = zoneRoutingProductReservedId,
                    BED = zoneRoutingProductToEdit.BED,
                    ServiceIds = servicesIds.ToList()
                };

                int? countryId = zoneManager.GetSaleZoneCountryId(zoneRoutingProductToEdit.ZoneId);
                if (countryId.HasValue)
                    zoneRoutingProduct.CountryId = countryId.Value;

                updateOperationOutput.UpdatedObject = new ZoneRoutingProductDetail
                {
                    Entity = zoneRoutingProduct,
                    RoutingProductName = routingProductName,
                    ZoneName = zoneName
                };
            }

            return updateOperationOutput;

        }

        #endregion

        #region private Methods

        private string CheckDateCondition(DateTime? zoneEED, DateTime? countryEED, DateTime zoneBED, DateTime countryBED, DateTime zoneRoutingProductBED, string zoneName)
        {
            if (zoneEED.HasValue)
                return string.Format("Cannot change zone routing product on closed zone {0}", zoneName);

            if (countryEED.HasValue)
                return string.Format("Cannot change zone routing product on closed country");

            if (zoneRoutingProductBED < zoneBED)
                return string.Format("Cannot edit routing product with a date less than zone BED {0}", zoneBED);

            if (zoneRoutingProductBED < countryBED)
                return string.Format("Cannot edit routing product with a date less than country BED {0}", countryBED);

            return null;
        }
        private IEnumerable<SaleZoneRoutingProduct> GetZoneRoutingProduct(int ownerId, SalePriceListOwnerType ownerType, long zoneId, DateTime effectiveDate)
        {
            SaleEntityRoutingProductManager saleEntityRoutingProductManager = new SaleEntityRoutingProductManager();
            var saleZoneroutingProducts = saleEntityRoutingProductManager.GetSaleZoneRoutingProductsEffectiveAfter(ownerType, ownerId, effectiveDate);
            return saleZoneroutingProducts.Where(rp => rp.SaleZoneId == zoneId);
        }
        private class ZoneRoutingProductHandler : BigDataRequestHandler<ZoneRoutingProductQuery, ZoneRoutingProduct, ZoneRoutingProductDetail>
        {
            RoutingProductManager _routingProductManager = new RoutingProductManager();
            SaleZoneManager _saleZoneManager = new SaleZoneManager();

            #region mapper
            private ZoneRoutingProduct ZoneRoutingProductMapper(SaleEntityZoneRoutingProduct saleEntityZoneRoutingProduct, long zoneId, DateTime zoneBED, int countryId, IEnumerable<int> serviceIds, bool IsInherited, DateTime countrySellDate, DateTime? countryEED)
            {
                List<DateTime> dates = new List<DateTime> { saleEntityZoneRoutingProduct.BED, zoneBED, countrySellDate };
                return new ZoneRoutingProduct
                {
                    BED = dates.Max<DateTime>(),
                    EED = saleEntityZoneRoutingProduct.EED,
                    SaleEntityZoneRoutingProductId = saleEntityZoneRoutingProduct.SaleEntityZoneRoutingProductId,
                    RoutingProductId = saleEntityZoneRoutingProduct.RoutingProductId,
                    ZoneId = zoneId,
                    CountryId = countryId,
                    ServiceIds = serviceIds.ToList(),
                    IsInherited = IsInherited,
                    CountryBED = countrySellDate,
                    CountryEED = countryEED
                };
            }
            public override ZoneRoutingProductDetail EntityDetailMapper(ZoneRoutingProduct entity)
            {
                SaleZone zone = _saleZoneManager.GetSaleZone(entity.ZoneId);
                return new ZoneRoutingProductDetail
                {
                    Entity = entity,
                    RoutingProductName = _routingProductManager.GetRoutingProductName(entity.RoutingProductId),
                    ZoneName = zone.Name,
                    ZoneBED = zone.BED,
                    ZoneEED = zone.EED
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
                            ZoneRoutingProduct routingProduct = ZoneRoutingProductMapper(saleEntityZoneRoutingProduct, saleZone.SaleZoneId, saleZone.BED, saleZone.CountryId, serviceIds, false, DateTime.MinValue, null);
                            zoneRoutingProducts.Add(routingProduct);
                        }
                    }
                }
                else
                {
                    CustomerCountryManager customerCountryManager = new CustomerCountryManager();
                    IEnumerable<CustomerCountry2> customerCountries = customerCountryManager.GetCustomerCountries(input.Query.OwnerId, input.Query.EffectiveOn, false);
                    Dictionary<int, DateTime> customerCountriesDatesById = new Dictionary<int, DateTime>();
                    Dictionary<int, DateTime?> customerCountriesEEDsById = new Dictionary<int, DateTime?>();
                    foreach (var customerCountrie in customerCountries)
                    {
                        customerCountriesDatesById.Add(customerCountrie.CountryId, customerCountrie.BED);
                        customerCountriesEEDsById.Add(customerCountrie.CountryId, customerCountrie.EED);
                    }

                    var customerSellingProductManager = new CustomerSellingProductManager();
                    var sellingProductId = customerSellingProductManager.GetEffectiveSellingProductId(input.Query.OwnerId,
                            input.Query.EffectiveOn, false);
                    if (!sellingProductId.HasValue)
                        return zoneRoutingProducts;

                    foreach (SaleZone saleZone in filteredSaleZone)
                    {
                        DateTime countrySellDate = customerCountriesDatesById.GetRecord(saleZone.CountryId);
                        DateTime? countryEED = customerCountriesEEDsById.GetRecord(saleZone.CountryId);

                        SaleEntityZoneRoutingProduct zoneRoutingProduct = routingProductLocator.GetCustomerZoneRoutingProduct(input.Query.OwnerId,
                                sellingProductId.Value, saleZone.SaleZoneId);

                        if (zoneRoutingProduct != null)
                        {
                            var serviceIds = _routingProductManager.GetZoneServiceIds(zoneRoutingProduct.RoutingProductId, saleZone.SaleZoneId);
                            ZoneRoutingProduct routingProduct = ZoneRoutingProductMapper(zoneRoutingProduct,
                                saleZone.SaleZoneId, saleZone.BED, saleZone.CountryId, serviceIds,
                                zoneRoutingProduct.Source != SaleEntityZoneRoutingProductSource.CustomerZone, countrySellDate, countryEED);
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
                            row.Cells.Add(new ExportExcelCell { Value = record.Entity.SaleEntityZoneRoutingProductId });
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
                return zoneRoutingProduct.SaleEntityZoneRoutingProductId;
            }

            public override string GetObjectName(IVRLoggableEntityGetObjectNameContext context)
            {
                return null;
            }
        }
    }
}
