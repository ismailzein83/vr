using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TOne.WhS.BusinessEntity.Data;
using TOne.WhS.BusinessEntity.Entities;
using Vanrise.Caching;
using Vanrise.Caching.Runtime;
using Vanrise.Common;
using Vanrise.Common.Business;
using Vanrise.Entities;
using Vanrise.GenericData.Entities;

namespace TOne.WhS.BusinessEntity.Business
{
    public class SaleZoneManager : BaseBusinessEntityManager, ISaleZoneManager
    {
        #region Public Methods

        public Dictionary<long, SaleZone> GetCachedSaleZones()
        {
            return Vanrise.Caching.CacheManagerFactory.GetCacheManager<CacheManager>().GetOrCreateObject("GetAllSaleZones", () =>
            {
                ISaleZoneDataManager dataManager = BEDataManagerFactory.GetDataManager<ISaleZoneDataManager>();
                IEnumerable<SaleZone> allSaleZones = dataManager.GetAllSaleZones();
                if (allSaleZones == null)
                    return null;

                return allSaleZones.ToDictionary(itm => itm.SaleZoneId, itm => itm);
            });
        }

        public IEnumerable<SaleZone> GetAllZones()
        {
            var allZones = GetCachedSaleZones();
            if (allZones == null)
                return null;

            return allZones.Values;
        }


        public UpdateOperationOutput<SaleZoneDetail> UpdateSaleZoneName(long zoneId, string zoneName, int sellingNumberPlanId)
        {
            if (String.IsNullOrWhiteSpace(zoneName))
                throw new MissingArgumentValidationException("SaleZone.Name");

            ISaleZoneDataManager dataManager = BEDataManagerFactory.GetDataManager<ISaleZoneDataManager>();

            bool updateActionSucc = dataManager.UpdateSaleZoneName(zoneId, zoneName, sellingNumberPlanId);
            UpdateOperationOutput<SaleZoneDetail> updateOperationOutput = new UpdateOperationOutput<SaleZoneDetail>();

            updateOperationOutput.Result = Vanrise.Entities.UpdateOperationResult.Failed;
            updateOperationOutput.UpdatedObject = null;

            if (updateActionSucc)
            {
                Vanrise.Caching.CacheManagerFactory.GetCacheManager<CacheManager>().SetCacheExpired();
                var saleZone = GetSaleZone(zoneId);
                VRActionLogger.Current.TrackAndLogObjectUpdated(SaleZoneLoggableEntity.Instance, saleZone);
                updateOperationOutput.Result = Vanrise.Entities.UpdateOperationResult.Succeeded;
                updateOperationOutput.UpdatedObject = SaleZoneDetailMapper(saleZone);
            }
            else
            {
                updateOperationOutput.Result = Vanrise.Entities.UpdateOperationResult.SameExists;
            }

            return updateOperationOutput;
        }

        public Vanrise.Entities.IDataRetrievalResult<SaleZoneDetail> GetFilteredSaleZones(Vanrise.Entities.DataRetrievalInput<SaleZoneQuery> input)
        {
            IEnumerable<SaleZone> saleZones = GetSaleZonesBySellingNumberPlan(input.Query.SellingNumberId);
            Func<SaleZone, bool> filterFunc = (saleZone) =>
            {
                if (input.Query.GetEffectiveAfter)
                {
                    if (!saleZone.IsEffectiveOrFuture(input.Query.EffectiveOn))
                        return false;
                }
                else if (!saleZone.IsEffective(input.Query.EffectiveOn))
                    return false;

                if (input.Query.Countries != null && !input.Query.Countries.Contains(saleZone.CountryId))
                    return false;

                if (input.Query.Name != null && !saleZone.Name.ToLower().Contains(input.Query.Name.ToLower()))
                    return false;

                return true;
            };
            var resultProcessingHandler = new ResultProcessingHandler<SaleZoneDetail>()
            {
                ExportExcelHandler = new SaleZoneDetailExportExcelHandler()
            };
            VRActionLogger.Current.LogGetFilteredAction(SaleZoneLoggableEntity.Instance, input);
            return Vanrise.Common.DataRetrievalManager.Instance.ProcessResult(input, saleZones.ToBigResult(input, filterFunc, SaleZoneDetailMapper), resultProcessingHandler);
        }

