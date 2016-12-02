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
            var salePricelists = GetCachedSalePriceLists();

            Func<SalePriceList, bool> filterExpression = (priceList) =>

                     (input.Query.OwnerId == null || input.Query.OwnerId.Contains(priceList.OwnerId)) &&
                      (input.Query.OwnerType == null || priceList.OwnerType == input.Query.OwnerType);


            return Vanrise.Common.DataRetrievalManager.Instance.ProcessResult(input, salePricelists.ToBigResult(input, filterExpression, SalePricelistDetailMapper));

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

        public void SavePricelistFiles(ISalePricelistFileContext context)
        {
            if (context.CustomerIds == null)
                throw new NullReferenceException("CustomerIds");

            IEnumerable<SaleCode> saleCodes = new SaleCodeManager().GetSaleCodesEffectiveAfter(context.SellingNumberPlanId, Vanrise.Common.Utilities.Min(context.EffectiveDate, DateTime.Today));
            IEnumerable<ExistingSaleCodeEntity> saleCodesExistingEntities = saleCodes.MapRecords(SaleCodeExistingEntityMapper);

            if (saleCodesExistingEntities == null)
                return;

            SalePriceListManager salePriceListManager = new SalePriceListManager();
            IEnumerable<SalePriceList> salePriceLists = salePriceListManager.GetCustomerSalePriceListsByProcessInstanceId(context.ProcessInstanceId);
            Dictionary<int, SalePriceList> salePriceListsByCustomer = StructureSalePriceListsByCustomer(salePriceLists);

            Dictionary<string, Dictionary<string, List<ExistingSaleCodeEntity>>> existingSaleCodesByZoneName = StructureExistingSaleCodesByZoneName(saleCodesExistingEntities);
            Dictionary<int, List<ExistingSaleZone>> zonesWrapperByCountry = StructureZonesWrapperByCountry(existingSaleCodesByZoneName);
            Dictionary<int, List<SalePLZoneChange>> zoneChangesByCountryId = StructureZoneChangesByCountry(context.ZoneChanges);

            List<RoutingCustomerInfoDetails> routingCustomersInfoDetails = new List<RoutingCustomerInfoDetails>();
            CustomerSellingProductManager customerSellingProductManager = new CustomerSellingProductManager();
            Dictionary<int, int> carrierAccounts = new Dictionary<int, int>();

            foreach (int customerId in context.CustomerIds)
            {
                CustomerSellingProduct customerSellingProduct = customerSellingProductManager.GetEffectiveSellingProduct(customerId, context.EffectiveDate, false);
                if (customerSellingProduct == null)
                    continue;

                if (!carrierAccounts.ContainsKey(customerId))
                    carrierAccounts.Add(customerId, customerSellingProduct.SellingProductId);

                routingCustomersInfoDetails.Add(new RoutingCustomerInfoDetails()
                {
                    CustomerId = customerId,
                    SellingProductId = customerSellingProduct.SellingProductId
                });
            }

            SaleRateReadAllNoCache saleRateReadWithNoCache = new SaleRateReadAllNoCache(routingCustomersInfoDetails, context.EffectiveDate, true);
            SaleEntityZoneRateLocator rateLocator = new SaleEntityZoneRateLocator(saleRateReadWithNoCache);

            SalePLZoneNotification salePLZoneNotifications = new SalePLZoneNotification();
            var customerCountryManager = new CustomerCountryManager();

            foreach (int customerId in context.CustomerIds)
            {
                CarrierAccount customer = _carrierAccountManager.GetCarrierAccount(customerId);

                List<SalePLZoneNotification> customerZoneNotifications = new List<SalePLZoneNotification>();

                int assignedSellingProductId = carrierAccounts.GetRecord(customerId);

                SalePriceListType customerSalePriceListType = GetSalePriceListType(customer.CustomerSettings.IsAToZ, context.ChangeType);
                IEnumerable<int> customerSoldCountries = customerCountryManager.GetCustomerCountryIds(customerId, context.EffectiveDate, false);

                if (customerSoldCountries != null)
                {

                    foreach (int soldCountryId in customerSoldCountries)
                    {
                        List<ExistingSaleZone> zonesWrapperForCountry = zonesWrapperByCountry.GetRecord(soldCountryId);
                        IEnumerable<SalePLZoneChange> countryZonesChanges = zoneChangesByCountryId.GetRecord(soldCountryId);

                        if (customer.CustomerSettings.IsAToZ)
                        {
                            CreateSalePLZoneNotifications(customerZoneNotifications, zonesWrapperForCountry, rateLocator, assignedSellingProductId, customerId);
                        }
                        else if (countryZonesChanges != null)
                        {
                            if (context.ChangeType == SalePLChangeType.CodeAndRate)
                            {
                                CreateSalePLZoneNotifications(customerZoneNotifications, zonesWrapperForCountry, rateLocator, assignedSellingProductId, customerId);
                            }
                            else if (context.ChangeType == SalePLChangeType.Rate)
                            {
                                if (zonesWrapperForCountry != null)
                                {
                                    List<ExistingSaleZone> zonesWrapperHaveChanges = new List<ExistingSaleZone>();
                                    foreach (ExistingSaleZone zoneWrapper in zonesWrapperForCountry)
                                    {
                                        SalePLZoneChange salePLZoneChange = countryZonesChanges.FindRecord(item => item.ZoneName.Equals(zoneWrapper.ZoneName));
                                        if (salePLZoneChange != null && salePLZoneChange.CustomersHavingRateChange.Contains(customerId))
                                        {
                                            zonesWrapperHaveChanges.Add(zoneWrapper);
                                        }
                                    }
                                    CreateSalePLZoneNotifications(customerZoneNotifications, zonesWrapperHaveChanges, rateLocator, assignedSellingProductId, customerId);
                                }
                            }
                        }
                    }
                }

                if (customerZoneNotifications.Count > 0)
                {
                    SavePriceListFile(customer, customerSalePriceListType, customerZoneNotifications, salePriceListsByCustomer, context.ProcessInstanceId);
                }
            }
        }

        public SalePriceList GetPriceListByCustomerAndProcessInstanceId(long processInstanceId, int customerId)
        {
            IEnumerable<SalePriceList> processSalePricelists = this.GetCustomerSalePriceListsByProcessInstanceId(processInstanceId);

            if (processSalePricelists == null)
                return null;

            return processSalePricelists.FindRecord(itm => itm.OwnerId == customerId);
        }

        #endregion

        #region  Private Members

        public Dictionary<int, SalePriceList> GetCachedSalePriceLists()
        {
            return Vanrise.Caching.CacheManagerFactory.GetCacheManager<CacheManager>().GetOrCreateObject(String.Format("GetCashedSalePriceLists"),
               () =>
               {
                   ISalePriceListDataManager dataManager = BEDataManagerFactory.GetDataManager<ISalePriceListDataManager>();
                   Dictionary<int, SalePriceList> allSalePriceLists = this.GetCachedSalePriceListsWithDeleted();
                   Dictionary<int, SalePriceList> dic = new Dictionary<int, SalePriceList>();

                   foreach (SalePriceList item in allSalePriceLists.Values)
                   {
                       if (!item.IsDeleted)
                           dic.Add(item.PriceListId, item);
                   }
                   return dic;
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

        private Dictionary<int, List<ExistingSaleZone>> StructureZonesWrapperByCountry(Dictionary<string, Dictionary<string, List<ExistingSaleCodeEntity>>> existingSaleCodesByZoneName)
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

                int countryId = zoneManager.GetSaleZoneCountryId(zoneWrapper.ZoneId);
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

         private void CreateSalePLZoneNotifications(List<SalePLZoneNotification> salePLZoneNotifications, List<ExistingSaleZone> zonesWrappers, SaleEntityZoneRateLocator rateLocator, int sellingProductId, int customerId)
        {
            if (zonesWrappers != null)
            {
                foreach (ExistingSaleZone zoneWrapper in zonesWrappers)
                {
                    SalePLZoneNotification zoneNotification = new SalePLZoneNotification()
                    {
                        ZoneName = zoneWrapper.ZoneName,
                        ZoneId = zoneWrapper.ZoneId
                    };

                    SaleEntityZoneRate saleEntityZoneRate = rateLocator.GetCustomerZoneRate(customerId, sellingProductId, zoneWrapper.ZoneId);

                    zoneNotification.Rate = saleEntityZoneRate != null ? SalePLRateNotificationMapper(saleEntityZoneRate) : GetExistingRate();
                    zoneNotification.Codes.AddRange(zoneWrapper.Codes.MapRecords(SalePLCodeNotificationMapper));
                    salePLZoneNotifications.Add(zoneNotification);
                }
            }
        }

        private SalePLRateNotification GetExistingRate()
        {
            //TODO: Must Get Existing Rate
            return null;
        }

         private SalePriceListType GetSalePriceListType(bool isAtoZ, SalePLChangeType changeType)
        {
            if (isAtoZ)
                return SalePriceListType.Full;

            return (changeType == SalePLChangeType.CodeAndRate) ? SalePriceListType.Country : SalePriceListType.RateChange;
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
            pricelistDetail.PriceListTypeName = Vanrise.Common.Utilities.GetEnumDescription(priceList.PriceListType);


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

        private ExistingSaleCodeEntity SaleCodeExistingEntityMapper(SaleCode saleCode)
        {
            return new ExistingSaleCodeEntity(saleCode)
            {
                CountryId = _saleZoneManager.GetSaleZoneCountryId(saleCode.ZoneId),
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

        #endregion

    }
}
