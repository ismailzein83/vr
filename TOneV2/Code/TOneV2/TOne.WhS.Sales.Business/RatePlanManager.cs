using Aspose.Cells;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using TOne.WhS.BusinessEntity.Business;
using TOne.WhS.BusinessEntity.Entities;
using TOne.WhS.Routing.Business;
using TOne.WhS.Routing.Entities;
using TOne.WhS.Sales.Business.Reader;
using TOne.WhS.Sales.Data;
using TOne.WhS.Sales.Entities;
using Vanrise.Common;
using Vanrise.Common.Business;
using Vanrise.Entities;

namespace TOne.WhS.Sales.Business
{
    public class RatePlanManager
    {
        #region Fields

        IRatePlanDataManager _dataManager = SalesDataManagerFactory.GetDataManager<IRatePlanDataManager>();
        RoutingProductManager _routingProductManager = new RoutingProductManager();

        #endregion

        #region Public Methods
        public void CleanTemporaryTables(long processInstanceId)
        {
            var ratePlanDataManager = SalesDataManagerFactory.GetDataManager<IRatePlanDataManager>();
            ratePlanDataManager.CleanTemporaryTables(processInstanceId);
        }
        public bool ValidateCustomer(int customerId, DateTime effectiveOn)
        {
            int? sellingProductId = new CarrierAccountManager().GetSellingProductId(customerId);
            return sellingProductId.HasValue;
        }

        #region Get Zone Letters

        public IEnumerable<char> GetZoneLetters(ZoneLettersInput input)
        {
            IEnumerable<SaleZone> saleZones = GetSaleZones(input.OwnerType, input.OwnerId, input.EffectiveOn, true);

            if (saleZones == null || saleZones.Count() == 0)
                return null;

            IEnumerable<long> applicableZoneIds = null;

            if (input.BulkAction != null)
            {
                if (input.BulkActionFilter == null)
                    throw new Vanrise.Entities.DataIntegrityValidationException("BulkActionZoneFilter of BulkAction was not found");

                Changes draft = new RatePlanDraftManager().GetDraft(input.OwnerType, input.OwnerId);
                IEnumerable<long> saleZoneIds = saleZones.MapRecords(x => x.SaleZoneId);

                int sellingProductId;
                Dictionary<int, DateTime> countryBEDsByCountryId;
                ISaleRateReader currentRateReader = GetCurrentRateReader(input.OwnerType, input.OwnerId, saleZones, input.EffectiveOn, out sellingProductId, out countryBEDsByCountryId);

                SaleEntityZoneRateLocator currentRateLocator;
                Func<int, long, SaleEntityZoneRate> getSellingProductZoneRate;
                Func<int, int, long, SaleEntityZoneRate> getCustomerZoneRate;
                UtilitiesManager.SetBulkActionContextCurrentRateHelpers(currentRateReader, out currentRateLocator, out getSellingProductZoneRate, out getCustomerZoneRate);

                var applicableZoneIdsContext = new ApplicableZoneIdsContext(getSellingProductZoneRate, getCustomerZoneRate)
                {
                    OwnerType = input.OwnerType,
                    OwnerId = input.OwnerId,
                    BulkAction = input.BulkAction,
                    DraftData = draft,
                    SaleZones = saleZones
                };

                applicableZoneIds = input.BulkActionFilter.GetApplicableZoneIds(applicableZoneIdsContext);
            }

            Func<SaleZone, bool> filterFunc = (saleZone) =>
            {
                if (input.ExcludedZoneIds != null && input.ExcludedZoneIds.Contains(saleZone.SaleZoneId))
                    return false;

                if (applicableZoneIds != null && !applicableZoneIds.Contains(saleZone.SaleZoneId))
                    return false;

                if (!SaleZoneFilter(saleZone, input.CountryIds, input.ZoneNameFilterType, input.ZoneNameFilter))
                    return false;

                return true;
            };

            return saleZones.MapRecords(x => char.ToUpper(x.Name[0]), filterFunc).Distinct().OrderBy(x => x);
        }

        #endregion

        #region Get Zone Item(s)

        // TODO: Divide GetZoneItem(s) into GetSellingProductZoneItem(s) and GetCustomerZoneItem(s) to get rid of unnecessary sellingProductId == null and ownerType checks
        public IEnumerable<ZoneItem> GetZoneItems(ZoneItemsInput input)
        {
            IEnumerable<SaleZone> saleZones = GetSaleZones(input.Filter.OwnerType, input.Filter.OwnerId, DateTime.Now, true);
            if (saleZones == null)
                return null;

            // Filter the sale zones
            Func<SaleZone, bool> filterFunc = (saleZone) =>
            {
                if (!SaleZoneFilter(saleZone, input.Filter.CountryIds, input.Filter.ZoneNameFilterType, input.Filter.ZoneNameFilter))
                    return false;
                if (char.ToLower(saleZone.Name.ElementAt(0)) != char.ToLower(input.Filter.ZoneLetter))
                    return false;
                return true;
            };
            saleZones = saleZones.FindAllRecords(filterFunc);

            if (saleZones == null)
                return null;

            // Page the sale zones
            saleZones = GetPagedZones(saleZones, input.FromRow, input.ToRow);
            if (saleZones == null)
                return null;

            // Prepare the zone items
            var zoneItems = new List<ZoneItem>();
            Changes draft = _dataManager.GetChanges(input.Filter.OwnerType, input.Filter.OwnerId, RatePlanStatus.Draft);

            var changedCountryIds = new List<int>();
            if (draft != null && draft.CountryChanges != null && draft.CountryChanges.ChangedCountries != null && draft.CountryChanges.ChangedCountries.Countries != null)
                changedCountryIds.AddRange(draft.CountryChanges.ChangedCountries.Countries.MapRecords(x => x.CountryId));

            int? sellingProductId = GetSellingProductId(input.Filter.OwnerType, input.Filter.OwnerId, DateTime.Now, false);
            if (sellingProductId == null)
                throw new Exception("Selling product does not exist");

            // TODO: Add EffectiveOn to input.Filter
            var effectiveOn = DateTime.Now.Date;

            var rateLocator = new SaleEntityZoneRateLocator(new SaleRateReadWithCache(effectiveOn));

            int longPrecision = -1;
            var rateManager = new ZoneRateManager(input.Filter.OwnerType, input.Filter.OwnerId, sellingProductId, effectiveOn, draft, input.CurrencyId, longPrecision, rateLocator);

            var rpManager = new ZoneRPManager(input.Filter.OwnerType, input.Filter.OwnerId, effectiveOn, draft, null);

            var baseRatesByZone = new BaseRatesByZone();
            var saleRateManager = new SaleRateManager();

            foreach (SaleZone saleZone in saleZones)
            {
                var zoneItem = new ZoneItem()
                {
                    ZoneId = saleZone.SaleZoneId,
                    ZoneName = saleZone.Name,
                    CountryId = saleZone.CountryId,
                    ZoneBED = saleZone.BED,
                    ZoneEED = saleZone.EED
                };

                zoneItem.IsFutureZone = (zoneItem.ZoneBED.Date > DateTime.Now.Date);

                ZoneChanges zoneDraft = null;
                if (draft != null && draft.ZoneChanges != null)
                    zoneDraft = draft.ZoneChanges.FindRecord(x => x.ZoneId == saleZone.SaleZoneId);

                rateManager.SetZoneRate(zoneItem);

                // TODO: Refactor rateManager to handle products and customers separately
                if (input.Filter.OwnerType == SalePriceListOwnerType.SellingProduct)
                {
                    rpManager.SetSellingProductZoneRP(zoneItem, input.Filter.OwnerId, zoneDraft);
                }
                else
                {
                    AddZoneBaseRates(baseRatesByZone, zoneItem); // Check if the customer zone has any inherited rate
                    rpManager.SetCustomerZoneRP(zoneItem, input.Filter.OwnerId, sellingProductId.Value, zoneDraft);
                }

                zoneItem.IsCountryEnded = changedCountryIds.Contains(zoneItem.CountryId);
                zoneItems.Add(zoneItem);
            }

            if (input.Filter.OwnerType == SalePriceListOwnerType.Customer)
            {
                IEnumerable<DraftNewCountry> draftNewCountries = (draft != null && draft.CountryChanges != null) ? draft.CountryChanges.NewCountries : null;
                IEnumerable<CustomerCountry2> soldCountries = GetSoldCountries(input.Filter.OwnerId, effectiveOn, false, draftNewCountries);
                saleRateManager.ProcessBaseRatesByZone(input.Filter.OwnerId, baseRatesByZone, soldCountries);
            }

            //IEnumerable<RPZone> rpZones = zoneItems.MapRecords(x => new RPZone() { RoutingProductId = x.EffectiveRoutingProductId.Value, SaleZoneId = x.ZoneId }, x => x.EffectiveRoutingProductId.HasValue);
            //var routeOptionManager = new ZoneRouteOptionManager(input.Filter.OwnerType, input.Filter.OwnerId, input.Filter.RoutingDatabaseId, input.Filter.PolicyConfigId, input.Filter.NumberOfOptions, rpZones, input.Filter.CostCalculationMethods, input.Filter.CostCalculationMethodConfigId, input.Filter.RateCalculationMethod, input.CurrencyId);
            //routeOptionManager.SetZoneRouteOptionProperties(zoneItems);

            return zoneItems;
        }

        #region Zone Base Rate Methods

