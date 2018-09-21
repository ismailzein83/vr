using System;
using System.Collections.Generic;
using System.Linq;
using TOne.WhS.BusinessEntity.Data;
using TOne.WhS.BusinessEntity.Entities;
using TOne.WhS.BusinessEntity.Entities.SalePricelistChanges;
using Vanrise.Common;
using Vanrise.Common.Business;
using Vanrise.Entities;
using Vanrise.Security.Business;

namespace TOne.WhS.BusinessEntity.Business
{
	public class SalePriceListChangeManager
	{
		public IDataRetrievalResult<SalePriceListNewDetail> GetFilteredTemporarySalePriceLists(Vanrise.Entities.DataRetrievalInput<TemporarySalePriceListQuery> input)
		{
			return BigDataManager.Instance.RetrieveData(input, new SalePriceListNewPreviewRequestHandler());
		}
		public Vanrise.Entities.IDataRetrievalResult<SalePricelistRateChangeDetail> GetFilteredPricelistRateChanges(Vanrise.Entities.DataRetrievalInput<SalePriceListChangeQuery> input)
		{
			ISalePriceListChangeDataManager dataManager = BEDataManagerFactory.GetDataManager<ISalePriceListChangeDataManager>();
			var salePriceListRateChanges = dataManager.GetFilteredSalePricelistRateChanges(input.Query.PriceListId, input.Query.Countries).Where(r => r.RateTypeId == null);
			return DataRetrievalManager.Instance.ProcessResult(input, salePriceListRateChanges.ToBigResult(input, null, SalePricelistRateChangeDetailMapper));
		}
		public bool DoCustomerTemporaryPricelistsExists(long processInstanceId)
		{
			ISalePriceListChangeDataManager dataManager = BEDataManagerFactory.GetDataManager<ISalePriceListChangeDataManager>();
			return dataManager.DoCustomerTemporaryPricelistsExists(processInstanceId);
		}
		public Vanrise.Entities.IDataRetrievalResult<CustomerRatePreviewDetail> GetFilteredCustomerRatePreviews(Vanrise.Entities.DataRetrievalInput<CustomerRatePreviewQuery> input)
		{
			return BigDataManager.Instance.RetrieveData(input, new CustomerRatePreviewRequestHandler());
		}
		public IEnumerable<int> GetAffectedCustomerIdsRPChangesByProcessInstanceId(long ProcessInstanceId)
		{
			ISalePriceListChangeDataManager dataManager = BEDataManagerFactory.GetDataManager<ISalePriceListChangeDataManager>();
			return dataManager.GetAffectedCustomerIdsRPChangesByProcessInstanceId(ProcessInstanceId);
		}
		public IEnumerable<int> GetAffectedCustomerIdsNewCountryChangesByProcessInstanceId(long ProcessInstanceId)
		{
			ISalePriceListChangeDataManager dataManager = BEDataManagerFactory.GetDataManager<ISalePriceListChangeDataManager>();
			return dataManager.GetAffectedCustomerIdsNewCountryChangesByProcessInstanceId(ProcessInstanceId);
		}
		public IEnumerable<int> GetAffectedCustomerIdsChangedCountryChangesByProcessInstanceId(long ProcessInstanceId)
		{
			ISalePriceListChangeDataManager dataManager = BEDataManagerFactory.GetDataManager<ISalePriceListChangeDataManager>();
			return dataManager.GetAffectedCustomerIdsNewCountryChangesByProcessInstanceId(ProcessInstanceId);
		}
		public IEnumerable<int> GetAffectedCustomerIdsRateChangesByProcessInstanceId(long ProcessInstanceId)
		{
			ISalePriceListChangeDataManager dataManager = BEDataManagerFactory.GetDataManager<ISalePriceListChangeDataManager>();
			return dataManager.GetAffectedCustomerIdsRateChangesByProcessInstanceId(ProcessInstanceId);
		}
		public Vanrise.Entities.IDataRetrievalResult<RoutingProductPreviewDetail> GetFilteredRoutingProductPreviews(Vanrise.Entities.DataRetrievalInput<RoutingProductPreviewQuery> input)
		{
			return BigDataManager.Instance.RetrieveData(input, new RoutingProductPreviewRequestHandler());
		}
		public Vanrise.Entities.IDataRetrievalResult<SalePricelistCodeChange> GetFilteredPricelistCodeChanges(Vanrise.Entities.DataRetrievalInput<SalePriceListChangeQuery> input)
		{
			ISalePriceListChangeDataManager dataManager = BEDataManagerFactory.GetDataManager<ISalePriceListChangeDataManager>();
			var salePriceListCodeChanges = dataManager.GetFilteredSalePricelistCodeChanges(input.Query.PriceListId, input.Query.Countries);
			return DataRetrievalManager.Instance.ProcessResult(input, salePriceListCodeChanges.ToBigResult(input, null, SalePricelistCodeChangeDetailMapper));
		}
		public Vanrise.Entities.IDataRetrievalResult<SalePricelistCode> GetFilteredSalePricelistCodes(Vanrise.Entities.DataRetrievalInput<SalePriceListCodeQuery> input)
		{
			var saleCodesByZoneId = GetAllSaleCodesSnapShotByPricelistId(input.Query.PriceListId);

			List<SalePricelistCode> filteredCodes;
			if (saleCodesByZoneId.TryGetValue(input.Query.ZoneId, out filteredCodes))
				return DataRetrievalManager.Instance.ProcessResult(input, filteredCodes.ToBigResult(input, null));

			return null;
		}
		public Vanrise.Entities.IDataRetrievalResult<SalePricelistRPChangeDetail> GetFilteredSalePriceListRPChanges(Vanrise.Entities.DataRetrievalInput<SalePriceListChangeQuery> input)
		{
			ISalePriceListChangeDataManager dataManager = BEDataManagerFactory.GetDataManager<ISalePriceListChangeDataManager>();
			var salePriceListRateChanges = dataManager.GetFilteredSalePriceListRPChanges(input.Query.PriceListId, input.Query.Countries);
			return DataRetrievalManager.Instance.ProcessResult(input, salePriceListRateChanges.ToBigResult(input, null, SalePricelistRPChangeDetailMapper));
		}

