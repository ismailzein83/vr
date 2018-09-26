using System;
using System.Collections.Generic;
using System.Linq;
using TOne.WhS.BusinessEntity.Data;
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

			if (warningMessageOnDate != null)
			{
				updateOperationOutput.Message = warningMessageOnDate;
				return updateOperationOutput;
			}

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
				updateOperationOutput.Message = string.Format("Routing product for zone {0} updated successfully", zoneName);


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
					ZoneName = zoneName,
					ZoneBED = zoneRoutingProductToEdit.ZoneBED,
					ZoneEED = zoneRoutingProductToEdit.ZoneEED
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
			#region Fields
			RoutingProductManager _routingProductManager = new RoutingProductManager();
			SaleZoneManager _saleZoneManager = new SaleZoneManager();
			#endregion

			#region mapper
			private ZoneRoutingProduct ZoneRoutingProductMapper(SaleEntityZoneRoutingProductHistoryRecord saleEntityZoneRoutingProductHistoryRecord, long zoneId, DateTime zoneBED, int countryId, IEnumerable<int> serviceIds, bool IsInherited, DateTime countrySellDate, DateTime? countryEED)
			{
				List<DateTime> dates = new List<DateTime> { saleEntityZoneRoutingProductHistoryRecord.BED, zoneBED, countrySellDate };
				return new ZoneRoutingProduct
				{
					BED = dates.Max<DateTime>(),
					EED = saleEntityZoneRoutingProductHistoryRecord.EED,
					SaleEntityZoneRoutingProductId = saleEntityZoneRoutingProductHistoryRecord.SaleEntityId,
					RoutingProductId = saleEntityZoneRoutingProductHistoryRecord.RoutingProductId,
					ZoneId = zoneId,
					CountryId = countryId,
					ServiceIds = serviceIds.ToList(),
					IsInherited = IsInherited
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

			public override IEnumerable<ZoneRoutingProduct> RetrieveAllData(Vanrise.Entities.DataRetrievalInput<ZoneRoutingProductQuery> input)
			{
				IEnumerable<SaleZone> filteredSaleZones = GetFilteredOwnerSaleZones(input.Query.SellingNumberPlanId, input.Query.OwnerType, input.Query.OwnerId, input.Query.EffectiveOn, input.Query.ZonesIds);
				if (filteredSaleZones == null || filteredSaleZones.Count() == 0)
					return null;

				IEnumerable<long> filteredSaleZoneIds = filteredSaleZones.Select(item => item.SaleZoneId);

				if (input.Query.OwnerType == SalePriceListOwnerType.SellingProduct)
					return GetSellingProductZoneRoutingProduct(input.Query.OwnerId, filteredSaleZones, filteredSaleZoneIds, input.Query.EffectiveOn);

				else
					return GetCustomerZoneRoutingProduct(input.Query.OwnerId, filteredSaleZones, filteredSaleZoneIds, input.Query.EffectiveOn);
			}

			#region Private Methods
			private IEnumerable<SaleZone> GetFilteredOwnerSaleZones(int? sellingNumberPlanId, SalePriceListOwnerType ownerType, int ownerId, DateTime effectiveOn, List<long> zonesIds)
			{
				IEnumerable<SaleZone> saleZones = (sellingNumberPlanId.HasValue) ?
					_saleZoneManager.GetSaleZonesByOwner(ownerType, ownerId, sellingNumberPlanId.Value, effectiveOn, false) :
					_saleZoneManager.GetSaleZonesByOwner(ownerType, ownerId, effectiveOn, false);
				if (saleZones == null)
					return null;
				var filteredSaleZone = saleZones.FindAllRecords(item => zonesIds == null || zonesIds.Contains(item.SaleZoneId));
				return filteredSaleZone;
			}
			private IEnumerable<ZoneRoutingProduct> GetSellingProductZoneRoutingProduct(int sellingProductId, IEnumerable<SaleZone> filteredSaleZones, IEnumerable<long> saleZoneIds, DateTime effectiveOn)
			{
				IEnumerable<int> sellingProductIds = new List<int>() { sellingProductId };
				var productZoneRoutingProductHistoryLocator = new ProductZoneRoutingProductHistoryLocator(new ProductZoneRoutingProductHistoryReader(sellingProductIds, saleZoneIds));

				var zoneRoutingProducts = new List<ZoneRoutingProduct>();

				foreach (SaleZone saleZone in filteredSaleZones)
				{
					var zoneRoutingProductHistoryRecord = productZoneRoutingProductHistoryLocator.GetProductZoneRoutingProductHistoryRecord(sellingProductId, saleZone.Name, effectiveOn);

					if (zoneRoutingProductHistoryRecord != null)
					{
						var serviceIds = _routingProductManager.GetZoneServiceIds(
							zoneRoutingProductHistoryRecord.RoutingProductId, saleZone.SaleZoneId);
						ZoneRoutingProduct routingProduct = ZoneRoutingProductMapper(zoneRoutingProductHistoryRecord, saleZone.SaleZoneId, saleZone.BED, saleZone.CountryId, serviceIds, false, DateTime.MinValue, null);
						zoneRoutingProducts.Add(routingProduct);
					}
				}

				return zoneRoutingProducts;
			}
			private IEnumerable<ZoneRoutingProduct> GetCustomerZoneRoutingProduct(int customerId, IEnumerable<SaleZone> filteredSaleZones, IEnumerable<long> saleZoneIds, DateTime effectiveOn)
			{
				IEnumerable<int> customerIds = new List<int>() { customerId };

				int sellingProductId = new CarrierAccountManager().GetSellingProductId(customerId);
				IEnumerable<int> sellingProductIds = new List<int>() { sellingProductId };

				var zoneRoutingProducts = new List<ZoneRoutingProduct>();

				var customerZoneRoutingProductHistoryLocator = new CustomerZoneRoutingProductHistoryLocator(new CustomerZoneRoutingProductHistoryReader(customerIds, sellingProductIds, saleZoneIds));



				foreach (SaleZone saleZone in filteredSaleZones)
				{
					var zoneRoutingProductHistoryRecord = customerZoneRoutingProductHistoryLocator.GetCustomerZoneRoutingProductHistoryRecord(customerId, sellingProductId, saleZone.Name, saleZone.CountryId, effectiveOn);

					if (zoneRoutingProductHistoryRecord != null)
					{
						var serviceIds = _routingProductManager.GetZoneServiceIds(
							zoneRoutingProductHistoryRecord.RoutingProductId, saleZone.SaleZoneId);
						ZoneRoutingProduct routingProduct = ZoneRoutingProductMapper(zoneRoutingProductHistoryRecord, saleZone.SaleZoneId, saleZone.BED, saleZone.CountryId, serviceIds, false, DateTime.MinValue, null);
						zoneRoutingProducts.Add(routingProduct);
					}
				}

				return zoneRoutingProducts;
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