        private void AddZoneBaseRates(BaseRatesByZone baseRatesByZone, ZoneItem zoneItem)
        {
            if (zoneItem.IsCurrentRateEditable.HasValue && !zoneItem.IsCurrentRateEditable.Value)
            {
                int? rateTypeId = null;
                baseRatesByZone.AddZoneBaseRate(zoneItem.ZoneId, zoneItem, zoneItem.CountryId, rateTypeId, zoneItem.CurrentRateBED.Value, zoneItem.CurrentRateEED);
            }
            if (zoneItem.CurrentOtherRates != null)
            {
                foreach (OtherRate currentOtherRate in zoneItem.CurrentOtherRates.Values)
                {
                    if (!currentOtherRate.IsRateEditable)
                        baseRatesByZone.AddZoneBaseRate(zoneItem.ZoneId, zoneItem, zoneItem.CountryId, currentOtherRate.RateTypeId, currentOtherRate.BED, currentOtherRate.EED);
                }
            }
        }

        private IEnumerable<CustomerCountry2> GetSoldCountries(int customerId, DateTime effectiveOn, bool isEffectiveInFuture, IEnumerable<DraftNewCountry> draftNewCountries)
        {
            var allCountries = new List<CustomerCountry2>();
            var customerCountryManager = new CustomerCountryManager();

            IEnumerable<CustomerCountry2> soldCountries = customerCountryManager.GetEffectiveOrFutureCustomerCountries(customerId, effectiveOn);
            if (soldCountries != null)
                allCountries.AddRange(soldCountries);

            if (draftNewCountries != null)
            {
                foreach (DraftNewCountry draftNewCountry in draftNewCountries)
                {
                    allCountries.Add(new CustomerCountry2()
                    {
                        CustomerId = customerId,
                        CountryId = draftNewCountry.CountryId,
                        BED = draftNewCountry.BED,
                        EED = draftNewCountry.EED
                    });
                }
            }

            return allCountries;
        }

        #endregion

        private IEnumerable<SaleZone> GetPagedZones(IEnumerable<SaleZone> saleZones, int fromRow, int toRow)
        {
            if (saleZones == null)
                return saleZones;

            List<SaleZone> pagedZones = null;

            saleZones = saleZones.OrderBy(x => x.Name);
            int count = saleZones.Count();

            if (count >= fromRow)
            {
                pagedZones = new List<SaleZone>();

                for (int i = fromRow - 1; i < count && i < toRow; i++)
                    pagedZones.Add(saleZones.ElementAt(i));
            }

            return pagedZones;
        }

        public ZoneItem GetZoneItem(ZoneItemInput input)
        {
            SaleZoneManager saleZoneManager = new SaleZoneManager();

            SaleZone saleZone = saleZoneManager.GetSaleZone(input.ZoneId);
            if (saleZone == null)
                throw new NullReferenceException(string.Format("SaleZone '{0}' was not found", input.ZoneId));

            ZoneItem zoneItem = new ZoneItem()
            {
                ZoneId = input.ZoneId,
                ZoneName = saleZone.Name,
                CountryId = saleZone.CountryId
            };

            // TODO: Add EffectiveOn to input
            var effectiveOn = DateTime.Now.Date;

            int? sellingProductId = GetSellingProductId(input.OwnerType, input.OwnerId, effectiveOn, false);
            Changes draft = _dataManager.GetChanges(input.OwnerType, input.OwnerId, RatePlanStatus.Draft);

            int normalPrecisionValue, longPrecisionValue;
            SetNumberPrecisionValues(out normalPrecisionValue, out longPrecisionValue);

            var rateLocator = new SaleEntityZoneRateLocator(new SaleRateReadWithCache(effectiveOn));
            ZoneRateManager rateSetter = new ZoneRateManager(input.OwnerType, input.OwnerId, sellingProductId, effectiveOn, draft, input.CurrencyId, longPrecisionValue, rateLocator);
            rateSetter.SetZoneRate(zoneItem);

            if (sellingProductId == null)
                throw new Exception("Selling product does not exist");

            int? customerId = null;
            if (input.OwnerType == SalePriceListOwnerType.Customer)
                customerId = input.OwnerId;

            SaleEntityZoneRoutingProductLocator zoneRPLocator = new SaleEntityZoneRoutingProductLocator(new SaleEntityRoutingProductReadWithCache(effectiveOn));
            var rpManager = new ZoneRPManager(input.OwnerType, input.OwnerId, effectiveOn, draft, zoneRPLocator);

            ZoneChanges zoneDraft = null;
            if (draft != null && draft.ZoneChanges != null)
                zoneDraft = draft.ZoneChanges.FindRecord(x => x.ZoneId == input.ZoneId);

            if (input.OwnerType == SalePriceListOwnerType.SellingProduct)
                rpManager.SetSellingProductZoneRP(zoneItem, input.OwnerId, zoneDraft);
            else
                rpManager.SetCustomerZoneRP(zoneItem, input.OwnerId, sellingProductId.Value, zoneDraft);

            var rpZones = new List<RPZone>();
            if (zoneItem.EffectiveRoutingProductId.HasValue)
            {
                rpZones.Add(new RPZone()
                {
                    RoutingProductId = zoneItem.EffectiveRoutingProductId.Value,
                    SaleZoneId = zoneItem.ZoneId
                });
            }

            var routeOptionManager = new ZoneRouteOptionManager(input.OwnerType, input.OwnerId, input.RoutingDatabaseId, input.PolicyConfigId, input.NumberOfOptions, rpZones, input.CostCalculationMethods, input.RateCalculationCostColumnConfigId, input.RateCalculationMethod, input.CurrencyId, longPrecisionValue, normalPrecisionValue);
            routeOptionManager.SetZoneRouteOptionProperties(new List<ZoneItem>() { zoneItem });

            return zoneItem;
        }

        #endregion

        public bool SyncImportedDataWithDB(long processInstanceId, int? salePriceListId, SalePriceListOwnerType ownerType, int ownerId, int currencyId, DateTime effectiveOn, long stateBackupId)
        {
            var ratePlanDataManager = SalesDataManagerFactory.GetDataManager<IRatePlanDataManager>();
            return ratePlanDataManager.SyncImportedDataWithDB(processInstanceId, salePriceListId, ownerType, ownerId, currencyId, effectiveOn, stateBackupId);
        }

        public int GetOwnerSellingNumberPlanId(SalePriceListOwnerType ownerType, int ownerId)
        {
            int? sellingNumberPlanId = (ownerType == SalePriceListOwnerType.SellingProduct) ?
                new SellingProductManager().GetSellingNumberPlanId(ownerId) :
                new CarrierAccountManager().GetSellingNumberPlanId(ownerId, CarrierAccountType.Customer);
            if (!sellingNumberPlanId.HasValue)
                throw new NullReferenceException("sellingNumberPlanId");
            return sellingNumberPlanId.Value;
        }

        public RatePlanSettingsData GetRatePlanSettingsData()
        {
            object ratePlanSettingData = GetSettingData(Sales.Business.Constants.RatePlanSettingsType);
            var ratePlanSettings = ratePlanSettingData as RatePlanSettingsData;
            if (ratePlanSettings == null)
                throw new NullReferenceException("ratePlanSettings");
            return ratePlanSettings;
        }

        public SaleAreaSettingsData GetSaleAreaSettingsData()
        {
            object saleAreaSettingData = GetSettingData(BusinessEntity.Business.Constants.SaleAreaSettings);
            var saleAreaSettings = saleAreaSettingData as SaleAreaSettingsData;
            if (saleAreaSettings == null)
                throw new NullReferenceException("saleAreaSettings");
            return saleAreaSettings;
        }

        private object GetSettingData(string settingType)
        {
            var settingManager = new SettingManager();
            Setting setting = settingManager.GetSettingByType(settingType);
            if (setting == null)
                throw new NullReferenceException("setting");
            if (setting.Data == null)
                throw new NullReferenceException("setting.Data");
            return setting.Data;
        }

        public int? GetSellingProductId(SalePriceListOwnerType ownerType, int ownerId, DateTime effectiveOn, bool isEffectiveInFuture)
        {
            if (ownerType == SalePriceListOwnerType.Customer)
            {
                CustomerSellingProductManager customerSellingProductManager = new CustomerSellingProductManager();
                return customerSellingProductManager.GetEffectiveSellingProductId(ownerId, effectiveOn, isEffectiveInFuture); // Review what isEffectiveFuture does
            }
            else // If the owner is a selling product
                return ownerId;
        }

        public int GetSellingProductId(int customerId, DateTime effectiveOn, bool isEffectiveInFuture)
        {
            var customerSellingProductManager = new CustomerSellingProductManager();
            int? sellingProductId = customerSellingProductManager.GetEffectiveSellingProductId(customerId, effectiveOn, isEffectiveInFuture);
            if (!sellingProductId.HasValue)
                throw new NullReferenceException(String.Format("Customer '{0}' is not assigned to a SellingProduct", customerId));
            return sellingProductId.Value;
        }

        public SaleEntityZoneRate GetRate(SalePriceListOwnerType ownerType, int ownerId, long zoneId, DateTime effectiveOn)
        {
            var rateLocator = new SaleEntityZoneRateLocator(new SaleRateReadWithCache(effectiveOn));

            if (ownerType == SalePriceListOwnerType.SellingProduct)
                return rateLocator.GetSellingProductZoneRate(ownerId, zoneId);
            else
            {
                int? sellingProductId = GetSellingProductId(ownerType, ownerId, effectiveOn, false);
                if (sellingProductId == null)
                    throw new NullReferenceException("sellingProductId");
                return rateLocator.GetCustomerZoneRate(ownerId, sellingProductId.Value, zoneId);
            }
        }