		public List<SaleCode> GetSalePriceListSaleCodeSnapShot(int pricelistId)
		{
			ISalePriceListChangeDataManager dataManager = BEDataManagerFactory.GetDataManager<ISalePriceListChangeDataManager>();
			var saleCodesSnapshot = dataManager.GetSalePriceListSnapShot(pricelistId);

			SaleCodeManager saleCodeManager = new SaleCodeManager();
			return saleCodeManager.GetSaleCodesByCodeId(saleCodesSnapshot.SnapShotDetail.CodeIds);
		}

		public SalePriceListOption GetOwnerOptions(int priceListId)
		{
			SalePriceListManager salePriceListManager = new SalePriceListManager();
			CarrierAccountManager carrierAccountManager = new CarrierAccountManager();
			var priceList = salePriceListManager.GetPriceList(priceListId);
			if (priceList != null)
			{
				var ownerId = priceList.OwnerId;
				var carrierAccount = carrierAccountManager.GetCarrierAccount(ownerId);
				return new SalePriceListOption
				{
					OwnerName = carrierAccountManager.GetCarrierAccountName(ownerId),
					CompressPriceListFile = carrierAccountManager.GetCustomerCompressPriceListFileStatus(ownerId),
				};
			}
			return null;
		}

		public int? GetOwnerPricelistTemplateId(int priceListId)
		{
			SalePriceListManager salePriceListManager = new SalePriceListManager();
			CarrierAccountManager carrierAccountManager = new CarrierAccountManager();
			var priceList = salePriceListManager.GetPriceList(priceListId);
			if (priceList != null)
			{
				var ownerId = priceList.OwnerId;
				return carrierAccountManager.GetCustomerPriceListTemplateId(ownerId);
			}
			return null;
		}

		public void BulkInsertSalePriceListSaleCodeSnapshot(IEnumerable<SalePriceListSnapShot> salePriceListSaleCodeSnapshots)
		{
			var dataManager = BEDataManagerFactory.GetDataManager<ISalePriceListChangeDataManager>();
			dataManager.SaveSalePriceListSnapshotToDb(salePriceListSaleCodeSnapshots);

		}

		public void BulkCustomerSalePriceListChanges(IEnumerable<SalePriceListCustomerChange> customerChanges, IEnumerable<SalePricelistCodeChange> codeChanges, IEnumerable<SalePricelistRateChange> rateChanges
			, IEnumerable<SalePricelistRPChange> rpChanges, long processInstanceId)
		{
			var dataManager = BEDataManagerFactory.GetDataManager<ISalePriceListChangeDataManager>();
			dataManager.SaveCustomerChangesToDb(customerChanges);
			dataManager.SaveCustomerCodeChangesToDb(codeChanges);
			dataManager.SaveCustomerRateChangesToDb(rateChanges, processInstanceId);
			dataManager.SaveCustomerRoutingProductChangesToDb(rpChanges, processInstanceId);
		}

