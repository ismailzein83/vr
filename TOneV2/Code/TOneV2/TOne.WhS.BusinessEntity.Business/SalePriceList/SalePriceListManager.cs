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

namespace TOne.WhS.BusinessEntity.Business
{
    public class SalePriceListManager
    {
        #region Private Fields

        private SaleZoneManager _saleZoneManager = new SaleZoneManager();

        private CarrierAccountManager _carrierAccountManager = new CarrierAccountManager();

        #endregion

        #region Save Price List Files
        public void SavePricelistFiles(ISalePricelistFileContext context)
        {
            var priceListsToSave = new List<SalePriceList>();

            if (context.SalePriceLists != null && context.SalePriceLists.Count() > 0)
                priceListsToSave.AddRange(context.SalePriceLists);

            if (context.CustomerPriceListChanges != null && context.CustomerPriceListChanges.Any())
            {
                var sellingProductIds = new List<int>();
                IEnumerable<SaleCode> saleCodes = new SaleCodeManager().GetSaleCodesEffectiveAfter(context.SellingNumberPlanId, context.EffectiveDate);
                if (saleCodes == null || !saleCodes.Any())
                    return;

                var customerChanges = StructureCustomerPriceListChanges(context.CustomerPriceListChanges);
                IEnumerable<ExistingSaleCodeEntity> existingSaleCodeEntities = saleCodes.MapRecords(ExistingSaleCodeEntityMapper);
                Dictionary<string, Dictionary<string, List<ExistingSaleCodeEntity>>> existingSaleCodesByZoneName = StructureExistingSaleCodesByZoneName(existingSaleCodeEntities);
                Dictionary<int, List<ExistingSaleZone>> zoneWrappersByCountry = StructureZoneWrappersByCountry(existingSaleCodesByZoneName);

                IEnumerable<RoutingCustomerInfoDetails> dataByCustomerList;
                Dictionary<int, int> dataByCustomerDictionary;

                var customerIdsWithChanges = customerChanges.Select(c => c.CustomerId);
                SetDataByCustomer(customerIdsWithChanges, context.EffectiveDate, out dataByCustomerList, out dataByCustomerDictionary);
                var futureRateLocator = new SaleEntityZoneRateLocator(new SaleRateReadAllNoCache(dataByCustomerList, context.EffectiveDate, true));
                var salePriceListChanges = new SalePriceListChangeManager();
                var notSentChanges = salePriceListChanges.GetNotSentChanges(customerIdsWithChanges);

                Dictionary<int, SalePriceList> contextPriceListsByPriceListId;
                Dictionary<int, SalePriceList> customerPriceListsByCustomerId;
                StructureSalePriceLists(context.SalePriceLists, out contextPriceListsByPriceListId, out customerPriceListsByCustomerId);

                foreach (var customerChange in customerChanges)
                {
                    CarrierAccount customer = _carrierAccountManager.GetCarrierAccount(customerChange.CustomerId);
                    customerChange.IsCustomerAtoZ = customer.CustomerSettings.IsAToZ;
                    customerChange.SellingProductId = dataByCustomerDictionary.GetRecord(customerChange.CustomerId);
                    sellingProductIds.Add(customerChange.SellingProductId);
                    List<CustomerPriceListChange> notSentPriceListChange;
                    notSentChanges.TryGetValue(customerChange.CustomerId, out notSentPriceListChange);

                    List<SalePLZoneNotification> customerZoneNotifications = MergeExistingWithChanges(customerChange, context, futureRateLocator, zoneWrappersByCountry, notSentPriceListChange);

                    if (customerZoneNotifications.Count > 0)
                    {
                        SalePriceListType customerPlType = GetSalePriceListType(customer.CustomerSettings.IsAToZ, context.ChangeType);
                        SalePriceList priceList = GetPriceList(customer, customerPlType, customerZoneNotifications, customerPriceListsByCustomerId, context.ProcessInstanceId);

                        if (priceList == null)
                            continue;

                        if (!contextPriceListsByPriceListId.ContainsKey(priceList.PriceListId))
                            priceListsToSave.Add(priceList);

                        if (context.CurrencyId.HasValue)
                            priceList.CurrencyId = context.CurrencyId.Value;

                        var tempPriceList = context.CustomerPriceListChanges.FirstOrDefault(r => r.CustomerId == customerChange.CustomerId);

                        if (tempPriceList != null)
                            tempPriceList.PriceListId = priceList.PriceListId;
                    }
                }
                BulkInsertCustomerChanges(context.CustomerPriceListChanges.ToList(), context.ProcessInstanceId);
            }
            BulkInsertPriceList(priceListsToSave);
        }
        private void StructureSalePriceLists(IEnumerable<SalePriceList> salePriceLists, out Dictionary<int, SalePriceList> priceListsByPriceListId, out Dictionary<int, SalePriceList> customerPriceListsByCustomerId)
        {
            priceListsByPriceListId = new Dictionary<int, SalePriceList>();
            customerPriceListsByCustomerId = new Dictionary<int, SalePriceList>();

            if (salePriceLists == null || salePriceLists.Count() == 0)
                return;

            foreach (SalePriceList salePriceList in salePriceLists)
            {
                priceListsByPriceListId.Add(salePriceList.PriceListId, salePriceList);

                if (salePriceList.OwnerType == SalePriceListOwnerType.Customer)
                {
                    if (!customerPriceListsByCustomerId.ContainsKey(salePriceList.OwnerId))
                        customerPriceListsByCustomerId.Add(salePriceList.OwnerId, salePriceList);
                }
            }
        }
        //private Dictionary<int, SalePriceList> StructureCustomerPriceListsByCustomerId(IEnumerable<SalePriceList> priceLists)
        //{
        //    Dictionary<int, SalePriceList> customerPriceListsByCustomerId = new Dictionary<int, SalePriceList>();

