using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TOne.WhS.BusinessEntity.Data;
using TOne.WhS.BusinessEntity.Entities;
using Vanrise.Common;
using Vanrise.Common.Business;
using Vanrise.Entities;

namespace TOne.WhS.BusinessEntity.Business
{
	public class SalePriceListManager
	{
		#region Private Fields

		private SaleZoneManager _saleZoneManager = new SaleZoneManager();

		private CarrierAccountManager _carrierAccountManager = new CarrierAccountManager();

		#endregion

		#region Public Methods

		public Vanrise.Entities.IDataRetrievalResult<SalePriceListDetail> GetFilteredPricelists(Vanrise.Entities.DataRetrievalInput<SalePriceListQuery> input)
		{
			Dictionary<int, SalePriceList> cachedSalePriceLists = GetCachedSalePriceLists();
			Func<SalePriceList, bool> filterExpression = (salePriceList) =>
			{
				if (input.Query.OwnerType.HasValue && salePriceList.OwnerType != input.Query.OwnerType.Value)
					return false;
				if (input.Query.OwnerIds != null && !input.Query.OwnerIds.Contains(salePriceList.OwnerId))
					return false;
				return true;
			};
			return Vanrise.Common.DataRetrievalManager.Instance.ProcessResult(input, cachedSalePriceLists.ToBigResult(input, filterExpression, SalePricelistDetailMapper));
		}

		public SalePriceList GetPriceList(int priceListId)
		{
			return GetCachedSalePriceLists().GetRecord(priceListId);
		}

		public long ReserveIdRange(int numberOfIds)
		{
			long startingId;
			IDManager.Instance.ReserveIDRange(this.GetType(), numberOfIds, out startingId);
			return startingId;
		}

		public int GetSalePriceListTypeId()
		{
			return Vanrise.Common.Business.TypeManager.Instance.GetTypeId(this.GetSalePriceListType());
		}

		public Type GetSalePriceListType()
		{
			return this.GetType();
		}

		public bool UpdateSalePriceList(SalePriceList salePriceList)
		{
			ISalePriceListDataManager dataManager = BEDataManagerFactory.GetDataManager<ISalePriceListDataManager>();
			return dataManager.Update(salePriceList);
		}

		public bool AddSalePriceList(SalePriceList salePriceList)
		{
			ISalePriceListDataManager dataManager = BEDataManagerFactory.GetDataManager<ISalePriceListDataManager>();
			return dataManager.Insert(salePriceList);
		}

		public IEnumerable<SalePriceList> GetCustomerSalePriceListsByProcessInstanceId(long processInstanceId)
		{
			Dictionary<int, SalePriceList> allSalePriceLists = GetCachedSalePriceLists();
			SalePriceListOwnerType customerOwnerType = SalePriceListOwnerType.Customer;

			return allSalePriceLists.Values.FindAllRecords(itm => itm.ProcessInstanceId == processInstanceId && itm.OwnerType == customerOwnerType);
		}

		public bool IsSalePriceListDeleted(int priceListId)
		{
			Dictionary<int, SalePriceList> allSalePriceLists = this.GetCachedSalePriceListsWithDeleted();
			SalePriceList salePriceList = allSalePriceLists.GetRecord(priceListId);

			if (salePriceList == null)
				throw new DataIntegrityValidationException(string.Format("Sale Price List with Id {0} does not exist", priceListId));

			return salePriceList.IsDeleted;
		}

		public SalePriceList GetPriceListByCustomerAndProcessInstanceId(long processInstanceId, int customerId)
		{
			IEnumerable<SalePriceList> processSalePricelists = this.GetCustomerSalePriceListsByProcessInstanceId(processInstanceId);

			if (processSalePricelists == null)
				return null;

			return processSalePricelists.FindRecord(itm => itm.OwnerId == customerId);
		}

		#endregion

		#region Save Price List Files