        public IEnumerable<SaleZone> GetSaleZonesBySellingNumberPlan(int sellingNumberPlanId)
        {
            IEnumerable<SaleZone> allSaleZones = GetCachedSaleZones().Values;
            return allSaleZones.FindAllRecords(x => x.SellingNumberPlanId == sellingNumberPlanId);
        }

        public SaleZone GetSaleZone(long saleZoneId)
        {
            return GetCachedSaleZones().GetRecord(saleZoneId);
        }

        public int? GetSaleZoneCountryId(long saleZoneId)
        {
            SaleZone saleZone = GetSaleZone(saleZoneId);
            if (saleZone == null)
                return null;
            return saleZone.CountryId;
        }

        public IOrderedEnumerable<long> GetSaleZoneIds(DateTime? effectiveOn, bool isEffectiveInFuture)
        {
            ISaleZoneDataManager dataManager = BEDataManagerFactory.GetDataManager<ISaleZoneDataManager>();
            return dataManager.GetSaleZoneIds(effectiveOn, isEffectiveInFuture);
        }

        public string GetDescription(int sellingNumberPlanId, IEnumerable<long> saleZoneIds)
        {
            IEnumerable<SaleZone> saleZonesBySellingNumberPlan = GetSaleZonesBySellingNumberPlan(sellingNumberPlanId);

            Func<SaleZone, bool> filterExpression = null;

            if (saleZoneIds != null)
                filterExpression = (itm) => (saleZoneIds.Contains(itm.SaleZoneId));

            saleZonesBySellingNumberPlan = saleZonesBySellingNumberPlan.FindAllRecords(filterExpression);

            if (saleZonesBySellingNumberPlan != null)
                return string.Join(", ", saleZonesBySellingNumberPlan.Select(x => x.Name));

            return string.Empty;
        }

        public IEnumerable<SaleZoneGroupConfig> GetSaleZoneGroupTemplates()
        {
            ExtensionConfigurationManager manager = new ExtensionConfigurationManager();
            return manager.GetExtensionConfigurations<SaleZoneGroupConfig>(SaleZoneGroupConfig.EXTENSION_TYPE);
        }

        public IEnumerable<SaleZone> GetSaleZonesByCountryIds(int sellingNumberPlanId, IEnumerable<int> countryIds, DateTime effectiveOn, bool withFutureZones)
        {
            if (countryIds == null)
                return null;

            IEnumerable<SaleZone> saleZones = GetSaleZonesBySellingNumberPlan(sellingNumberPlanId).FindAllRecords(x => countryIds.Contains(x.CountryId));

            if (withFutureZones)
                return saleZones.FindAllRecords(x => x.IsEffectiveOrFuture(effectiveOn));
            else
                return saleZones.FindAllRecords(x => x.IsEffective(effectiveOn));
        }

        public List<long> GetSoldZonesBySellingNumberPlan(int sellingNumberPlanId, List<int> countryIds, List<long> zoneIds, string zoneName, DateTime effectiveOn)
        {
            var saleZones = GetSaleZonesBySellingNumberPlan(sellingNumberPlanId);

            Func<SaleZone, bool> filterExpression = (x) =>
            {
                if (countryIds != null && countryIds.Count > 0 && !countryIds.Contains(x.CountryId))
                    return false;
                if (zoneIds != null && zoneIds.Count > 0 && !zoneIds.Contains(x.SaleZoneId))
                    return false;
                if (zoneName != null && !x.Name.ToLower().Contains(zoneName.ToLower()))
                    return false;
                if (!x.IsEffective(effectiveOn))
                    return false;
                return true;
            };

            var filterdRecords = saleZones.FindAllRecords(filterExpression);

            if (filterdRecords.Count() == 0)
                return null;

            return filterdRecords.Select(x => x.SaleZoneId).Distinct().ToList();
        }

        public IEnumerable<SaleZone> GetCustomerSaleZones(int customerId, DateTime? effectiveOn, bool isEffectiveInFuture)
        {
            var carrierAccountManager = new CarrierAccountManager();
            int sellingNumberPlanId = carrierAccountManager.GetSellingNumberPlanId(customerId, CarrierAccountType.Customer);
            return GetCustomerSaleZones(customerId, sellingNumberPlanId, effectiveOn, isEffectiveInFuture);
        }