		public CustomerPriceListChange GetCustomerChangesByPriceListId(int pricelistId)
		{
			ISalePriceListChangeDataManager dataManager = BEDataManagerFactory.GetDataManager<ISalePriceListChangeDataManager>();
			var salePriceListRateChanges = dataManager.GetFilteredSalePricelistRateChanges(pricelistId, null);
			var salePriceListCodeChanges = dataManager.GetFilteredSalePricelistCodeChanges(pricelistId, null);
			var routingProductChanges = dataManager.GetFilteredSalePriceListRPChanges(pricelistId, null);
			CustomerPriceListChange changes = new CustomerPriceListChange();
			changes.CodeChanges.AddRange(salePriceListCodeChanges);
			changes.RateChanges.AddRange(salePriceListRateChanges);
			changes.RoutingProductChanges.AddRange(routingProductChanges);
			changes.PriceListId = pricelistId;

			return changes;
		}

		private Dictionary<long, List<SalePricelistCode>> StructureSaleCodeByZoneId(IEnumerable<SaleCode> saleCodes, IEnumerable<SalePricelistCodeChange> codeChanges)
		{
			Dictionary<long, List<SalePricelistCode>> salecodeByZoneId = new Dictionary<long, List<SalePricelistCode>>();

			Dictionary<string, SalePricelistCodeChange> codeChangesByCode = codeChanges.ToDictionary(x => x.Code);

			foreach (var codeChange in codeChanges)
			{
				List<SalePricelistCode> codes = salecodeByZoneId.GetOrCreateItem(codeChange.ZoneId.Value);
				codes.Add(new SalePricelistCode
				{
					Code = codeChange.Code,
					BED = codeChange.BED,
					EED = codeChange.EED
				});
			}

			foreach (var code in saleCodes)
			{
				if (codeChangesByCode.ContainsKey(code.Code)) continue;
				List<SalePricelistCode> codes = salecodeByZoneId.GetOrCreateItem(code.ZoneId);
				codes.Add(new SalePricelistCode
				{
					Code = code.Code,
					BED = code.BED,
					EED = code.EED
				});
			}
			return salecodeByZoneId;
		}
		private Dictionary<long, List<SalePricelistCode>> GetAllSaleCodesSnapShotByPricelistId(int pricelistId)
		{
			ISalePriceListChangeDataManager dataManager = BEDataManagerFactory.GetDataManager<ISalePriceListChangeDataManager>();
			string cacheName = String.Format("GetAllSaleCodesSnapShotByPricelistId-{0}", pricelistId);
			return Vanrise.Caching.CacheManagerFactory.GetCacheManager<CacheManager>().GetOrCreateObject(cacheName,
			   () =>
			   {
				   IEnumerable<SaleCode> saleCodes = GetSalePriceListSaleCodeSnapShot(pricelistId);
				   var salePriceListCodeChanges = dataManager.GetFilteredSalePricelistCodeChanges(pricelistId, null);
				   return StructureSaleCodeByZoneId(saleCodes, salePriceListCodeChanges);
			   });

		}
		private string GetCurrencyName(int? currencyId)
		{
			if (currencyId.HasValue)
			{
				CurrencyManager manager = new CurrencyManager();
				return manager.GetCurrencySymbol(currencyId.Value);

			}

			return "Currency Not Found";
		}
		private class CacheManager : Vanrise.Caching.BaseCacheManager
		{
			ISalePriceListChangeDataManager _dataManager = BEDataManagerFactory.GetDataManager<ISalePriceListChangeDataManager>();
			object _updateHandle;

			protected override bool ShouldSetCacheExpired(object parameter)
			{
				return _dataManager.AreSalePriceListCodeSnapShotUpdated(ref _updateHandle);
			}
		}

		private class CustomerRatePreviewRequestHandler : BigDataRequestHandler<CustomerRatePreviewQuery, CustomerRatePreview, CustomerRatePreviewDetail>
		{