		public void SavePricelistFiles(ISalePricelistFileContext context)
		{
			if (context.CustomerIds == null || context.CustomerIds.Count() == 0)
				return;

			IEnumerable<SaleCode> saleCodes = new SaleCodeManager().GetSaleCodesEffectiveAfter(context.SellingNumberPlanId, Vanrise.Common.Utilities.Min(context.EffectiveDate, DateTime.Today));

			if (saleCodes == null || saleCodes.Count() == 0)
				return;

			IEnumerable<ExistingSaleCodeEntity> existingSaleCodeEntities = saleCodes.MapRecords(ExistingSaleCodeEntityMapper);
			Dictionary<string, Dictionary<string, List<ExistingSaleCodeEntity>>> existingSaleCodesByZoneName = StructureExistingSaleCodesByZoneName(existingSaleCodeEntities);
			Dictionary<int, List<ExistingSaleZone>> zoneWrappersByCountry = StructureZoneWrappersByCountry(existingSaleCodesByZoneName);

			Dictionary<int, List<SalePLZoneChange>> zoneChangesByCountryId = StructureZoneChangesByCountry(context.ZoneChanges);

			IEnumerable<SalePriceList> salePriceLists = GetCustomerSalePriceListsByProcessInstanceId(context.ProcessInstanceId);
			Dictionary<int, SalePriceList> salePriceListsByCustomer = StructureSalePriceListsByCustomer(salePriceLists);

			IEnumerable<RoutingCustomerInfoDetails> dataByCustomerList;
			Dictionary<int, int> dataByCustomerDictionary;
			SetDataByCustomer(context.CustomerIds, context.EffectiveDate, out dataByCustomerList, out dataByCustomerDictionary);

			var customerCountryManager = new CustomerCountryManager();
			var futureRateLocator = new SaleEntityZoneRateLocator(new SaleRateReadAllNoCache(dataByCustomerList, context.EffectiveDate, true));
			var saleRateManager = new SaleRateManager();

			SaleEntityZoneRateLocator rateLocator = null;
			if (context.EndedCountryIds != null && context.EndedCountryIds.Count() > 0)
			{
				if (!context.CountriesEndedOn.HasValue)
					throw new Vanrise.Entities.DataIntegrityValidationException(string.Format("Countries '{0}' are ended on a date that was not found", string.Join(",", context.EndedCountryIds)));
				DateTime countriesEndedOn = context.CountriesEndedOn.Value.AddSeconds(-5);
				rateLocator = new SaleEntityZoneRateLocator(new SaleRateReadAllNoCache(dataByCustomerList, countriesEndedOn, false));
			}

			foreach (int customerId in context.CustomerIds)
			{
				IEnumerable<CustomerCountry2> soldCountries = customerCountryManager.GetCustomerCountries(customerId, context.EffectiveDate, false);

				if (soldCountries == null || soldCountries.Count() == 0)
					continue;

				var customerZoneNotifications = new List<SalePLZoneNotification>();

				CarrierAccount customer = _carrierAccountManager.GetCarrierAccount(customerId);
				bool isCustomerAToZ = customer.CustomerSettings.IsAToZ;
				int sellingProductId = dataByCustomerDictionary.GetRecord(customerId);

				var baseRatesByZone = new BaseRatesByZone();

				foreach (CustomerCountry2 soldCountry in soldCountries)
				{
					List<ExistingSaleZone> countryZoneWrappers = zoneWrappersByCountry.GetRecord(soldCountry.CountryId);

					if (countryZoneWrappers == null || countryZoneWrappers.Count == 0)
						continue;

					IEnumerable<SalePLZoneChange> countryZoneChanges = zoneChangesByCountryId.GetRecord(soldCountry.CountryId);
					IEnumerable<ExistingSaleZone> plCountryZoneWrappers = GetPLCountryZoneWrappers(customerId, countryZoneWrappers, countryZoneChanges, isCustomerAToZ, context.ChangeType);

					AddCustomerCountryZoneNotifications(customerZoneNotifications, plCountryZoneWrappers, customerId, sellingProductId, futureRateLocator, baseRatesByZone, soldCountry.CountryId, context.EndedCountryIds, context.CountriesEndedOn, rateLocator);
				}

				if (customerZoneNotifications.Count > 0)
				{
					saleRateManager.ProcessBaseRatesByZone(customerId, baseRatesByZone, soldCountries);
					SalePriceListType customerPLType = GetSalePriceListType(customer.CustomerSettings.IsAToZ, context.ChangeType);
					SavePriceListFile(customer, customerPLType, customerZoneNotifications, salePriceListsByCustomer, context.ProcessInstanceId);
				}
			}
		}

