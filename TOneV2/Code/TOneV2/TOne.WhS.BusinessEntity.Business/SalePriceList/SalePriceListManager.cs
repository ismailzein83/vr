using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Net.Mime;
using System.Text;
using System.Threading.Tasks;
using TOne.WhS.BusinessEntity.Data;
using TOne.WhS.BusinessEntity.Entities;
using Vanrise.Common;
using Vanrise.Common.Business;
using Vanrise.Entities;
using Vanrise.GenericData.Pricing;
using Vanrise.Security.Business;
using Vanrise.Security.Entities;

namespace TOne.WhS.BusinessEntity.Business
{
	public class SalePriceListManager
	{
		#region Private Fields

		private SaleZoneManager _saleZoneManager = new SaleZoneManager();

		private CarrierAccountManager _carrierAccountManager = new CarrierAccountManager();

		#endregion

		#region Public Methods

		public void SavePriceList(ISalePricelistFileContext context)
		{
			List<NewPriceList> newPriceLists = new List<NewPriceList>();
			if (context.SalePriceLists != null)
			{
				var sellingProductPricelists = context.SalePriceLists.FindAllRecords(x => x.OwnerType == SalePriceListOwnerType.SellingProduct);
				if (sellingProductPricelists != null && sellingProductPricelists.Any()) newPriceLists.AddRange(sellingProductPricelists);
				newPriceLists.AddRange(context.SalePriceLists.FindAllRecords(item => item.PriceListType == SalePriceListType.None));
			}
			if (context.CustomerPriceListChanges != null && context.CustomerPriceListChanges.Any())
			{
				var customerSellingProductManager = new CustomerSellingProductManager();

				IEnumerable<SaleCode> saleCodes = new SaleCodeManager().GetSaleCodesEffectiveAfter(context.SellingNumberPlanId, context.EffectiveDate, context.ProcessInstanceId);
				if (saleCodes == null || !saleCodes.Any())
					return;

				IEnumerable<ExistingSaleCodeEntity> existingSaleCodeEntities = saleCodes.MapRecords(ExistingSaleCodeEntityMapper);
				Dictionary<string, Dictionary<string, List<ExistingSaleCodeEntity>>> existingSaleCodesByZoneName = StructureExistingSaleCodesByZoneName(existingSaleCodeEntities);
				Dictionary<int, List<ExistingSaleZone>> zoneWrappersByCountry = StructureZoneWrappersByCountry(existingSaleCodesByZoneName);
				var customerIdsWithChanges = context.CustomerPriceListChanges.Select(c => c.CustomerId);

				IEnumerable<RoutingCustomerInfoDetails> dataByCustomerList = GetDataByCustomer(customerIdsWithChanges, context.EffectiveDate);

				var customerZoneRateHistoryLocator = new CustomerZoneRateHistoryLocator(new CustomerZoneRateHistoryReader(dataByCustomerList.Select(item => item.CustomerId), dataByCustomerList.Select(item => item.SellingProductId), new List<long>(), true, true));
				List<long> pricelistIds = new List<long>();

				int numberOFRemainingPricelists = context.CustomerPriceListChanges.Count();
				foreach (var customerChange in context.CustomerPriceListChanges)
				{
					string customerName = _carrierAccountManager.GetCarrierAccountName(customerChange.CustomerId);
					context.WriteMessageToWorkflowLogs("Creating pricelist for customer {0}", customerName);

					SalePriceListType? priceListTypeForMultipleCurrency = null;
					if (customerChange.PriceLists.Count() > 1)
						priceListTypeForMultipleCurrency = SalePriceListType.Country;

					int customerId = customerChange.CustomerId;
					int? sellingProductId = customerSellingProductManager.GetEffectiveSellingProductId(customerId, DateTime.Now, false);

					if (!sellingProductId.HasValue)
						throw new DataIntegrityValidationException(string.Format("Customer with Id {0} is not assigned to a selling product", customerId));

					foreach (var customerPriceList in customerChange.PriceLists)
					{
						SalePriceListType overriddenListType;
						var customerPriceListType = _carrierAccountManager.GetCustomerPriceListType(customerId);
						var pricelistType = priceListTypeForMultipleCurrency ?? GetSalePriceListType(customerPriceListType, context.ChangeType);

						List<SalePLZoneNotification> customerZoneNotifications = CreateNotifications(customerId, sellingProductId.Value, pricelistType,
								customerPriceList.CountryChanges, zoneWrappersByCountry, out overriddenListType, customerZoneRateHistoryLocator);
						customerPriceList.PriceList.PriceListType = overriddenListType;

						customerZoneNotifications = FilterSalePlZoneNotification(customerId, customerZoneNotifications);

						if (customerZoneNotifications.Any())
						{
							int salePricelistTemplateId = _carrierAccountManager.GetCustomerPriceListTemplateId(customerId);
							long fileId = AddPriceListFile(customerId, customerZoneNotifications, context.EffectiveDate, customerPriceList.PriceList.PriceListType.Value, salePricelistTemplateId, customerPriceList.CurrencyId, customerPriceList.PriceList.PriceListId);
							customerPriceList.PriceList.FileId = fileId;
							pricelistIds.Add(customerPriceList.PriceList.PriceListId);
							newPriceLists.Add(customerPriceList.PriceList);
						}
					}

					numberOFRemainingPricelists--;
					context.WriteMessageToWorkflowLogs("Finished creating pricelist for customer {0}. {1} pricelist(s) are remaining", customerName, numberOFRemainingPricelists);
				}

				SaveChangesToDB(context.CustomerPriceListChanges, context.ProcessInstanceId);
				BulkInsertSalePriceListSnapshot(saleCodes.Select(item => item.SaleCodeId).ToList(), pricelistIds);
			}
			if (newPriceLists.Any()) BulkInsertPriceList(newPriceLists);
		}

		public List<StructuredCustomerPricelistChange> StructureCustomerPricelistChange(List<CustomerPriceListChange> customerPriceListChanges)
		{
			return
				customerPriceListChanges.Select(
					customerPriceListChange => StructureCustomerPricelistChange(customerPriceListChange)).ToList();
		}