        public IEnumerable<SaleZone> GetCustomerSaleZones(int customerId, int sellingNumberPlanId, DateTime? effectiveOn, bool isEffectiveInFuture)
        {
            var customerCountryManager = new CustomerCountryManager();
            IEnumerable<int> countryIds = customerCountryManager.GetCustomerCountryIds(customerId, effectiveOn, isEffectiveInFuture);

            if (countryIds == null || countryIds.Count() == 0)
                return null;
            else
                return GetEffectiveSaleZonesByCountryIds(sellingNumberPlanId, countryIds, effectiveOn, isEffectiveInFuture);
        }

        public IEnumerable<SaleZone> GetEffectiveSaleZonesByCountryIds(int sellingNumberPlanId, IEnumerable<int> countryIds, DateTime? effectiveOn, bool isEffectiveInFuture)
        {
            if (countryIds == null)
                return null;
            else
            {
                Func<SaleZone, bool> filterFunc = (saleZone) =>
                {
                    if (!countryIds.Contains(saleZone.CountryId))
                        return false;
                    if (!saleZone.IsEffective(effectiveOn, isEffectiveInFuture))
                        return false;
                    return true;
                };
                return GetSaleZonesBySellingNumberPlan(sellingNumberPlanId).FindAllRecords(filterFunc);
            }
        }
        public SaleZone GetCustomerSaleZoneByCode(int customerId, string codeNumber)
        {
            SaleCodeManager saleCodeManager = new SaleCodeManager();
            var saleCodes = saleCodeManager.GetEffectiveSaleCodesByCode(customerId, codeNumber);
            if (saleCodes == null || saleCodes.Count() == 0)
                return null;

            var carrierAccountManager = new CarrierAccountManager();
            int sellingNumberPlanId = carrierAccountManager.GetSellingNumberPlanId(customerId, CarrierAccountType.Customer);


            var customerSaleZones = GetSaleZonesBySellingNumberPlan(sellingNumberPlanId);
            if (customerSaleZones == null || customerSaleZones.Count() == 0)
                return null;

            var saleCodesOfEffectiveZones = saleCodes.FindAllRecords(x => customerSaleZones.Any(y => y.SaleZoneId == x.ZoneId));
            if (saleCodesOfEffectiveZones == null || saleCodesOfEffectiveZones.Count() == 0)
                return null;
            var saleCode = saleCodesOfEffectiveZones.FirstOrDefault();
            if (saleCode == null)
                return null;

            return GetSaleZone(saleCode.ZoneId);
        }
        public IEnumerable<SaleZone> GetSaleZonesByCountryId(int sellingNumberPlanId, int countryId, DateTime effectiveOn)
        {
            IEnumerable<SaleZone> saleZonesBySellingNumberPlan = GetSaleZonesBySellingNumberPlan(sellingNumberPlanId);

            if (saleZonesBySellingNumberPlan != null)
                saleZonesBySellingNumberPlan = saleZonesBySellingNumberPlan.FindAllRecords(z => (!z.EED.HasValue || (z.EED > effectiveOn)) && countryId == z.CountryId);

            return saleZonesBySellingNumberPlan;
        }