		private void SetDataByCustomer(IEnumerable<int> customerIds, DateTime effectiveOn, out IEnumerable<RoutingCustomerInfoDetails> dataByCustomerList, out Dictionary<int, int> dataByCustomerDictionary)
		{
			var list = new List<RoutingCustomerInfoDetails>();
			var dictionary = new Dictionary<int, int>();

			var customerSellingProductManager = new CustomerSellingProductManager();

			foreach (int customerId in customerIds)
			{
				int? sellingProductId = customerSellingProductManager.GetEffectiveSellingProductId(customerId, effectiveOn, false);

				if (!sellingProductId.HasValue)
					throw new Vanrise.Entities.DataIntegrityValidationException(string.Format("Customer '{0}' is not assigned to a Selling Product", customerId));

				list.Add(new RoutingCustomerInfoDetails()
				{
					CustomerId = customerId,
					SellingProductId = sellingProductId.Value
				});

				dictionary.Add(customerId, sellingProductId.Value);
			}

			dataByCustomerList = list;
			dataByCustomerDictionary = dictionary;
		}

		private IEnumerable<ExistingSaleZone> GetPLCountryZoneWrappers(int customerId, IEnumerable<ExistingSaleZone> countryZoneWrappers, IEnumerable<SalePLZoneChange> countryZoneChanges, bool isCustomerAToZ, SalePLChangeType plChangeType)
		{
			if (isCustomerAToZ || plChangeType == SalePLChangeType.CountryAndRate)
				return countryZoneWrappers;
			else if (countryZoneChanges != null)
			{
				if (plChangeType == SalePLChangeType.CodeAndRate)
					return countryZoneWrappers;
				else if (plChangeType == SalePLChangeType.Rate)
				{
					var zoneWrappersWithChanges = new List<ExistingSaleZone>();
					foreach (ExistingSaleZone zoneWrapper in countryZoneWrappers)
					{
						SalePLZoneChange zoneChange = countryZoneChanges.FindRecord(x => x.ZoneName.Equals(zoneWrapper.ZoneName));
						if (zoneChange != null && zoneChange.CustomersHavingRateChange.Contains(customerId))
							zoneWrappersWithChanges.Add(zoneWrapper);
					}
					return zoneWrappersWithChanges;
				}
			}
			return null;
		}

		private Dictionary<int, SalePriceList> StructureSalePriceListsByCustomer(IEnumerable<SalePriceList> salePriceLists)
		{
			Dictionary<int, SalePriceList> salePriceListsByCustomer = new Dictionary<int, SalePriceList>();

			if (salePriceLists != null)
			{
				foreach (SalePriceList salePriceList in salePriceLists)
				{
					if (!salePriceListsByCustomer.ContainsKey(salePriceList.OwnerId))
						salePriceListsByCustomer.Add(salePriceList.OwnerId, salePriceList);

				}
			}

			return salePriceListsByCustomer;
		}