			public override CustomerRatePreviewDetail EntityDetailMapper(CustomerRatePreview entity)
			{
				RoutingProductManager routingProductManager = new RoutingProductManager();
				CurrencyManager currencyManager = new CurrencyManager();
				CarrierAccountManager carrierAccountManager = new CarrierAccountManager();
				var entityDetail = new CustomerRatePreviewDetail()
				{
					ZoneName = entity.ZoneName,
					Rate = entity.Rate,
					RecentCurrencyId = entity.RecentCurrencyId,
					RecentCurrencySymbol = (entity.RecentCurrencyId.HasValue) ? currencyManager.GetCurrencySymbol(entity.RecentCurrencyId.Value) : null,
					RecentRate = entity.RecentRate,
					RecentRateConverted = entity.RecentRateConverted,
					BED = entity.BED,
					EED = entity.EED,
					ChangeType = entity.ChangeType,
					ServicesId = !entity.ZoneId.HasValue
				  ? routingProductManager.GetDefaultServiceIds(entity.RoutingProductId)
				  : routingProductManager.GetZoneServiceIds(entity.RoutingProductId,
					  entity.ZoneId.Value),
					CustomerName = carrierAccountManager.GetCarrierAccountName(entity.CustomerId),
					CustomerId = entity.CustomerId,
					CurrencyId = entity.CurrencyId
				};
				if (entity.CurrencyId.HasValue)
					entityDetail.CurrencySymbol = currencyManager.GetCurrencySymbol(entity.CurrencyId.Value);
				return entityDetail;
			}

			public override IEnumerable<CustomerRatePreview> RetrieveAllData(Vanrise.Entities.DataRetrievalInput<CustomerRatePreviewQuery> input)
			{
				var dataManager = BEDataManagerFactory.GetDataManager<ISalePriceListChangeDataManager>();
				return dataManager.GetCustomerRatePreviews(input.Query);
			}
		}

		private class RoutingProductPreviewRequestHandler : BigDataRequestHandler<RoutingProductPreviewQuery, RoutingProductPreview, RoutingProductPreviewDetail>
		{


			public override RoutingProductPreviewDetail EntityDetailMapper(RoutingProductPreview entity)
			{
				RoutingProductManager routingProductManager = new RoutingProductManager();
				CarrierAccountManager carrierAccountManager = new CarrierAccountManager();
				var entityDetail = new RoutingProductPreviewDetail()
				{
					ZoneName = entity.ZoneName,
					BED = entity.BED,
					EED = entity.EED,
					RoutingProductName = routingProductManager.GetRoutingProductName(entity.RoutingProductId),
					CustomerName = carrierAccountManager.GetCarrierAccountName(entity.CustomerId),
					RoutingProductServicesId = !entity.ZoneId.HasValue
						? routingProductManager.GetDefaultServiceIds(entity.RoutingProductId)
						: routingProductManager.GetZoneServiceIds(entity.RoutingProductId,
							entity.ZoneId.Value)
				};
				if (entity.RecentRoutingProductId.HasValue)
				{
					int recentRoutingProductId = entity.RecentRoutingProductId.Value;
					entityDetail.RecentRoutingProductName = routingProductManager.GetRoutingProductName(recentRoutingProductId);

					entityDetail.RecentRouringProductServicesId = !entity.ZoneId.HasValue
						? routingProductManager.GetDefaultServiceIds(recentRoutingProductId)
						: routingProductManager.GetZoneServiceIds(recentRoutingProductId, entity.ZoneId.Value);
				}
				return entityDetail;
			}

			public override IEnumerable<RoutingProductPreview> RetrieveAllData(Vanrise.Entities.DataRetrievalInput<RoutingProductPreviewQuery> input)
			{
				var dataManager = BEDataManagerFactory.GetDataManager<ISalePriceListChangeDataManager>();
				return dataManager.GetRoutingProductPreviews(input.Query);
			}
		}
		private class SalePriceListNewPreviewRequestHandler : BigDataRequestHandler<TemporarySalePriceListQuery, SalePriceListNew, SalePriceListNewDetail>
		{
			CarrierAccountManager carrierAccountManager = new CarrierAccountManager();
			CurrencyManager currencyManager = new CurrencyManager();

			public override SalePriceListNewDetail EntityDetailMapper(SalePriceListNew entity)
			{

				SalePriceListNewDetail pricelistDetail = new SalePriceListNewDetail
				{
					Entity = entity,
					OwnerType = Vanrise.Common.Utilities.GetEnumDescription(entity.OwnerType),
					PriceListTypeName = entity.PriceListType.HasValue
						? Vanrise.Common.Utilities.GetEnumDescription(entity.PriceListType.Value)
						: null,
					PricelistSourceName = entity.PricelistSource.HasValue
					? Vanrise.Common.Utilities.GetEnumDescription(entity.PricelistSource.Value)
					: null,
				};

				if (entity.OwnerType != SalePriceListOwnerType.Customer)
				{
					SellingProductManager productManager = new SellingProductManager();
					pricelistDetail.OwnerName = productManager.GetSellingProductName(entity.OwnerId);
				}

				else
					pricelistDetail.OwnerName = carrierAccountManager.GetCarrierAccountName(entity.OwnerId);

				UserManager userManager = new UserManager();
				pricelistDetail.UserName = userManager.GetUserName(entity.UserId);
				pricelistDetail.CurrencyName = currencyManager.GetCurrencySymbol(entity.CurrencyId);

				return pricelistDetail;
			}