        public IEnumerable<SaleZoneInfo> GetSaleZonesInfo(string nameFilter, int sellingNumberPlanId, SaleZoneInfoFilter filter)
        {
            string zoneName = nameFilter != null ? nameFilter.ToLower() : null;
            IEnumerable<SaleZone> saleZonesBySellingNumberPlan = GetSaleZonesBySellingNumberPlan(sellingNumberPlanId);

            if (filter == null)
            {
                return saleZonesBySellingNumberPlan.MapRecords(SaleZoneInfoMapper, x => zoneName == null || x.Name.ToLower() == zoneName).OrderBy(x => x.Name);
            }

            var today = DateTime.Today;
            HashSet<long> filteredZoneIds = null;

            if (filter.SaleZoneFilterSettings != null)
            {
                filteredZoneIds = SaleZoneGroupContext.GetFilteredZoneIds(filter.SaleZoneFilterSettings);
            }

            var customObjects = new List<object>();
            if (filter.Filters != null)
            {
                foreach (ISaleZoneFilter saleZoneFilter in filter.Filters)
                    customObjects.Add(null);
            }

            Func<SaleZone, bool> filterPredicate = (zone) =>
            {
                if (!zone.IsEffective(filter.EffectiveMode, today))
                    return false;

                if (filteredZoneIds != null && !filteredZoneIds.Contains(zone.SaleZoneId))
                    return false;

                if (zoneName != null && !zone.Name.ToLower().Contains(zoneName))
                    return false;

                if (filter.CountryIds != null && !filter.CountryIds.Contains(zone.CountryId))
                    return false;

                if (filter.AvailableZoneIds != null && filter.AvailableZoneIds.Count() > 0 && !filter.AvailableZoneIds.Contains(zone.SaleZoneId))
                    return false;

                if (filter.ExcludedZoneIds != null && filter.ExcludedZoneIds.Count() > 0 && filter.ExcludedZoneIds.Contains(zone.SaleZoneId))
                    return false;

                if (filter.Filters != null)
                {
                    for (int i = 0; i < filter.Filters.Count(); i++)
                    {
                        var saleZoneFilterContext = new SaleZoneFilterContext() { SaleZone = zone, CustomData = customObjects[i] };
                        bool filterResult = filter.Filters.ElementAt(i).IsExcluded(saleZoneFilterContext);
                        customObjects[i] = saleZoneFilterContext.CustomData;
                        if (filterResult)
                            return false;
                    }
                }

                return true;
            };

            return saleZonesBySellingNumberPlan.MapRecords(SaleZoneInfoMapper, filterPredicate).OrderBy(x => x.Name);
        }

        public IEnumerable<SaleZoneInfo> GetSaleZonesInfoByIds(HashSet<long> saleZoneIds, SaleZoneFilterSettings saleZoneFilterSettings)
        {
            IEnumerable<SaleZone> saleZonesBySellingNumberPlan = GetCachedSaleZones().Values;
            HashSet<long> filteredZoneIds = SaleZoneGroupContext.GetFilteredZoneIds(saleZoneFilterSettings);
            Func<SaleZone, bool> zoneFilter = (zone) =>
            {
                if (!saleZoneIds.Contains(zone.SaleZoneId))
                    return false;
                if (filteredZoneIds != null && !filteredZoneIds.Contains(zone.SaleZoneId))
                    return false;
                return true;
            };
            return saleZonesBySellingNumberPlan.MapRecords(SaleZoneInfoMapper, zoneFilter).OrderBy(x => x.Name);
        }

        public IEnumerable<SaleZoneInfo> GetSellingNumberPlanIdBySaleZoneIds(List<long> saleZoneIds)
        {
            IEnumerable<SaleZone> saleZones = this.GetCachedSaleZones().Values;
            Func<SaleZone, bool> saleZoneFilter = (salezone) =>
            {
                if (!saleZoneIds.Contains(salezone.SaleZoneId))
                    return false;
                return true;
            };
            return saleZones.MapRecords(SaleZoneInfoMapper, saleZoneFilter).OrderBy(x => x.Name);
        }

        public string GetSaleZoneName(long saleZoneId)
        {
            SaleZone saleZone = GetSaleZone(saleZoneId);

            if (saleZone != null)
                return saleZone.Name;

            return null;
        }

        public Dictionary<long, string> GetSaleZonesNames(IEnumerable<long> saleZoneIds)
        {
            var allSaleZones = GetCachedSaleZones().FindAllRecords(x => saleZoneIds.Contains(x.Key));
            if (allSaleZones == null)
                return null;

            return allSaleZones.ToDictionary(x => x.Key, x => x.Value.Name);
        }

        public bool IsCountryEmpty(int sellingNumberPlanId, int countryId, DateTime effectiveOn)
        {
            IEnumerable<SaleZone> allSaleZones = GetSaleZonesBySellingNumberPlan(sellingNumberPlanId);
            return !AnySaleZoneEffectiveAfterExists(allSaleZones, countryId, effectiveOn);
        }

        public bool AnySaleZoneEffectiveAfterExists(IEnumerable<SaleZone> saleZones, int countryId, DateTime effectiveOn)
        {
            Func<SaleZone, bool> filterFunc = (saleZone) =>
            {
                if (saleZone.EED.HasValue && saleZone.EED <= saleZone.BED) // Exclude deleted sale zones
                    return false;
                if (saleZone.CountryId != countryId)
                    return false;
                if (!saleZone.IsEffectiveOrFuture(effectiveOn))
                    return false;
                return true;
            };
            return saleZones.Any(filterFunc);
        }