		private void AddCustomerCountryZoneNotifications(List<SalePLZoneNotification> zoneNotifications, IEnumerable<ExistingSaleZone> zonesWrappers, int customerId, int sellingProductId, SaleEntityZoneRateLocator futureRateLocator, BaseRatesByZone baseRatesByZone, int countryId, IEnumerable<int> endedCountryIds, DateTime? countriesEndedOn, SaleEntityZoneRateLocator rateLocator)
		{
			if (zonesWrappers == null)
				return;

			foreach (ExistingSaleZone zoneWrapper in zonesWrappers)
			{
				SaleEntityZoneRate zoneRate;
				
				bool isZoneCountryEnded = IsZoneCountryEnded(countryId, endedCountryIds);

				if (isZoneCountryEnded)
					zoneRate = rateLocator.GetCustomerZoneRate(customerId, sellingProductId, zoneWrapper.ZoneId);
				else
					zoneRate = futureRateLocator.GetCustomerZoneRate(customerId, sellingProductId, zoneWrapper.ZoneId);

				if (zoneRate == null || zoneRate.Rate == null)
					continue;

				var zoneNotification = new SalePLZoneNotification()
				{
					ZoneName = zoneWrapper.ZoneName,
					ZoneId = zoneWrapper.ZoneId
				};

				zoneNotification.Rate = SalePLRateNotificationMapper(zoneRate);
				zoneNotification.Codes.AddRange(zoneWrapper.Codes.MapRecords(SalePLCodeNotificationMapper));

				zoneNotifications.Add(zoneNotification);
				if (zoneRate.Source == SalePriceListOwnerType.SellingProduct)
				{
					baseRatesByZone.AddZoneBaseRate(zoneNotification.ZoneId, zoneNotification, countryId, null, zoneRate.Rate.BED, zoneRate.Rate.EED);
					if (isZoneCountryEnded)
						zoneNotification.Rate.EED = countriesEndedOn;
				}
			}
		}

		private bool IsZoneCountryEnded(int countryId, IEnumerable<int> endedCountryIds)
		{
			if (endedCountryIds == null)
				return false;
			return endedCountryIds.Contains(countryId);
		}

		private void SavePriceListFile(CarrierAccount customer, SalePriceListType customerSalePriceListType, List<SalePLZoneNotification> customerZonesNotifications, Dictionary<int, SalePriceList> salePriceListsByCustomer, long processInstanceId)
		{
			int priceListTemplateId = _carrierAccountManager.GetSalePriceListTemplateId(customer.CarrierAccountId);

			SalePriceListTemplateManager salePriceListTemplateManager = new SalePriceListTemplateManager();
			SalePriceListTemplate template = salePriceListTemplateManager.GetSalePriceListTemplate(priceListTemplateId);

			if (template == null)
				throw new DataIntegrityValidationException(string.Format("Customer with Id {0} does not have a Sale Price List Template", customer.CarrierAccountId));

			ISalePriceListTemplateSettingsContext salePLTemplateSettingsContext = new SalePriceListTemplateSettingsContext()
			{
				Zones = customerZonesNotifications
			};

			byte[] salePLTemplateBytes = template.Settings.Execute(salePLTemplateSettingsContext);



			string customerName = _carrierAccountManager.GetCarrierAccountName(customer.CarrierAccountId);
			string fileName = string.Concat("Pricelist_", customerName, "_", DateTime.Today, ".xls");

			VRFile file = new VRFile()
			{
				Content = salePLTemplateBytes,
				Name = fileName,
				ModuleName = "WhS_BE_SalePriceList",
				Extension = "xls",
				CreatedTime = DateTime.Today,
			};

			VRFileManager fileManager = new VRFileManager();
			long fileId = fileManager.AddFile(file);

			SalePriceList salePriceList;
			SalePriceListManager salePriceListManager = new SalePriceListManager();
			if (salePriceListsByCustomer.TryGetValue(customer.CarrierAccountId, out salePriceList))
			{
				salePriceList.FileId = fileId;
				salePriceList.PriceListType = customerSalePriceListType;
				salePriceListManager.UpdateSalePriceList(salePriceList);
			}
			else
			{
				int salePriceListId = (int)salePriceListManager.ReserveIdRange(1);

				salePriceList = new SalePriceList()
				{
					OwnerId = customer.CarrierAccountId,
					OwnerType = SalePriceListOwnerType.Customer,
					PriceListType = customerSalePriceListType,
					FileId = fileId,
					PriceListId = salePriceListId,
					ProcessInstanceId = processInstanceId,
					EffectiveOn = DateTime.Today,
					CurrencyId = customer.CarrierAccountSettings.CurrencyId
				};
				salePriceListManager.AddSalePriceList(salePriceList);
			}
		}