			public override IEnumerable<SalePriceListNew> RetrieveAllData(Vanrise.Entities.DataRetrievalInput<TemporarySalePriceListQuery> input)
			{
				var dataManager = BEDataManagerFactory.GetDataManager<ISalePriceListChangeDataManager>();
				return dataManager.GetTemporaryPriceLists(input.Query);
			}
		}
		private class SalePriceListExportExcelHandler : ExcelExportHandler<SalePriceListDetail>
		{
			public override void ConvertResultToExcelData(IConvertResultToExcelDataContext<SalePriceListDetail> context)
			{
				var sheet = new ExportExcelSheet()
				{
					SheetName = "Sale Pricelists",
					Header = new ExportExcelHeader() { Cells = new List<ExportExcelHeaderCell>() }
				};
				sheet.Header.Cells.Add(new ExportExcelHeaderCell() { Title = "Customer Name", Width = 50 });
				sheet.Header.Cells.Add(new ExportExcelHeaderCell() { Title = "Description", Width = 100 });
				sheet.Header.Cells.Add(new ExportExcelHeaderCell() { Title = "Pricelist Type" });
				sheet.Header.Cells.Add(new ExportExcelHeaderCell() { Title = "Currency" });
				sheet.Header.Cells.Add(new ExportExcelHeaderCell() { Title = "Created By" });
				sheet.Header.Cells.Add(new ExportExcelHeaderCell() { Title = "Created Time", CellType = ExcelCellType.DateTime, DateTimeType = DateTimeType.DateTime });
				sheet.Header.Cells.Add(new ExportExcelHeaderCell() { Title = "Sent" });

				sheet.Rows = new List<ExportExcelRow>();
				if (context.BigResult != null && context.BigResult.Data != null)
				{
					foreach (var record in context.BigResult.Data)
					{
						if (record.Entity != null)
						{
							var row = new ExportExcelRow() { Cells = new List<ExportExcelCell>() };
							row.Cells.Add(new ExportExcelCell() { Value = record.OwnerName });
							row.Cells.Add(new ExportExcelCell() { Value = record.Entity.Description });
							row.Cells.Add(new ExportExcelCell() { Value = record.PriceListTypeName });
							row.Cells.Add(new ExportExcelCell() { Value = record.CurrencyName });
							row.Cells.Add(new ExportExcelCell() { Value = record.UserName });
							row.Cells.Add(new ExportExcelCell() { Value = record.Entity.CreatedTime });
							row.Cells.Add(new ExportExcelCell() { Value = record.Entity.IsSent });
							sheet.Rows.Add(row);
						}
					}
				}

				context.MainSheet = sheet;
			}
		}

		#region Mapper