        public OwnerInfo GetOwnerInfo(SalePriceListOwnerType ownerType, int ownerId, DateTime effectiveOn)
        {
            var ownerInfo = new OwnerInfo();

            var routingProductLocator = new SaleEntityZoneRoutingProductLocator(new SaleEntityRoutingProductReadWithCache(effectiveOn));
            var routingProductManager = new RoutingProductManager();

            SaleEntityZoneRoutingProduct defaultRoutingProduct;
            int? sellingProductId = null;

            if (ownerType == SalePriceListOwnerType.SellingProduct)
                defaultRoutingProduct = routingProductLocator.GetSellingProductDefaultRoutingProduct(ownerId);
            else
            {
                sellingProductId = new CarrierAccountManager().GetSellingProductId(ownerId);

                if (!sellingProductId.HasValue)
                    throw new Vanrise.Entities.DataIntegrityValidationException(string.Format("Customer '{0}' is not assigned to a Selling Product on '{1}'", ownerId, effectiveOn));
                ownerInfo.AssignedToSellingProductName = new SellingProductManager().GetSellingProductName(sellingProductId.Value);
                int currencyId = new SellingProductManager().GetSellingProductCurrencyId(sellingProductId.Value);
                ownerInfo.AssignedToSellingProductCurrencySymbol = new CurrencyManager().GetCurrencySymbol(currencyId);
                defaultRoutingProduct = routingProductLocator.GetCustomerDefaultRoutingProduct(ownerId, sellingProductId.Value);
            }

            if (defaultRoutingProduct != null)
            {
                ownerInfo.CurrentDefaultRoutingProductName = routingProductManager.GetRoutingProductName(defaultRoutingProduct.RoutingProductId);
                ownerInfo.IsCurrentDefaultRoutingProductInherited = (ownerType == SalePriceListOwnerType.Customer && defaultRoutingProduct.Source != SaleEntityZoneRoutingProductSource.CustomerDefault);
            }

            Changes draft = new RatePlanDraftManager().GetDraft(ownerType, ownerId);

            if (draft != null && draft.DefaultChanges != null)
            {
                if (draft.DefaultChanges.NewDefaultRoutingProduct != null)
                    ownerInfo.NewDefaultRoutingProductName = routingProductManager.GetRoutingProductName(draft.DefaultChanges.NewDefaultRoutingProduct.DefaultRoutingProductId);
                else if (draft.DefaultChanges.DefaultRoutingProductChange != null)
                {
                    SaleEntityZoneRoutingProduct sellingProductDefaultRoutingProduct = routingProductLocator.GetSellingProductDefaultRoutingProduct(sellingProductId.Value);
                    ownerInfo.ResetToDefaultRoutingProductName = routingProductManager.GetRoutingProductName(sellingProductDefaultRoutingProduct.RoutingProductId);
                }
            }

            return ownerInfo;
        }

        public string GetSystemDateTimeFormat(Vanrise.Entities.DateTimeType dateTimeFormat)
        {
            return Vanrise.Common.Utilities.GetDateTimeFormat(dateTimeFormat);
        }

        public byte[] GetImportTemplateFileWithSystemDateFormat(byte[] buffer)
        {
            Vanrise.Common.Utilities.ActivateAspose();
            MemoryStream stream = new MemoryStream(buffer);
            Workbook workbook = new Workbook(stream);
            Worksheet worksheet = workbook.Worksheets[0];
            string cellValue = worksheet.Cells[0, 2].StringValue;
            string systemDateFormat = Vanrise.Common.Utilities.GetDateTimeFormat(Vanrise.Entities.DateTimeType.Date);
            worksheet.Cells[0, 2].PutValue(cellValue + " (" + systemDateFormat + ")");

            byte[] array;

            using (MemoryStream ms = new MemoryStream())
            {
                workbook.Save(ms, SaveFormat.Xlsx);
                array = ms.ToArray();
            }
            return array;
        }

        #endregion

        #region Private Methods

        public IEnumerable<SaleZone> GetSaleZones(SalePriceListOwnerType ownerType, int ownerId, DateTime effectiveAfter, bool isEffectiveInFuture)
        {
            if (ownerType == SalePriceListOwnerType.SellingProduct)
            {
                int? sellingNumberPlanId = new SellingProductManager().GetSellingNumberPlanId(ownerId);
                if (!sellingNumberPlanId.HasValue)
                    throw new NullReferenceException("sellingNumberPlanId");
                return GetSellingProductZones(sellingNumberPlanId.Value, effectiveAfter);
            }
            else
            {
                int sellingNumberPlanId = new CarrierAccountManager().GetSellingNumberPlanId(ownerId, CarrierAccountType.Customer);
                Dictionary<int, CountryDateConfig> countryDateConfigsByCountryId = GetCountryDateConfigsByCountryId(ownerId, effectiveAfter);
                return GetCustomerZones(sellingNumberPlanId, countryDateConfigsByCountryId, effectiveAfter);
            }
        }
        private IEnumerable<SaleZone> GetCustomerDraftSaleZones(int customerId, int sellingNumberPlanId, DateTime effectiveOn, bool includeFutureZones)
        {
            var draftManager = new RatePlanDraftManager();
            Changes draft = draftManager.GetDraft(SalePriceListOwnerType.Customer, customerId);

            if (draft == null || draft.CountryChanges == null || draft.CountryChanges.NewCountries == null)
                return null;

            var saleZoneManager = new SaleZoneManager();
            IEnumerable<int> newCountryIds = draft.CountryChanges.NewCountries.MapRecords(x => x.CountryId, x => x.IsEffectiveOrFuture(effectiveOn));
            return saleZoneManager.GetSaleZonesByCountryIds(sellingNumberPlanId, newCountryIds, effectiveOn, includeFutureZones);
        }
        public bool SaleZoneFilter(SaleZone saleZone, IEnumerable<int> countryIds, Vanrise.Entities.TextFilterType? zoneNameFilterType, string zoneNameFilter)
        {
            if (countryIds != null && !countryIds.Contains(saleZone.CountryId))
                return false;
            if (saleZone.Name == null || saleZone.Name.Length == 0)
                return false;
            if (zoneNameFilterType.HasValue && !Vanrise.Common.Utilities.IsTextMatched(saleZone.Name, zoneNameFilter, zoneNameFilterType.Value))
                return false;
            return true;
        }

        #endregion

        #region Bulk Action Members