		#region Structuring Methods

		private Dictionary<string, Dictionary<string, List<ExistingSaleCodeEntity>>> StructureExistingSaleCodesByZoneName(IEnumerable<ExistingSaleCodeEntity> saleCodesExistingEntities)
		{
			Dictionary<string, Dictionary<string, List<ExistingSaleCodeEntity>>> existingSaleCodesByZoneName = new Dictionary<string, Dictionary<string, List<ExistingSaleCodeEntity>>>();
			if (saleCodesExistingEntities != null)
			{
				Dictionary<string, List<ExistingSaleCodeEntity>> saleCodesByCodeValue;
				List<ExistingSaleCodeEntity> saleCodes;
				foreach (ExistingSaleCodeEntity saleCodeExistingEntity in saleCodesExistingEntities)
				{
					if (!existingSaleCodesByZoneName.TryGetValue(saleCodeExistingEntity.ZoneName, out saleCodesByCodeValue))
					{
						saleCodesByCodeValue = new Dictionary<string, List<ExistingSaleCodeEntity>>();
						saleCodes = new List<ExistingSaleCodeEntity>();
						saleCodes.Add(saleCodeExistingEntity);
						saleCodesByCodeValue.Add(saleCodeExistingEntity.CodeEntity.Code, saleCodes);
						existingSaleCodesByZoneName.Add(saleCodeExistingEntity.ZoneName, saleCodesByCodeValue);
					}
					else
					{
						if (!saleCodesByCodeValue.TryGetValue(saleCodeExistingEntity.CodeEntity.Code, out saleCodes))
						{
							saleCodes = new List<ExistingSaleCodeEntity>();
							saleCodes.Add(saleCodeExistingEntity);
							saleCodesByCodeValue.Add(saleCodeExistingEntity.CodeEntity.Code, saleCodes);
						}
						else
							saleCodes.Add(saleCodeExistingEntity);
					}
				}
			}

			return existingSaleCodesByZoneName;
		}

