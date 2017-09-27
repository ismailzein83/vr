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

                var futureRateLocator = new SaleEntityZoneRateLocator(new SaleRateReadLastRateNoCache(dataByCustomerList, context.EffectiveDate.Date.AddSeconds(-1)));
                List<long> pricelistIds = new List<long>();

                foreach (var customerChange in context.CustomerPriceListChanges)
                {
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
                                customerPriceList.CountryChanges, zoneWrappersByCountry, futureRateLocator, out overriddenListType);
                        customerPriceList.PriceList.PriceListType = overriddenListType;

                        if (customerZoneNotifications.Any())
                        {
                            int salePricelistTemplateId = _carrierAccountManager.GetCustomerPriceListTemplateId(customerId);
                            long fileId = AddPriceListFile(customerId, customerZoneNotifications, context.EffectiveDate, pricelistType, salePricelistTemplateId, customerPriceList.CurrencyId);
                            customerPriceList.PriceList.FileId = fileId;
                            pricelistIds.Add(customerPriceList.PriceList.PriceListId);
                        }
                    }
                }
                SaveChangesToDB(context.CustomerPriceListChanges, context.ProcessInstanceId);
                BulkInsertSalePriceListSnapshot(saleCodes.Select(item => item.SaleCodeId).ToList(), pricelistIds);
            }
            BulkInsertPriceList(context.SalePriceLists);
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

            var objects = new Dictionary<string, dynamic>
            {
                {"Customer", customer},
                {"User", initiator},
                {"Sale Pricelist", clonedPriceList}
            };
            VRMailManager vrMailManager = new VRMailManager();
            return vrMailManager.EvaluateMailTemplate(salePlmailTemplateId, objects);
        }
        public IEnumerable<SalePricelistVRFile> GenerateSalePriceListFiles(SalePriceListInput pricelisInput)
        {
            var salePriceListManager = new SalePriceListManager();

            SalePriceList customerPriceList = salePriceListManager.GetPriceList(pricelisInput.PriceListId);
            IEnumerable<SalePricelistVRFile> files = PreparePriceListVrFiles(customerPriceList, (SalePriceListType)pricelisInput.PriceListTypeId, pricelisInput.PricelistTemplateId);

            return files;
        }
        public IDataRetrievalResult<SalePriceListDetail> GetFilteredPricelists(Vanrise.Entities.DataRetrievalInput<SalePriceListQuery> input)
        {
            Dictionary<int, SalePriceList> cachedSalePriceLists = GetCustomerCachedSalePriceLists();
            Func<SalePriceList, bool> filterExpression = salePriceList =>
            {
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
            return GetCustomerCachedSalePriceLists().MapRecords(x => x.Value.PriceListId, x => x.Value.ProcessInstanceId == processInstanceId);
        }

        public bool SendCustomerPriceLists(IEnumerable<int> customerPriceListIds, bool compressFile)
        {
            if (customerPriceListIds == null || !customerPriceListIds.Any())
                throw new MissingArgumentValidationException("customerPriceListIds");

            IEnumerable<SalePriceList> customerPriceLists = GetCustomerCachedSalePriceLists().MapRecords(x => x.Value, x => customerPriceListIds.Contains(x.Value.PriceListId));

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
                    bool isCompressed = compressFile || _carrierAccountManager.GetCustomerCompressPriceListFileStatus(customer.CarrierAccountId);
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

            var futureRateLocator = new SaleEntityZoneRateLocator(new SaleRateReadLastRateNoCache(new List<RoutingCustomerInfoDetails> { routingCustomerInfoDetails }
                        , context.EffectiveDate.Date.AddSeconds(-1)));

            return new SalePriceListOutputContext
            {
                FutureLocator = futureRateLocator,
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
        Dictionary<int, List<ExistingSaleZone>> existingDataByCountryId, SaleEntityZoneRateLocator futureLocator, out SalePriceListType overiddenPriceListType)
        {
            overiddenPriceListType = pricelistType;
            var salePlZoneNotifications = GetChangeOrCountryNotification(customerId, sellingProductId, pricelistType, existingDataByCountryId, futureLocator, countryChanges);

            if (pricelistType != SalePriceListType.Full) //Send zone changes with missing zones from their countries
                return salePlZoneNotifications;

            int changesCurrency = salePlZoneNotifications.First().Rate.CurrencyId.Value;
            List<SalePLZoneNotification> saleNotifications;

            Dictionary<int, List<SalePLZoneNotification>> zoneNotifictionByCurrencyId = GetFullSalePlZoneNotification(customerId, sellingProductId, existingDataByCountryId, countryChanges,
                    futureLocator);

            if (zoneNotifictionByCurrencyId.Count > 1 || !zoneNotifictionByCurrencyId.TryGetValue(changesCurrency, out saleNotifications))
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
      Dictionary<int, List<ExistingSaleZone>> existingDataByCountryId, SaleEntityZoneRateLocator futureLocator, out SalePriceListType overiddenPriceListType)
        {
            var saleZoneNotificarionByCurrencyId = new Dictionary<int, List<SalePLZoneNotification>>();
            var salePlZoneNotifications = GetChangeOrCountryNotification(customerId, sellingProductId, pricelistType, existingDataByCountryId, futureLocator, countryChanges);
            overiddenPriceListType = pricelistType;

            int changesCurrency = salePlZoneNotifications.First().Rate.CurrencyId.Value;
            saleZoneNotificarionByCurrencyId.Add(changesCurrency, salePlZoneNotifications);

            if (pricelistType != SalePriceListType.Full)
                return saleZoneNotificarionByCurrencyId;

            List<SalePLZoneNotification> saleNotifications;
            Dictionary<int, List<SalePLZoneNotification>> zoneNotifictionByCurrencyId = GetFullSalePlZoneNotification(customerId, sellingProductId, existingDataByCountryId, countryChanges, futureLocator);

            if (!zoneNotifictionByCurrencyId.TryGetValue(changesCurrency, out saleNotifications))
            {
                overiddenPriceListType = SalePriceListType.Country;
                zoneNotifictionByCurrencyId.Add(changesCurrency, salePlZoneNotifications);
            }
            else
                salePlZoneNotifications.AddRange(salePlZoneNotifications);

            var rpChanges = countryChanges.SelectMany(it => it.ZoneChanges.Where(r => r.RPChange != null).Select(rp => rp.RPChange)).ToList();
            AddRPChangesToSalePLNotification(zoneNotifictionByCurrencyId.Values.SelectMany(z => z), rpChanges, customerId, sellingProductId);

            return zoneNotifictionByCurrencyId;
        }

        private Dictionary<int, List<SalePLZoneNotification>> GetFullSalePlZoneNotification(int customerId, int sellingProductId, Dictionary<int, List<ExistingSaleZone>> existingDataByCountryId, List<CountryChange> countryChanges
            , SaleEntityZoneRateLocator futureLocator)
        {
            var salePlZoneNotificationsByCurrencyId = new Dictionary<int, List<SalePLZoneNotification>>();
            //Add all missing sold countries to notification from exiting data
            var customerCountryManager = new CustomerCountryManager();
            var soldCountries = customerCountryManager.GetNotClosedCustomerCountries(customerId);

            if (soldCountries == null)
                return salePlZoneNotificationsByCurrencyId;

            var changedCountryIds = countryChanges.Select(it => it.CountryId);

            var zoneNotifictionByCurrencyId = new Dictionary<int, List<SalePLZoneNotification>>();
            foreach (var soldCountry in soldCountries)
            {
                if (changedCountryIds.Contains(soldCountry.CountryId))
                    continue;
                List<ExistingSaleZone> existingZones = existingDataByCountryId.GetRecord(soldCountry.CountryId);
                List<SalePLZoneNotification> notifications = null;
                if (existingZones != null)
                    notifications = GetZoneNotificationsFromExistingData(customerId, sellingProductId, existingZones, futureLocator);

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
        private List<SalePLZoneNotification> GetChangeOrCountryNotification(int customerId, int sellingProductId, SalePriceListType pricelistType, Dictionary<int, List<ExistingSaleZone>> existingDataByCountryId
            , SaleEntityZoneRateLocator futureLocator, List<CountryChange> countryChanges)
        {
            var salePlZoneNotifications = new List<SalePLZoneNotification>();
            //Create zone notifications from zone changes
            salePlZoneNotifications = CreateNotificationsForAllZoneChanges(customerId, sellingProductId, countryChanges, existingDataByCountryId, futureLocator);

            if (pricelistType == SalePriceListType.RateChange) //Only send changes zones
                return salePlZoneNotifications;

            foreach (var countryChange in countryChanges)//Add missing zones to notification from existing data for all changed countries
            {
                List<ExistingSaleZone> existingZones = existingDataByCountryId.GetRecord(countryChange.CountryId);
                salePlZoneNotifications.AddRange(GetZoneNotificationsFromExistingData(customerId, sellingProductId, existingZones, countryChange.ZoneChanges.Select(z => z.ZoneName), futureLocator));
            }
            return salePlZoneNotifications;
        }
        private List<SalePLZoneNotification> CreateNotificationsForAllZoneChanges(int customerId, int sellingProductId, List<CountryChange> countryChanges,
            Dictionary<int, List<ExistingSaleZone>> zoneWrappersByCountry, SaleEntityZoneRateLocator futureLocator)
        {
            List<SalePLZoneNotification> salePlZoneNotifications = new List<SalePLZoneNotification>();

            foreach (var country in countryChanges)
            {
                List<ExistingSaleZone> existingSaleZones = zoneWrappersByCountry.GetRecord(country.CountryId);

                if (existingSaleZones == null) continue;

                foreach (var zoneChange in country.ZoneChanges)
                {
                    ExistingSaleZone existingSaleZone = existingSaleZones.FirstOrDefault(z => z.ZoneName.Equals(zoneChange.ZoneName));

                    SalePLZoneNotification salePlZone = new SalePLZoneNotification
                    {
                        ZoneName = zoneChange.ZoneName,
                        //ZoneId = zoneChange.ZoneId
                    };

                    salePlZone.ZoneId = existingSaleZone != null
                        ? existingSaleZone.ZoneId
                        : zoneChange.ZoneId;

                    if (zoneChange.CodeChanges != null)
                    {
                        //Add all code changes as notifications
                        salePlZone.Codes.AddRange(zoneChange.CodeChanges.MapRecords(SalePLCodeChangeToSalePLNotificationMapper));
                    }

                    if (existingSaleZone != null)
                    {
                        //Add missing codes from existing data
                        foreach (ExistingSaleCode existingCode in existingSaleZone.Codes)
                        {
                            if (salePlZone.Codes.Any(x => x.Code == existingCode.Code))
                                continue;

                            salePlZone.Codes.Add(ExistingCodeToSalePLCodeNotificationMapper(existingCode));
                        }
                    }

                    var rateChange = zoneChange.RateChange;
                    if (rateChange != null)
                    {
                        //Add the rate change as notification
                        salePlZone.Rate = new SalePLRateNotification
                        {
                            Rate = rateChange.Rate,
                            BED = rateChange.BED,
                            RateChangeType = rateChange.ChangeType,
                            EED = rateChange.EED,
                            CurrencyId = rateChange.CurrencyId
                        };
                    }
                    else
                    {
                        if (existingSaleZone != null)
                            salePlZone.Rate = GetRateNotificationFromExistingData(customerId, sellingProductId, existingSaleZone.ZoneId, existingSaleZone.ZoneName, futureLocator);
                    }

                    salePlZoneNotifications.Add(salePlZone);
                }
            }
            List<SalePricelistRPChange> rpChanges = StructureRPChange(countryChanges);
            AddRPChangesToSalePLNotification(salePlZoneNotifications, rpChanges, customerId, sellingProductId);
            return salePlZoneNotifications;
        }
        private List<SalePLZoneNotification> GetZoneNotificationsFromExistingData(int customerId, int sellingProductId, IEnumerable<ExistingSaleZone> existingZones, SaleEntityZoneRateLocator futureLocator)
        {
            List<SalePLZoneNotification> salePlZoneNotifications = new List<SalePLZoneNotification>();

            foreach (ExistingSaleZone existingZone in existingZones)
            {
                SalePLZoneNotification zoneNotification = new SalePLZoneNotification
                {
                    ZoneId = existingZone.ZoneId,
                    ZoneName = existingZone.ZoneName
                };
                zoneNotification.Codes.AddRange(existingZone.Codes.MapRecords(ExistingCodeToSalePLCodeNotificationMapper));
                zoneNotification.Rate = this.GetRateNotificationFromExistingData(customerId, sellingProductId, existingZone.ZoneId, existingZone.ZoneName, futureLocator);

                salePlZoneNotifications.Add(zoneNotification);
            }
            return salePlZoneNotifications;
        }

        private SalePLRateNotification GetRateNotificationFromExistingData(int customerId, int sellingProductId, long zoneId, string zoneName, SaleEntityZoneRateLocator futureLocator)
        {
            SaleEntityZoneRate zoneRate = futureLocator.GetCustomerZoneRate(customerId, sellingProductId, zoneId);
            SaleRateManager saleRateManager = new SaleRateManager();
            if (zoneRate == null)
                throw new DataIntegrityValidationException(string.Format("Zone '{0}' neither has an explicit rate nor has selling product rate. Country is sold to customer with id {1}", zoneName, customerId));

            return new SalePLRateNotification
            {
                Rate = zoneRate.Rate.Rate,
                BED = zoneRate.Rate.BED,
                EED = zoneRate.Rate.EED,
                RateChangeType = RateChangeType.NotChanged,
                CurrencyId = saleRateManager.GetCurrencyId(zoneRate.Rate)
            };
        }

        private List<SalePLZoneNotification> GetZoneNotificationsFromExistingData(int customerId, int sellingProductId, IEnumerable<ExistingSaleZone> existingZones, IEnumerable<string> changedZoneNames, SaleEntityZoneRateLocator futureLocator)
        {
            List<SalePLZoneNotification> salePlZoneNotifications = new List<SalePLZoneNotification>();

            foreach (ExistingSaleZone existingZone in existingZones)
            {
                if (changedZoneNames.Contains(existingZone.ZoneName))
                    continue;
                SalePLZoneNotification zoneNotification = new SalePLZoneNotification()
                {
                    ZoneId = existingZone.ZoneId,
                    ZoneName = existingZone.ZoneName
                };
                zoneNotification.Codes.AddRange(existingZone.Codes.MapRecords(ExistingCodeToSalePLCodeNotificationMapper));
                zoneNotification.Rate = this.GetRateNotificationFromExistingData(customerId, sellingProductId, existingZone.ZoneId, existingZone.ZoneName, futureLocator);

                salePlZoneNotifications.Add(zoneNotification);
            }

            AddRPChangesToSalePLNotification(salePlZoneNotifications, new List<SalePricelistRPChange>(), customerId, sellingProductId);
            return salePlZoneNotifications;
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
        public string GetPriceListName(int carrierAccountId, DateTime priceListDate, SalePriceListType salePriceListType, string extension)
        {
            var customerName = _carrierAccountManager.GetCarrierAccountName(carrierAccountId);
            var carrierProfileManager = new CarrierProfileManager();
            var carrierProfileId = _carrierAccountManager.GetCarrierProfileId(carrierAccountId).Value;
            var profileName = carrierProfileManager.GetCarrierProfileName(carrierProfileId);

            var priceListName = new StringBuilder(_carrierAccountManager.GetCustomerPricelistFileNamePattern(carrierAccountId));
            priceListName.Replace("#CustomerName#", customerName);
            priceListName.Replace("#ProfileName#", profileName);
            priceListName.Replace("#PricelistDate#", priceListDate.ToString("yyyy-MM-dd_HH-mm-ss"));
            priceListName.Replace("#PriclistType#", salePriceListType.ToString());

            priceListName = priceListName.Append(extension);
            return priceListName.ToString();
        }
        private VRFile GetPriceListFile(int carrierAccountId, List<SalePLZoneNotification> customerZonesNotifications, DateTime effectiveDate, SalePriceListType salePriceListType, int salePriceListTemplateId, int pricelistCurrencyId)
        {
            var salePriceListTemplateManager = new SalePriceListTemplateManager();

            SalePriceListTemplate template = salePriceListTemplateManager.GetSalePriceListTemplate(salePriceListTemplateId);
            if (template == null)
                throw new DataIntegrityValidationException(string.Format("Customer with Id {0} does not have a Sale Price List Template", carrierAccountId));

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
            string fileName = GetPriceListName(carrierAccountId, effectiveDate, salePriceListType, extension);

            return new VRFile
            {
                Content = salePlTemplateBytes,
                Name = fileName,
                ModuleName = "WhS_BE_SalePriceList",
                Extension = extension,
                CreatedTime = effectiveDate
            };
        }
        private long AddPriceListFile(int carrierAccountId, List<SalePLZoneNotification> customerZonesNotifications, DateTime effectiveDate, SalePriceListType salePriceListType, int salePriceListTemplateId, int pricelistCurrencyId)
        {
            var fileManager = new VRFileManager();
            var file = GetPriceListFile(carrierAccountId, customerZonesNotifications, effectiveDate, salePriceListType,
                salePriceListTemplateId, pricelistCurrencyId);
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
                        foreach (var code in countryCodeChange)
                        {
                            code.BatchId = processInstanceId;
                            code.PricelistId = priceList.PriceList.PriceListId;
                        }

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
        private IEnumerable<PriceListChange> GetPricelistChanges(StructuredCustomerPricelistChange customerPriceListChange, SaleEntityZoneRateLocator lastRateNoCacheLocator, Dictionary<int, List<NewPriceList>> salePriceListsByCurrencyId
            , DateTime effectiveDate, long processInstanceId, int userId)
        {
            CarrierAccountManager carrierAccountManager = new CarrierAccountManager();
            int sellingProductId = carrierAccountManager.GetSellingProductId(customerPriceListChange.CustomerId);
            SaleRateManager saleRateManager = new SaleRateManager();
            Dictionary<int, PriceListChange> customerPricelisChangetByCurrencyId = new Dictionary<int, PriceListChange>();
            foreach (var country in customerPriceListChange.CountryGroups)
            {
                int? currencyId = null;
                CountryChange countryChange = new CountryChange
                {
                    CountryId = country.CountryId
                };
                var zoneChanges = new Dictionary<string, SalePricelistZoneChange>();

                foreach (var rate in country.RateChanges)
                {
                    currencyId = currencyId ?? rate.CurrencyId;
                    SalePricelistZoneChange zone;
                    if (!zoneChanges.TryGetValue(rate.ZoneName, out zone))
                    {
                        zone = new SalePricelistZoneChange
                        {
                            ZoneId = rate.ZoneId.Value,
                            ZoneName = rate.ZoneName,
                            RateChange = rate
                        };
                        zoneChanges.Add(rate.ZoneName, zone);
                    }
                }
                foreach (var code in country.CodeChanges)
                {
                    SalePricelistZoneChange zone;
                    if (!zoneChanges.TryGetValue(code.ZoneName, out zone))
                    {
                        zone = new SalePricelistZoneChange
                        {
                            ZoneId = code.ZoneId.Value,
                            ZoneName = code.ZoneName
                        };
                        zoneChanges.Add(zone.ZoneName, zone);
                    }
                    zone.CodeChanges.Add(code);
                    if (currencyId == null)
                    {
                        var rate = lastRateNoCacheLocator.GetCustomerZoneRate(customerPriceListChange.CustomerId, sellingProductId, zone.ZoneId);
                        if (rate != null)
                            currencyId = saleRateManager.GetCurrencyId(rate.Rate);
                    }
                }
                foreach (var rp in country.RPChanges)
                {
                    SalePricelistZoneChange zone;
                    if (!zoneChanges.TryGetValue(rp.ZoneName, out zone))
                    {
                        zone = new SalePricelistZoneChange
                        {
                            ZoneId = rp.ZoneId.Value,
                            ZoneName = rp.ZoneName,
                            RPChange = rp
                        };
                        zoneChanges.Add(zone.ZoneName, zone);
                    }
                }
                List<NewPriceList> newPriceLists;
                NewPriceList customerPricelist = null;
                if (!salePriceListsByCurrencyId.TryGetValue(currencyId.Value, out newPriceLists))
                {
                    newPriceLists = new List<NewPriceList>();
                    salePriceListsByCurrencyId.Add(currencyId.Value, newPriceLists);
                }
                foreach (var newPriceList in newPriceLists)
                {
                    if (newPriceList.OwnerId == customerPriceListChange.CustomerId && newPriceList.OwnerType == SalePriceListOwnerType.Customer)
                    {
                        customerPricelist = newPriceList;
                        break;
                    }
                }
                if (customerPricelist == null)
                {
                    customerPricelist = new NewPriceList
                    {
                        OwnerId = customerPriceListChange.CustomerId,
                        CurrencyId = currencyId.Value,
                        OwnerType = SalePriceListOwnerType.Customer,
                        PriceListType = SalePriceListType.Country,
                        EffectiveOn = effectiveDate,
                        ProcessInstanceId = processInstanceId,
                        UserId = userId
                    };
                    newPriceLists.Add(customerPricelist);
                }
                PriceListChange priceListChange;
                if (!customerPricelisChangetByCurrencyId.TryGetValue(currencyId.Value, out priceListChange))
                {
                    priceListChange = new PriceListChange
                    {
                        PriceList = customerPricelist,
                        CurrencyId = currencyId.Value
                    };
                    customerPricelisChangetByCurrencyId.Add(currencyId.Value, priceListChange);
                }
                countryChange.ZoneChanges.AddRange(zoneChanges.Values);
                priceListChange.CountryChanges.Add(countryChange);
            }
            return customerPricelisChangetByCurrencyId.Values;
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
        private IEnumerable<SalePricelistVRFile> PreparePriceListVrFiles(SalePriceList salePriceList, SalePriceListType salePriceListType, int salePricelistTemplateId)
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
                EffectiveDate = salePriceList.CreatedTime,
                SellingNumberPlanId = sellingNumberPlanId,
                ProcessInstanceId = salePriceList.ProcessInstanceId,
                UserId = salePriceList.UserId,
                SaleCodes = saleCodeSnapshot,
                CustomerId = salePriceList.OwnerId,
                SellingProductId = sellingProductId.Value
            };

            SalePriceListOutputContext salePriceListOutput = PrepareSalePriceListContext(salePriceListContext);

            SalePriceListType overriddenListType;
            Dictionary<int, List<SalePLZoneNotification>> customerZoneNotificationsByCurrencyId = CreateMultipleNotifications(salePriceList.OwnerId, sellingProductId.Value, salePriceListType, salePriceListOutput.CountryChanges,
                salePriceListOutput.ZoneWrappersByCountry, salePriceListOutput.FutureLocator, out overriddenListType);

            var currencyManager = new CurrencyManager();

            foreach (var zoneNotification in customerZoneNotificationsByCurrencyId)
            {
                VRFile vrFile = GetPriceListFile(customer.CarrierAccountId, zoneNotification.Value, salePriceListContext.EffectiveDate,
                    salePriceListType, salePricelistTemplateId, salePriceList.CurrencyId);

                vrFiles.Add(new SalePricelistVRFile
                {
                    CurrencySymbol =currencyManager.GetCurrencySymbol(zoneNotification.Key),
                    FileId = vrFileManager.AddFile(vrFile),
                    FileName = vrFile.Name,
                    FileExtension = vrFile.Extension
                });
            }
            return vrFiles;
        }
        private void AddRPChangesToSalePLNotification(IEnumerable<SalePLZoneNotification> customerZoneNotifications, List<SalePricelistRPChange> routingProductChanges, int customerId, int sellingProductId)
        {

            SaleEntityZoneRoutingProductLocator routingProductLocator = null;
            Dictionary<long, DateTime> rateBedByZoneId = StructureZoneIdsWithActionBED(customerZoneNotifications);
            if (rateBedByZoneId.Any())
                routingProductLocator = new SaleEntityZoneRoutingProductLocator(new SaleEntityRoutingProductReadByRateBED(new List<int> { customerId }, rateBedByZoneId));

            var routingProductChangesByZoneName = StructureCustomerSaleRpChangesByZoneName(routingProductChanges);
            RoutingProductManager routingProductManager = new RoutingProductManager();
            foreach (var zoneNotification in customerZoneNotifications)
            {
                IEnumerable<int> servicesIds = new List<int>();

                int? routingProductId = null;
                SalePricelistRPChange routinProductChange = routingProductChangesByZoneName.GetRecord(zoneNotification.ZoneName);
                if (routinProductChange != null)
                    routingProductId = routinProductChange.RoutingProductId;
                else if (routingProductLocator != null && zoneNotification.ZoneId.HasValue)
                {
                    SaleEntityZoneRoutingProduct saleEntityZoneRoutingProduct = routingProductLocator.GetCustomerZoneRoutingProduct(customerId, sellingProductId, zoneNotification.ZoneId.Value);
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
            public SaleEntityZoneRateLocator FutureLocator { get; set; }
            public Dictionary<int, List<ExistingSaleZone>> ZoneWrappersByCountry { get; set; }
            public List<CountryChange> CountryChanges { get; set; }
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

                sheet.Header.Cells.Add(new ExportExcelHeaderCell() { Title = "Customer Name" });
                sheet.Header.Cells.Add(new ExportExcelHeaderCell() { Title = "Created Time", CellType = ExcelCellType.DateTime, DateTimeType = DateTimeType.DateTime });
                sheet.Header.Cells.Add(new ExportExcelHeaderCell() { Title = "Currency" });
                sheet.Header.Cells.Add(new ExportExcelHeaderCell() { Title = "Pricelist Type" });

                sheet.Rows = new List<ExportExcelRow>();
                if (context.BigResult != null && context.BigResult.Data != null)
                {
                    foreach (var record in context.BigResult.Data)
                    {
                        if (record.Entity != null)
                        {
                            var row = new ExportExcelRow() { Cells = new List<ExportExcelCell>() };
                            row.Cells.Add(new ExportExcelCell() { Value = record.OwnerName });
                            row.Cells.Add(new ExportExcelCell() { Value = record.Entity.CreatedTime });
                            row.Cells.Add(new ExportExcelCell() { Value = record.CurrencyName });
                            row.Cells.Add(new ExportExcelCell() { Value = record.PriceListTypeName });
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
                    : null
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

        private SalePLCodeNotification ExistingCodeToSalePLCodeNotificationMapper(ExistingSaleCode saleCode)
        {
            return new SalePLCodeNotification()
            {
                Code = saleCode.Code,
                BED = saleCode.BED,
                EED = saleCode.EED,
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
    }
}