        public IEnumerable<ZoneItem> GetZoneItems(GetZoneItemsInput input)
        {
            if (input == null)
                throw new Vanrise.Entities.MissingArgumentValidationException("input");

            if (input.Filter == null)
                throw new Vanrise.Entities.MissingArgumentValidationException("input.Filter");

            // Get the sale zones
            IEnumerable<SaleZone> saleZones = GetSaleZones(input.OwnerType, input.OwnerId, input.EffectiveOn, true);
            if (saleZones == null)
                return null;

            Changes draft = new RatePlanDraftManager().GetDraft(input.OwnerType, input.OwnerId);

            int sellingProductId;
            Dictionary<int, DateTime> countryBEDsByCountryId;
            ISaleRateReader currentRateReader = GetCurrentRateReader(input.OwnerType, input.OwnerId, saleZones, input.EffectiveOn, out sellingProductId, out countryBEDsByCountryId);

            SaleEntityZoneRateLocator currentRateLocator;
            Func<int, long, SaleEntityZoneRate> getSellingProductZoneRate;
            Func<int, int, long, SaleEntityZoneRate> getCustomerZoneRate;
            UtilitiesManager.SetBulkActionContextCurrentRateHelpers(currentRateReader, out currentRateLocator, out getSellingProductZoneRate, out getCustomerZoneRate);

            // Filter and page the sale zones
            IEnumerable<long> applicableZoneIds = null;

            if (input.BulkAction != null)
            {
                if (input.Filter.BulkActionFilter == null)
                    throw new Vanrise.Entities.MissingArgumentValidationException("input.Filter.BulkActionFilter");

                var applicableZoneContext = new ApplicableZoneIdsContext(getSellingProductZoneRate, getCustomerZoneRate)
                {
                    OwnerType = input.OwnerType,
                    OwnerId = input.OwnerId,
                    SaleZones = saleZones,
                    DraftData = draft,
                    BulkAction = input.BulkAction
                };
                applicableZoneIds = input.Filter.BulkActionFilter.GetApplicableZoneIds(applicableZoneContext);
            }

            Func<SaleZone, bool> filterFunc = (saleZone) =>
            {
                if (input.Filter.ExcludedZoneIds != null && input.Filter.ExcludedZoneIds.Contains(saleZone.SaleZoneId))
                    return false;

                if (applicableZoneIds != null && !applicableZoneIds.Contains(saleZone.SaleZoneId))
                    return false;

                if (!SaleZoneFilter(saleZone, input.Filter.CountryIds, input.Filter.ZoneNameFilterType, input.Filter.ZoneNameFilter))
                    return false;

                if (input.Filter.ZoneLetter.HasValue)
                {
                    if (char.ToLower(saleZone.Name.ElementAt(0)) != char.ToLower(input.Filter.ZoneLetter.Value))
                        return false;
                }

                return true;
            };

            IEnumerable<SaleZone> filteredSaleZones = saleZones.FindAllRecords(filterFunc);
            IEnumerable<SaleZone> pagedSaleZones = GetPagedZones(filteredSaleZones, input.Filter.FromRow, input.Filter.ToRow);

            if (pagedSaleZones == null || pagedSaleZones.Count() == 0)
                return null;

            SaleEntityZoneRoutingProductLocator routingProductLocator = new SaleEntityZoneRoutingProductLocator(new SaleEntityRoutingProductReadWithCache(input.EffectiveOn));

            var ratePlanZoneCreationInput = new RatePlanZoneCreationInput()
            {
                SaleZones = pagedSaleZones,
                OwnerType = input.OwnerType,
                OwnerId = input.OwnerId,
                SellingProductId = sellingProductId,
                EffectiveOn = input.EffectiveOn,
                CurrencyId = input.CurrencyId,
                RoutingDatabaseId = input.RoutingDatabaseId,
                PolicyConfigId = input.PolicyConfigId,
                NumberOfOptions = input.NumberOfOptions,
                CostCalculationMethods = input.CostCalculationMethods,
                BulkAction = input.BulkAction,
                Draft = draft,
                CountryBEDsByCountryId = countryBEDsByCountryId,
                RoutingProductLocator = routingProductLocator,
                CurrentRateLocator = currentRateLocator
            };

            return BuildZoneItems(ratePlanZoneCreationInput);
        }
        public BulkActionValidationResult ValidateBulkActionZones(BulkActionZoneValidationInput input)
        {
            if (input.BulkAction == null)
                throw new Vanrise.Entities.MissingArgumentValidationException("BulkAction was not passed");

            if (input.BulkActionZoneFilter == null)
                throw new Vanrise.Entities.MissingArgumentValidationException("BulkActionZoneFilter was not passed");

            // Get the sale zones
            IEnumerable<SaleZone> saleZones = GetSaleZones(input.OwnerType, input.OwnerId, input.EffectiveOn, true);
            if (saleZones == null)
                return null;

            Changes draft;
            IEnumerable<int> newCountryIds;
            IEnumerable<int> changedCountryIds;
            Dictionary<long, ZoneChanges> zoneDraftsByZoneId;
            SetDraftVariables(input.OwnerType, input.OwnerId, out draft, out zoneDraftsByZoneId, out newCountryIds, out changedCountryIds);

            // Filter and page the sale zones
            IEnumerable<long> applicableZoneIds = null;

            int sellingProductId;
            Dictionary<int, DateTime> countryBEDsByCountryId;
            ISaleRateReader currentRateReader = GetCurrentRateReader(input.OwnerType, input.OwnerId, saleZones, input.EffectiveOn, out sellingProductId, out countryBEDsByCountryId);

            SaleEntityZoneRateLocator currentRateLocator;
            Func<int, long, SaleEntityZoneRate> getSellingProductZoneRate;
            Func<int, int, long, SaleEntityZoneRate> getCustomerZoneRate;
            UtilitiesManager.SetBulkActionContextCurrentRateHelpers(currentRateReader, out currentRateLocator, out getSellingProductZoneRate, out getCustomerZoneRate);

            var applicableZoneContext = new ApplicableZoneIdsContext(getSellingProductZoneRate, getCustomerZoneRate)
            {
                OwnerType = input.OwnerType,
                OwnerId = input.OwnerId,
                SaleZones = saleZones,
                DraftData = draft,
                BulkAction = input.BulkAction
            };

            applicableZoneIds = input.BulkActionZoneFilter.GetApplicableZoneIds(applicableZoneContext);

            if (applicableZoneIds == null || applicableZoneIds.Count() == 0)
                return null;

            BulkActionValidationResult validationResult = null;
            IEnumerable<SaleZone> applicableSaleZones = saleZones.FindAllRecords(x => applicableZoneIds.Contains(x.SaleZoneId));

            int normalPrecisionValue, longPrecisionValue;
            SetNumberPrecisionValues(out normalPrecisionValue, out longPrecisionValue);

            var rateLocator = new SaleEntityZoneRateLocator(currentRateReader);
            var rateManager = new ZoneRateManager(input.OwnerType, input.OwnerId, sellingProductId, input.EffectiveOn, draft, input.CurrencyId, longPrecisionValue, rateLocator);

            var zoneRPLocator = new SaleEntityZoneRoutingProductLocator(new SaleEntityRoutingProductReadWithCache(input.EffectiveOn));
            var routingProductManager = new ZoneRPManager(input.OwnerType, input.OwnerId, input.EffectiveOn, draft, zoneRPLocator);
            var saleRateManager = new SaleRateManager();

            Dictionary<long, ZoneItem> contextZoneItemsByZoneId = null;

            Func<Dictionary<long, ZoneItem>> getContextZoneItems = () =>
            {
                if (contextZoneItemsByZoneId == null)
                {
                    var setContextZoneItemsInput = new ContextZoneItemInput()
                    {
                        OwnerType = input.OwnerType,
                        OwnerId = input.OwnerId,
                        SaleZones = saleZones,
                        Draft = draft,
                        ZoneDraftsByZoneId = zoneDraftsByZoneId,
                        SellingProductId = sellingProductId,
                        NewCountryIds = newCountryIds,
                        ChangedCountryIds = changedCountryIds,
                        EffectiveOn = input.EffectiveOn,
                        CountryBEDsByCountryId = countryBEDsByCountryId,
                        RoutingDatabaseId = input.RoutingDatabaseId,
                        PolicyConfigId = input.PolicyConfigId,
                        NumberOfOptions = null,
                        CostCalculationMethods = input.CostCalculationMethods,
                        CurrencyId = input.CurrencyId,
                        LongPrecisionValue = longPrecisionValue,
                        NormalPrecisionValue = normalPrecisionValue,
                        RateManager = rateManager,
                        RoutingProductManager = routingProductManager,
                        SaleRateManager = saleRateManager
                    };
                    SetContextZoneItems(ref contextZoneItemsByZoneId, setContextZoneItemsInput);
                }
                return contextZoneItemsByZoneId;
            };

            Func<decimal, decimal> getRoundedRate = (rate) =>
            {
                return decimal.Round(rate, longPrecisionValue);
            };

            foreach (SaleZone applicableSaleZone in applicableSaleZones)
            {
                var validationContext = new ZoneValidationContext(getContextZoneItems, input.CostCalculationMethods, getRoundedRate)
                {
                    ZoneId = applicableSaleZone.SaleZoneId,
                    ValidationResult = validationResult
                };
                input.BulkAction.ValidateZone(validationContext);
                validationResult = validationContext.ValidationResult;
            }

            return validationResult;
        }
        public void ApplyBulkActionToDraft(ApplyActionToDraftInput input)
        {
            // Get the sale zones
            IEnumerable<SaleZone> saleZones = GetSaleZones(input.OwnerType, input.OwnerId, input.EffectiveOn, true);

            var ratePlanDraftManager = new RatePlanDraftManager();
            Changes draft = ratePlanDraftManager.GetDraft(input.OwnerType, input.OwnerId);

            int sellingProductId;
            Dictionary<int, DateTime> countryBEDsByCountryId;
            ISaleRateReader currentRateReader = GetCurrentRateReader(input.OwnerType, input.OwnerId, saleZones, input.EffectiveOn, out sellingProductId, out countryBEDsByCountryId);

            SaleEntityZoneRateLocator currentRateLocator;
            Func<int, long, SaleEntityZoneRate> getSellingProductZoneRate;
            Func<int, int, long, SaleEntityZoneRate> getCustomerZoneRate;
            UtilitiesManager.SetBulkActionContextCurrentRateHelpers(currentRateReader, out currentRateLocator, out getSellingProductZoneRate, out getCustomerZoneRate);

            var pricingSettings = UtilitiesManager.GetPricingSettings(input.OwnerType, input.OwnerId);

            // Filter the sale zones by applicable zones
            var applicableZoneIdsContext = new ApplicableZoneIdsContext(getSellingProductZoneRate, getCustomerZoneRate)
            {
                OwnerType = input.OwnerType,
                OwnerId = input.OwnerId,
                SaleZones = saleZones,
                DraftData = draft,
                BulkAction = input.BulkAction
            };

            IEnumerable<long> applicableZoneIds = input.BulkActionFilter.GetApplicableZoneIds(applicableZoneIdsContext);

            Func<SaleZone, bool> filterFunc = (saleZone) =>
            {
                if (applicableZoneIds != null && !applicableZoneIds.Contains(saleZone.SaleZoneId))
                    return false;

                return true;
            };

            IEnumerable<SaleZone> filteredSaleZones = saleZones.FindAllRecords(filterFunc);

            IEnumerable<ZoneChanges> existingZoneDrafts = new List<ZoneChanges>();
            if (draft != null && draft.ZoneChanges != null)
                existingZoneDrafts = draft.ZoneChanges.FindAllRecords(x => applicableZoneIds.Contains(x.ZoneId));

            SaleEntityZoneRoutingProductLocator routingProductLocator = null;
            IEnumerable<ZoneItem> zoneItems = null;

            Func<IEnumerable<ZoneItem>> buildZoneItems = () =>
            {
                if (zoneItems == null)
                {
                    if (routingProductLocator == null)
                        routingProductLocator = new SaleEntityZoneRoutingProductLocator(new SaleEntityRoutingProductReadWithCache(input.EffectiveOn));

                    var ratePlanZoneCreationInput = new RatePlanZoneCreationInput()
                    {
                        SaleZones = filteredSaleZones,
                        OwnerType = input.OwnerType,
                        OwnerId = input.OwnerId,
                        SellingProductId = sellingProductId,
                        EffectiveOn = input.EffectiveOn,
                        CurrencyId = input.CurrencyId,
                        RoutingDatabaseId = input.RoutingDatabaseId,
                        PolicyConfigId = input.PolicyConfigId,
                        NumberOfOptions = null,
                        CostCalculationMethods = input.CostCalculationMethods,
                        BulkAction = null,
                        Draft = draft,
                        CountryBEDsByCountryId = countryBEDsByCountryId,
                        RoutingProductLocator = routingProductLocator,
                        CurrentRateLocator = currentRateLocator
                    };

                    zoneItems = BuildZoneItems(ratePlanZoneCreationInput);
                }

                return zoneItems;
            };

            int longPrecision = new Vanrise.Common.Business.GeneralSettingsManager().GetLongPrecisionValue();
            Func<decimal, decimal> getRoundedRate = (rate) =>
            {
                return decimal.Round(rate, longPrecision);
            };

            var applyBulkActionToDraftContext = new ApplyBulkActionToZoneDraftContext(buildZoneItems, input.CostCalculationMethods, getRoundedRate)
            {
                OwnerId = input.OwnerId,
                OwnerType = input.OwnerType,
                NewRateDayOffset = pricingSettings.NewRateDayOffset.Value,
                IncreasedRateDayOffset = pricingSettings.IncreasedRateDayOffset.Value,
                DecreasedRateDayOffset = pricingSettings.DecreasedRateDayOffset.Value
            };

            var newDraft = new Changes()
            {
                CurrencyId = input.CurrencyId,
                ZoneChanges = new List<ZoneChanges>(),
                DefaultChanges = new DefaultChanges()
            };

            if (draft != null)
            {
                if (draft.DefaultChanges != null)
                    newDraft.DefaultChanges = draft.DefaultChanges;
                if (draft.CountryChanges != null)
                    newDraft.CountryChanges = draft.CountryChanges;
            }

            var saleZoneManager = new SaleZoneManager();

            Func<long, ZoneChanges> getZoneDraft = (zoneId) =>
            {
                ZoneChanges zoneDraft = existingZoneDrafts.FindRecord(x => x.ZoneId == zoneId);
                if (zoneDraft == null)
                {
                    SaleZone zone = saleZoneManager.GetSaleZone(zoneId);
                    zoneDraft = new ZoneChanges()
                    {
                        ZoneId = zoneId,
                        ZoneName = zone.Name,
                        CountryId = zone.CountryId
                    };
                }
                newDraft.ZoneChanges.Add(zoneDraft);
                return zoneDraft;
            };

            foreach (SaleZone zone in filteredSaleZones)
            {
                if (input.ExcludedZoneIds != null && input.ExcludedZoneIds.Contains(zone.SaleZoneId))
                    continue;
                applyBulkActionToDraftContext.ZoneDraft = getZoneDraft(zone.SaleZoneId);
                input.BulkAction.ApplyBulkActionToZoneDraft(applyBulkActionToDraftContext);
            }

            Func<SaleEntityZoneRoutingProduct> getCustomerDefaultRoutingProduct = () =>
            {
                if (input.OwnerType == SalePriceListOwnerType.SellingProduct)
                    return null;

                if (routingProductLocator == null)
                    routingProductLocator = new SaleEntityZoneRoutingProductLocator(new SaleEntityRoutingProductReadWithCache(input.EffectiveOn));

                return routingProductLocator.GetCustomerDefaultRoutingProduct(input.OwnerId, sellingProductId);
            };

            if (input.BulkActionCorrectedData != null)
            {
                var applyCorrectedDataContext = new ApplyCorrectedDataContext(getZoneDraft)
                {
                    OwnerType = input.OwnerType,
                    OwnerId = input.OwnerId,
                    CorrectedData = input.BulkActionCorrectedData,
                    NewRateDayOffset = pricingSettings.NewRateDayOffset.Value,
                    IncreasedRateDayOffset = pricingSettings.IncreasedRateDayOffset.Value,
                    DecreasedRateDayOffset = pricingSettings.DecreasedRateDayOffset.Value
                };
                input.BulkAction.ApplyCorrectedData(applyCorrectedDataContext);
            }

            var applyBulkActionToDefaultDraftContext = new ApplyBulkActionToDefaultDraftContext(getCustomerDefaultRoutingProduct) { DefaultDraft = newDraft.DefaultChanges };
            input.BulkAction.ApplyBulkActionToDefaultDraft(applyBulkActionToDefaultDraftContext);

            ratePlanDraftManager.SaveDraft(input.OwnerType, input.OwnerId, newDraft);
        }
        public ImportedDataValidationResult ValidateImportedData(ImportedDataValidationInput input)
        {
            IEnumerable<string> worksheetHeaders;
            IEnumerable<ImportedRow> importedRows = GetImportedRows(input.FileId, input.HeaderRowExists, out worksheetHeaders);

            Dictionary<string, SaleZone> saleZonesByName = GetSaleZonesEffectiveAfterByName(input.OwnerType, input.OwnerId, DateTime.Today);
            Dictionary<string, ZoneChanges> zoneDraftsByZoneName = GetZoneDraftsByZoneName(input.OwnerType, input.OwnerId);

            var countryBEDsByCountry = new Dictionary<int, DateTime>();
            IEnumerable<int> closedCountryIds = new List<int>();

            if (input.OwnerType == SalePriceListOwnerType.Customer)
            {
                countryBEDsByCountry = UtilitiesManager.GetDatesByCountry(input.OwnerId, DateTime.Today, true);
                closedCountryIds = UtilitiesManager.GetClosedCountryIds(input.OwnerId, null, DateTime.Today);
            }

            var result = new ImportedDataValidationResult();
            var validator = new ImportedRowValidator();

            // Validate the data of the entire file
            var importedFileValidationContext = new ImportedFileValidationContext()
            {
                ImportedRows = importedRows,
                SaleZonesByZoneName = saleZonesByName
            };

            validator.ValidateImportedFile(importedFileValidationContext);

            if (importedFileValidationContext.InvalidImportedRows != null)
            {
                foreach (InvalidImportedRow invalidImportedRow in importedFileValidationContext.InvalidImportedRows)
                    result.InvalidDataByRowIndex.Add(invalidImportedRow.RowIndex, invalidImportedRow);
            }

            // Validate the data of each zone
            for (int i = 0; i < importedRows.Count(); i++)
            {
                ImportedRow importedRow = importedRows.ElementAt(i);
                string zoneName = (importedRow.Zone != null) ? importedRow.Zone.ToLower() : null;

                if (zoneName != null && importedFileValidationContext.InvalidImportedRows.FindRecord(x => x.ImportedRow.Zone.ToLower() == zoneName) != null)
                    continue;

                var context = new IsImportedRowValidContext()
                {
                    OwnerType = input.OwnerType,
                    ImportedRow = importedRow,
                    ExistingZone = saleZonesByName.GetRecord(zoneName),
                    ZoneDraft = zoneDraftsByZoneName.GetRecord(zoneName),
                    CountryBEDsByCountry = countryBEDsByCountry,
                    ClosedCountryIds = closedCountryIds,
                    DateTimeFormat = input.DateTimeFormat
                };

                if (validator.IsImportedRowValid(context))
                {
                    result.ValidDataByZoneId.Add(context.ExistingZone.SaleZoneId, importedRow);
                }
                else
                {
                    result.InvalidDataByRowIndex.Add(i, new InvalidImportedRow()
                    {
                        RowIndex = i,
                        ImportedRow = importedRow,
                        ErrorMessage = context.ErrorMessage
                    });
                }
            }

            return result;
        }

