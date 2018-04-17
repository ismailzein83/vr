using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Vanrise.NumberingPlan.Entities;
using Vanrise.Common;
using Vanrise.GenericData.Entities;
using Vanrise.NumberingPlan.Data;
using Vanrise.Common.Business;
using Vanrise.Entities;
using Vanrise.GenericData.MainExtensions.DataRecordFields;
using Vanrise.GenericData.Business;

namespace Vanrise.NumberingPlan.Business
{
    public class SaleZoneManager : BaseBusinessEntityManager
    {
        #region Public Methods

        public Dictionary<long, SaleZone> GetCachedSaleZones()
        {
            return Vanrise.Caching.CacheManagerFactory.GetCacheManager<CacheManager>().GetOrCreateObject("GetAllSaleZones", () =>
            {
                ISaleZoneDataManager dataManager = CodePrepDataManagerFactory.GetDataManager<ISaleZoneDataManager>();
                IEnumerable<SaleZone> allSaleZones = dataManager.GetAllSaleZones();
                if (allSaleZones == null)
                    return null;

                return allSaleZones.ToDictionary(itm => itm.SaleZoneId, itm => itm);
            });
        }
        public Dictionary<string, List<SaleZone>> GetCachedSaleZonesByName()
        {
            return Vanrise.Caching.CacheManagerFactory.GetCacheManager<CacheManager>().GetOrCreateObject("GetCachedZonesByName",
            () =>
            {
                Dictionary<string, List<SaleZone>> cachedZonesByName = new Dictionary<string, List<SaleZone>>();
                var cachedZones = GetCachedSaleZones();
                foreach (var zone in cachedZones.Values)
                {
                    var zoneList = cachedZonesByName.GetOrCreateItem(zone.Name.ToLower());
                    zoneList.Add(zone);
                }
                return cachedZonesByName;
            });
        }
        public Vanrise.Entities.IDataRetrievalResult<SaleZoneDetail> GetFilteredSaleZones(Vanrise.Entities.DataRetrievalInput<SaleZoneQuery> input)
        {
            var saleZonesBySellingNumberPlan = GetSaleZonesBySellingNumberPlan(input.Query.SellingNumberId);
            Func<SaleZone, bool> filterExpression = (prod) =>
                     (input.Query.Name == null || prod.Name.ToLower().Contains(input.Query.Name.ToLower()))
                    && (input.Query.Countries == null || input.Query.Countries.Contains(prod.CountryId))
                  && ((prod.BED <= input.Query.EffectiveOn))
                  && ((!prod.EED.HasValue || (prod.EED > input.Query.EffectiveOn)));

            var resultProcessingHandler = new ResultProcessingHandler<SaleZoneDetail>()
            {
                ExportExcelHandler = new SaleZoneDetailExportExcelHandler()
            };

            return Vanrise.Common.DataRetrievalManager.Instance.ProcessResult(input, saleZonesBySellingNumberPlan.ToBigResult(input, filterExpression, SaleZoneDetailMapper), resultProcessingHandler);
        }

        public IEnumerable<SaleZone> GetSaleZonesBySellingNumberPlan(int sellingNumberPlanId)
        {
            IEnumerable<SaleZone> allSaleZones = GetCachedSaleZones().Values;
            return allSaleZones.FindAllRecords(x => x.SellingNumberPlanId == sellingNumberPlanId);
        }

        public IEnumerable<SaleZoneInfo> GetSaleZonesInfo(string nameFilter, int sellingNumberPlanId, SaleZoneInfoFilter filter)
        {
            string zoneName = nameFilter != null ? nameFilter.ToLower() : null;
            IEnumerable<SaleZone> saleZonesBySellingNumberPlan = GetSaleZonesBySellingNumberPlan(sellingNumberPlanId);

            if (filter == null)
            {
                return saleZonesBySellingNumberPlan.MapRecords(SaleZoneInfoMapper, x => zoneName == null || x.Name.ToLower() == zoneName).OrderBy(x => x.Name);
            }

            var now = DateTime.Now;

            var customObjects = new List<object>();
            if (filter.Filters != null)
            {
                foreach (ISaleZoneFilter saleZoneFilter in filter.Filters)
                    customObjects.Add(null);
            }

            Func<SaleZone, bool> filterPredicate = (zone) =>
            {
                if (filter.GetEffectiveOnly && (zone.BED > now || (zone.EED.HasValue && zone.EED.Value < now)))
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
                        if (filter.Filters.ElementAt(i).IsExcluded(saleZoneFilterContext))
                            return false;
                        customObjects[i] = saleZoneFilterContext.CustomData;
                    }
                }