        public IEnumerable<SaleZone> GetSaleZonesEffectiveAfter(int sellingNumberPlanId, DateTime minimumDate)
        {
            IEnumerable<SaleZone> allSaleZones = GetCachedSaleZones().Values;
            return allSaleZones.FindAllRecords(item => item.SellingNumberPlanId == sellingNumberPlanId && (!item.EED.HasValue || (item.EED.Value > minimumDate && item.EED > item.BED)));
        }

        public int GetSaleZoneTypeId()
        {
            return Vanrise.Common.Business.TypeManager.Instance.GetTypeId(this.GetSaleZoneType());
        }

        public Type GetSaleZoneType()
        {
            return this.GetType();
        }

        public IEnumerable<SaleZone> GetSaleZonesByOwner(SalePriceListOwnerType ownerType, int ownerId, DateTime effectiveOn, bool isEffectiveInFuture)
        {
            int? sellingNumberPlanId = (ownerType == SalePriceListOwnerType.SellingProduct) ?
                new SellingProductManager().GetSellingNumberPlanId(ownerId) :
                new CarrierAccountManager().GetSellingNumberPlanId(ownerId, CarrierAccountType.Customer);
            if (!sellingNumberPlanId.HasValue)
                throw new NullReferenceException("sellingNumberPlanId");
            return GetSaleZonesByOwner(ownerType, ownerId, sellingNumberPlanId.Value, effectiveOn, isEffectiveInFuture);
        }

        public IEnumerable<SaleZone> GetSaleZonesByOwner(SalePriceListOwnerType ownerType, int ownerId, int sellingNumberPlanId, DateTime effectiveOn, bool isEffectiveInFuture)
        {
            if (ownerType == SalePriceListOwnerType.SellingProduct)
            {
                IEnumerable<SaleZone> saleZones = GetSaleZonesBySellingNumberPlan(sellingNumberPlanId);
                return isEffectiveInFuture ? saleZones.FindAllRecords(x => x.IsEffectiveOrFuture(effectiveOn)) : saleZones.FindAllRecords(x => x.IsEffective(effectiveOn));
            }
            else
            {
                var customerCountryManager = new CustomerCountryManager();
                IEnumerable<int> customerCountryIds = (isEffectiveInFuture) ? customerCountryManager.GetCountryIdsEffectiveAfterByCustomer(ownerId, effectiveOn) : customerCountryManager.GetCustomerCountryIds(ownerId, effectiveOn, isEffectiveInFuture);
                return GetSaleZonesByCountryIds(sellingNumberPlanId, customerCountryIds, effectiveOn, isEffectiveInFuture);
            }
        }

        public DateTime GetCustomerInheritedZoneRateBED(int? rateTypeId, ZoneOverlappedRates zoneOverlappedRates, DateTime inheritedRateBED, DateTime? inheritedRateEED, DateTime soldOn)
        {
            DateTime? lastOverlappedRateEED = null;
            if (zoneOverlappedRates != null)
            {
                lastOverlappedRateEED = (rateTypeId.HasValue) ?
                  zoneOverlappedRates.GetLastOverlappedOtherRateEED(rateTypeId.Value, inheritedRateBED, inheritedRateEED) :
                  zoneOverlappedRates.GetLastOverlappedNormalRateEED(inheritedRateBED, inheritedRateEED);
            }
            return (lastOverlappedRateEED.HasValue) ? Vanrise.Common.Utilities.Max(soldOn, Vanrise.Common.Utilities.Max(inheritedRateBED, lastOverlappedRateEED.Value)) : Vanrise.Common.Utilities.Max(inheritedRateBED, soldOn);
        }