		private Dictionary<int, List<ExistingSaleZone>> StructureZoneWrappersByCountry(Dictionary<string, Dictionary<string, List<ExistingSaleCodeEntity>>> existingSaleCodesByZoneName)
		{
			if (existingSaleCodesByZoneName == null)
				return null;

			Dictionary<int, List<ExistingSaleZone>> zonesWrapperByCountry = new Dictionary<int, List<ExistingSaleZone>>();
			List<ExistingSaleZone> zonesWrapper;
			SaleZoneManager zoneManager = new SaleZoneManager();

			DateTime today = DateTime.Today;
			foreach (KeyValuePair<string, Dictionary<string, List<ExistingSaleCodeEntity>>> zoneItem in existingSaleCodesByZoneName)
			{

				List<ExistingSaleCode> codes = new List<ExistingSaleCode>();
				long zoneId = zoneItem.Value.First().Value.First().CodeEntity.ZoneId;
				DateTime maxCodeBED = DateTime.MinValue;
				foreach (KeyValuePair<string, List<ExistingSaleCodeEntity>> codeItem in zoneItem.Value)
				{
					IEnumerable<ExistingSaleCodeEntity> connectedSaleCodes = codeItem.Value.OrderBy(itm => itm.BED).ToList().GetLastConnectedEntities();

					ExistingSaleCode codeObject = new ExistingSaleCode()
					{
						Code = codeItem.Key,
						BED = connectedSaleCodes.First().BED,
						EED = connectedSaleCodes.Last().EED
					};

					if (maxCodeBED < codeObject.BED)
					{
						maxCodeBED = codeObject.BED;
						zoneId = connectedSaleCodes.First().CodeEntity.ZoneId;
					}

					codes.Add(codeObject);
				}

				ExistingSaleZone zoneWrapper = new ExistingSaleZone()
				{
					ZoneName = zoneItem.Key,
					ZoneId = zoneId,
					Codes = codes
				};

				int countryId = GetSaleZoneCountryId(zoneWrapper.ZoneId);
				if (!zonesWrapperByCountry.TryGetValue(countryId, out zonesWrapper))
				{
					zonesWrapper = new List<ExistingSaleZone>();
					zonesWrapper.Add(zoneWrapper);
					zonesWrapperByCountry.Add(countryId, zonesWrapper);
				}
				else
					zonesWrapper.Add(zoneWrapper);
			}

			return zonesWrapperByCountry;
		}

		private Dictionary<int, List<SalePLZoneChange>> StructureZoneChangesByCountry(IEnumerable<SalePLZoneChange> zoneChanges)
		{
			Dictionary<int, List<SalePLZoneChange>> existingSaleCodesByCountryId = new Dictionary<int, List<SalePLZoneChange>>();
			if (zoneChanges != null)
			{
				List<SalePLZoneChange> zoneChangesList;
				foreach (SalePLZoneChange zoneChange in zoneChanges)
				{
					if (!existingSaleCodesByCountryId.TryGetValue(zoneChange.CountryId, out zoneChangesList))
					{
						zoneChangesList = new List<SalePLZoneChange>();
						zoneChangesList.Add(zoneChange);
						existingSaleCodesByCountryId.Add(zoneChange.CountryId, zoneChangesList);
					}
					else
						zoneChangesList.Add(zoneChange);
				}
			}

			return existingSaleCodesByCountryId;
		}

		#endregion

		#endregion

		#region  Private Members

		public Dictionary<int, SalePriceList> GetCachedSalePriceLists()
		{
			return Vanrise.Caching.CacheManagerFactory.GetCacheManager<CacheManager>().GetOrCreateObject(String.Format("GetCashedSalePriceLists"), () =>
			{
				Dictionary<int, SalePriceList> allSalePriceLists = GetCachedSalePriceListsWithDeleted();

				var notDeletedSalePriceLists = new Dictionary<int, SalePriceList>();
				foreach (SalePriceList salePriceList in allSalePriceLists.Values)
				{
					if (!salePriceList.IsDeleted)
						notDeletedSalePriceLists.Add(salePriceList.PriceListId, salePriceList);
				}
				return notDeletedSalePriceLists;
			});
		}

		private Dictionary<int, SalePriceList> GetCachedSalePriceListsWithDeleted()
		{
			return Vanrise.Caching.CacheManagerFactory.GetCacheManager<CacheManager>().GetOrCreateObject(String.Format("AllSalePriceLists"),
			   () =>
			   {
				   ISalePriceListDataManager dataManager = BEDataManagerFactory.GetDataManager<ISalePriceListDataManager>();
				   IEnumerable<SalePriceList> salePriceLists = dataManager.GetPriceLists();
				   Dictionary<int, SalePriceList> dic = new Dictionary<int, SalePriceList>();

				   foreach (SalePriceList item in salePriceLists)
				   {
					   if (item.OwnerType == SalePriceListOwnerType.Customer && _carrierAccountManager.IsCarrierAccountDeleted(item.OwnerId))
						   item.IsDeleted = true;

					   dic.Add(item.PriceListId, item);
				   }
				   return dic;
			   });
		}