                return true;
            };

            return saleZonesBySellingNumberPlan.MapRecords(SaleZoneInfoMapper, filterPredicate).OrderBy(x => x.Name);
        }

        public IEnumerable<SaleZoneInfo> GetSaleZonesInfoByIds(HashSet<long> saleZoneIds)
        {
            IEnumerable<SaleZone> saleZonesBySellingNumberPlan = GetCachedSaleZones().Values;
            Func<SaleZone, bool> zoneFilter = (zone) =>
            {
                if (!saleZoneIds.Contains(zone.SaleZoneId))
                    return false;
                return true;
            };
            return saleZonesBySellingNumberPlan.MapRecords(SaleZoneInfoMapper, zoneFilter).OrderBy(x => x.Name);
        }

        public SaleZone GetSaleZone(long saleZoneId)
        {
            return GetCachedSaleZones().GetRecord(saleZoneId);
        }

        public int GetSaleZoneCountryId(long saleZoneId)
        {
            SaleZone saleZone = GetSaleZone(saleZoneId);
            saleZone.ThrowIfNull("saleZone", saleZoneId);
            return saleZone.CountryId;
        }

        public IEnumerable<long> GetSaleZoneIds(DateTime? effectiveOn, bool isEffectiveInFuture)
        {
            ISaleZoneDataManager dataManager = CodePrepDataManagerFactory.GetDataManager<ISaleZoneDataManager>();
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
            IEnumerable<SaleZone> saleZones = GetSaleZonesBySellingNumberPlan(sellingNumberPlanId).FindAllRecords(x => countryIds.Contains(x.CountryId));
            return withFutureZones ? saleZones.FindAllRecords(x => x.IsEffectiveOrFuture(effectiveOn)) : saleZones.FindAllRecords(x => x.IsEffective(effectiveOn));
        }

        public IEnumerable<SaleZone> GetSaleZonesByCountryId(int sellingNumberPlanId, int countryId, DateTime effectiveOn)
        {
            IEnumerable<SaleZone> saleZonesBySellingNumberPlan = GetSaleZonesBySellingNumberPlan(sellingNumberPlanId);

            if (saleZonesBySellingNumberPlan != null)
                saleZonesBySellingNumberPlan = saleZonesBySellingNumberPlan.FindAllRecords(z => (!z.EED.HasValue || (z.EED > effectiveOn)) && countryId == z.CountryId);

            return saleZonesBySellingNumberPlan;
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

        public bool IsCountryEmpty(int sellingNumberPlanId, int countryId, DateTime minimumDate)
        {
            IEnumerable<SaleZone> saleZones = this.GetSaleZonesEffectiveAfter(sellingNumberPlanId, countryId, minimumDate);
            if (saleZones == null || saleZones.Count() == 0)
                return true;
            return false;
        }

        public IEnumerable<SaleZone> GetSaleZonesEffectiveAfter(int sellingNumberPlanId, int countryId, DateTime minimumDate)
        {
            IEnumerable<SaleZone> allSaleZones = GetCachedSaleZones().Values;
            return allSaleZones.FindAllRecords(item => item.SellingNumberPlanId == sellingNumberPlanId && item.CountryId == countryId && (!item.EED.HasValue || (item.EED.Value > minimumDate && item.EED > item.BED)));
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

        public override void GetIdByDescription(IBusinessEntityGetIdByDescriptionContext context)
        {
            if (context.FieldType != null)
            {
                var beFieldType = context.FieldType.CastWithValidate<FieldBusinessEntityType>("context.FieldType");
                var beDefinitionId = beFieldType.BusinessEntityDefinitionId;
                if (beDefinitionId == SaleZone.MASTERPLAN_BUSINESSENTITY_DEFINITION_ID)
                {
                    var cachedSaleZonesByName = GetCachedSaleZonesByName();
                    if (cachedSaleZonesByName != null)
                    {
                        var matchingSaleZones = cachedSaleZonesByName.GetRecord(context.FieldDescription.ToString().Trim().ToLower());
                        if (matchingSaleZones != null && matchingSaleZones.Count != 0)
                        {
                            SellingNumberPlanManager sellingNumberPlanManager = new SellingNumberPlanManager();
                            var masterSellingNumberPlan = sellingNumberPlanManager.GetMasterSellingNumberPlan();
                            masterSellingNumberPlan.ThrowIfNull("masterSellingNumberPlan", masterSellingNumberPlan.SellingNumberPlanId);
                            var masterSaleZoneList = matchingSaleZones.FindAllRecords(x => x.SellingNumberPlanId == masterSellingNumberPlan.SellingNumberPlanId);
                            var orderedMasterSaleZoneList = masterSaleZoneList.OrderByDescending(x => x.BED);

                            if (context.BERuntimeSelectorFilter == null)
                            {
                                context.FieldValue = orderedMasterSaleZoneList.First().SaleZoneId;
                            }
                            else
                            {
                                bool isMatched = false;
                                foreach (var saleZone in orderedMasterSaleZoneList)
                                {
                                    BERuntimeSelectorFilterSelectorFilterContext filterContext = new BERuntimeSelectorFilterSelectorFilterContext
                                    {
                                        BusinessEntityId = saleZone.SaleZoneId,
                                        BusinessEntityDefinitionId = beDefinitionId
                                    };

                                    if (context.BERuntimeSelectorFilter.IsMatched(filterContext))
                                    {
                                        context.FieldValue = saleZone.SaleZoneId;
                                        isMatched = true;
                                        break;
                                    }
                                }
                                if(!isMatched)
                                    context.ErrorMessage = string.Format("The zone {0} does not exist.", context.FieldDescription.ToString());
                            }
                        }
                        else
                        {
                            context.ErrorMessage = string.Format("The zone {0} does not exist.", context.FieldDescription.ToString());
                        }
                    }
                }
            }
        }

        #endregion

        #region Private Members

        private class CacheManager : Vanrise.Caching.BaseCacheManager
        {
            ISaleZoneDataManager _dataManager = CodePrepDataManagerFactory.GetDataManager<ISaleZoneDataManager>();
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
            SellingNumberPlanManager sellingNumberPlanManager = new SellingNumberPlanManager();

            int countryId = saleZone.CountryId;
            var country = manager.GetCountry(countryId);
            saleZoneDetail.CountryName = (country != null) ? country.Name : "";

            int sellingNumberPlanId = saleZone.SellingNumberPlanId;
            var sellingNumberPlan = sellingNumberPlanManager.GetSellingNumberPlan(sellingNumberPlanId);
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

        #endregion

        #region IBusinessEntityManager

        public override string GetEntityDescription(IBusinessEntityDescriptionContext context)
        {
            return GetSaleZoneName(Convert.ToInt64(context.EntityId));
        }

        public override dynamic GetEntityId(IBusinessEntityIdContext context)
        {
            var saleZone = context.Entity as SaleZone;
            return saleZone.SaleZoneId;
        }

        public override dynamic GetEntity(IBusinessEntityGetByIdContext context)
        {
            return GetSaleZone(context.EntityId);
        }

        public override List<dynamic> GetAllEntities(IBusinessEntityGetAllContext context)
        {
            var allZones = GetCachedSaleZones();
            if (allZones == null)
                return null;
            else
                return allZones.Values.Select(itm => itm as dynamic).ToList();
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