        public IEnumerable<long> GetSaleZoneIdsBySaleZoneName(int sellingNumberPlanId, int countryId, string saleZoneName)
        {
            if (saleZoneName == null)
                throw new Vanrise.Entities.MissingArgumentValidationException("saleZoneName");

            IEnumerable<SaleZone> cachedSaleZones = GetSaleZonesBySellingNumberPlan(sellingNumberPlanId);
            if (cachedSaleZones == null || cachedSaleZones.Count() == 0)
                return null;

            string targetZoneName = saleZoneName.Trim().ToLower();
            return cachedSaleZones.MapRecords(x => x.SaleZoneId, x => x.CountryId == countryId && (x.Name != null && x.Name.Trim().ToLower() == targetZoneName));
        }

        #endregion

        #region Private Members

        private class CacheManager : Vanrise.Caching.BaseCacheManager
        {
            ISaleZoneDataManager _dataManager = BEDataManagerFactory.GetDataManager<ISaleZoneDataManager>();
            object _updateHandle;

            protected override bool ShouldSetCacheExpired()
            {
                return _dataManager.AreZonesUpdated(ref _updateHandle);
            }
        }

        private SaleZoneDetail SaleZoneDetailMapper(SaleZone saleZone)
        {
            SaleZoneDetail saleZoneDetail = new SaleZoneDetail();

            saleZoneDetail.Entity = saleZone;

            CountryManager manager = new CountryManager();
            SellingNumberPlanManager sellingManager = new SellingNumberPlanManager();

            int countryId = saleZone.CountryId;
            var country = manager.GetCountry(countryId);
            saleZoneDetail.CountryName = (country != null) ? country.Name : "";

            int sellingNumberPlanId = saleZone.SellingNumberPlanId;
            var sellingNumberPlan = sellingManager.GetSellingNumberPlan(sellingNumberPlanId);
            saleZoneDetail.SellingNumberPlanName = (sellingNumberPlan != null) ? sellingNumberPlan.Name : "";


            return saleZoneDetail;
        }
        public long ReserveIDRange(int numberOfIDs)
        {
            long startingId;
            IDManager.Instance.ReserveIDRange(GetSaleZoneType(), numberOfIDs, out startingId);
            return startingId;
        }

        private SaleZoneInfo SaleZoneInfoMapper(SaleZone saleZone)
        {
            return new SaleZoneInfo { SaleZoneId = saleZone.SaleZoneId, Name = saleZone.Name, SellingNumberPlanId = saleZone.SellingNumberPlanId };
        }

        #endregion

        #region Private Classes

        private class SaleZoneDetailExportExcelHandler : ExcelExportHandler<SaleZoneDetail>
        {
            public override void ConvertResultToExcelData(IConvertResultToExcelDataContext<SaleZoneDetail> context)
            {
                var sheet = new ExportExcelSheet()
                {
                    SheetName = "Sales Zones",
                    Header = new ExportExcelHeader() { Cells = new List<ExportExcelHeaderCell>() }
                };

                sheet.Header.Cells.Add(new ExportExcelHeaderCell() { Title = "ID" });
                sheet.Header.Cells.Add(new ExportExcelHeaderCell() { Title = "Name", Width = 35 });
                sheet.Header.Cells.Add(new ExportExcelHeaderCell() { Title = "Country", Width = 30 });
                sheet.Header.Cells.Add(new ExportExcelHeaderCell() { Title = "Selling Number Plan" });
                sheet.Header.Cells.Add(new ExportExcelHeaderCell() { Title = "BED", CellType = ExcelCellType.DateTime, DateTimeType = DateTimeType.Date });
                sheet.Header.Cells.Add(new ExportExcelHeaderCell() { Title = "EED", CellType = ExcelCellType.DateTime, DateTimeType = DateTimeType.Date });

                sheet.Rows = new List<ExportExcelRow>();
                if (context.BigResult != null && context.BigResult.Data != null)
                {
                    foreach (var record in context.BigResult.Data)
                    {
                        if (record.Entity != null)
                        {
                            var row = new ExportExcelRow() { Cells = new List<ExportExcelCell>() };
                            row.Cells.Add(new ExportExcelCell() { Value = record.Entity.SaleZoneId });
                            row.Cells.Add(new ExportExcelCell() { Value = record.Entity.Name });
                            row.Cells.Add(new ExportExcelCell() { Value = record.CountryName });
                            row.Cells.Add(new ExportExcelCell() { Value = record.SellingNumberPlanName });
                            row.Cells.Add(new ExportExcelCell() { Value = record.Entity.BED });
                            row.Cells.Add(new ExportExcelCell() { Value = record.Entity.EED });
                            sheet.Rows.Add(row);
                        }
                    }
                }

                context.MainSheet = sheet;
            }
        }
        private class SaleZoneLoggableEntity : VRLoggableEntityBase
        {
            public static SaleZoneLoggableEntity Instance = new SaleZoneLoggableEntity();