		private class CacheManager : Vanrise.Caching.BaseCacheManager
		{
			ISalePriceListDataManager _dataManager = BEDataManagerFactory.GetDataManager<ISalePriceListDataManager>();
			object _updateHandle;

			public override Vanrise.Caching.CacheObjectSize ApproximateObjectSize
			{
				get
				{
					return Vanrise.Caching.CacheObjectSize.Large;
				}
			}

			protected override bool ShouldSetCacheExpired(object parameter)
			{
				return _dataManager.ArGetSalePriceListsUpdated(ref _updateHandle);
			}
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

		private SalePriceListType GetSalePriceListType(bool isAtoZ, SalePLChangeType changeType)
		{
			if (isAtoZ || changeType == SalePLChangeType.CountryAndRate)
				return SalePriceListType.Full;
			else
			{
				return (changeType == SalePLChangeType.CodeAndRate) ? SalePriceListType.Country : SalePriceListType.RateChange;
			}
		}

		#endregion

		#region Private Classes

		private class ExistingSaleZone
		{
			public long ZoneId { get; set; }

			public string ZoneName { get; set; }

			public List<ExistingSaleCode> Codes { get; set; }
		}

		private class ExistingSaleCode
		{
			public string Code { get; set; }

			public DateTime BED { get; set; }

			public DateTime? EED { get; set; }
		}

		#endregion

		#region Mappers

		private SalePriceListDetail SalePricelistDetailMapper(SalePriceList priceList)
		{
			SalePriceListDetail pricelistDetail = new SalePriceListDetail();
			pricelistDetail.Entity = priceList;
			pricelistDetail.OwnerType = Vanrise.Common.Utilities.GetEnumDescription(priceList.OwnerType);
			pricelistDetail.PriceListTypeName = priceList.PriceListType.HasValue ? Vanrise.Common.Utilities.GetEnumDescription(priceList.PriceListType.Value) : null;


			if (priceList.OwnerType != SalePriceListOwnerType.Customer)
			{
				SellingProductManager productManager = new SellingProductManager();
				pricelistDetail.OwnerName = productManager.GetSellingProductName(priceList.OwnerId);
			}

			else
			{
				pricelistDetail.OwnerName = _carrierAccountManager.GetCarrierAccountName(priceList.OwnerId);
			}


			pricelistDetail.CurrencyName = GetCurrencyName(priceList.CurrencyId);
			return pricelistDetail;
		}

		private ExistingSaleCodeEntity ExistingSaleCodeEntityMapper(SaleCode saleCode)
		{
			return new ExistingSaleCodeEntity(saleCode)
			{
				CountryId = GetSaleZoneCountryId(saleCode.ZoneId),
				ZoneName = _saleZoneManager.GetSaleZoneName(saleCode.ZoneId)
			};
		}

		private SalePLRateNotification SalePLRateNotificationMapper(SaleEntityZoneRate saleEntityZoneRate)
		{
			return new SalePLRateNotification()
			{
				Rate = saleEntityZoneRate.Rate.Rate,
				BED = saleEntityZoneRate.Rate.BED,
				EED = saleEntityZoneRate.Rate.EED
			};
		}

		private SalePLCodeNotification SalePLCodeNotificationMapper(ExistingSaleCode saleCode)
		{
			return new SalePLCodeNotification()
			{
				Code = saleCode.Code,
				BED = saleCode.BED,
				EED = saleCode.EED
			};
		}

		private int GetSaleZoneCountryId(long saleZoneId)
		{
			int? countryId = _saleZoneManager.GetSaleZoneCountryId(saleZoneId);
			if (!countryId.HasValue)
				throw new Vanrise.Entities.DataIntegrityValidationException(string.Format("Could not find the Country of Sale Zone '{0}'", saleZoneId));
			return countryId.Value;
		}

		#endregion
	}
}