		private SalePricelistRateChangeDetail SalePricelistRateChangeDetailMapper(SalePricelistRateChange salePricelistRateChange)
		{
			RoutingProductManager routingProductManager = new RoutingProductManager();
			CurrencyManager currencyManager = new CurrencyManager();
			var salePricelistRateChangeDetail = new SalePricelistRateChangeDetail
			{
				ZoneName = salePricelistRateChange.ZoneName,
				ZoneId = salePricelistRateChange.ZoneId.Value,
				BED = salePricelistRateChange.BED,
				EED = salePricelistRateChange.EED,
				Rate = salePricelistRateChange.Rate,
				RecentRate = salePricelistRateChange.RecentRate,
				ChangeType = salePricelistRateChange.ChangeType,
				ServicesId = !salePricelistRateChange.ZoneId.HasValue
					? routingProductManager.GetDefaultServiceIds(salePricelistRateChange.RoutingProductId)
					: routingProductManager.GetZoneServiceIds(salePricelistRateChange.RoutingProductId,
						salePricelistRateChange.ZoneId.Value)
			};
			if (salePricelistRateChange.CurrencyId.HasValue)
				salePricelistRateChangeDetail.CurrencySymbol = currencyManager.GetCurrencySymbol(salePricelistRateChange.CurrencyId.Value);

			return salePricelistRateChangeDetail;
		}
		private SalePricelistCodeChange SalePricelistCodeChangeDetailMapper(SalePricelistCodeChange salePricelistCodeChange)
		{
			var salePriceListManager = new SalePriceListManager();
			//TODO: the below line is not needed anymore. We need to check why we need pricelist id in detail, if we need it we need to check if we can 
			//create a detail class having it. it is wrong to keep it in the object and it is not saved in the database
			SalePriceList pricelist = salePriceListManager.GetPriceList((int)salePricelistCodeChange.PricelistId);
			SalePricelistCodeChange codeChange = new SalePricelistCodeChange
			{
				ZoneId = salePricelistCodeChange.ZoneId,
				CountryId = salePricelistCodeChange.CountryId,
				ZoneName = salePricelistCodeChange.ZoneName,

				ChangeType = salePricelistCodeChange.ChangeType,
				Code = salePricelistCodeChange.Code,
				PricelistId = salePricelistCodeChange.PricelistId,
				BatchId = salePricelistCodeChange.BatchId,
				RecentZoneName = salePricelistCodeChange.RecentZoneName
			};
			codeChange.BED = salePricelistCodeChange.BED;
			codeChange.EED = salePricelistCodeChange.EED.VRGreaterThan(codeChange.BED) ? salePricelistCodeChange.EED : codeChange.BED;
			return codeChange;
		}
		private SalePricelistRPChangeDetail SalePricelistRPChangeDetailMapper(SalePricelistRPChange salePricelistRpChange)
		{
			RoutingProductManager routingProductManager = new RoutingProductManager();
			SalePricelistRPChangeDetail salePricelistRpChangeDetail = new SalePricelistRPChangeDetail
			{
				ZoneName = salePricelistRpChange.ZoneName,
				BED = salePricelistRpChange.BED,
				EED = salePricelistRpChange.EED,
				RoutingProductName = routingProductManager.GetRoutingProductName(salePricelistRpChange.RoutingProductId),
				RoutingProductServicesId = !salePricelistRpChange.ZoneId.HasValue
					? routingProductManager.GetDefaultServiceIds(salePricelistRpChange.RoutingProductId)
					: routingProductManager.GetZoneServiceIds(salePricelistRpChange.RoutingProductId,
						salePricelistRpChange.ZoneId.Value)
			};
			if (salePricelistRpChange.RecentRoutingProductId.HasValue)
			{
				int recentRoutingProductId = salePricelistRpChange.RecentRoutingProductId.Value;
				salePricelistRpChangeDetail.RecentRoutingProductName = routingProductManager.GetRoutingProductName(recentRoutingProductId);

				salePricelistRpChangeDetail.RecentRouringProductServicesId = !salePricelistRpChange.ZoneId.HasValue
					? routingProductManager.GetDefaultServiceIds(recentRoutingProductId)
					: routingProductManager.GetZoneServiceIds(recentRoutingProductId, salePricelistRpChange.ZoneId.Value);
			}
			return salePricelistRpChangeDetail;
		}
		private SalePriceListDetail TemporarySalePricelistDetailMapper(SalePriceList priceList)
		{
			CarrierAccountManager _carrierAccountManager = new CarrierAccountManager();
			SalePriceListDetail pricelistDetail = new SalePriceListDetail
			{
				Entity = priceList,
				OwnerType = Vanrise.Common.Utilities.GetEnumDescription(priceList.OwnerType),
				PriceListTypeName = priceList.PriceListType.HasValue
					? Vanrise.Common.Utilities.GetEnumDescription(priceList.PriceListType.Value)
					: null,
				PricelistSourceName = priceList.PricelistSource.HasValue
				? Vanrise.Common.Utilities.GetEnumDescription(priceList.PricelistSource.Value)
				: null,
			};

			if (priceList.OwnerType != SalePriceListOwnerType.Customer)
			{
				SellingProductManager productManager = new SellingProductManager();
				pricelistDetail.OwnerName = productManager.GetSellingProductName(priceList.OwnerId);
			}

			else
				pricelistDetail.OwnerName = _carrierAccountManager.GetCarrierAccountName(priceList.OwnerId);

			UserManager userManager = new UserManager();
			pricelistDetail.UserName = userManager.GetUserName(priceList.UserId);
			pricelistDetail.CurrencyName = GetCurrencyName(priceList.CurrencyId);

			return pricelistDetail;
		}

		#endregion

	}
}