            private SaleZoneLoggableEntity()
            {

            }

            static SaleZoneManager s_saleZoneManager = new SaleZoneManager();

            public override string EntityUniqueName
            {
                get { return "WhS_BusinessEntity_SaleZone"; }
            }

            public override string ModuleName
            {
                get { return "Business Entity"; }
            }

            public override string EntityDisplayName
            {
                get { return "Sale Zone"; }
            }

            public override string ViewHistoryItemClientActionName
            {
                get { return "WhS_BusinessEntity_SaleZone_ViewHistoryItem"; }
            }

            public override object GetObjectId(IVRLoggableEntityGetObjectIdContext context)
            {
                SaleZone saleZone = context.Object.CastWithValidate<SaleZone>("context.Object");
                return saleZone.SaleZoneId;
            }

            public override string GetObjectName(IVRLoggableEntityGetObjectNameContext context)
            {
                SaleZone saleZone = context.Object.CastWithValidate<SaleZone>("context.Object");
                return s_saleZoneManager.GetSaleZoneName(saleZone.SaleZoneId);
            }
        }

        #endregion

        #region IBusinessEntityManager

        public override dynamic GetEntity(IBusinessEntityGetByIdContext context)
        {
            return GetSaleZone(Convert.ToInt64(context.EntityId));
        }

        public override List<dynamic> GetAllEntities(IBusinessEntityGetAllContext context)
        {
            var allZones = GetCachedSaleZones();
            if (allZones == null)
                return null;
            else
                return allZones.Values.Select(itm => itm as dynamic).ToList();
        }

        public override string GetEntityDescription(IBusinessEntityDescriptionContext context)
        {
            return GetSaleZoneName(Convert.ToInt64(context.EntityId));
        }

        public override dynamic GetEntityId(IBusinessEntityIdContext context)
        {
            var saleZone = context.Entity as SaleZone;
            return saleZone.SaleZoneId;
        }

        public override bool IsCacheExpired(IBusinessEntityIsCacheExpiredContext context, ref DateTime? lastCheckTime)
        {
            return Vanrise.Caching.CacheManagerFactory.GetCacheManager<CacheManager>().IsCacheExpired(ref lastCheckTime);
        }

        public override IEnumerable<dynamic> GetIdsByParentEntityId(IBusinessEntityGetIdsByParentEntityIdContext context)
        {
            Func<SaleZone, bool> filter;
            switch (context.ParentEntityDefinition.Name)
            {
                case Vanrise.Entities.Country.BUSINESSENTITY_DEFINITION_NAME: filter = (zone) => zone.CountryId == context.ParentEntityId; break;
                case SellingNumberPlan.BUSINESSENTITY_DEFINITION_NAME: filter = (zone) => zone.SellingNumberPlanId == context.ParentEntityId; break;
                default: throw new NotImplementedException(String.Format("Business Entity Definition Name '{0}'", context.ParentEntityDefinition.Name));
            }
            return GetCachedSaleZones().FindAllRecords(filter).MapRecords(zone => zone.SaleZoneId as dynamic);
        }

        public override dynamic GetParentEntityId(IBusinessEntityGetParentEntityIdContext context)
        {
            var saleZone = context.Entity as SaleZone;
            if (saleZone == null)
                throw new NullReferenceException("saleZone");
            switch (context.ParentEntityDefinition.Name)
            {
                case Vanrise.Entities.Country.BUSINESSENTITY_DEFINITION_NAME: return saleZone.CountryId;
                case SellingNumberPlan.BUSINESSENTITY_DEFINITION_NAME: return saleZone.SellingNumberPlanId;
                default: throw new NotImplementedException(String.Format("Business Entity Definition Name '{0}'", context.ParentEntityDefinition.Name));
            }
        }

        public override dynamic MapEntityToInfo(IBusinessEntityMapToInfoContext context)
        {
            throw new NotImplementedException();
        }

        #endregion
    }
}