		public StructuredCustomerPricelistChange StructureCustomerPricelistChange(CustomerPriceListChange customerPricelistChange)
		{
			var changesByCountryId = new Dictionary<int, CountryGroup>();
			foreach (var rate in customerPricelistChange.RateChanges)
			{
				CountryGroup countryGroup;
				if (!changesByCountryId.TryGetValue(rate.CountryId, out countryGroup))
				{
					countryGroup = new CountryGroup
					{
						CountryId = rate.CountryId
					};
					changesByCountryId.Add(rate.CountryId, countryGroup);
				}
				countryGroup.RateChanges.Add(rate);
			}
			foreach (var code in customerPricelistChange.CodeChanges)
			{
				CountryGroup countryGroup;
				if (!changesByCountryId.TryGetValue(code.CountryId, out countryGroup))
				{
					countryGroup = new CountryGroup
					{
						CountryId = code.CountryId
					};
					changesByCountryId.Add(code.CountryId, countryGroup);
				}
				countryGroup.CodeChanges.Add(code);
			}
			foreach (var routingProduct in customerPricelistChange.RoutingProductChanges)
			{
				CountryGroup countryGroup;
				if (!changesByCountryId.TryGetValue(routingProduct.CountryId, out countryGroup))
				{
					countryGroup = new CountryGroup
					{
						CountryId = routingProduct.CountryId
					};
					changesByCountryId.Add(routingProduct.CountryId, countryGroup);
				}
				countryGroup.RPChanges.Add(routingProduct);
			}

			return new StructuredCustomerPricelistChange
			{
				CustomerId = customerPricelistChange.CustomerId,
				CountryGroups = changesByCountryId.Values.ToList()
			};
		}
		public bool SendPriceList(long salePriceListId)
		{
			var salePriceListManager = new SalePriceListManager();
			SalePriceList customerPriceList = salePriceListManager.GetPriceList((int)salePriceListId);
			int salePricelistTemplateId = _carrierAccountManager.GetCustomerPriceListTemplateId(customerPriceList.OwnerId);

			if (!customerPriceList.PriceListType.HasValue)
				throw new VRBusinessException(string.Format("Customer Pricelist with id {0} has its type as null", customerPriceList.PriceListId));

			VRFile file = null;//PreparePriceListVrFile(customerPriceList, customerPriceList.PriceListType.Value, salePricelistTemplateId);
			var notificationManager = new NotificationManager();
			int userId = Vanrise.Security.Entities.ContextFactory.GetContext().GetLoggedInUserId();

			return file != null && notificationManager.SendSalePriceList(userId, customerPriceList, file);
		}
		public bool CheckIfAnyPriceListExists(SalePriceListOwnerType ownerType, int ownerId)
		{
			Dictionary<int, SalePriceList> allSalePriceLists = GetCachedSalePriceLists();
			return allSalePriceLists.Values.Any(x => x.OwnerType == ownerType && x.OwnerId == ownerId);
		}
		public VRMailEvaluatedTemplate EvaluateEmail(long pricelistId, SalePriceListType salePriceListType)
		{
			SalePriceListManager priceListManager = new SalePriceListManager();
			var customerPricelist = priceListManager.GetPriceList((int)pricelistId);

			if (customerPricelist == null)
				return null;

			var clonedPriceList = Utilities.CloneObject(customerPricelist);
			clonedPriceList.PriceListType = salePriceListType;

			CarrierAccountManager carrierAccountManager = new CarrierAccountManager();
			int ownerId = clonedPriceList.OwnerId;
			var customer = carrierAccountManager.GetCarrierAccount(ownerId);
			var salePlmailTemplateId = carrierAccountManager.GetCustomerPriceListMailTemplateId(ownerId);

			UserManager userManager = new UserManager();
			User initiator = userManager.GetUserbyId(SecurityContext.Current.GetLoggedInUserId());
			CompanySetting companySetting = _carrierAccountManager.GetCompanySetting(ownerId);

			var objects = new Dictionary<string, dynamic>
			{
				{"Customer", customer},
				{"User", initiator},
				{"Sale Pricelist", clonedPriceList},
				{"Company Setting", companySetting}
			};
			VRMailManager vrMailManager = new VRMailManager();
			return vrMailManager.EvaluateMailTemplate(salePlmailTemplateId, objects);
		}
		public IEnumerable<SalePricelistVRFile> GenerateSalePriceListFiles(SalePriceListInput pricelisInput, out SalePriceListType overriddenListType)
		{
			var salePriceListManager = new SalePriceListManager();

			SalePriceList customerPriceList = salePriceListManager.GetPriceList(pricelisInput.PriceListId);

			IEnumerable<SalePricelistVRFile> files = PreparePriceListVrFiles(customerPriceList, (SalePriceListType)pricelisInput.PriceListTypeId, pricelisInput.PricelistTemplateId, out overriddenListType);

			return files;
		}
		public IDataRetrievalResult<SalePriceListDetail> GetFilteredPricelists(Vanrise.Entities.DataRetrievalInput<SalePriceListQuery> input)
		{
			Dictionary<int, SalePriceList> cachedSalePriceLists = GetCustomerCachedSalePriceLists();
			Func<SalePriceList, bool> filterExpression = salePriceList =>
			{
				if (salePriceList.PriceListType.HasValue && salePriceList.PriceListType.Value == SalePriceListType.None)
					return false;
				if (input.Query.OwnerId != null && input.Query.OwnerId != salePriceList.OwnerId)
					return false;
				if (input.Query.CreationDate.HasValue && salePriceList.CreatedTime.Date != input.Query.CreationDate)
					return false;
				if (input.Query.IncludedSalePriceListIds != null && !input.Query.IncludedSalePriceListIds.Contains(salePriceList.PriceListId))
					return false;
				if (input.Query.SalePricelistTypes != null && salePriceList.PriceListType == null)
					return false;
				if (input.Query.SalePricelistTypes != null && salePriceList.PriceListType != null && !input.Query.SalePricelistTypes.Contains(salePriceList.PriceListType.Value))
					return false;
				if (input.Query.UserIds != null && !input.Query.UserIds.Contains(salePriceList.UserId))
					return false;

				return true;
			};

			var resultProcessingHandler = new ResultProcessingHandler<SalePriceListDetail>()
			{
				ExportExcelHandler = new SalePriceListExportExcelHandler()
			};

			return Vanrise.Common.DataRetrievalManager.Instance.ProcessResult(input, cachedSalePriceLists.ToBigResult(input, filterExpression, SalePricelistDetailMapper), resultProcessingHandler);
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
		public bool SetCustomerPricelistsAsSent(IEnumerable<int> customerIds, int? priceListId)
		{
			ISalePriceListDataManager dataManager = BEDataManagerFactory.GetDataManager<ISalePriceListDataManager>();
			bool setAsSent = dataManager.SetCustomerPricelistsAsSent(customerIds, priceListId);
			if (setAsSent) Vanrise.Caching.CacheManagerFactory.GetCacheManager<CacheManager>().SetCacheExpired();
			return setAsSent;
		}
		public IEnumerable<SalePriceList> GetCustomerSalePriceListsByProcessInstanceId(long processInstanceId)
		{
			Dictionary<int, SalePriceList> allSalePriceLists = GetCachedSalePriceLists();
			SalePriceListOwnerType customerOwnerType = SalePriceListOwnerType.Customer;

			return allSalePriceLists.Values.FindAllRecords(itm => itm.ProcessInstanceId == processInstanceId && itm.OwnerType == customerOwnerType);
		}
		public IEnumerable<SalePriceList> GetCustomerSalePriceListsById(long customerId)
		{
			Dictionary<int, SalePriceList> allSalePriceLists = GetCachedSalePriceLists();
			SalePriceListOwnerType customerOwnerType = SalePriceListOwnerType.Customer;
			return allSalePriceLists.Values.FindAllRecords(itm => itm.OwnerId == customerId && itm.OwnerType == customerOwnerType);
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
		public IEnumerable<int> GetSalePriceListIdsByProcessInstanceId(long processInstanceId)
		{
			return GetCustomerCachedSalePriceLists().MapRecords(x => x.Value.PriceListId, x => x.Value.ProcessInstanceId == processInstanceId && x.Value.PriceListType != SalePriceListType.None);
		}

		public bool SendCustomerPriceLists(SendPricelistsInput input)
		{
			IEnumerable<SalePriceList> customerPriceLists = GetCustomerCachedSalePriceLists().Values.Where(item => input.PricelistIds.Contains(item.PriceListId));

			var fileManager = new VRFileManager();
			var salePriceListManager = new SalePriceListManager();
			var vrMailManager = new VRMailManager();
			List<VRMailAttachement> vrMailAttachements;
			var allEmailsHaveBeenSent = true;

			foreach (SalePriceList customerPriceList in customerPriceLists)
			{
				if (customerPriceList.IsSent)
					continue;

				vrMailAttachements = new List<VRMailAttachement>();
				VRFile customerPriceListFile = fileManager.GetFile(customerPriceList.FileId);
				var evaluatedObject = salePriceListManager.EvaluateEmail(customerPriceList.PriceListId, (SalePriceListType)customerPriceList.PriceListType);

				CarrierAccount customer = _carrierAccountManager.GetCarrierAccount(customerPriceList.OwnerId);
				vrMailAttachements.Add(new VRMailAttachmentExcel
				{
					Name = customerPriceListFile.Name,
					Content = customerPriceListFile.Content
				});

				try
				{
					bool isCompressed = input.CompressAttachement || _carrierAccountManager.GetCustomerCompressPriceListFileStatus(customer.CarrierAccountId);
					vrMailManager.SendMail(evaluatedObject.To, evaluatedObject.CC, evaluatedObject.BCC, evaluatedObject.Subject, evaluatedObject.Body
						, vrMailAttachements, isCompressed);
					salePriceListManager.SetCustomerPricelistsAsSent(new List<int> { customerPriceList.OwnerId }, customerPriceList.PriceListId);
				}
				catch (Exception)
				{
					allEmailsHaveBeenSent = false;
				}
			}
			return allEmailsHaveBeenSent;
		}
		public SendCustomerPricelistsResponse SendPricelistsWithCheckNotSendPreviousPricelists(SendPricelistsWithCheckPreviousInput input)
		{
			Func<SalePriceList, bool> idFilterExpression = null;

			if (input.SelectAll)
			{
				if (input.NotSelectedPriceListIds != null && input.NotSelectedPriceListIds.Count() > 0)
					idFilterExpression = (salePriceList) => { return !input.NotSelectedPriceListIds.Contains(salePriceList.PriceListId); };
			}
			else
			{
				if (input.SelectedPriceListIds == null || input.SelectedPriceListIds.Count() == 0)
					throw new Vanrise.Entities.MissingArgumentValidationException("selectedPriceListIds");
				idFilterExpression = (salePriceList) => { return input.SelectedPriceListIds.Contains(salePriceList.PriceListId); };
			}

			Func<SalePriceList, bool> filterExpression = (salePriceList) =>
			{
				if (salePriceList.ProcessInstanceId != input.ProcessInstanceId)
					return false;

				if (idFilterExpression != null && !idFilterExpression(salePriceList))
					return false;

				return true;
			};

			IEnumerable<SalePriceList> customerPricelists = GetCustomerCachedSalePriceLists().FindAllRecords(filterExpression);
			CarrierAccountManager carrierAccountManager = new CarrierAccountManager();
			Dictionary<int, CarrierAccountInfo> customers = new Dictionary<int, CarrierAccountInfo>();
			List<int> customerPricelistIds = customerPricelists.Select(item => item.PriceListId).ToList();
			bool allEmailsHaveBeenSent = false;

			foreach (var customerPricelist in customerPricelists)
			{
				if (!customers.ContainsKey(customerPricelist.OwnerId) && CheckIfCustomerHasNotSendPricelists(customerPricelist.OwnerId, customerPricelist.CreatedTime))
				{
					customers.Add(
						customerPricelist.OwnerId,
						new CarrierAccountInfo
						{
							CarrierAccountId = customerPricelist.OwnerId,
							Name = carrierAccountManager.GetCarrierAccountName(customerPricelist.OwnerId)
						});
				}
			}
			if (customers.Count == 0)
				allEmailsHaveBeenSent = SendCustomerPriceLists(new SendPricelistsInput { PricelistIds = customerPricelistIds, CompressAttachement = input.CompressAttachement });

			var sendCustomerPricelistsResponse = new SendCustomerPricelistsResponse
			{
				Customers = customers.Values.ToList(),
				PricelistIds = customerPricelistIds,
				AllEmailsHaveBeenSent = allEmailsHaveBeenSent,
			};

			return sendCustomerPricelistsResponse;
		}
		private bool CheckIfCustomerHasNotSendPricelists(int customerId, DateTime pricelistCreateDate)
		{
			Dictionary<int, SalePriceList> allSalePriceLists = GetCustomerCachedSalePriceLists();
			IEnumerable<SalePriceList> salePricelists = allSalePriceLists.Values.FindAllRecords(itm => itm.OwnerId == customerId && itm.CreatedTime < pricelistCreateDate && itm.IsSent != true);
			return (salePricelists.Count() > 0);
		}
		public bool CheckIfCustomerHasNotSendPricelists(int pricelistId)
		{
			SalePriceList salePriceList = GetPriceList(pricelistId);
			return CheckIfCustomerHasNotSendPricelists(salePriceList.OwnerId, salePriceList.CreatedTime);
		}
		public List<NewCustomerPriceListChange> CreateCustomerChanges(List<StructuredCustomerPricelistChange> customerPriceListChanges, SaleEntityZoneRateLocator lastRateNoCacheLocator
		, Dictionary<int, List<NewPriceList>> salePriceListsByCurrencyId, DateTime effectiveDate, long processInstanceId, int userId)
		{
			List<NewCustomerPriceListChange> customerChanges = new List<NewCustomerPriceListChange>();
			foreach (var customerPriceListChange in customerPriceListChanges)
			{
				var pricelistChanges = GetPricelistChanges(customerPriceListChange, lastRateNoCacheLocator, salePriceListsByCurrencyId, effectiveDate, processInstanceId, userId);
				customerChanges.Add(new NewCustomerPriceListChange
				{
					CustomerId = customerPriceListChange.CustomerId,
					PriceLists = pricelistChanges
				});
			}
			ReservePriceListIds(salePriceListsByCurrencyId);
			return customerChanges;
		}

		#endregion

		#region Generate Pricelist Methods

		#region Preparation Methods

		private SalePriceListOutputContext PrepareSalePriceListContext(SalePriceListInputContext context)
		{
			var srtucturedCustomer = StructureCustomerPricelistChange(context.CustomerPriceListChange);
			var countryChanges = TransformToNewCustomerPriceListChange(srtucturedCustomer);

			IEnumerable<ExistingSaleCodeEntity> existingSaleCodeEntities = context.SaleCodes.MapRecords(ExistingSaleCodeEntityMapper);
			Dictionary<string, Dictionary<string, List<ExistingSaleCodeEntity>>> existingSaleCodesByZoneName = StructureExistingSaleCodesByZoneName(existingSaleCodeEntities);
			Dictionary<int, List<ExistingSaleZone>> zoneWrappersByCountry = StructureZoneWrappersByCountry(existingSaleCodesByZoneName);

			RoutingCustomerInfoDetails routingCustomerInfoDetails = new RoutingCustomerInfoDetails
			{
				CustomerId = context.CustomerId,
				SellingProductId = context.SellingProductId
			};

			var customerZoneRateHistoryLocator = new CustomerZoneRateHistoryLocator(new CustomerZoneRateHistoryReader(new List<int> { context.CustomerId }, new List<int> { context.SellingProductId }, new List<long>(), true, true));	
			return new SalePriceListOutputContext
			{
				CustomerZoneRateHistoryLocator = customerZoneRateHistoryLocator,
				CountryChanges = countryChanges,
				ZoneWrappersByCountry = zoneWrappersByCountry
			};
		}
		private IEnumerable<RoutingCustomerInfoDetails> GetDataByCustomer(IEnumerable<int> customerIds, DateTime effectiveOn)
		{
			var list = new List<RoutingCustomerInfoDetails>();
			var customerSellingProductManager = new CustomerSellingProductManager();

			foreach (int customerId in customerIds)
			{
				int? sellingProductId = customerSellingProductManager.GetEffectiveSellingProductId(customerId, effectiveOn, false);

				if (!sellingProductId.HasValue)
					throw new DataIntegrityValidationException(string.Format("Customer '{0}' is not assigned to a Selling Product", customerId));

				list.Add(new RoutingCustomerInfoDetails
				{
					CustomerId = customerId,
					SellingProductId = sellingProductId.Value
				});
			}
			return list;
		}

		#endregion

		#region Merge with Existing Data
		private List<SalePLZoneNotification> CreateNotifications(int customerId, int sellingProductId, SalePriceListType pricelistType, List<CountryChange> countryChanges,
		Dictionary<int, List<ExistingSaleZone>> existingDataByCountryId, out SalePriceListType overiddenPriceListType, CustomerZoneRateHistoryLocator customerZoneRateHistoryLocator)
		{
			Dictionary<int, DateTime> customerCountriesSellDatesByCountryId = GetCustomerCountriesSellDatesDictionary(customerId);
			overiddenPriceListType = pricelistType;
			var salePlZoneNotifications = GetChangeOrCountryNotification(customerId, sellingProductId, pricelistType, existingDataByCountryId, countryChanges, customerCountriesSellDatesByCountryId, customerZoneRateHistoryLocator);

			if (pricelistType != SalePriceListType.Full) //Send zone changes with missing zones from their countries
				return salePlZoneNotifications;

			int changesCurrency = salePlZoneNotifications.First().Rate.CurrencyId.Value;
			List<SalePLZoneNotification> saleNotifications;

			Dictionary<int, List<SalePLZoneNotification>> zoneNotificationByCurrencyId = GetFullSalePlZoneNotification(customerId, sellingProductId, existingDataByCountryId, countryChanges,
					customerCountriesSellDatesByCountryId, customerZoneRateHistoryLocator);

			if (zoneNotificationByCurrencyId.Count > 1 || !zoneNotificationByCurrencyId.TryGetValue(changesCurrency, out saleNotifications))
			{
				overiddenPriceListType = SalePriceListType.Country;
				return salePlZoneNotifications;
			}

			var rpChanges = countryChanges.SelectMany(it => it.ZoneChanges.Where(r => r.RPChange != null).Select(rp => rp.RPChange)).ToList();
			AddRPChangesToSalePLNotification(saleNotifications, rpChanges, customerId, sellingProductId);

			salePlZoneNotifications.AddRange(saleNotifications);

			//In this case the pricelist type is Full then we need to return all changed zones with their missing zones in their countries and the other sold countries
			return salePlZoneNotifications;
		}

		private Dictionary<int, List<SalePLZoneNotification>> CreateMultipleNotifications(int customerId, int sellingProductId, SalePriceListType pricelistType, List<CountryChange> countryChanges,
	  Dictionary<int, List<ExistingSaleZone>> existingDataByCountryId, out SalePriceListType overiddenPriceListType, CustomerZoneRateHistoryLocator customerZoneRateHistoryLocator)
		{
			Dictionary<int, DateTime> customerCountriesSellDatesByCountryId = GetCustomerCountriesSellDatesDictionary(customerId);
			var saleZoneNotificationByCurrencyId = new Dictionary<int, List<SalePLZoneNotification>>();
			var salePlZoneNotifications = GetChangeOrCountryNotification(customerId, sellingProductId, pricelistType, existingDataByCountryId, countryChanges, customerCountriesSellDatesByCountryId, customerZoneRateHistoryLocator);
			overiddenPriceListType = pricelistType;

			int changesCurrency = salePlZoneNotifications.First().Rate.CurrencyId.Value;
			saleZoneNotificationByCurrencyId.Add(changesCurrency, salePlZoneNotifications);

			if (pricelistType != SalePriceListType.Full)
				return saleZoneNotificationByCurrencyId;

			List<SalePLZoneNotification> saleNotifications;
			Dictionary<int, List<SalePLZoneNotification>> zoneNotifictionByCurrencyId = GetFullSalePlZoneNotification(customerId, sellingProductId, existingDataByCountryId, countryChanges, customerCountriesSellDatesByCountryId, customerZoneRateHistoryLocator);

			if (!zoneNotifictionByCurrencyId.TryGetValue(changesCurrency, out saleNotifications))
			{
				overiddenPriceListType = SalePriceListType.Country;
				zoneNotifictionByCurrencyId.Add(changesCurrency, salePlZoneNotifications);
			}
			else
				saleNotifications.AddRange(salePlZoneNotifications);

			var rpChanges = countryChanges.SelectMany(it => it.ZoneChanges.Where(r => r.RPChange != null).Select(rp => rp.RPChange)).ToList();
			AddRPChangesToSalePLNotification(zoneNotifictionByCurrencyId.Values.SelectMany(z => z), rpChanges, customerId, sellingProductId);

			return zoneNotifictionByCurrencyId;
		}

		private Dictionary<int, List<SalePLZoneNotification>> GetFullSalePlZoneNotification(int customerId, int sellingProductId, Dictionary<int, List<ExistingSaleZone>> existingDataByCountryId, List<CountryChange> countryChanges
			, Dictionary<int, DateTime> customerCountriesSellDatesByCountryId, CustomerZoneRateHistoryLocator customerZoneRateHistoryLocator)
		{
			var salePlZoneNotificationsByCurrencyId = new Dictionary<int, List<SalePLZoneNotification>>();
			//Add all missing sold countries to notification from exiting data
			var customerCountryManager = new CustomerCountryManager();
			var soldCountries = customerCountryManager.GetCustomerCountriesEffectiveAfter(customerId, DateTime.Now);
			if (soldCountries == null)
				return salePlZoneNotificationsByCurrencyId;

			var changedCountryIds = countryChanges.Select(it => it.CountryId);

			var zoneNotifictionByCurrencyId = new Dictionary<int, List<SalePLZoneNotification>>();
			foreach (var soldCountry in soldCountries)
			{
				if (changedCountryIds.Contains(soldCountry.CountryId))
					continue;
				DateTime countrySellDate;
				if (!customerCountriesSellDatesByCountryId.TryGetValue(soldCountry.CountryId, out countrySellDate))
					countrySellDate = DateTime.MinValue;
				List<ExistingSaleZone> existingZones = existingDataByCountryId.GetRecord(soldCountry.CountryId);
				List<SalePLZoneNotification> notifications = null;
				if (existingZones != null)
					notifications = GetZoneNotificationsFromExistingData(customerId, sellingProductId, existingZones, soldCountry.CountryId, countrySellDate, soldCountry.EED, customerZoneRateHistoryLocator);

				if (notifications == null) continue;

				//all zones in the same country have the same currency 
				int countryCurrency = notifications.First().Rate.CurrencyId.Value;

				List<SalePLZoneNotification> salePlZones;
				if (!zoneNotifictionByCurrencyId.TryGetValue(countryCurrency, out salePlZones))
				{
					salePlZones = new List<SalePLZoneNotification>();
					zoneNotifictionByCurrencyId.Add(countryCurrency, salePlZones);
				}
				salePlZones.AddRange(notifications);
			}
			return zoneNotifictionByCurrencyId;
		}
		private List<SalePLZoneNotification> GetChangeOrCountryNotification(int customerId, int sellingProductId, SalePriceListType pricelistType, Dictionary<int, List<ExistingSaleZone>> existingDataByCountryId, List<CountryChange> countryChanges, Dictionary<int, DateTime> customerCountriesSellDatesByCountryId, CustomerZoneRateHistoryLocator customerZoneRateHistoryLocator)
		{
			var salePlZoneNotifications = new List<SalePLZoneNotification>();
			//Create zone notifications from zone changes
			salePlZoneNotifications = CreateNotificationsForAllZoneChanges(customerId, sellingProductId, countryChanges, existingDataByCountryId, customerCountriesSellDatesByCountryId, customerZoneRateHistoryLocator);

			if (pricelistType == SalePriceListType.RateChange) //Only send changes zones
				return salePlZoneNotifications;

			foreach (var countryChange in countryChanges)//Add missing zones to notification from existing data for all changed countries
			{
				DateTime countrySellDate;
				if (customerCountriesSellDatesByCountryId.TryGetValue(countryChange.CountryId, out countrySellDate))
				{
					List<ExistingSaleZone> existingZones = existingDataByCountryId.GetRecord(countryChange.CountryId);
					salePlZoneNotifications.AddRange(GetZoneNotificationsFromExistingData(customerId, sellingProductId, existingZones, salePlZoneNotifications.Select(z => z.ZoneName), countryChange.CountryId, countrySellDate, customerZoneRateHistoryLocator));
				}
			}
			return salePlZoneNotifications;
		}
		private List<SalePLZoneNotification> CreateNotificationsForAllZoneChanges(int customerId, int sellingProductId, List<CountryChange> countryChanges,
		  Dictionary<int, List<ExistingSaleZone>> zoneWrappersByCountry, Dictionary<int, DateTime> customerCountriesSellDatesByCountryId, CustomerZoneRateHistoryLocator customerZoneRateHistoryLocator)
		{
			List<SalePLZoneNotification> salePlZoneNotifications = new List<SalePLZoneNotification>();

			Dictionary<long, SalePLZoneNotification> countryZoneNotificationsByZoneId;

			foreach (var country in countryChanges)
			{
				countryZoneNotificationsByZoneId = new Dictionary<long, SalePLZoneNotification>();
				List<ExistingSaleZone> existingSaleZones = zoneWrappersByCountry.GetRecord(country.CountryId);

				DateTime countrySellDate;
				if (!customerCountriesSellDatesByCountryId.TryGetValue(country.CountryId, out countrySellDate))
					countrySellDate = DateTime.MinValue;

				if (existingSaleZones == null) continue;

				foreach (var zoneChange in country.ZoneChanges)
				{
					ExistingSaleZone existingSaleZone = existingSaleZones.FirstOrDefault(z => z.ZoneName.Equals(zoneChange.ZoneName));

					long zoneId = existingSaleZone != null
						 ? existingSaleZone.ZoneId
						 : zoneChange.ZoneId;

					SalePLZoneNotification salePlZoneNotification = GetSalePlNotification(zoneId, zoneChange.ZoneName, countryZoneNotificationsByZoneId);

					if (zoneChange.CodeChanges != null)
					{
						foreach (var codeChange in zoneChange.CodeChanges)
						{
							if (codeChange.ChangeType == CodeChange.Moved)
							{
								ExistingSaleZone recentZone = existingSaleZones.FirstOrDefault(z => z.ZoneName.ToLower().Equals(codeChange.RecentZoneName.ToLower()));
								IEnumerable<ExistingSaleCode> existingCodes = recentZone.Codes.FindAllRecords(x => x.Code == codeChange.Code);
								ExistingSaleCode existingCode = existingCodes.FirstOrDefault();
								if (existingCode == null)
									throw new DataIntegrityValidationException(string.Format("No existing codes found for zone {0}", existingSaleZone.ZoneName));

								SalePLZoneNotification salePlZoneNotificationForRecentZone = GetSalePlNotification(recentZone.ZoneId, recentZone.ZoneName, countryZoneNotificationsByZoneId);
								DateTime existingCodeBED = (existingCode.BED > countrySellDate) ? existingCode.BED : countrySellDate;
								DateTime existingCodeEED = codeChange.BED > existingCodeBED ? codeChange.BED : existingCodeBED;

								salePlZoneNotificationForRecentZone.Codes.Add(new SalePLCodeNotification
								{
									Code = codeChange.Code,
									BED = existingCodeBED,
									EED = existingCodeEED,
									CodeChange = CodeChange.Closed
								});
							}

							DateTime codeBED = (codeChange.BED > countrySellDate) ? codeChange.BED : countrySellDate;
							DateTime? codeEED = codeChange.EED.VRGreaterThan(codeBED) ? codeChange.EED : codeBED;
							salePlZoneNotification.Codes.Add(new SalePLCodeNotification
							{
								Code = codeChange.Code,
								BED = codeBED,
								EED = codeEED,
								CodeChange = codeChange.ChangeType
							});
						}
					}
					var rateChange = zoneChange.RateChange;
					if (rateChange != null)
					{
						//Add the rate change as notification
						salePlZoneNotification.Rate = new SalePLRateNotification
						{
							Rate = rateChange.Rate,
							BED = rateChange.BED,
							RateChangeType = rateChange.ChangeType,
							EED = rateChange.EED,
							CurrencyId = rateChange.CurrencyId
						};
						salePlZoneNotification.Increment = GetIncrementDescription(customerId, zoneId, rateChange.BED);

					}
					else if (existingSaleZone != null)
					{
						salePlZoneNotification.Rate = GetRateNotificationFromExistingData(customerId, sellingProductId, existingSaleZone.ZoneId, existingSaleZone.ZoneName, country.CountryId, customerZoneRateHistoryLocator);
						salePlZoneNotification.Increment = GetIncrementDescription(customerId, zoneId, salePlZoneNotification.Rate.BED);
					}
				}
				List<SalePLZoneNotification> countrySaleNotifications = countryZoneNotificationsByZoneId.Values.ToList();
				//Add missing codes from existing data
				foreach (var salePlzone in countrySaleNotifications)
				{
					ExistingSaleZone existingSaleZone = existingSaleZones.FirstOrDefault(z => z.ZoneName.Equals(salePlzone.ZoneName));
					if (existingSaleZone != null)
					{
						foreach (ExistingSaleCode existingCode in existingSaleZone.Codes)
						{
							if (salePlzone.Codes.Any(x => x.Code == existingCode.Code))
								continue;

							salePlzone.Codes.Add(ExistingCodeToSalePLCodeNotificationMapper(existingCode, countrySellDate, null));
						}
						if (salePlzone.Rate == null)
							salePlzone.Rate = GetRateNotificationFromExistingData(customerId, sellingProductId, existingSaleZone.ZoneId, existingSaleZone.ZoneName, country.CountryId, customerZoneRateHistoryLocator);
					}
				}
				salePlZoneNotifications.AddRange(countryZoneNotificationsByZoneId.Values);
			}

			List<SalePricelistRPChange> rpChanges = StructureRPChange(countryChanges);
			AddRPChangesToSalePLNotification(salePlZoneNotifications, rpChanges, customerId, sellingProductId);
			return salePlZoneNotifications;
		}

		private List<SalePLZoneNotification> FilterSalePlZoneNotification(int customerId, List<SalePLZoneNotification> salePlZoneNotifications)
		{
			// this function returns a new set of notification filtered by code based on IncludeClosedEntitiesStatus
			// if OnlyFirstTime we will show the ended codes only the first time = > in changes
			// if UntilClosureDate we will show ended codes until EED< DateTime.Now
			//if Never closed codes are not included in the pricelist sheet

			List<SalePLZoneNotification> filteredZoneNotifications = new List<SalePLZoneNotification>();
			var closedCodeOption = _carrierAccountManager.GetCustomerIncludeClosedEntitiesStatus(customerId);
			foreach (var salePlZoneNotification in salePlZoneNotifications)
			{
				List<SalePLCodeNotification> filteredCodeNotifications = new List<SalePLCodeNotification>();

				var opennedCodes = salePlZoneNotification.Codes.Where(c => !c.EED.HasValue);
				var notChangedClosedCodes = salePlZoneNotification.Codes.Where(c => c.EED.HasValue && c.EED.Value > DateTime.Today && c.CodeChange == CodeChange.NotChanged);
				var changedClosedCodes = salePlZoneNotification.Codes.Where(c => c.CodeChange == CodeChange.Closed);

				filteredCodeNotifications.AddRange(opennedCodes);

				if (closedCodeOption == IncludeClosedEntitiesEnum.OnlyFirstTime)
					filteredCodeNotifications.AddRange(changedClosedCodes);

				if (closedCodeOption == IncludeClosedEntitiesEnum.UntilClosureDate)
				{
					filteredCodeNotifications.AddRange(changedClosedCodes);
					filteredCodeNotifications.AddRange(notChangedClosedCodes);
				}

				if (filteredCodeNotifications.Any())
				{
					SalePLZoneNotification filteredZoneNotification = new SalePLZoneNotification
					{
						ZoneId = salePlZoneNotification.ZoneId,
						ZoneName = salePlZoneNotification.ZoneName,
						Rate = salePlZoneNotification.Rate,
						Increment = salePlZoneNotification.Increment
					};
					filteredZoneNotification.Codes.AddRange(filteredCodeNotifications);
					filteredZoneNotifications.Add(filteredZoneNotification);
				}
			}

			if (filteredZoneNotifications.Any())
				return filteredZoneNotifications;
			return salePlZoneNotifications.FindAllRecords(item => item.Rate.RateChangeType == RateChangeType.Deleted).ToList();
		}

		private SalePLZoneNotification GetSalePlNotification(long zoneId, string zoneName, Dictionary<long, SalePLZoneNotification> salePLZoneNotificationByZoneId)
		{
			SalePLZoneNotification salePlZoneNotificationForRecentZone;
			if (!salePLZoneNotificationByZoneId.TryGetValue(zoneId, out salePlZoneNotificationForRecentZone))
			{
				salePlZoneNotificationForRecentZone = new SalePLZoneNotification
				{
					ZoneName = zoneName,
					ZoneId = zoneId
				};
				salePLZoneNotificationByZoneId.Add(zoneId, salePlZoneNotificationForRecentZone);
			}
			return salePlZoneNotificationForRecentZone;
		}

		private List<SalePLZoneNotification> GetZoneNotificationsFromExistingData(int customerId, int sellingProductId, IEnumerable<ExistingSaleZone> existingZones, int countryId, DateTime countrySellDate, DateTime? countryEED, CustomerZoneRateHistoryLocator customerZoneRateHistoryLocator)
		{
			List<SalePLZoneNotification> salePlZoneNotifications = new List<SalePLZoneNotification>();
			SaleZoneManager saleZoneManager = new SaleZoneManager();

			foreach (ExistingSaleZone existingZone in existingZones)
			{
				var existingZoneEntity = saleZoneManager.GetSaleZone(existingZone.ZoneId);
				if (existingZoneEntity == null)
					throw new DataIntegrityValidationException(string.Format("Zone {0} not found", existingZone.ZoneName));
				DateTime? existingZoneEED = existingZoneEntity.EED;
				if (existingZoneEED.HasValue && countrySellDate > existingZoneEED.Value)
					continue;
				if (countryEED.HasValue && countryEED <= existingZoneEntity.BED)
					continue;
				SalePLZoneNotification zoneNotification = new SalePLZoneNotification
				{
					ZoneId = existingZone.ZoneId,
					ZoneName = existingZone.ZoneName
				};
				zoneNotification.Codes.AddRange(existingZone.Codes.Select(item => ExistingCodeToSalePLCodeNotificationMapper(item, countrySellDate, countryEED)));
				zoneNotification.Rate = this.GetRateNotificationFromExistingData(customerId, sellingProductId, existingZone.ZoneId, existingZone.ZoneName, countryId, customerZoneRateHistoryLocator);
				zoneNotification.Increment = GetIncrementDescription(customerId, existingZone.ZoneId, zoneNotification.Rate.BED);

				salePlZoneNotifications.Add(zoneNotification);
			}
			return salePlZoneNotifications;
		}

		private SalePLRateNotification GetRateNotificationFromExistingData(int customerId, int sellingProductId, long zoneId, string zoneName, int countryId, CustomerZoneRateHistoryLocator customerZoneRateHistoryLocator)
		{
			var customerZoneRateHistory = customerZoneRateHistoryLocator.GetCustomerZoneRateHistory(customerId, sellingProductId, zoneName, null, countryId,null,null);
			var customerZoneLastRate = customerZoneRateHistory.Last();
			return new SalePLRateNotification()
			{
				Rate = customerZoneLastRate.Rate,
				BED = customerZoneLastRate.BED,
				EED = customerZoneLastRate.EED,
				RateChangeType = RateChangeType.NotChanged,
				CurrencyId = customerZoneLastRate.CurrencyId
			};
		}

		private List<SalePLZoneNotification> GetZoneNotificationsFromExistingData(int customerId, int sellingProductId, IEnumerable<ExistingSaleZone> existingZones, IEnumerable<string> changedZoneNames, int countryId, DateTime countrySellDate, CustomerZoneRateHistoryLocator customerZoneRateHistoryLocator)
		{
			List<SalePLZoneNotification> salePlZoneNotifications = new List<SalePLZoneNotification>();
			SaleZoneManager saleZoneManager = new SaleZoneManager();

			foreach (ExistingSaleZone existingZone in existingZones)
			{
				var existingZoneEntity = saleZoneManager.GetSaleZone(existingZone.ZoneId);
				if (existingZoneEntity == null)
					throw new DataIntegrityValidationException(string.Format("Zone {0} not found", existingZone.ZoneName));
				DateTime? existingZoneEED = existingZoneEntity.EED;
				if (changedZoneNames.Contains(existingZone.ZoneName) || (existingZoneEED.HasValue && countrySellDate > existingZoneEED.Value))
					continue;

				SalePLZoneNotification zoneNotification = new SalePLZoneNotification()
				{
					ZoneId = existingZone.ZoneId,
					ZoneName = existingZone.ZoneName
				};

				zoneNotification.Codes.AddRange(existingZone.Codes.Select(item => ExistingCodeToSalePLCodeNotificationMapper(item, countrySellDate, null)));
				zoneNotification.Rate = this.GetRateNotificationFromExistingData(customerId, sellingProductId, existingZone.ZoneId, existingZone.ZoneName, countryId, customerZoneRateHistoryLocator);
				zoneNotification.Increment = GetIncrementDescription(customerId, existingZone.ZoneId, zoneNotification.Rate.BED);
				salePlZoneNotifications.Add(zoneNotification);
			}

			AddRPChangesToSalePLNotification(salePlZoneNotifications, new List<SalePricelistRPChange>(), customerId, sellingProductId);
			return salePlZoneNotifications;
		}

		private string GetIncrementDescription(int customerId, long saleZoneId, DateTime pricelistDate)
		{
			var tariffRuleManager = new TariffRuleManager();
			var configManager = new ConfigManager();
			var ruleDefinitionId = configManager.GetCustomerTariffRuleDefinitionId();

			Dictionary<string, object> targetFieldsValue = new Dictionary<string, object>
			{
				{"CustomerId", customerId},
				{"SaleZoneId", saleZoneId}
			};

			Vanrise.GenericData.Entities.GenericRuleTarget target = new Vanrise.GenericData.Entities.GenericRuleTarget
			{
				EffectiveOn = pricelistDate,
				IsEffectiveInFuture = false,
				TargetFieldValues = targetFieldsValue
			};

			TariffRule tariffRule = tariffRuleManager.GetMatchRule(ruleDefinitionId, target);
			return tariffRule != null ? tariffRule.Settings.GetPricingDescription() : string.Empty;
		}
		#endregion

		#region Pricelist Management
		private void ReservePriceListIds(Dictionary<int, List<NewPriceList>> salePriceListsByCurrencyId)
		{
			SalePriceListManager salePriceListManager = new SalePriceListManager();
			var pricelists = salePriceListsByCurrencyId.Values.SelectMany(p => p).Where(p => p.PriceListId == 0);
			long startingReservedId = salePriceListManager.ReserveIdRange(pricelists.Count());
			foreach (var pricelist in pricelists)
			{
				pricelist.PriceListId = startingReservedId++;
			}
		}
		public string GetPriceListName(int carrierAccountId, DateTime priceListDate, SalePriceListType salePriceListType, string extension, int currencyId)
		{
			var currencyManager = new CurrencyManager();
			var customerName = _carrierAccountManager.GetCarrierAccountName(carrierAccountId);
			var carrierProfileManager = new CarrierProfileManager();
			var carrierProfileId = _carrierAccountManager.GetCarrierProfileId(carrierAccountId).Value;
			var profileName = carrierProfileManager.GetCarrierProfileName(carrierProfileId);
			var currencySymbol = currencyManager.GetCurrencySymbol(currencyId);

			var priceListName = new StringBuilder(_carrierAccountManager.GetCustomerPricelistFileNamePattern(carrierAccountId));
			priceListName.Replace("#CustomerName#", customerName);
			priceListName.Replace("#Currency#", currencySymbol);
			priceListName.Replace("#ProfileName#", profileName);
			priceListName.Replace("#PricelistDate#", priceListDate.ToString("yyyy-MM-dd"));
			priceListName.Replace("#PricelistType#", salePriceListType.ToString());

			priceListName = priceListName.Append(extension);
			return priceListName.ToString();
		}
		private VRFile GetPriceListFile(int carrierAccountId, List<SalePLZoneNotification> customerZonesNotifications, DateTime effectiveDate, SalePriceListType salePriceListType, int salePriceListTemplateId, int pricelistCurrencyId)
		{
			var salePriceListTemplateManager = new SalePriceListTemplateManager();

			var customerName = new CarrierAccountManager().GetCarrierAccountName(carrierAccountId);
			customerName.ThrowIfNull("customerName", carrierAccountId);

			SalePriceListTemplate template = salePriceListTemplateManager.GetSalePriceListTemplate(salePriceListTemplateId);

			if (template == null)
				throw new DataIntegrityValidationException(string.Format("Customer {0} is not assigned to any pricelist template", customerName));

			PriceListExtensionFormat priceListExtensionFormat = _carrierAccountManager.GetCustomerPriceListExtensionFormatId(carrierAccountId);
			ISalePriceListTemplateSettingsContext salePlTemplateSettingsContext = new SalePriceListTemplateSettingsContext
			{
				Zones = customerZonesNotifications,
				PriceListExtensionFormat = priceListExtensionFormat,
				CustomerId = carrierAccountId,
				PricelistType = salePriceListType,
				PricelistCurrencyId = pricelistCurrencyId,
				PricelistDate = effectiveDate
			};

			byte[] salePlTemplateBytes = template.Settings.Execute(salePlTemplateSettingsContext);

			string extension = GetExtensionString(priceListExtensionFormat);
			string fileName = GetPriceListName(carrierAccountId, effectiveDate, salePriceListType, extension, pricelistCurrencyId);

			return new VRFile
			{
				Content = salePlTemplateBytes,
				Name = fileName,
				ModuleName = "WhS_BE_SalePriceList",
				Extension = extension,
				IsTemp = true,
			};
		}
		private long AddPriceListFile(int carrierAccountId, List<SalePLZoneNotification> customerZonesNotifications, DateTime effectiveDate, SalePriceListType salePriceListType, int salePriceListTemplateId, int pricelistCurrencyId, long pricelistId)
		{
			var fileManager = new VRFileManager();
			var file = GetPriceListFile(carrierAccountId, customerZonesNotifications, effectiveDate, salePriceListType,
				salePriceListTemplateId, pricelistCurrencyId);
			var fileSettings = new VRFileSettings { ExtendedSettings = new SalePricelistFileSettings { PricelistId = pricelistId } };
			file.Settings = fileSettings;
			return fileManager.AddFile(file);
		}

		private string GetExtensionString(PriceListExtensionFormat priceListExtension)
		{
			switch (priceListExtension)
			{
				case PriceListExtensionFormat.XLS:
					return ".xls";
				case PriceListExtensionFormat.XLSX:
					return ".xlsx";
				default:
					return ".xls";
			}
		}

		private void SaveChangesToDB(IEnumerable<NewCustomerPriceListChange> customerChanges, long processInstanceId)
		{
			var salePricelistRateChanges = new List<SalePricelistRateChange>();
			var salePriceListCustomerChanges = new List<SalePriceListCustomerChange>();
			var salePricelistRpChanges = new List<SalePricelistRPChange>();
			Dictionary<long, List<SalePricelistCodeChange>> codeChangeByCountryId = new Dictionary<long, List<SalePricelistCodeChange>>();

			foreach (var customerChange in customerChanges)
			{
				foreach (var priceList in customerChange.PriceLists)
				{
					var priceListObject = priceList.PriceList;
					foreach (var countryChange in priceList.CountryChanges)
					{
						SalePriceListCustomerChange salePriceListCustomer = new SalePriceListCustomerChange
						{
							CountryId = countryChange.CountryId,
							CustomerId = customerChange.CustomerId,
							PriceListId = priceListObject.PriceListId,
							BatchId = processInstanceId

						};
						List<SalePricelistCodeChange> countryCodeChange;
						if (!codeChangeByCountryId.TryGetValue(countryChange.CountryId, out countryCodeChange))
						{
							countryCodeChange = new List<SalePricelistCodeChange>();
							countryCodeChange.AddRange(countryChange.ZoneChanges.SelectMany(c => c.CodeChanges));
							codeChangeByCountryId.Add(countryChange.CountryId, countryCodeChange);
						}
						//TODO: remove this loop, batch id can be set on structuring activity in the workflow and pricelistid is not needed and it is overriden on each pricelist
						foreach (var code in countryCodeChange)
						{
							code.BatchId = processInstanceId;
							code.PricelistId = priceList.PriceList.PriceListId;
						}

						//TODO: check if we can add pricelist id on structuring activity, we want to avoid looping again here
						foreach (var zoneChange in countryChange.ZoneChanges)
						{
							if (zoneChange.RPChange != null)
								zoneChange.RPChange.PriceListId = priceList.PriceList.PriceListId;
							if (zoneChange.RateChange != null)
								zoneChange.RateChange.PricelistId = priceList.PriceList.PriceListId;
						}
						salePriceListCustomerChanges.Add(salePriceListCustomer);
						salePricelistRateChanges.AddRange(countryChange.ZoneChanges.Where(r => r.RateChange != null).Select(r => r.RateChange));
						salePricelistRpChanges.AddRange(countryChange.ZoneChanges.Where(r => r.RPChange != null).Select(rp => rp.RPChange));
					}
				}
			}
			SalePriceListChangeManager salePriceListChangeManager = new SalePriceListChangeManager();
			salePriceListChangeManager.BulkCustomerSalePriceListChanges(salePriceListCustomerChanges, codeChangeByCountryId.Values.SelectMany(c => c), salePricelistRateChanges, salePricelistRpChanges, processInstanceId);
		}
		private void BulkInsertPriceList(IEnumerable<NewPriceList> salePriceLists)
		{
			ISalePriceListDataManager dataManager = BEDataManagerFactory.GetDataManager<ISalePriceListDataManager>();
			dataManager.SavePriceListsToDb(salePriceLists);
		}
		private void BulkInsertSalePriceListSnapshot(List<long> saleCodeIds, IEnumerable<long> priceListIds)
		{
			var salePriceListSaleCodeSnapshots = priceListIds.Select(priceListId => new SalePriceListSnapShot
			{
				PriceListId = priceListId,
				SnapShotDetail = new SnapShotDetail
				{
					CodeIds = saleCodeIds
				}
			});
			SalePriceListChangeManager salePriceListChangeManager = new SalePriceListChangeManager();
			salePriceListChangeManager.BulkInsertSalePriceListSaleCodeSnapshot(salePriceListSaleCodeSnapshots);
		}
		#endregion

		#region Structuring Methods

		private List<CountryChange> TransformToNewCustomerPriceListChange(StructuredCustomerPricelistChange structuredCustomerPricelistChange)
		{
			List<CountryChange> countryChanges = new List<CountryChange>();

			foreach (var countryGroup in structuredCustomerPricelistChange.CountryGroups)
			{
				var zoneChangesByZoneName = new Dictionary<string, SalePricelistZoneChange>();
				CountryChange countryChange = new CountryChange
				{
					CountryId = countryGroup.CountryId
				};
				foreach (var codeChange in countryGroup.CodeChanges)
				{
					SalePricelistZoneChange zoneChange;
					if (!zoneChangesByZoneName.TryGetValue(codeChange.ZoneName, out zoneChange))
					{
						zoneChange = new SalePricelistZoneChange
						{
							ZoneName = codeChange.ZoneName,
							ZoneId = codeChange.ZoneId.Value
						};
						zoneChangesByZoneName.Add(codeChange.ZoneName, zoneChange);
					}
					zoneChange.CodeChanges.Add(codeChange);
				}
				foreach (var rateChange in countryGroup.RateChanges)
				{
					SalePricelistZoneChange zoneChange;
					if (!zoneChangesByZoneName.TryGetValue(rateChange.ZoneName, out zoneChange))
					{
						zoneChange = new SalePricelistZoneChange
						{
							ZoneName = rateChange.ZoneName,
							ZoneId = rateChange.ZoneId.Value
						};
						zoneChangesByZoneName.Add(rateChange.ZoneName, zoneChange);
					}
					zoneChange.RateChange = rateChange;
				}
				foreach (var routingProduct in countryGroup.RPChanges)
				{
					SalePricelistZoneChange zoneChange;
					if (!zoneChangesByZoneName.TryGetValue(routingProduct.ZoneName, out zoneChange))
					{
						zoneChange = new SalePricelistZoneChange
						{
							ZoneName = routingProduct.ZoneName,
							ZoneId = routingProduct.ZoneId.Value
						};
						zoneChangesByZoneName.Add(routingProduct.ZoneName, zoneChange);
					}
					zoneChange.RPChange = routingProduct;
				}
				countryChange.ZoneChanges.AddRange(zoneChangesByZoneName.Values);
				countryChanges.Add(countryChange);
			}
			return countryChanges;
		}
		private List<SalePricelistRPChange> StructureRPChange(List<CountryChange> countryChanges)
		{
			List<SalePricelistRPChange> routingPRoducts = new List<SalePricelistRPChange>();
			foreach (var countryChange in countryChanges)
			{
				foreach (var salePricelistZoneChange in countryChange.ZoneChanges)
				{
					if (salePricelistZoneChange.RPChange != null)
						routingPRoducts.Add(salePricelistZoneChange.RPChange);
				}
			}
			return routingPRoducts;
		}

		private IEnumerable<PriceListChange> GetPricelistChanges(StructuredCustomerPricelistChange customerPricelistChange, SaleEntityZoneRateLocator lastRateNoCacheLocator, Dictionary<int, List<NewPriceList>> newPricelistsByCurrencyId
			, DateTime effectiveDate, long processInstanceId, int userId)
		{
			var carrierAccountManager = new CarrierAccountManager();
			int sellingProductId = carrierAccountManager.GetSellingProductId(customerPricelistChange.CustomerId);

			var saleRateManager = new SaleRateManager();

			Dictionary<int, PriceListChange> customerPricelistChangetByCurrencyId = new Dictionary<int, PriceListChange>();
			foreach (var countryGroup in customerPricelistChange.CountryGroups)
			{
				int countryCurrencyId = GetCurrencyIdForThisCountry(customerPricelistChange.CustomerId, sellingProductId, countryGroup.RateChanges, countryGroup.CodeChanges, countryGroup.RPChanges, lastRateNoCacheLocator, saleRateManager);

				IEnumerable<SalePricelistZoneChange> zoneChanges = this.GetZoneChanges(countryGroup);

				PriceListChange pricelistChange;
				if (!customerPricelistChangetByCurrencyId.TryGetValue(countryCurrencyId, out pricelistChange))
				{
					pricelistChange = new PriceListChange
					{
						PriceList = GetOrCreateNewPricelist(customerPricelistChange.CustomerId, countryCurrencyId, userId, processInstanceId, effectiveDate, newPricelistsByCurrencyId),
						CurrencyId = countryCurrencyId
					};
					customerPricelistChangetByCurrencyId.Add(countryCurrencyId, pricelistChange);
				}

				CountryChange countryChange = new CountryChange();
				countryChange.CountryId = countryGroup.CountryId;
				countryChange.ZoneChanges.AddRange(zoneChanges);

				pricelistChange.CountryChanges.Add(countryChange);
			}

			return customerPricelistChangetByCurrencyId.Values;
		}

		private NewPriceList GetOrCreateNewPricelist(int customerId, int currencyId, int userId, long processInstanceId, DateTime effectiveDate, Dictionary<int, List<NewPriceList>> newPricelistsByCurrencyId)
		{
			List<NewPriceList> newPriceLists;
			if (!newPricelistsByCurrencyId.TryGetValue(currencyId, out newPriceLists))
			{
				newPriceLists = new List<NewPriceList>();
				newPricelistsByCurrencyId.Add(currencyId, newPriceLists);
			}
			NewPriceList customerPricelist = newPriceLists.FindRecord(x => x.OwnerId == customerId && x.OwnerType == SalePriceListOwnerType.Customer && x.PriceListType != SalePriceListType.None);
			//foreach (var newPriceList in newPriceLists)
			//{
			//    if (newPriceList.OwnerId == customerId && newPriceList.OwnerType == SalePriceListOwnerType.Customer)
			//    {
			//        customerPricelist = newPriceList;
			//        break;
			//    }
			//}
			if (customerPricelist == null)
			{
				customerPricelist = new NewPriceList
				{
					OwnerId = customerId,
					CurrencyId = currencyId,
					OwnerType = SalePriceListOwnerType.Customer,
					PriceListType = SalePriceListType.Country,
					EffectiveOn = effectiveDate,
					ProcessInstanceId = processInstanceId,
					UserId = userId
				};
				newPriceLists.Add(customerPricelist);
			}
			return customerPricelist;
		}
		private IEnumerable<SalePricelistZoneChange> GetZoneChanges(CountryGroup countryGroup)
		{
			SalePricelistZoneChangeByZoneName zoneChanges = new SalePricelistZoneChangeByZoneName();

			foreach (var rate in countryGroup.RateChanges)
			{
				SalePricelistZoneChange zone = new SalePricelistZoneChange
				{
					ZoneId = rate.ZoneId.Value,
					ZoneName = rate.ZoneName,
					RateChange = rate
				};
				zone = zoneChanges.TryAddValue(zone);
			}
			foreach (var code in countryGroup.CodeChanges)
			{
				SalePricelistZoneChange zone = new SalePricelistZoneChange
				{
					ZoneId = code.ZoneId.Value,
					ZoneName = code.ZoneName
				};
				zone = zoneChanges.TryAddValue(zone);
				zone.CodeChanges.Add(code);
			}
			foreach (var rp in countryGroup.RPChanges)
			{
				SalePricelistZoneChange zone = new SalePricelistZoneChange
				{
					ZoneId = rp.ZoneId.Value,
					ZoneName = rp.ZoneName
				};
				zone = zoneChanges.TryAddValue(zone);
				zone.RPChange = rp;
			}

			return zoneChanges.GetZoneChanges();
		}

		private int GetCurrencyIdForThisCountry(int customerId, int sellingProductId, List<SalePricelistRateChange> rateChanges, List<SalePricelistCodeChange> codeChanges, List<SalePricelistRPChange> routingProductChanges
			, SaleEntityZoneRateLocator lastRateNoCacheLocator, SaleRateManager saleRateManager)
		{
			if (rateChanges != null && rateChanges.Any())
				return rateChanges.First().CurrencyId.Value;

			var codeChange = codeChanges.FirstOrDefault();

			long zoneId = codeChange != null ? codeChange.ZoneId.Value : routingProductChanges.First().ZoneId.Value;

			var rate = lastRateNoCacheLocator.GetCustomerZoneRate(customerId, sellingProductId, zoneId);
			if (rate == null)
				throw new DataIntegrityValidationException(string.Format("Zone {0} does neither have an explicit rate nor a default rate set for selling product. Additional info: customer with id {1}",
						codeChange.ZoneName, customerId));

			return saleRateManager.GetCurrencyId(rate.Rate);
		}


		private Dictionary<string, SalePricelistRPChange> StructureCustomerSaleRpChangesByZoneName(IEnumerable<SalePricelistRPChange> routingProductChanges)
		{
			Dictionary<string, SalePricelistRPChange> routingProductChangesByZoneName = new Dictionary<string, SalePricelistRPChange>();
			foreach (var rpChange in routingProductChanges)
			{
				if (!routingProductChangesByZoneName.ContainsKey(rpChange.ZoneName))
					routingProductChangesByZoneName.Add(rpChange.ZoneName, rpChange);
			}
			return routingProductChangesByZoneName;
		}

		#endregion

		#endregion

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

		#endregion

		#region  Private Members
		private IEnumerable<SalePricelistVRFile> PreparePriceListVrFiles(SalePriceList salePriceList, SalePriceListType salePriceListType, int salePricelistTemplateId, out SalePriceListType overriddenListType)
		{
			var carrierAccountManager = new CarrierAccountManager();
			var salePriceListChangeManager = new SalePriceListChangeManager();
			var vrFileManager = new VRFileManager();
			var customerSellingProductManager = new CustomerSellingProductManager();

			int sellingNumberPlanId = carrierAccountManager.GetSellingNumberPlanId(salePriceList.OwnerId);

			var customerPriceListChange = salePriceListChangeManager.GetCustomerChangesByPriceListId(salePriceList.PriceListId);
			var saleCodeSnapshot = salePriceListChangeManager.GetSalePriceListSaleCodeSnapShot(salePriceList.PriceListId);

			var customer = carrierAccountManager.GetCarrierAccount(salePriceList.OwnerId);
			var sellingProductId = customerSellingProductManager.GetEffectiveSellingProductId(customer.CarrierAccountId, DateTime.Now, false);
			var vrFiles = new List<SalePricelistVRFile>();

			if (!sellingProductId.HasValue)
				throw new DataIntegrityValidationException(string.Format("Customer with Id {0} is not assigned to a selling product", customer.CarrierAccountId));

			SalePriceListInputContext salePriceListContext = new SalePriceListInputContext
			{
				CustomerPriceListChange = customerPriceListChange,
				EffectiveDate = salePriceList.EffectiveOn,
				SellingNumberPlanId = sellingNumberPlanId,
				ProcessInstanceId = salePriceList.ProcessInstanceId,
				UserId = salePriceList.UserId,
				SaleCodes = saleCodeSnapshot,
				CustomerId = salePriceList.OwnerId,
				SellingProductId = sellingProductId.Value
			};

			SalePriceListOutputContext salePriceListOutput = PrepareSalePriceListContext(salePriceListContext);

			Dictionary<int, List<SalePLZoneNotification>> customerZoneNotificationsByCurrencyId = CreateMultipleNotifications(salePriceList.OwnerId, sellingProductId.Value, salePriceListType, salePriceListOutput.CountryChanges,
				salePriceListOutput.ZoneWrappersByCountry, out overriddenListType, salePriceListOutput.CustomerZoneRateHistoryLocator);

			var currencyManager = new CurrencyManager();

			foreach (var zoneNotification in customerZoneNotificationsByCurrencyId)
			{
				var filteredNotification = FilterSalePlZoneNotification(salePriceList.OwnerId, zoneNotification.Value);
				VRFile vrFile = GetPriceListFile(customer.CarrierAccountId, filteredNotification, salePriceListContext.EffectiveDate,
					overriddenListType, salePricelistTemplateId, zoneNotification.Key);

				vrFiles.Add(new SalePricelistVRFile
				{
					CurrencySymbol = currencyManager.GetCurrencySymbol(zoneNotification.Key),
					FileId = vrFileManager.AddFile(vrFile),
					FileName = vrFile.Name,
					FileExtension = vrFile.Extension
				});
			}
			return vrFiles;
		}
		private void AddRPChangesToSalePLNotification(IEnumerable<SalePLZoneNotification> customerZoneNotifications, List<SalePricelistRPChange> routingProductChanges, int customerId, int sellingProductId)
		{
			SaleEntityZoneRoutingProductLocator lastRoutingProductLocator = null;
			SaleEntityZoneRoutingProductLocator effectiveRoutingProductLocator = new SaleEntityZoneRoutingProductLocator(new SaleEntityRoutingProductReadWithCache(DateTime.Today));

			lastRoutingProductLocator = new SaleEntityZoneRoutingProductLocator(new SaleEntityRoutingProductReadLastRoutingProduct());

			var routingProductChangesByZoneName = StructureCustomerSaleRpChangesByZoneName(routingProductChanges);
			RoutingProductManager routingProductManager = new RoutingProductManager();
			foreach (var zoneNotification in customerZoneNotifications)
			{
				IEnumerable<int> servicesIds = new List<int>();
				var activeCodes = zoneNotification.Codes.FindAllRecords(item => item.EED == null);

				int? routingProductId = null;
				SalePricelistRPChange routinProductChange = routingProductChangesByZoneName.GetRecord(zoneNotification.ZoneName);
				if (routinProductChange != null)
					routingProductId = routinProductChange.RoutingProductId;
				else if (lastRoutingProductLocator != null && zoneNotification.ZoneId.HasValue)
				{
					SaleEntityZoneRoutingProduct saleEntityZoneRoutingProduct = null;
					if (activeCodes.Any())
						saleEntityZoneRoutingProduct = lastRoutingProductLocator.GetCustomerZoneRoutingProduct(customerId, sellingProductId, zoneNotification.ZoneId.Value);

					else
						saleEntityZoneRoutingProduct = effectiveRoutingProductLocator.GetCustomerZoneRoutingProduct(customerId, sellingProductId, zoneNotification.ZoneId.Value);

					if (saleEntityZoneRoutingProduct != null) routingProductId = saleEntityZoneRoutingProduct.RoutingProductId;
				}
				if (routingProductId.HasValue)
				{
					servicesIds = zoneNotification.ZoneId.HasValue
						 ? routingProductManager.GetZoneServiceIds(routingProductId.Value, zoneNotification.ZoneId.Value)
						 : routingProductManager.GetDefaultServiceIds(routingProductId.Value);
					zoneNotification.Rate.ServicesIds = servicesIds;
				}
				else throw new Exception(string.Format("No routing product is assigned for zone {0}. Additional Info: country containing this zone is sold for customer with id {1}.", zoneNotification.ZoneName, customerId));
			}
		}
		private Dictionary<long, DateTime> StructureZoneIdsWithActionBED(IEnumerable<SalePLZoneNotification> customerZoneNotifications)
		{
			Dictionary<long, DateTime> zoneIdsWithRateBED = new Dictionary<long, DateTime>();
			foreach (var notification in customerZoneNotifications)
			{
				if (notification.ZoneId.HasValue)
					zoneIdsWithRateBED.Add(notification.ZoneId.Value, notification.Rate.BED);
			}
			return zoneIdsWithRateBED;
		}
		private Dictionary<int, SalePriceList> GetCustomerCachedSalePriceLists()
		{
			return Vanrise.Caching.CacheManagerFactory.GetCacheManager<CacheManager>().GetOrCreateObject("GetCustomerCachedSalePriceLists", () =>
			{
				Dictionary<int, SalePriceList> allSalePriceLists = GetCachedSalePriceListsWithDeleted();

				var notDeletedSalePriceLists = new Dictionary<int, SalePriceList>();
				foreach (SalePriceList salePriceList in allSalePriceLists.Values)
				{
					if (!salePriceList.IsDeleted && salePriceList.OwnerType == SalePriceListOwnerType.Customer)
						notDeletedSalePriceLists.Add(salePriceList.PriceListId, salePriceList);
				}
				return notDeletedSalePriceLists;
			});
		}
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

		private SalePriceListType GetSalePriceListType(SalePriceListType priceListType, SalePLChangeType changeType)
		{
			if (priceListType == SalePriceListType.Full || changeType == SalePLChangeType.CountryAndRate)
				return SalePriceListType.Full;

			if (priceListType == SalePriceListType.Country || changeType == SalePLChangeType.CodeAndRate)
				return SalePriceListType.Country;
			return SalePriceListType.RateChange;
		}

		#endregion

		#region Private Classes

		public class SalePricelistFileSettings : Vanrise.Entities.VRFileExtendedSettings
		{
			public override Guid ConfigId
			{
				get { return new Guid("58969E92-EA02-4BAB-9F70-311DA96E39CB"); }
			}

			public long PricelistId { get; set; }

			Vanrise.Security.Business.SecurityManager s_securityManager = new Vanrise.Security.Business.SecurityManager();
			public override bool DoesUserHaveViewAccess(Vanrise.Entities.IVRFileDoesUserHaveViewAccessContext context)
			{
				return s_securityManager.HasPermissionToActions("WhS_BE/SalePricelist/GetFilteredSalePricelist", context.UserId);
			}
		}


		private class SalePriceListInputContext
		{
			public CustomerPriceListChange CustomerPriceListChange { get; set; }
			public DateTime EffectiveDate { get; set; }
			public int SellingNumberPlanId { get; set; }
			public long ProcessInstanceId { get; set; }
			public int UserId { get; set; }
			public IEnumerable<SaleCode> SaleCodes { get; set; }
			public int CustomerId { get; set; }
			public int SellingProductId { get; set; }
		}
		private class SalePriceListOutputContext
		{
			public Dictionary<int, List<ExistingSaleZone>> ZoneWrappersByCountry { get; set; }
			public List<CountryChange> CountryChanges { get; set; }
			public CustomerZoneRateHistoryLocator CustomerZoneRateHistoryLocator { get; set; }
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

		private ExistingSaleCodeEntity ExistingSaleCodeEntityMapper(SaleCode saleCode)
		{
			return new ExistingSaleCodeEntity(saleCode)
			{
				CountryId = GetSaleZoneCountryId(saleCode.ZoneId),
				ZoneName = _saleZoneManager.GetSaleZoneName(saleCode.ZoneId)
			};
		}

		private SalePLCodeNotification ExistingCodeToSalePLCodeNotificationMapper(ExistingSaleCode saleCode, DateTime countrySellDate, DateTime? countryEED)
		{
			return new SalePLCodeNotification()
			{
				Code = saleCode.Code,
				BED = (saleCode.BED > countrySellDate) ? saleCode.BED : countrySellDate,
				EED = countryEED.VRLessThan(saleCode.EED) ? countryEED : saleCode.EED,
				CodeChange = CodeChange.NotChanged
			};
		}

		private SalePLCodeNotification SalePLCodeChangeToSalePLNotificationMapper(SalePricelistCodeChange splCodeChange)
		{
			return new SalePLCodeNotification()
			{
				Code = splCodeChange.Code,
				BED = splCodeChange.BED,
				EED = splCodeChange.EED,
				CodeChange = splCodeChange.ChangeType
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

		#region GetCustomerCountriesSellDatesDictionary
		private Dictionary<int, DateTime> GetCustomerCountriesSellDatesDictionary(int customerId)
		{
			Dictionary<int, DateTime> customerCountriesByCountryId = new Dictionary<int, DateTime>();
			CustomerCountryManager customerCountryManager = new CustomerCountryManager();

			IEnumerable<CustomerCountry2> customerCountries = customerCountryManager.GetCustomerCountries(customerId);
			if (customerCountries != null)
			{
				foreach (var customerCountry in customerCountries)
				{
					DateTime countrySellDate;
					if (!customerCountriesByCountryId.TryGetValue(customerCountry.CountryId, out countrySellDate))
						customerCountriesByCountryId.Add(customerCountry.CountryId, customerCountry.BED);
					else if (customerCountry.BED < countrySellDate)
						customerCountriesByCountryId[customerCountry.CountryId] = customerCountry.BED;
				}
			}

			return customerCountriesByCountryId;
		}

		#endregion

		/*
        #region Merge Pricelists
        private List<CountryChange> StructureCountryChanges(List<SalePricelistRateChange> rateChanges, List<SalePricelistCodeChange> codeChanges, List<SalePricelistRPChange> routingProductChanges)
        {
            var countryChanges = new Dictionary<int, CountryChange>();

            foreach (var rateChange in rateChanges)
            {
                CountryChange countryChange;

                SalePricelistZoneChange zoneChange = new SalePricelistZoneChange() { ZoneId = rateChange.ZoneId.Value, ZoneName = rateChange.ZoneName };
                zoneChange.RateChange = rateChange;

                if (!countryChanges.TryGetValue(rateChange.CountryId, out countryChange))
                {
                    countryChange = new CountryChange
                    {
                        CountryId = rateChange.CountryId,
                        ZoneChanges = { zoneChange },
                    };
                    countryChanges.Add(rateChange.CountryId, countryChange);
                }
                else
                    countryChange.ZoneChanges.Add(zoneChange);
            }

            foreach (var codeChange in codeChanges)
            {
                CountryChange countryChange;
                SalePricelistZoneChange zoneChange = new SalePricelistZoneChange() { ZoneId = codeChange.ZoneId.Value, ZoneName = codeChange.ZoneName };
                zoneChange.CodeChanges.Add(codeChange);

                if (!countryChanges.TryGetValue(codeChange.CountryId, out countryChange))
                {

                    countryChange = new CountryChange
                    {
                        CountryId = codeChange.CountryId,
                        ZoneChanges = { zoneChange },
                    };
                    countryChanges.Add(codeChange.CountryId, countryChange);
                }
                else
                {
                    if (countryChange.ZoneChanges.Where(item => item.ZoneId == codeChange.ZoneId).First() == null)
                    {
                        countryChange.ZoneChanges.Add(zoneChange);
                    }
                    else
                    {
                        countryChange.ZoneChanges.Where(item => item.ZoneId == codeChange.ZoneId).First().CodeChanges.Add(codeChange);
                    }
                }
            }

            foreach (var routingProductChange in routingProductChanges)
            {
                CountryChange countryChange;
                SalePricelistZoneChange zoneChange = new SalePricelistZoneChange() { ZoneId = routingProductChange.ZoneId.Value, ZoneName = routingProductChange.ZoneName };
                zoneChange.RPChange = routingProductChange;

                if (!countryChanges.TryGetValue(routingProductChange.CountryId, out countryChange))
                {

                    countryChange = new CountryChange
                    {
                        CountryId = routingProductChange.CountryId,
                        ZoneChanges = { zoneChange },
                    };
                    countryChanges.Add(routingProductChange.CountryId, countryChange);
                }
                else
                {
                    if (countryChange.ZoneChanges.Where(item => item.ZoneId == routingProductChange.ZoneId).First() == null)
                    {
                        countryChange.ZoneChanges.Add(zoneChange);
                    }
                    else
                    {
                        countryChange.ZoneChanges.Where(item => item.ZoneId == routingProductChange.ZoneId).First().RPChange = routingProductChange;
                    }
                }
            }

            return countryChanges.Values.ToList();
        }
        #endregion*/

	}
}