        private IEnumerable<ZoneItem> BuildZoneItems(RatePlanZoneCreationInput input)
        {
            var zoneItems = new List<ZoneItem>();

            var zoneDraftsByZone = new Dictionary<long, ZoneChanges>();
            IEnumerable<int> newCountryIds = new List<int>();

            IEnumerable<int> closedCountryIds = new List<int>();
            if (input.OwnerType == SalePriceListOwnerType.Customer)
                closedCountryIds = UtilitiesManager.GetClosedCountryIds(input.OwnerId, input.Draft, input.EffectiveOn);

            if (input.Draft != null)
            {
                zoneDraftsByZone = SturctureZoneDraftsByZone(input.Draft.ZoneChanges);

                if (input.Draft.CountryChanges != null)
                {
                    if (input.Draft.CountryChanges.NewCountries != null)
                        newCountryIds = input.Draft.CountryChanges.NewCountries.MapRecords(x => x.CountryId);
                }
            }

            int normalPrecisionValue, longPrecisionValue;
            SetNumberPrecisionValues(out normalPrecisionValue, out longPrecisionValue);

            var baseRatesByZone = new BaseRatesByZone();
            var saleRateManager = new SaleRateManager();

            var rateManager = new ZoneRateManager(input.OwnerType, input.OwnerId, input.SellingProductId, input.EffectiveOn, input.Draft, input.CurrencyId, longPrecisionValue, input.CurrentRateLocator);
            var rpManager = new ZoneRPManager(input.OwnerType, input.OwnerId, input.EffectiveOn, input.Draft, input.RoutingProductLocator);

            var pricingSettings = UtilitiesManager.GetPricingSettings(input.OwnerType, input.OwnerId);

            ZoneRouteOptionManager routeOptionManager;

            Dictionary<long, ZoneItem> contextZoneItemsByZoneId = null;

            Func<Dictionary<long, ZoneItem>> getContextZoneItems = () =>
            {
                if (contextZoneItemsByZoneId == null)
                {
                    var setContextZoneItemsInput = new ContextZoneItemInput()
                    {
                        OwnerType = input.OwnerType,
                        OwnerId = input.OwnerId,
                        SaleZones = input.SaleZones,
                        Draft = input.Draft,
                        ZoneDraftsByZoneId = zoneDraftsByZone,
                        SellingProductId = input.SellingProductId.Value,
                        NewCountryIds = newCountryIds,
                        ChangedCountryIds = closedCountryIds,
                        EffectiveOn = input.EffectiveOn,
                        CountryBEDsByCountryId = input.CountryBEDsByCountryId,
                        RoutingDatabaseId = input.RoutingDatabaseId,
                        PolicyConfigId = input.PolicyConfigId,
                        NumberOfOptions = null,
                        CostCalculationMethods = input.CostCalculationMethods,
                        CurrencyId = input.CurrencyId,
                        LongPrecisionValue = longPrecisionValue,
                        NormalPrecisionValue = normalPrecisionValue,
                        RateManager = rateManager,
                        RoutingProductManager = rpManager,
                        SaleRateManager = saleRateManager
                    };
                    SetContextZoneItems(ref contextZoneItemsByZoneId, setContextZoneItemsInput);
                }
                return contextZoneItemsByZoneId;
            };

            Func<long, SaleEntityZoneRoutingProduct> getSellingProductZoneRoutingProduct = (zoneId) =>
            {
                return input.RoutingProductLocator.GetSellingProductZoneRoutingProduct(input.SellingProductId.Value, zoneId);
            };

            Func<decimal, decimal> getRoundedRate = (rate) =>
            {
                return decimal.Round(rate, longPrecisionValue);
            };

            foreach (SaleZone saleZone in input.SaleZones)
            {
                var zoneItem = new ZoneItem()
                {
                    ZoneId = saleZone.SaleZoneId,
                    ZoneName = saleZone.Name,
                    CountryId = saleZone.CountryId,
                    ZoneBED = saleZone.BED,
                    ZoneEED = saleZone.EED,
                    TargetCurrencyId = input.CurrencyId
                };

                DateTime countryBED;
                if (input.CountryBEDsByCountryId.TryGetValue(saleZone.CountryId, out countryBED))
                    zoneItem.CountryBED = countryBED;

                ZoneChanges zoneDraft = zoneDraftsByZone.GetRecord(saleZone.SaleZoneId);

                if (input.BulkAction != null)
                {
                    var applyBulkActionToZoneItemContext = new ApplyBulkActionToZoneItemContext(getContextZoneItems, input.CostCalculationMethods, getSellingProductZoneRoutingProduct, getRoundedRate)
                    {
                        OwnerId = input.OwnerId,
                        OwnerType = input.OwnerType,
                        ZoneItem = zoneItem,
                        ZoneDraft = zoneDraft,
                        NewRateDayOffset = pricingSettings.NewRateDayOffset.Value,
                        IncreasedRateDayOffset = pricingSettings.IncreasedRateDayOffset.Value,
                        DecreasedRateDayOffset = pricingSettings.DecreasedRateDayOffset.Value
                    };
                    input.BulkAction.ApplyBulkActionToZoneItem(applyBulkActionToZoneItemContext);
                }

                zoneItem.IsFutureZone = (zoneItem.ZoneBED.Date > DateTime.Now.Date);

                rateManager.SetZoneRate(zoneItem);

                // TODO: Refactor rateManager to handle products and customers separately
                if (input.OwnerType == SalePriceListOwnerType.SellingProduct)
                {
                    rpManager.SetSellingProductZoneRP(zoneItem, input.OwnerId, zoneDraft);
                }
                else
                {
                    AddZoneBaseRates(baseRatesByZone, zoneItem); // Check if the customer zone has any inherited rate
                    rpManager.SetCustomerZoneRP(zoneItem, input.OwnerId, input.SellingProductId.Value, zoneDraft);
                }

                zoneItem.IsCountryNew = newCountryIds.Contains(zoneItem.CountryId);
                zoneItem.IsCountryEnded = closedCountryIds.Contains(zoneItem.CountryId);

                zoneItems.Add(zoneItem);
            }

            if (input.OwnerType == SalePriceListOwnerType.Customer)
            {
                IEnumerable<DraftNewCountry> draftNewCountries = (input.Draft != null && input.Draft.CountryChanges != null) ? input.Draft.CountryChanges.NewCountries : null;
                IEnumerable<CustomerCountry2> soldCountries = GetSoldCountries(input.OwnerId, input.EffectiveOn, false, draftNewCountries);
                saleRateManager.ProcessBaseRatesByZone(input.OwnerId, baseRatesByZone, soldCountries);
            }

            IEnumerable<RPZone> rpZones = zoneItems.MapRecords(x => new RPZone() { SaleZoneId = x.ZoneId, RoutingProductId = x.EffectiveRoutingProductId.Value }, x => x.EffectiveRoutingProductId.HasValue);
            routeOptionManager = new ZoneRouteOptionManager(input.OwnerType, input.OwnerId, input.RoutingDatabaseId, input.PolicyConfigId, input.NumberOfOptions, rpZones, input.CostCalculationMethods, null, null, input.CurrencyId, longPrecisionValue, normalPrecisionValue);
            routeOptionManager.SetZoneRouteOptionProperties(zoneItems);

            return zoneItems;
        }
        private void SetContextZoneItems(ref Dictionary<long, ZoneItem> contextZoneItems, ContextZoneItemInput input)
        {
            contextZoneItems = new Dictionary<long, ZoneItem>();
            var baseRatesByZone = new BaseRatesByZone();

            foreach (SaleZone saleZone in input.SaleZones)
            {
                var contextZoneItem = new ZoneItem()
                {
                    ZoneId = saleZone.SaleZoneId,
                    ZoneName = saleZone.Name,
                    CountryId = saleZone.CountryId,
                    ZoneBED = saleZone.BED,
                    ZoneEED = saleZone.EED,
                    TargetCurrencyId = input.CurrencyId
                };

                DateTime countryBED;
                if (input.CountryBEDsByCountryId.TryGetValue(saleZone.CountryId, out countryBED))
                    contextZoneItem.CountryBED = countryBED;

                ZoneChanges zoneDraft = input.ZoneDraftsByZoneId.GetRecord(saleZone.SaleZoneId);

                contextZoneItem.IsFutureZone = (contextZoneItem.ZoneBED.Date > DateTime.Now.Date);

                input.RateManager.SetZoneRate(contextZoneItem);

                if (input.OwnerType == SalePriceListOwnerType.SellingProduct)
                {
                    input.RoutingProductManager.SetSellingProductZoneRP(contextZoneItem, input.OwnerId, zoneDraft);
                }
                else
                {
                    AddZoneBaseRates(baseRatesByZone, contextZoneItem); // Check if the customer zone has any inherited rate
                    input.RoutingProductManager.SetCustomerZoneRP(contextZoneItem, input.OwnerId, input.SellingProductId, zoneDraft);
                }

                contextZoneItem.IsCountryNew = input.NewCountryIds.Contains(contextZoneItem.CountryId);
                contextZoneItem.IsCountryEnded = input.ChangedCountryIds.Contains(contextZoneItem.CountryId);

                contextZoneItems.Add(contextZoneItem.ZoneId, contextZoneItem);
            }

            if (input.OwnerType == SalePriceListOwnerType.Customer)
            {
                IEnumerable<DraftNewCountry> draftNewCountries = (input.Draft != null && input.Draft.CountryChanges != null) ? input.Draft.CountryChanges.NewCountries : null;
                IEnumerable<CustomerCountry2> soldCountries = GetSoldCountries(input.OwnerId, input.EffectiveOn, false, draftNewCountries);
                input.SaleRateManager.ProcessBaseRatesByZone(input.OwnerId, baseRatesByZone, soldCountries);
            }

            IEnumerable<RPZone> contextRPZones = contextZoneItems.MapRecords(x => new RPZone() { SaleZoneId = x.ZoneId, RoutingProductId = x.EffectiveRoutingProductId.Value }, x => x.EffectiveRoutingProductId.HasValue);
            ZoneRouteOptionManager routeOptionManager = new ZoneRouteOptionManager(input.OwnerType, input.OwnerId, input.RoutingDatabaseId, input.PolicyConfigId, input.NumberOfOptions, contextRPZones, input.CostCalculationMethods, null, null, input.CurrencyId, input.LongPrecisionValue, input.NormalPrecisionValue);
            routeOptionManager.SetZoneRouteOptionProperties(contextZoneItems.Values);
        }
        private void SetDraftVariables(SalePriceListOwnerType ownerType, int ownerId, out Changes draft, out Dictionary<long, ZoneChanges> zoneDraftsByZoneId, out IEnumerable<int> newCountryIds, out IEnumerable<int> changedCountryIds)
        {
            draft = new RatePlanDraftManager().GetDraft(ownerType, ownerId);
            zoneDraftsByZoneId = new Dictionary<long, ZoneChanges>();
            newCountryIds = new List<int>();
            changedCountryIds = new List<int>();

            if (draft != null)
            {
                zoneDraftsByZoneId = SturctureZoneDraftsByZone(draft.ZoneChanges);

                if (draft.CountryChanges != null)
                {
                    if (draft.CountryChanges.NewCountries != null)
                        newCountryIds = draft.CountryChanges.NewCountries.MapRecords(x => x.CountryId);

                    if (draft.CountryChanges.ChangedCountries != null && draft.CountryChanges.ChangedCountries.Countries != null)
                        changedCountryIds = draft.CountryChanges.ChangedCountries.Countries.MapRecords(x => x.CountryId);
                }
            }
        }
        private Dictionary<long, ZoneChanges> SturctureZoneDraftsByZone(IEnumerable<ZoneChanges> zoneDrafts)
        {
            var zoneDraftsByZone = new Dictionary<long, ZoneChanges>();
            if (zoneDrafts != null)
            {
                foreach (ZoneChanges zoneDraft in zoneDrafts)
                {
                    if (!zoneDraftsByZone.ContainsKey(zoneDraft.ZoneId))
                        zoneDraftsByZone.Add(zoneDraft.ZoneId, zoneDraft);
                }
            }
            return zoneDraftsByZone;
        }
        private ISaleRateReader GetCurrentRateReader(SalePriceListOwnerType ownerType, int ownerId, IEnumerable<SaleZone> saleZones, DateTime effectiveOn, out int sellingProductId, out Dictionary<int, DateTime> countryBEDsByCountryId)
        {
            ISaleRateReader currentRateReader;

            int sellingProductIdValue;
            Dictionary<int, DateTime> countryBEDsByCountryIdValue;

            if (ownerType == SalePriceListOwnerType.SellingProduct)
            {
                sellingProductIdValue = ownerId;
                countryBEDsByCountryIdValue = new Dictionary<int, DateTime>();
                currentRateReader = new SaleRateReadWithCache(effectiveOn);
            }
            else
            {
                sellingProductIdValue = new CarrierAccountManager().GetSellingProductId(ownerId);
                IEnumerable<long> zoneIds = saleZones.MapRecords(x => x.SaleZoneId);
                countryBEDsByCountryIdValue = UtilitiesManager.GetDatesByCountry(ownerId, effectiveOn, true);
                Dictionary<long, DateTime> zoneEffectiveDatesByZoneId = UtilitiesManager.GetZoneEffectiveDatesByZoneId(saleZones, countryBEDsByCountryIdValue);
                currentRateReader = new SaleRateReadRPChanges(ownerId, sellingProductIdValue, zoneIds, DateTime.Today, zoneEffectiveDatesByZoneId);
            }

            sellingProductId = sellingProductIdValue;
            countryBEDsByCountryId = countryBEDsByCountryIdValue;
            return currentRateReader;
        }

