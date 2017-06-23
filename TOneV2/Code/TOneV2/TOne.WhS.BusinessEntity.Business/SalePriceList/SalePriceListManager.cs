using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
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
        public void SavePricelistFiles(ISalePricelistFileContext context)
        {
            Dictionary<int, SalePriceList> customerPriceListsByCustomerId = new Dictionary<int, SalePriceList>();
            var sellingProductPriceList = new List<SalePriceList>();
            if (context.SalePriceLists != null && context.SalePriceLists.Any())
                customerPriceListsByCustomerId = GetSalePriceListByCustomerId(context.SalePriceLists, out sellingProductPriceList);

            if (context.CustomerPriceListChanges != null && context.CustomerPriceListChanges.Any())
            {
                IEnumerable<SaleCode> saleCodes = new SaleCodeManager().GetSaleCodesEffectiveAfter(context.SellingNumberPlanId, context.EffectiveDate, context.ProcessInstanceId);
                if (saleCodes == null || !saleCodes.Any())
                    return;

                SalePriceListInputContext inputcontext = new SalePriceListInputContext
                {
                    EffectiveDate = context.EffectiveDate,
                    SellingNumberPlanId = context.SellingNumberPlanId,
                    CustomerChanges = context.CustomerPriceListChanges,
                    ProcessInstanceId = context.ProcessInstanceId,
                    SaleCodes = saleCodes
                };
                SalePriceListOutputContext outputContext = PrepareSalePriceListContext(inputcontext);

                var customerSellingProductManager = new CustomerSellingProductManager();

                foreach (var customerChange in outputContext.CustomerChanges)
                {
                    int customerId = customerChange.CustomerId;
                    CarrierAccount customer = _carrierAccountManager.GetCarrierAccount(customerId);

                    int? sellingProductId = customerSellingProductManager.GetEffectiveSellingProductId(customerId, DateTime.Now, false);
                    if (!sellingProductId.HasValue)
                        throw new DataIntegrityValidationException(string.Format("Customer with Id {0} is not assigned to a selling product", customerId));

                    var customerPriceListType = _carrierAccountManager.GetPriceListType(customerId);
                    SalePriceListType pricelistType = GetSalePriceListType(customerPriceListType, context.ChangeType);

                    ZoneChangesByCountryId allChangesByCountryId = customerChange.ZoneChangesByCountryId;// MergeCurrentWithNotSentChanges(customerId, customerChange.ZoneChangesByCountryId,outputContext.NotSentChangesByCustomerId);

                    List<SalePLZoneNotification> customerZoneNotifications = CreateSalePricelistNotifications(customerId, sellingProductId.Value, pricelistType, allChangesByCountryId,
                        outputContext.ZoneWrappersByCountry, outputContext.FutureLocator, inputcontext.EffectiveDate, context.ProcessInstanceId);

                    if (customerZoneNotifications.Count > 0)
                    {
                        AddRPChangesToSalePLNotification(customerZoneNotifications, customerChange.RoutingProductChanges, customerId, sellingProductId.Value);
                        VRFile file = GetPriceListFile(customerId, customerZoneNotifications);
                        SalePriceList priceList = AddOrUpdateSalePriceList(customer, pricelistType, context.ProcessInstanceId, file, context.CurrencyId, customerPriceListsByCustomerId);

                        var customerPriceListChange = context.CustomerPriceListChanges.First(r => r.CustomerId == customerId);
                        customerPriceListChange.PriceListId = priceList.PriceListId;
                    }
                }
                BulkInsertCustomerChanges(context.CustomerPriceListChanges.ToList(), context.ProcessInstanceId);
                BulkInsertSalePriceListSnapshot(saleCodes.Select(item => item.SaleCodeId).ToList(), context.CustomerPriceListChanges.Select(item => item.PriceListId));
            }
            var priceListsToSave = customerPriceListsByCustomerId.Values.Concat(sellingProductPriceList);
            BulkInsertPriceList(priceListsToSave.ToList());
        }
        public bool SendPriceList(long salePriceListId)
        {
            var salePriceListManager = new SalePriceListManager();
            SalePriceList customerPriceList = salePriceListManager.GetPriceList((int)salePriceListId);

            if (!customerPriceList.PriceListType.HasValue)
                throw new VRBusinessException(string.Format("Customer Pricelist with id {0} has its type as null", customerPriceList.PriceListId));

            VRFile file = PreparePriceListVrFile(customerPriceList, customerPriceList.PriceListType.Value);
            var notificationManager = new NotificationManager();
            int userId = Vanrise.Security.Entities.ContextFactory.GetContext().GetLoggedInUserId();

            return file != null && notificationManager.SendSalePriceList(userId, customerPriceList, file);
        }

        public VRMailEvaluatedTemplate EvaluateEmail(long pricelistId)
        {
            SalePriceListManager priceListManager = new SalePriceListManager();
            var customerPricelist = priceListManager.GetPriceList((int)pricelistId);

            if (customerPricelist == null)
                return null;

            CarrierAccountManager carrierAccountManager = new CarrierAccountManager();
            var customer = carrierAccountManager.GetCarrierAccount(customerPricelist.OwnerId);
            var salePlmailTemplateId = carrierAccountManager.GetSalePLMailTemplateId(customerPricelist.OwnerId);

            UserManager userManager = new UserManager();
            User initiator = userManager.GetUserbyId(SecurityContext.Current.GetLoggedInUserId());

            var objects = new Dictionary<string, dynamic>
            {
                {"Customer", customer},
                {"User", initiator},
                {"Sale Pricelist", customerPricelist}
            };
            VRMailManager vrMailManager = new VRMailManager();
            return vrMailManager.EvaluateMailTemplate(salePlmailTemplateId, objects);
        }
        public VRFile GenerateSalePriceListFile(long salePriceListId, SalePriceListType salePriceListType)
        {
            var salePriceListManager = new SalePriceListManager();
            SalePriceList customerPriceList = salePriceListManager.GetPriceList((int)salePriceListId);

            return PreparePriceListVrFile(customerPriceList, salePriceListType);

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
        public bool SendCustomerPriceLists(IEnumerable<int> customerPriceListIds)
        {
            if (customerPriceListIds == null || customerPriceListIds.Count() == 0)
                throw new Vanrise.Entities.MissingArgumentValidationException("customerPriceListIds");

            IEnumerable<SalePriceList> customerPriceLists = GetCustomerCachedSalePriceLists().MapRecords(x => x.Value, x => customerPriceListIds.Contains(x.Value.PriceListId));

            if (customerPriceLists == null || customerPriceLists.Count() < customerPriceListIds.Count())
                throw new Vanrise.Entities.DataIntegrityValidationException(string.Format("Some of the sale pricelists, with the following ids '{0}', were not found", string.Join(",", customerPriceListIds)));

            int loggedInUserId = SecurityContext.Current.GetLoggedInUserId();

            var notificationManager = new TOne.WhS.BusinessEntity.Business.NotificationManager();
            var fileManager = new Vanrise.Common.Business.VRFileManager();

            var haveAllEmailsBeenSent = true;

            foreach (SalePriceList customerPriceList in customerPriceLists)
            {
                if (customerPriceList.IsSent)
                    continue;

                VRFile customerPriceListFile = fileManager.GetFile(customerPriceList.FileId);
                bool hasEmailBeenSent = notificationManager.SendSalePriceList(loggedInUserId, customerPriceList, customerPriceListFile);

                if (!hasEmailBeenSent)
                    haveAllEmailsBeenSent = false;
            }

            return haveAllEmailsBeenSent;
        }
        #endregion

        #region Generate Pricelist Methods

        #region Preparation Methods

        private SalePriceListOutputContext PrepareSalePriceListContext(SalePriceListInputContext context)
        {
            var customerChanges = StructureCustomerPriceListChanges(context.CustomerChanges);

            IEnumerable<ExistingSaleCodeEntity> existingSaleCodeEntities = context.SaleCodes.MapRecords(ExistingSaleCodeEntityMapper);
            Dictionary<string, Dictionary<string, List<ExistingSaleCodeEntity>>> existingSaleCodesByZoneName = StructureExistingSaleCodesByZoneName(existingSaleCodeEntities);
            Dictionary<int, List<ExistingSaleZone>> zoneWrappersByCountry = StructureZoneWrappersByCountry(existingSaleCodesByZoneName);

            var customerIdsWithChanges = customerChanges.Select(c => c.CustomerId);

            IEnumerable<RoutingCustomerInfoDetails> dataByCustomerList = GetDataByCustomer(customerIdsWithChanges, context.EffectiveDate);

            var futureRateLocator = new SaleEntityZoneRateLocator(new SaleRateReadLastRateNoCache(dataByCustomerList, context.EffectiveDate));
            //var salePriceListChanges = new SalePriceListChangeManager();
            var notSentChanges = new Dictionary<int, List<CustomerPriceListChange>>();// salePriceListChanges.GetNotSentChangesByCustomer(customerIdsWithChanges);
            return new SalePriceListOutputContext
            {
                FutureLocator = futureRateLocator,
                NotSentChangesByCustomerId = notSentChanges,
                CustomerChanges = customerChanges,
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

        #region Merge With Not Sent Changes

        private ZoneChangesByCountryId MergeCurrentWithNotSentChanges(int customerId, ZoneChangesByCountryId currentChangesByCountryId, Dictionary<int, List<CustomerPriceListChange>> notSentChangesByCustomerId)
        {
            List<CustomerPriceListChange> notSentPriceListChange;
            notSentChangesByCustomerId.TryGetValue(customerId, out notSentPriceListChange);

            if (notSentPriceListChange == null) return currentChangesByCountryId;

            var manager = new SalePriceListManager();
            var salePriceListChangeManager = new SalePriceListChangeManager();
            IEnumerable<SalePriceList> customerPriceLists = manager.GetCustomerSalePriceListsById(customerId).Where(p => p.IsSent);
            int lastSentPriceListId = 0;
            if (customerPriceLists.Any())
                lastSentPriceListId = customerPriceLists.Max(p => p.PriceListId);

            List<CustomerPriceListChange> notSentChangesForThisCustomer = null;
            notSentChangesByCustomerId.TryGetValue(customerId, out notSentChangesForThisCustomer);

            var notSentCustomerChanges = StructureCustomerPriceListChanges(notSentChangesForThisCustomer.Where(p => p.PriceListId >= lastSentPriceListId)
                        .OrderByDescending((p => p.PriceListId)));

            if (notSentCustomerChanges == null) return currentChangesByCountryId;

            var lastSentPriceListChanges = salePriceListChangeManager.GetCustomerChangesByPriceListId(lastSentPriceListId);
            foreach (var notSentpricelist in notSentCustomerChanges)
            {
                foreach (var notSentCountryByZone in notSentpricelist.ZoneChangesByCountryId)
                {
                    ZoneChangesByZoneName currentZone;
                    if (currentChangesByCountryId.TryGetValue(notSentCountryByZone.Key, out currentZone))
                    {
                        foreach (var notSentZone in notSentCountryByZone.Value)
                        {
                            ZoneChange zoneChanges;
                            if (currentZone.TryGetValue(notSentZone.Value.ZoneName, out zoneChanges))
                            {
                                zoneChanges.RateChanges = MatchRate(lastSentPriceListChanges.RateChanges,
                                    zoneChanges.RateChanges);
                                zoneChanges.CodeChanges = MatchCode(lastSentPriceListChanges.CodeChanges,
                                    zoneChanges.CodeChanges);
                            }
                            else
                            {
                                currentZone.Add(notSentZone.Value.ZoneName, notSentZone.Value);
                            }
                        }
                    }
                    else
                    {
                        currentChangesByCountryId.Add(notSentCountryByZone.Key, notSentCountryByZone.Value);
                    }
                }
            }
            return currentChangesByCountryId;
        }

        private List<SalePricelistRateChange> MatchRate(List<SalePricelistRateChange> notSentChanges, List<SalePricelistRateChange> currentChanges)
        {
            List<SalePricelistRateChange> saleproPricelistRateChanges = new List<SalePricelistRateChange>();
            if (notSentChanges.Count == 0) return saleproPricelistRateChanges;
            var orderedNotSentChanges = notSentChanges.OrderByDescending(r => r.PricelistId);

            if (currentChanges != null)
            {
                foreach (var currentChange in currentChanges)
                {
                    var matchedRate = orderedNotSentChanges.First();
                    if (currentChange.Rate > matchedRate.Rate) currentChange.ChangeType = RateChangeType.Increase;
                    if (currentChange.Rate < matchedRate.Rate) currentChange.ChangeType = RateChangeType.Decrease;
                    if (currentChange.Rate == matchedRate.Rate) currentChange.ChangeType = RateChangeType.NotChanged;
                    return new List<SalePricelistRateChange> { currentChange };
                }
            }

            return saleproPricelistRateChanges;
        }
        private List<SalePricelistCodeChange> MatchCode(List<SalePricelistCodeChange> lastChanges, List<SalePricelistCodeChange> currentChanges)
        {
            List<SalePricelistCodeChange> codeChanges = new List<SalePricelistCodeChange>();
            var grouppedCode = lastChanges.GroupBy(t => t.Code)
                    .Select(group => new { Code = group.Key, Items = group.ToList() })
                    .ToDictionary(c => c.Code, c => c.Items);
            foreach (var codeChange in currentChanges)
            {
                List<SalePricelistCodeChange> salePricelistCodeChanges;
                if (grouppedCode.TryGetValue(codeChange.Code, out salePricelistCodeChanges))
                {

                }
                else
                {
                    codeChanges.Add(codeChange);
                }
            }
            return codeChanges;
        }

        #endregion

        #region Merge with Existing Data

        private List<SalePLZoneNotification> CreateSalePricelistNotifications(int customerId, int sellingProductId, SalePriceListType pricelistType, ZoneChangesByCountryId allChangesByCountryId,
            Dictionary<int, List<ExistingSaleZone>> existingDataByCountryId, SaleEntityZoneRateLocator futureLocator, DateTime effectiveDate, long processInstanceId)
        {
            //Create zone notifications from zone changes
            var salePlZoneNotifications = this.CreateNotificationsForAllZoneChanges(customerId, sellingProductId, allChangesByCountryId, existingDataByCountryId, futureLocator);

            if (pricelistType == SalePriceListType.RateChange) //Only send changes zones
                return salePlZoneNotifications;

            IEnumerable<int> changedCountryIds = allChangesByCountryId.Keys;
            IEnumerable<string> changedZoneNames = allChangesByCountryId.Values.SelectMany(x => x.Keys);

            foreach (int changeCountryId in changedCountryIds)//Add missing zones to notification from existing data for all changed countries
            {
                List<ExistingSaleZone> existingZones = existingDataByCountryId.GetRecord(changeCountryId);
                salePlZoneNotifications.AddRange(this.GetZoneNotificationsFromExistingData(customerId, sellingProductId, existingZones, changedZoneNames, futureLocator));
            }

            if (pricelistType == SalePriceListType.Country) //Send zone changes with missing zones from their countries
                return salePlZoneNotifications;

            //Add all missing sold countries to notification from exiting data
            var customerCountryManager = new CustomerCountryManager();
            var soldCountries = customerCountryManager.GetNotClosedCustomerCountries(customerId);

            if (soldCountries == null)
                return salePlZoneNotifications;

            foreach (var soldCountry in soldCountries)
            {
                if (changedCountryIds.Contains(soldCountry.CountryId))
                    continue;

                List<ExistingSaleZone> existingZones = existingDataByCountryId.GetRecord(soldCountry.CountryId);
                if (existingZones != null)
                    salePlZoneNotifications.AddRange(this.GetZoneNotificationsFromExistingData(customerId, sellingProductId, existingZones, changedZoneNames, futureLocator));
            }

            //In this case the pricelist type is Full then we need to return all changed zones with their missing zones in their countries and the other sold countries
            return salePlZoneNotifications;
        }

        private List<SalePLZoneNotification> CreateNotificationsForAllZoneChanges(int customerId, int sellingProductId, ZoneChangesByCountryId allChangesByCountryId,
             Dictionary<int, List<ExistingSaleZone>> zoneWrappersByCountry, SaleEntityZoneRateLocator futureLocator)
        {
            List<SalePLZoneNotification> salePlZoneNotifications = new List<SalePLZoneNotification>();
            foreach (var country in allChangesByCountryId)
            {
                List<ExistingSaleZone> existingSaleZones = zoneWrappersByCountry.GetRecord(country.Key);
                if (existingSaleZones == null) continue;
                foreach (var zoneChange in country.Value)
                {
                    ExistingSaleZone existingSaleZone = existingSaleZones.FirstOrDefault(z => z.ZoneName.Equals(zoneChange.Value.ZoneName));

                    SalePLZoneNotification salePlZone = new SalePLZoneNotification
                    {
                        ZoneName = zoneChange.Value.ZoneName
                    };

                    if (existingSaleZone != null) salePlZone.ZoneId = existingSaleZone.ZoneId;

                    if (zoneChange.Value.CodeChanges != null)
                    {
                        //Add all code changes as notifications
                        salePlZone.Codes.AddRange(zoneChange.Value.CodeChanges.MapRecords(SalePLCodeChangeToSalePLNotificationMapper));
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

                    if (zoneChange.Value.RateChanges != null && zoneChange.Value.RateChanges.Count > 0)
                    {
                        //Add the rate change as notification
                        SalePricelistRateChange rateChange = zoneChange.Value.RateChanges.First();
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
                            salePlZone.Rate = this.GetRateNotificationFromExistingData(customerId, sellingProductId, existingSaleZone.ZoneId, existingSaleZone.ZoneName, futureLocator);
                    }
                    salePlZoneNotifications.Add(salePlZone);
                }
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

            return salePlZoneNotifications;
        }

        #endregion

        #region Pricelist Management

        private VRFile GetPriceListFile(int carrierAccountId, List<SalePLZoneNotification> customerZonesNotifications)
        {
            var salePriceListTemplateManager = new SalePriceListTemplateManager();
            int priceListTemplateId = _carrierAccountManager.GetSalePriceListTemplateId(carrierAccountId);

            SalePriceListTemplate template = salePriceListTemplateManager.GetSalePriceListTemplate(priceListTemplateId);
            if (template == null)
                throw new DataIntegrityValidationException(string.Format("Customer with Id {0} does not have a Sale Price List Template", carrierAccountId));

            PriceListExtensionFormat priceListExtensionFormat = _carrierAccountManager.GetPriceListExtensionFormat(carrierAccountId);
            ISalePriceListTemplateSettingsContext salePlTemplateSettingsContext = new SalePriceListTemplateSettingsContext
            {
                Zones = customerZonesNotifications,
                PriceListExtensionFormat = priceListExtensionFormat
            };

            byte[] salePlTemplateBytes = template.Settings.Execute(salePlTemplateSettingsContext);
            string customerName = _carrierAccountManager.GetCarrierAccountName(carrierAccountId);
            string extension = GetExtensionString(priceListExtensionFormat);
            string fileName = string.Concat("Pricelist_", customerName, "_", DateTime.Today, extension);
            return new VRFile
            {
                Content = salePlTemplateBytes,
                Name = fileName,
                ModuleName = "WhS_BE_SalePriceList",
                Extension = extension,
                CreatedTime = DateTime.Today
            };
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
        private SalePriceList AddOrUpdateSalePriceList(CarrierAccount customer, SalePriceListType customerSalePriceListType, long processInstanceId, VRFile file, int? currencyId, Dictionary<int, SalePriceList> currentSalePriceLists)
        {
            SalePriceList salePriceList;
            var salePriceListManager = new SalePriceListManager();
            var fileManager = new VRFileManager();

            if (!currentSalePriceLists.TryGetValue(customer.CarrierAccountId, out salePriceList))
            {
                int salePriceListId = (int)salePriceListManager.ReserveIdRange(1);
                salePriceList = new SalePriceList
                {
                    OwnerId = customer.CarrierAccountId,
                    OwnerType = SalePriceListOwnerType.Customer,
                    PriceListId = salePriceListId,
                    EffectiveOn = DateTime.Today,
                    CreatedTime = DateTime.Now
                };
                currentSalePriceLists.Add(salePriceList.PriceListId, salePriceList);
            }
            salePriceList.PriceListType = customerSalePriceListType;
            salePriceList.FileId = fileManager.AddFile(file);
            salePriceList.ProcessInstanceId = processInstanceId;
            salePriceList.EffectiveOn = DateTime.Today;
            salePriceList.CurrencyId = currencyId ?? customer.CarrierAccountSettings.CurrencyId;
            return salePriceList;
        }
        private Dictionary<int, SalePriceList> GetSalePriceListByCustomerId(IEnumerable<SalePriceList> salePriceLists, out List<SalePriceList> sellingProductPriceList)
        {
            sellingProductPriceList = new List<SalePriceList>();
            Dictionary<int, SalePriceList> customerPriceListsByCustomerId = new Dictionary<int, SalePriceList>();

            if (salePriceLists == null || !salePriceLists.Any())
                return customerPriceListsByCustomerId;

            foreach (SalePriceList salePriceList in salePriceLists)
            {
                switch (salePriceList.OwnerType)
                {
                    case SalePriceListOwnerType.Customer:
                        if (!customerPriceListsByCustomerId.ContainsKey(salePriceList.OwnerId))
                            customerPriceListsByCustomerId.Add(salePriceList.OwnerId, salePriceList);
                        break;
                    case SalePriceListOwnerType.SellingProduct:
                        sellingProductPriceList.Add(salePriceList);
                        break;
                }
            }
            return customerPriceListsByCustomerId;
        }

        private void BulkInsertCustomerChanges(List<CustomerPriceListChange> customerChanges, long processInstanceId)
        {
            SalePriceListChangeManager salePriceListChangeManager = new SalePriceListChangeManager();
            salePriceListChangeManager.SaveSalePriceListCustomerChanges(customerChanges, processInstanceId);
        }
        private void BulkInsertPriceList(List<SalePriceList> salePriceLists)
        {
            ISalePriceListDataManager dataManager = BEDataManagerFactory.GetDataManager<ISalePriceListDataManager>();
            dataManager.SavePriceListsToDb(salePriceLists);
        }

        private void BulkInsertSalePriceListSnapshot(List<long> saleCodeIds, IEnumerable<int> priceListIds)
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
        private List<CustomerSalePriceListInfo> StructureCustomerPriceListChanges(IEnumerable<CustomerPriceListChange> customerPriceListChanges)
        {
            var customers = new List<CustomerSalePriceListInfo>();

            if (customerPriceListChanges == null || !customerPriceListChanges.Any())
                return null;

            foreach (var customerChanges in customerPriceListChanges)
            {
                ZoneChangesByCountryId countryInfo = new ZoneChangesByCountryId();
                foreach (var codeChange in customerChanges.CodeChanges)
                {
                    var zoneInfo = GetZoneChanges(countryInfo, codeChange.CountryId, codeChange.ZoneName);
                    zoneInfo.CodeChanges.Add(codeChange);
                }
                foreach (var rateChange in customerChanges.RateChanges)
                {
                    var zoneInfo = GetZoneChanges(countryInfo, rateChange.CountryId, rateChange.ZoneName);
                    zoneInfo.RateChanges = new List<SalePricelistRateChange> { rateChange };
                }
                foreach (var routingProductChange in customerChanges.RoutingProductChanges)
                {
                    var zoneInfo = GetZoneChanges(countryInfo, routingProductChange.CountryId, routingProductChange.ZoneName);
                    zoneInfo.RoutingProductChanges = new List<SalePricelistRPChange> { routingProductChange };
                }
                customers.Add(new CustomerSalePriceListInfo
                {
                    CustomerId = customerChanges.CustomerId,
                    ZoneChangesByCountryId = countryInfo,
                    RoutingProductChanges = customerChanges.RoutingProductChanges
                });
            }
            return customers;
        }
        private ZoneChange GetZoneChanges(ZoneChangesByCountryId countryInfo, int countryId, string zoneName)
        {
            ZoneChangesByZoneName zoneDictionary;
            if (!countryInfo.TryGetValue(countryId, out zoneDictionary))
            {
                zoneDictionary = new ZoneChangesByZoneName();
                countryInfo.Add(countryId, zoneDictionary);
            }
            ZoneChange zoneInfo;
            if (!zoneDictionary.TryGetValue(zoneName, out zoneInfo))
            {
                zoneInfo = new ZoneChange
                {
                    ZoneName = zoneName,
                    CodeChanges = new List<SalePricelistCodeChange>()
                };
                zoneDictionary[zoneName] = zoneInfo;
            }
            return zoneInfo;
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

        #region  Private Members

        private VRFile PreparePriceListVrFile(SalePriceList salePriceList, SalePriceListType salePriceListType)
        {
            VRFile file = null;

            var carrierAccountManager = new CarrierAccountManager();
            int sellingNumberPlanId = carrierAccountManager.GetSellingNumberPlanId(salePriceList.OwnerId);

            var salePriceListChangeManager = new SalePriceListChangeManager();
            var customerPriceListChange = salePriceListChangeManager.GetCustomerChangesByPriceListId(salePriceList.PriceListId);
            customerPriceListChange.CustomerId = salePriceList.OwnerId;

            var saleCodeSnapshot = salePriceListChangeManager.GetSalePriceListSaleCodeSnapShot(salePriceList.PriceListId);

            SalePriceListInputContext salePriceListContext = new SalePriceListInputContext
            {
                CustomerChanges = new List<CustomerPriceListChange> { customerPriceListChange },
                EffectiveDate = salePriceList.CreatedTime.Date,
                SellingNumberPlanId = sellingNumberPlanId,
                ProcessInstanceId = salePriceList.ProcessInstanceId,
                SaleCodes = saleCodeSnapshot
            };
            SalePriceListOutputContext salePriceListOutput = PrepareSalePriceListContext(salePriceListContext);

            CustomerSalePriceListInfo customerInfo = salePriceListOutput.CustomerChanges.First();
            int customerId = customerInfo.CustomerId;

            CarrierAccount customer = carrierAccountManager.GetCarrierAccount(customerInfo.CustomerId);

            var customerSellingProductManager = new CustomerSellingProductManager();
            var sellingProductId = customerSellingProductManager.GetEffectiveSellingProductId(customerInfo.CustomerId, DateTime.Now, false);
            if (!sellingProductId.HasValue)
                throw new DataIntegrityValidationException(string.Format("Customer with Id {0} is not assigned to a selling product", customerId));

            CustomerSalePriceListInfo customerChange = salePriceListOutput.CustomerChanges.FindRecord(x => x.CustomerId == customerId);

            ZoneChangesByCountryId allChangesByCountryId = customerChange.ZoneChangesByCountryId;// MergeCurrentWithNotSentChanges(customerId, customerChange.ZoneChangesByCountryId,salePriceListOutput.NotSentChangesByCustomerId);

            List<SalePLZoneNotification> customerZoneNotifications = CreateSalePricelistNotifications(customerId, sellingProductId.Value, salePriceListType, allChangesByCountryId,
                salePriceListOutput.ZoneWrappersByCountry, salePriceListOutput.FutureLocator, salePriceListContext.EffectiveDate, salePriceList.ProcessInstanceId);

            if (customerZoneNotifications.Count > 0)
            {
                AddRPChangesToSalePLNotification(customerZoneNotifications, customerChange.RoutingProductChanges, customerId, sellingProductId.Value);
                file = GetPriceListFile(customer.CarrierAccountId, customerZoneNotifications);
            }

            return file;
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
            public IEnumerable<CustomerPriceListChange> CustomerChanges { get; set; }
            public DateTime EffectiveDate { get; set; }
            public int SellingNumberPlanId { get; set; }
            public long ProcessInstanceId { get; set; }
            public IEnumerable<SaleCode> SaleCodes { get; set; }
        }
        private class SalePriceListOutputContext
        {
            public SaleEntityZoneRateLocator FutureLocator { get; set; }
            public Dictionary<int, List<CustomerPriceListChange>> NotSentChangesByCustomerId { get; set; }
            public List<CustomerSalePriceListInfo> CustomerChanges { get; set; }
            public Dictionary<int, List<ExistingSaleZone>> ZoneWrappersByCountry { get; set; }
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

                sheet.Header.Cells.Add(new ExportExcelHeaderCell() { Title = "ID" });
                sheet.Header.Cells.Add(new ExportExcelHeaderCell() { Title = "Owner Type" });
                sheet.Header.Cells.Add(new ExportExcelHeaderCell() { Title = "Owner Name" });
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
                            row.Cells.Add(new ExportExcelCell() { Value = record.Entity.PriceListId });
                            row.Cells.Add(new ExportExcelCell() { Value = record.OwnerType });
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

        public class CustomerSalePriceListInfo
        {
            public int CustomerId { get; set; }
            public ZoneChangesByCountryId ZoneChangesByCountryId { get; set; }
            public int SellingProductId { get; set; }
            public List<SalePricelistRPChange> RoutingProductChanges { get; set; }

        }
        public class ZoneChangesByCountryId : Dictionary<int, ZoneChangesByZoneName> { }
        public class ZoneChangesByZoneName : Dictionary<string, ZoneChange> { }
        public class ZoneChange
        {
            public string ZoneName { get; set; }
            public List<SalePricelistRateChange> RateChanges { get; set; }
            public List<SalePricelistCodeChange> CodeChanges { get; set; }
            public List<SalePricelistRPChange> RoutingProductChanges { get; set; }
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