        //    if (priceLists == null)
        //        return customerPriceListsByCustomerId;

        //    foreach (SalePriceList priceList in priceLists)
        //    {
        //        if (priceList.OwnerType == SalePriceListOwnerType.Customer)
        //        {
        //            if (!customerPriceListsByCustomerId.ContainsKey(priceList.OwnerId))
        //                customerPriceListsByCustomerId.Add(priceList.OwnerId, priceList);
        //        }
        //    }

        //    return customerPriceListsByCustomerId;
        //}
        private List<CustomerSalePriceListInfo> StructureCustomerPriceListChanges(IEnumerable<CustomerPriceListChange> customerPriceListChanges)
        {
            var customers = new List<CustomerSalePriceListInfo>();

            if (customerPriceListChanges == null || !customerPriceListChanges.Any())
                return null;

            foreach (var customerChanges in customerPriceListChanges)
            {
                CountryByZone countryInfo = new CountryByZone();
                foreach (var codeChange in customerChanges.CodeChanges)
                {
                    var zoneInfo = GetZoneChanges(countryInfo, codeChange.CountryId, codeChange.ZoneName);
                    zoneInfo.CodeChanges.Add(codeChange);
                }
                foreach (var rateChange in customerChanges.RateChanges)
                {
                    var zoneInfo = GetZoneChanges(countryInfo, rateChange.CountryId, rateChange.ZoneName);
                    zoneInfo.Ratechanges = new List<SalePricelistRateChange> { rateChange };
                }
                customers.Add(new CustomerSalePriceListInfo
                {
                    CustomerId = customerChanges.CustomerId,
                    CountryByZone = countryInfo
                });
            }
            return customers;
        }
        private ZoneChanges GetZoneChanges(CountryByZone countryInfo, int countryId, string zoneName)
        {
            ZoneByZoneName zoneDictionary;
            if (!countryInfo.TryGetValue(countryId, out zoneDictionary))
            {
                zoneDictionary = new ZoneByZoneName();
                countryInfo.Add(countryId, zoneDictionary);
            }
            ZoneChanges zoneInfo;
            if (!zoneDictionary.TryGetValue(zoneName, out zoneInfo))
            {
                zoneInfo = new ZoneChanges
                {
                    ZoneName = zoneName,
                    CodeChanges = new List<SalePricelistCodeChange>()
                };
                zoneDictionary[zoneName] = zoneInfo;
            }
            return zoneInfo;
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
        private List<SalePricelistRateChange> MatchRate(List<SalePricelistRateChange> notSentChanges, List<SalePricelistRateChange> currentChanges)
        {
            List<SalePricelistRateChange> saleproPricelistRateChanges = new List<SalePricelistRateChange>();
            if (notSentChanges.Count == 0) return saleproPricelistRateChanges;
            var orderedNotSentChanges = notSentChanges.OrderByDescending(r => r.PricelistId);
            foreach (var currentChange in currentChanges)
            {
                var matchedRate = orderedNotSentChanges.First();
                if (currentChange.Rate > matchedRate.Rate) currentChange.ChangeType = RateChangeType.Increase;
                if (currentChange.Rate < matchedRate.Rate) currentChange.ChangeType = RateChangeType.Decrease;
                if (currentChange.Rate == matchedRate.Rate) currentChange.ChangeType = RateChangeType.NotChanged;
                return new List<SalePricelistRateChange> { currentChange };
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
        private CustomerSalePriceListInfo Manage(CustomerSalePriceListInfo customerInfo, List<CustomerPriceListChange> notSentPriceLists)
        {
            if (notSentPriceLists == null) return customerInfo;

            var manager = new SalePriceListManager();
            var salePriceListChangeManager = new SalePriceListChangeManager();
            IEnumerable<SalePriceList> customerPriceLists = manager.GetCustomerSalePriceListsById(customerInfo.CustomerId).Where(p => p.IsSent);
            int lastSentPriceListId = 0;
            if (customerPriceLists.Any())
                lastSentPriceListId = customerPriceLists.Max(p => p.PriceListId);

            var notSentCustomerChanges = StructureCustomerPriceListChanges(notSentPriceLists.Where(p => p.PriceListId >= lastSentPriceListId)
                        .OrderByDescending((p => p.PriceListId)));

            var lastSentPriceListChanges = salePriceListChangeManager.GetCustomerChangesByPriceListId(lastSentPriceListId);
            foreach (var notSentpricelist in notSentCustomerChanges)
            {
                foreach (var notSentCountryByZone in notSentpricelist.CountryByZone)
                {
                    ZoneByZoneName currentZone;
                    if (customerInfo.CountryByZone.TryGetValue(notSentCountryByZone.Key, out currentZone))
                    {
                        foreach (var notSentZone in notSentCountryByZone.Value)
                        {
                            ZoneChanges zoneChanges;
                            if (currentZone.TryGetValue(notSentZone.Value.ZoneName, out zoneChanges))
                            {
                                zoneChanges.Ratechanges = MatchRate(lastSentPriceListChanges.RateChanges,
                                    zoneChanges.Ratechanges);
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
                        customerInfo.CountryByZone.Add(notSentCountryByZone.Key, notSentCountryByZone.Value);
                    }
                }
            }
            return customerInfo;
        }
        private List<SalePLZoneNotification> MergeExistingWithChanges(CustomerSalePriceListInfo customerInfo, ISalePricelistFileContext context, SaleEntityZoneRateLocator locator,
            Dictionary<int, List<ExistingSaleZone>> zoneWrappersByCountry, List<CustomerPriceListChange> notSentChanges)
        {
            var salePlZoneNotifications = new List<SalePLZoneNotification>();
            var customerCountryManager = new CustomerCountryManager();
            var updateCountryByZone = Manage(customerInfo, notSentChanges);
            salePlZoneNotifications = MapToSalePlZoneNotification(updateCountryByZone.CountryByZone, zoneWrappersByCountry, locator, customerInfo);
            var soldCountries = customerCountryManager.GetCustomerCountriesEffectiveAfter(customerInfo.CustomerId, context.EffectiveDate);

            if (soldCountries == null) return salePlZoneNotifications;

            if (context.ChangeType == SalePLChangeType.CodeAndRate && !customerInfo.IsCustomerAtoZ)
            {
                List<int> tempCountries = updateCountryByZone.CountryByZone.Keys.ToList();
                soldCountries = soldCountries.Where(c => tempCountries.Contains(c.CountryId)).ToList();
            }
            if (context.ChangeType == SalePLChangeType.Rate)
                return salePlZoneNotifications;

            Dictionary<string, SalePLZoneNotification> grouppedByZoneName = GroupSalePlZoneNotificationByZoneName(salePlZoneNotifications);

            foreach (var soldCountry in soldCountries)
            {
                List<ExistingSaleZone> zones = zoneWrappersByCountry.GetRecord(soldCountry.CountryId);
                if (zones == null) continue;
                foreach (var zone in zones)
                {
                    SalePLZoneNotification zoneChangeNotification;
                    if (!grouppedByZoneName.TryGetValue(zone.ZoneName, out zoneChangeNotification))
                    {
                        SalePLZoneNotification tempZone = CreateZoneNotification(zone, soldCountry, locator,
                            customerInfo.CustomerId, customerInfo.SellingProductId);
                        if (tempZone != null) salePlZoneNotifications.Add(tempZone);
                    }
                    else
                    {
                        List<SalePLCodeNotification> codechanges = new List<SalePLCodeNotification>();

                        foreach (var code in zone.Codes)
                        {
                            var match = zoneChangeNotification.Codes.Where(c => c.Code == code.Code);
                            if (match.Any()) continue;
                            codechanges.Add(new SalePLCodeNotification
                                {
                                    Code = code.Code,
                                    BED = code.BED,
                                    EED = code.EED,
                                    CodeChange = CodeChange.NotChanged
                                });

                        }
                        zoneChangeNotification.Codes.AddRange(codechanges);
                    }
                }
            }
            return salePlZoneNotifications;
        }
        private SalePLZoneNotification CreateZoneNotification(ExistingSaleZone zoneWrapper, CustomerCountry2 soldCountry, SaleEntityZoneRateLocator locator, int customerId, int sellingProductId)
        {
            SaleEntityZoneRate zoneRate = locator.GetCustomerZoneRate(customerId, sellingProductId, zoneWrapper.ZoneId);
            if (zoneRate == null) return null;
            var baseRatesByZone = new BaseRatesByZone();
            var zoneNotification = new SalePLZoneNotification
            {
                ZoneName = zoneWrapper.ZoneName,
                ZoneId = zoneWrapper.ZoneId,
                Rate = new SalePLRateNotification
                {
                    Rate = zoneRate.Rate.Rate,
                    BED = zoneRate.Rate.BED,
                }
            };
            zoneNotification.Codes.AddRange(zoneWrapper.Codes.MapRecords(SalePLCodeNotificationMapper));

            if (zoneRate.Source == SalePriceListOwnerType.SellingProduct)
                baseRatesByZone.AddZoneBaseRate(zoneNotification.ZoneId, zoneNotification, soldCountry.CountryId, null, zoneRate.Rate.BED, zoneRate.Rate.EED);
            return zoneNotification;
        }
        private Dictionary<string, SalePLZoneNotification> GroupSalePlZoneNotificationByZoneName(List<SalePLZoneNotification> salePlZoneNotifications)
        {
            Dictionary<string, SalePLZoneNotification> grouppedDictionary = new Dictionary<string, SalePLZoneNotification>();
            foreach (var zone in salePlZoneNotifications)
            {
                if (!grouppedDictionary.ContainsKey(zone.ZoneName))
                {
                    grouppedDictionary.Add(zone.ZoneName, zone);
                }
            }
            return grouppedDictionary;
        }
        private List<SalePLZoneNotification> MapToSalePlZoneNotification(CountryByZone customerCountryByZone, Dictionary<int, List<ExistingSaleZone>> zoneWrappersByCountry, SaleEntityZoneRateLocator locator, CustomerSalePriceListInfo customerIfo)
        {
            List<SalePLZoneNotification> salePlZoneNotifications = new List<SalePLZoneNotification>();
            foreach (var country in customerCountryByZone)
            {
                List<ExistingSaleZone> existingSaleZones = zoneWrappersByCountry.GetRecord(country.Key);
                if (existingSaleZones == null) continue;
                foreach (var zone in country.Value)
                {
                    ExistingSaleZone zoneWrapper = existingSaleZones.FirstOrDefault(z => z.ZoneName.Equals(zone.Value.ZoneName));
                    SalePLZoneNotification salePlZone = new SalePLZoneNotification
                    {
                        ZoneName = zone.Key,
                        Codes = new List<SalePLCodeNotification>()
                    };
                    if (zone.Value.CodeChanges.Count > 0)
                        salePlZone.Codes.AddRange(zone.Value.CodeChanges.Select(code => new SalePLCodeNotification
                        {
                            Code = code.Code,
                            BED = code.BED,
                            EED = code.EED,
                            CodeChange = code.ChangeType
                        }));
                    else
                    {
                        salePlZone.Codes.AddRange(zoneWrapper.Codes.MapRecords(SalePLCodeNotificationMapper));
                    }
                    if (zone.Value.Ratechanges != null && zone.Value.Ratechanges.Count > 0)
                    {
                        SalePricelistRateChange rateChange = zone.Value.Ratechanges.First();
                        salePlZone.Rate = new SalePLRateNotification
                        {
                            Rate = rateChange.Rate,
                            BED = rateChange.BED,
                            RateChangeType = rateChange.ChangeType,
                            EED = rateChange.EED
                        };
                    }
                    else
                    {
                        if (zoneWrapper != null)
                        {
                            var zoneRate = locator.GetCustomerZoneRate(customerIfo.CustomerId,
                                customerIfo.SellingProductId,
                                zoneWrapper.ZoneId);
                            if (zoneRate != null)
                                salePlZone.Rate = new SalePLRateNotification
                                {
                                    Rate = zoneRate.Rate.Rate,
                                    BED = zoneRate.Rate.BED,
                                    RateChangeType = zoneRate.Rate.RateChange,
                                    EED = zoneRate.Rate.EED
                                };
                        }

                    }
                    salePlZoneNotifications.Add(salePlZone);
                }
            }
            return salePlZoneNotifications;
        }
        private SalePriceList GetPriceList(CarrierAccount customer, SalePriceListType customerSalePriceListType, List<SalePLZoneNotification> customerZonesNotifications, Dictionary<int, SalePriceList> currentSalePriceLists, long processInstanceId)
        {
            SalePriceList salePriceList;
            var salePriceListTemplateManager = new SalePriceListTemplateManager();
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
                    EffectiveOn = DateTime.Today
                };
            }
            int priceListTemplateId = _carrierAccountManager.GetSalePriceListTemplateId(customer.CarrierAccountId);

            SalePriceListTemplate template = salePriceListTemplateManager.GetSalePriceListTemplate(priceListTemplateId);

            if (template == null)
                throw new DataIntegrityValidationException(string.Format("Customer with Id {0} does not have a Sale Price List Template", customer.CarrierAccountId));

            ISalePriceListTemplateSettingsContext salePlTemplateSettingsContext = new SalePriceListTemplateSettingsContext
            {
                Zones = customerZonesNotifications
            };
            byte[] salePlTemplateBytes = template.Settings.Execute(salePlTemplateSettingsContext);
            string customerName = _carrierAccountManager.GetCarrierAccountName(customer.CarrierAccountId);
            string fileName = string.Concat("Pricelist_", customerName, "_", DateTime.Today, ".xls");
            VRFile file = new VRFile
            {
                Content = salePlTemplateBytes,
                Name = fileName,
                ModuleName = "WhS_BE_SalePriceList",
                Extension = "xls",
                CreatedTime = DateTime.Today,
            };
            salePriceList.PriceListType = customerSalePriceListType;
            salePriceList.FileId = fileManager.AddFile(file);
            salePriceList.ProcessInstanceId = processInstanceId;
            salePriceList.EffectiveOn = DateTime.Today;
            salePriceList.CurrencyId = customer.CarrierAccountSettings.CurrencyId;
            return salePriceList;
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
                    throw new DataIntegrityValidationException(string.Format("Customer '{0}' is not assigned to a Selling Product", customerId));

                list.Add(new RoutingCustomerInfoDetails
                {
                    CustomerId = customerId,
                    SellingProductId = sellingProductId.Value
                });

                dictionary.Add(customerId, sellingProductId.Value);
            }
            dataByCustomerList = list;
            dataByCustomerDictionary = dictionary;
        }

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

        public bool SetCustomerPricelistsAsSent(IEnumerable<int> customerIds)
        {
            ISalePriceListDataManager dataManager = BEDataManagerFactory.GetDataManager<ISalePriceListDataManager>();
            return dataManager.SetCustomerPricelistsAsSent(customerIds);
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

            return (changeType == SalePLChangeType.CodeAndRate)
                ? SalePriceListType.Country
                : SalePriceListType.RateChange;
        }

        #endregion

        #region Private Classes

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
            public CountryByZone CountryByZone { get; set; }
            public bool IsCustomerAtoZ { get; set; }
            public int SellingProductId { get; set; }

        }
        public class CountryByZone : Dictionary<int, ZoneByZoneName> { }
        public class ZoneByZoneName : Dictionary<string, ZoneChanges> { }
        public class ZoneChanges
        {
            public string ZoneName { get; set; }
            public List<SalePricelistRateChange> Ratechanges { get; set; }
            public List<SalePricelistCodeChange> CodeChanges { get; set; }
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