        public class RatePlanZoneCreationInput
        {
            public IEnumerable<SaleZone> SaleZones { get; set; }
            public SalePriceListOwnerType OwnerType { get; set; }
            public int OwnerId { get; set; }
            public int? SellingProductId { get; set; }
            public DateTime EffectiveOn { get; set; }
            public int CurrencyId { get; set; }
            public int RoutingDatabaseId { get; set; }
            public Guid PolicyConfigId { get; set; }
            public int? NumberOfOptions { get; set; }
            public List<CostCalculationMethod> CostCalculationMethods { get; set; }
            public BulkActionType BulkAction { get; set; }
            public Changes Draft { get; set; }
            public Dictionary<int, DateTime> CountryBEDsByCountryId { get; set; }
            public SaleEntityZoneRoutingProductLocator RoutingProductLocator { get; set; }
            public SaleEntityZoneRateLocator CurrentRateLocator { get; set; }
        }

        private void SetNumberPrecisionValues(out int normalPrecisionValue, out int longPrecisionValue)
        {
            var generalSettingsManager = new Vanrise.Common.Business.GeneralSettingsManager();
            normalPrecisionValue = generalSettingsManager.GetNormalPrecisionValue();
            longPrecisionValue = generalSettingsManager.GetLongPrecisionValue();
        }

        #endregion

        #region Import Rate Plan

        public ImportRatePlanResult ImportRatePlan(ImportRatePlanInput input)
        {
            IEnumerable<string> worksheetHeaders;
            IEnumerable<ImportedRow> importedRows = GetImportedRows(input.FileId, input.HeaderRowExists, out worksheetHeaders);

            Dictionary<string, SaleZone> saleZonesByName = GetSaleZonesEffectiveAfterByName(input.OwnerType, input.OwnerId, DateTime.Today);
            Dictionary<string, ZoneChanges> zoneDraftsByZoneName = GetZoneDraftsByZoneName(input.OwnerType, input.OwnerId);

            var countryBEDsByCountry = new Dictionary<int, DateTime>();
            if (input.OwnerType == SalePriceListOwnerType.Customer)
                countryBEDsByCountry = UtilitiesManager.GetDatesByCountry(input.OwnerId, DateTime.Today, false);

            bool invalidImportedRowsExist;
            IEnumerable<ValidatedImportedRow> validatedImportedRows = GetValidatedImportedRows(input.OwnerType, importedRows, saleZonesByName, zoneDraftsByZoneName, countryBEDsByCountry, out invalidImportedRowsExist);

            var result = new ImportRatePlanResult();

            if (invalidImportedRowsExist)
            {
                long fileId = CreateExcelFile(validatedImportedRows, worksheetHeaders);
                result.FileId = fileId;
            }
            else
            {
                UpdateDraft(input.OwnerType, input.OwnerId, validatedImportedRows, saleZonesByName, zoneDraftsByZoneName);
            }

            return result;
        }

        private IEnumerable<ImportedRow> GetImportedRows(long fileId, bool headerRowExists, out IEnumerable<string> worksheetHeaders)
        {
            var fileManager = new VRFileManager();
            VRFile file = fileManager.GetFile(fileId);

            if (file == null)
                throw new Vanrise.Entities.DataIntegrityValidationException(string.Format("File '{0}' was not found", fileId));
            if (file.Content == null)
                throw new Vanrise.Entities.DataIntegrityValidationException(string.Format("File '{0}' is empty", fileId));

            byte[] bytes = file.Content;
            var fileStream = new MemoryStream(bytes);

            Vanrise.Common.Utilities.ActivateAspose();

            var workbook = new Workbook(fileStream);
            if (workbook.Worksheets == null || workbook.Worksheets.Count == 0)
                throw new Vanrise.Entities.DataIntegrityValidationException(string.Format("Workbook created from file '{0}' does not contain any worksheets", fileId));

            Worksheet worksheet = workbook.Worksheets.ElementAt(0);

            var importedRows = new List<ImportedRow>();
            int startingRowIndex = 0;

            if (headerRowExists)
            {
                startingRowIndex = 1;
                worksheetHeaders = new List<string>()
				{
					worksheet.Cells[0, 0].StringValue,
					worksheet.Cells[0, 1].StringValue,
					worksheet.Cells[0, 2].StringValue
				};
            }
            else
            {
                worksheetHeaders = new List<string>() { "Zone", "Rate", "Effective Date" };
            }

            for (int i = startingRowIndex; i < worksheet.Cells.Rows.Count; i++)
            {
                Cell zoneCell = worksheet.Cells[i, 0], rateCell = worksheet.Cells[i, 1], dateCell = worksheet.Cells[i, 2];
                string importedZone, importedRate, importedDate;

                if (!IsRowEmpty(zoneCell, rateCell, dateCell, out importedZone, out importedRate, out importedDate))
                {
                    importedRows.Add(new ImportedRow()
                    {
                        Zone = importedZone,
                        Rate = importedRate,
                        EffectiveDate = importedDate
                    });
                }
            }

            return importedRows;
        }

        private bool IsRowEmpty(Cell zoneCell, Cell rateCell, Cell dateCell, out string importedZone, out string importedRate, out string importedDate)
        {
            bool isImportedZoneEmpty = IsStringEmpty(zoneCell.StringValue, out importedZone);
            bool isImportedRateEmpty = IsStringEmpty(rateCell.StringValue, out importedRate);
            bool isImportedDateEmpty = IsStringEmpty(dateCell.StringValue, out importedDate);

            return (isImportedZoneEmpty && isImportedRateEmpty && isImportedDateEmpty);
        }

        private bool IsStringEmpty(string originalString, out string trimmedString)
        {
            if (originalString == null)
            {
                trimmedString = null;
                return true;
            }
            trimmedString = originalString.Trim();
            return (trimmedString == string.Empty);
        }

        private IEnumerable<ValidatedImportedRow> GetValidatedImportedRows(SalePriceListOwnerType ownerType, IEnumerable<ImportedRow> importedRows, Dictionary<string, SaleZone> saleZonesByName, Dictionary<string, ZoneChanges> zoneDraftsByZoneName, Dictionary<int, DateTime> countryBEDsByCountry, out bool invalidImportedRowsExist)
        {
            var validatedImportedRows = new List<ValidatedImportedRow>();
            invalidImportedRowsExist = false;

            var validator = new ImportedRowValidator();

            foreach (ImportedRow importedRow in importedRows)
            {
                var validatedImportedRow = new ValidatedImportedRow()
                {
                    ImportedRow = importedRow
                };

                var context = new IsImportedRowValidContext()
                {
                    OwnerType = ownerType,
                    ImportedRow = importedRow,
                    ExistingZone = saleZonesByName.GetRecord(importedRow.Zone),
                    ZoneDraft = zoneDraftsByZoneName.GetRecord(importedRow.Zone),
                    CountryBEDsByCountry = countryBEDsByCountry
                };

                if (!validator.IsImportedRowValid(context))
                    invalidImportedRowsExist = true;

                validatedImportedRow.Status = context.Status;
                validatedImportedRow.ErrorMessage = context.ErrorMessage;

                validatedImportedRows.Add(validatedImportedRow);
            }

            return validatedImportedRows;
        }

        private long CreateExcelFile(IEnumerable<ValidatedImportedRow> validatedImportedRows, IEnumerable<string> worksheetHeaders)
        {
            Vanrise.Common.Utilities.ActivateAspose();

            var workbook = new Workbook();
            workbook.Worksheets.Clear();

            Worksheet worksheet = workbook.Worksheets.Add("Import Rate Plan Result");
            SetWorksheetHeaders(worksheet, worksheetHeaders);

            int currentRowIndex = 1;
            foreach (ValidatedImportedRow validatedImportedRow in validatedImportedRows)
            {
                worksheet.Cells[currentRowIndex, 0].PutValue(validatedImportedRow.ImportedRow.Zone);
                worksheet.Cells[currentRowIndex, 1].PutValue(validatedImportedRow.ImportedRow.Rate);
                worksheet.Cells[currentRowIndex, 2].PutValue(validatedImportedRow.ImportedRow.EffectiveDate);
                worksheet.Cells[currentRowIndex, 3].PutValue(validatedImportedRow.Status);
                worksheet.Cells[currentRowIndex, 4].PutValue(validatedImportedRow.ErrorMessage);
                currentRowIndex++;
            }

            MemoryStream memoryStream = workbook.SaveToStream();
            return new VRFileManager().AddFile(new VRFile()
            {
                Name = "CountryLog",
                Content = memoryStream.ToArray(),
                Extension = ".xlsx",
                IsTemp=true,
            });
        }

        private void SetWorksheetHeaders(Worksheet worksheet, IEnumerable<string> worksheetHeaders)
        {
            var allWorksheetHeaders = new List<string>(worksheetHeaders);
            allWorksheetHeaders.Add("Status");
            allWorksheetHeaders.Add("Error Message");

            for (int i = 0; i < allWorksheetHeaders.Count(); i++)
            {
                worksheet.Cells[0, i].PutValue(allWorksheetHeaders.ElementAt(i));
                worksheet.Cells.SetColumnWidth(i, 20);

                // Set the style of the header cells
                Cell cell = worksheet.Cells.GetCell(0, i);
                Style style = cell.GetStyle();
                style.Font.Name = "Times New Roman";
                style.Font.Color = System.Drawing.Color.Black;
                style.Font.Size = 14;
                style.Font.IsBold = true;
                cell.SetStyle(style);
            }
        }

        private void UpdateDraft(SalePriceListOwnerType ownerType, int ownerId, IEnumerable<ValidatedImportedRow> validatedImportedRows, Dictionary<string, SaleZone> saleZonesByName, Dictionary<string, ZoneChanges> zoneDraftsByZoneName)
        {
            var newDraft = new Changes()
            {
                CurrencyId = null,
                DefaultChanges = null,
                CountryChanges = null,
                ZoneChanges = new List<ZoneChanges>()
            };

            foreach (ValidatedImportedRow validatedImportedRow in validatedImportedRows)
            {
                ZoneChanges zoneDraft = zoneDraftsByZoneName.GetRecord(validatedImportedRow.ImportedRow.Zone);
                if (zoneDraft == null)
                {
                    SaleZone saleZone = saleZonesByName.GetRecord(validatedImportedRow.ImportedRow.Zone);
                    if (saleZone == null)
                        throw new Vanrise.Entities.DataIntegrityValidationException("SaleZone '{0}' was not found");
                    zoneDraft = new ZoneChanges()
                    {
                        ZoneId = saleZone.SaleZoneId,
                        ZoneName = validatedImportedRow.ImportedRow.Zone,
                        CountryId = saleZone.CountryId
                    };
                }

                var newRates = new List<DraftRateToChange>();
                if (zoneDraft.NewRates != null)
                {
                    foreach (DraftRateToChange newRate in zoneDraft.NewRates)
                        if (newRate.RateTypeId.HasValue)
                            newRates.Add(newRate);
                }

                DraftRateToChange newNormalRate = new DraftRateToChange()
                {
                    ZoneId = zoneDraft.ZoneId,
                    RateTypeId = null,
                    Rate = Convert.ToDecimal(validatedImportedRow.ImportedRow.Rate),
                    BED = Convert.ToDateTime(validatedImportedRow.ImportedRow.EffectiveDate),
                    EED = null
                };

                newRates.Add(newNormalRate);
                zoneDraft.NewRates = newRates;

                newDraft.ZoneChanges.Add(zoneDraft);
            }

            new RatePlanDraftManager().SaveDraft(ownerType, ownerId, newDraft);
        }

        private Dictionary<string, SaleZone> GetSaleZonesEffectiveAfterByName(SalePriceListOwnerType ownerType, int ownerId, DateTime date)
        {
            var saleZonesByName = new Dictionary<string, SaleZone>();

            int sellingNumberPlanId = GetOwnerSellingNumberPlanId(ownerType, ownerId);
            IEnumerable<SaleZone> saleZones = new SaleZoneManager().GetSaleZonesEffectiveAfter(sellingNumberPlanId, date);

            if (saleZones != null)
            {
                foreach (SaleZone saleZone in saleZones)
                {
                    string saleZoneName = saleZone.Name.Trim().ToLower();
                    if (!saleZonesByName.ContainsKey(saleZoneName))
                        saleZonesByName.Add(saleZoneName, saleZone);
                }
            }

            return saleZonesByName;
        }

        private Dictionary<string, ZoneChanges> GetZoneDraftsByZoneName(SalePriceListOwnerType ownerType, int ownerId)
        {
            var zoneDraftsByZoneName = new Dictionary<string, ZoneChanges>();

            Changes draft = new RatePlanDraftManager().GetDraft(ownerType, ownerId);
            if (draft == null || draft.ZoneChanges == null)
                return zoneDraftsByZoneName;

            foreach (ZoneChanges zoneDraft in draft.ZoneChanges)
            {
                if (zoneDraft.ZoneName != null && !zoneDraftsByZoneName.ContainsKey(zoneDraft.ZoneName))
                    zoneDraftsByZoneName.Add(zoneDraft.ZoneName, zoneDraft);
            }

            return zoneDraftsByZoneName;
        }

        public byte[] DownloadImportRatePlanResult(long fileId)
        {
            VRFile file = new VRFileManager().GetFile(fileId);
            if (file == null)
                throw new Vanrise.Entities.DataIntegrityValidationException("File '{0}' was not found");
            return file.Content;
        }

        #endregion

        public IEnumerable<SaleZone> GetSellingProductZones(int sellingNumberPlanId, DateTime effectiveAfter)
        {
            IEnumerable<SaleZone> allZones = new SaleZoneManager().GetSaleZonesBySellingNumberPlan(sellingNumberPlanId);
            return allZones.FindAllRecords(x => x.IsEffectiveOrFuture(effectiveAfter));
        }
        public IEnumerable<SaleZone> GetCustomerZones(int sellingNumberPlanId, Dictionary<int, CountryDateConfig> countryDateConfigsByCountryId, DateTime effectiveOn)
        {
            IEnumerable<SaleZone> allZones = new SaleZoneManager().GetSaleZonesBySellingNumberPlan(sellingNumberPlanId);

            Func<SaleZone, bool> filterExpression = (saleZone) =>
            {
                CountryDateConfig countryDateConfig;
                if (!countryDateConfigsByCountryId.TryGetValue(saleZone.CountryId, out countryDateConfig))
                    return false;

                return ((!saleZone.EED.HasValue || saleZone.EED.Value > effectiveOn) && saleZone.IsOverlappedWith(countryDateConfig));
            };

            return allZones.FindAllRecords(filterExpression);
        }
        private Dictionary<int, CountryDateConfig> GetCountryDateConfigsByCountryId(int customerId, DateTime effectiveAfter)
        {
            var countryDateConfigsByCountryId = new Dictionary<int, CountryDateConfig>();

            Changes draft = new RatePlanDraftManager().GetDraft(SalePriceListOwnerType.Customer, customerId);

            if (draft != null && draft.CountryChanges != null && draft.CountryChanges.NewCountries != null)
            {
                foreach (DraftNewCountry newCountry in draft.CountryChanges.NewCountries)
                    countryDateConfigsByCountryId.Add(newCountry.CountryId, new CountryDateConfig() { BED = newCountry.BED, EED = newCountry.EED });
            }

            IEnumerable<CustomerCountry2> soldCountries = new CustomerCountryManager().GetCustomerCountriesEffectiveAfter(customerId, effectiveAfter);

            if (soldCountries != null)
            {
                foreach (CustomerCountry2 soldCountry in soldCountries)
                {
                    if (!countryDateConfigsByCountryId.ContainsKey(soldCountry.CountryId))
                        countryDateConfigsByCountryId.Add(soldCountry.CountryId, new CountryDateConfig() { BED = soldCountry.BED, EED = soldCountry.EED });
                }
            }

            return countryDateConfigsByCountryId;
        }
    }

    public class CountryDateConfig : Vanrise.Entities.IDateEffectiveSettings
    {
        public DateTime BED { get; set; }
        public DateTime? EED { get; set; }
    }

    public class CountryToCloseFilter : ICountryFilter
    {
        public int CustomerId { get; set; }
        public DateTime EffectiveOn { get; set; }

        public bool IsExcluded(ICountryFilterContext context)
        {
            if (context.CustomObject == null)
                context.CustomObject = new CustomObject(CustomerId, EffectiveOn);

            var customObject = context.CustomObject as CustomObject;
            return !customObject.CountryIdsToClose.Contains(context.Country.CountryId);
        }

        private class CustomObject
        {
            public IEnumerable<int> CountryIdsToClose { get; set; }

            public CustomObject(int customerId, DateTime effectiveOn)
            {
                var countryIdsToClose = new List<int>();
                IEnumerable<CustomerCountry2> soldCountries = new CustomerCountryManager().GetSoldCountries(customerId, effectiveOn);

                if (soldCountries != null && soldCountries.Count() > 0)
                    countryIdsToClose.AddRange(soldCountries.MapRecords(x => x.CountryId, x => x.IsEffective(effectiveOn) && !x.EED.HasValue));

                CountryIdsToClose = countryIdsToClose;
            }
        }
    }
}
