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
    public class SupplierZoneManager : IBusinessEntityManager, ISupplierZoneManager
    {
        #region Public Methods
        public Dictionary<long, SupplierZone> GetCachedSupplierZones()
        {
            return Vanrise.Caching.CacheManagerFactory.GetCacheManager<CacheManager>().GetOrCreateObject("GetSupplierZones",
               () =>
               {
                   ISupplierZoneDataManager dataManager = BEDataManagerFactory.GetDataManager<ISupplierZoneDataManager>();
                   List<SupplierZone> allSupplierZones = dataManager.GetSupplierZones();
                   return allSupplierZones.ToDictionary(itm => itm.SupplierZoneId, itm => itm);
               });
        }

        public Vanrise.Entities.IDataRetrievalResult<SupplierZoneDetails> GetFilteredSupplierZones(Vanrise.Entities.DataRetrievalInput<SupplierZoneQuery> input)
        {
            var allsupplierZones = GetCachedSupplierZones();
            Func<SupplierZone, bool> filterExpression = (prod) =>
                     (input.Query.Name == null || prod.Name.ToLower().Contains(input.Query.Name.ToLower()))
                    && (input.Query.Countries == null || input.Query.Countries.Contains(prod.CountryId))
                  && (input.Query.SupplierId.Equals(prod.SupplierId))
                  && ((prod.BED <= input.Query.EffectiveOn))
                  && ((!prod.EED.HasValue || (prod.EED > input.Query.EffectiveOn)));

            ResultProcessingHandler<SupplierZoneDetails> handler = new ResultProcessingHandler<SupplierZoneDetails>()
            {
                ExportExcelHandler = new SupplierZoneExcelExportHandler()
            };
            VRActionLogger.Current.LogGetFilteredAction(SupplierZoneLoggableEntity.Instance, input);
            return Vanrise.Common.DataRetrievalManager.Instance.ProcessResult(input, allsupplierZones.ToBigResult(input, filterExpression, SupplierZoneDetailMapper), handler);
        }

        public IEnumerable<SupplierZoneInfo> GetSupplierZoneInfo(SupplierZoneInfoFilter filter, int supplierId, string searchValue)
        {
            string nameFilterLower = searchValue != null ? searchValue.ToLower() : null;
            DateTime today = DateTime.Today;

            IEnumerable<SupplierZone> supplierZones = GetSupplierZonesBySupplier(supplierId);

            Func<SupplierZone, bool> filterExpression = (supplierZone) =>
            {
                if (filter.CountryIds != null && !filter.CountryIds.Contains(supplierZone.CountryId))
                    return false;

                if (!supplierZone.IsEffective(filter.EffectiveMode, today))
                    return false;

                if (nameFilterLower != null && !supplierZone.Name.ToLower().Contains(nameFilterLower))
                    return false;

                if (filter.AvailableZoneIds != null && !filter.AvailableZoneIds.Contains(supplierZone.SupplierZoneId))
                    return false;

                if (filter.ExcludedZoneIds != null && filter.ExcludedZoneIds.Contains(supplierZone.SupplierZoneId))
                    return false;

                return true;
            };

            return supplierZones.MapRecords(SupplierZoneInfoMapper, filterExpression).OrderBy(item => item.Name);
        }
        public IEnumerable<SupplierZone> GetSupplierZonesBySupplierId(int supplierId)
        {
            IEnumerable<SupplierZone> allSupplierZones = GetCachedSupplierZones().Values;
            return allSupplierZones.FindAllRecords(x => x.SupplierId == supplierId);
        }
        public SupplierZone GetSupplierZoneByCode(int supplierId, string codeNumber)
        {
            SupplierCodeManager supplierCodeManager = new SupplierCodeManager();
            var supplierCodes = supplierCodeManager.GetEffectiveSupplierCodesByCode(supplierId, codeNumber);
            if (supplierCodes == null || supplierCodes.Count() == 0)
                return null;

            var supplierZones = GetSupplierZonesBySupplierId(supplierId);
            if (supplierZones == null || supplierZones.Count() == 0)
                return null;

            var supplierCodesOfEffectiveZones = supplierCodes.FindAllRecords(x => supplierZones.Any(y => y.SupplierZoneId == x.ZoneId));
            if (supplierCodesOfEffectiveZones == null || supplierCodesOfEffectiveZones.Count() == 0)
                return null;
            var supplierCode = supplierCodesOfEffectiveZones.FirstOrDefault();
            if (supplierCode == null)
                return null;

            return GetSupplierZone(supplierCode.ZoneId);
        }
        public string GetSupplierZoneNameBySupplierId(int supplierId, string codeNumber)
        {
           var supplierZone = GetSupplierZoneByCode(supplierId, codeNumber);
            supplierZone.ThrowIfNull("supplierZone",supplierZone);
            return supplierZone.Name;
        }

        public IEnumerable<SupplierZoneInfo> GetSupplierZonesInfo(int supplierId)
        {
            IEnumerable<SupplierZone> supplierZones = GetCachedSupplierZones().Values;
            supplierZones = supplierZones.FindAllRecords(item => item.SupplierId == supplierId).OrderBy(item => item.Name);

            return supplierZones.MapRecords(SupplierZoneInfoMapper);
        }

        public IEnumerable<int> GetDistinctSupplierIdsBySupplierZoneIds(IEnumerable<long> supplierZoneIds)
        {
            return this.GetCachedSupplierZones().MapRecords(zone => zone.SupplierId, zone => supplierZoneIds.Contains(zone.SupplierZoneId)).Distinct();
        }

        public List<SupplierZone> GetSupplierZonesEffectiveAfter(int supplierId, DateTime minimumDate)
        {
            ISupplierZoneDataManager dataManager = BEDataManagerFactory.GetDataManager<ISupplierZoneDataManager>();
            return dataManager.GetSupplierZonesEffectiveAfter(supplierId, minimumDate);
        }

        public List<SupplierZone> GetSupplierZones(int supplierId, DateTime effectiveDate)
        {
            ISupplierZoneDataManager dataManager = BEDataManagerFactory.GetDataManager<ISupplierZoneDataManager>();
            return dataManager.GetSupplierZones(supplierId, effectiveDate);
        }

        public List<SupplierZone> GetEffectiveSupplierZones(int supplierId, DateTime? effectiveOn, bool isEffectiveInFuture)
        {
            ISupplierZoneDataManager dataManager = BEDataManagerFactory.GetDataManager<ISupplierZoneDataManager>();
            return dataManager.GetEffectiveSupplierZones(supplierId, effectiveOn, isEffectiveInFuture);
        }

        public SupplierZone GetSupplierZone(long zoneId)
        {
            var supplierZones = GetCachedSupplierZones();
            return supplierZones.GetRecord(zoneId);
        }

        public string GetSupplierZoneName(long zoneId)
        {
            SupplierZone supplierZone = GetSupplierZone(zoneId);
            return supplierZone != null ? supplierZone.Name : null;
        }

        public IEnumerable<SupplierZoneInfo> GetSupplierZoneInfoByIds(List<long> selectedIds)
        {
            var allSupplierZones = GetCachedSupplierZones();
            return allSupplierZones.MapRecords(SupplierZoneInfoMapper, x => selectedIds.Contains(x.SupplierZoneId)).OrderBy(x => x.Name);
        }
        public IEnumerable<long> GetSupplierZoneIdsEffectiveByZoneName(String ZoneName, DateTime BED, int supplierId)
        {
            var allSupplierZones = GetCachedSupplierZones();
           
            return allSupplierZones.Values.Where(x => ((supplierId == x.SupplierId) && ZoneName.Equals(x.Name) && (!x.EED.HasValue || x.EED > BED))).Select(it => it.SupplierZoneId);
        }

        public long ReserveIDRange(int numberOfIDs)
        {
            long startingId;
            IDManager.Instance.ReserveIDRange(GetSupplierZoneType(), numberOfIDs, out startingId);
            return startingId;
        }

        public int GetSupplierZoneTypeId()
        {
            return Vanrise.Common.Business.TypeManager.Instance.GetTypeId(this.GetSupplierZoneType());
        }

        public Type GetSupplierZoneType()
        {
            return this.GetType();
        }

        public string GetDescription(IEnumerable<long> supplierZoneIds)
        {
            IEnumerable<SupplierZone> supplierZones = GetCachedSupplierZones().Values;
            Func<SupplierZone, bool> filterExpression = null;
            if (supplierZoneIds != null)
                filterExpression = (itm) => (supplierZoneIds.Contains(itm.SupplierZoneId));
            var supplierZoneIdsValues = supplierZones.FindAllRecords(filterExpression);
            if (supplierZoneIdsValues != null)
                return string.Join(", ", supplierZoneIdsValues.Select(x => x.Name));
            return string.Empty;
        }

        public IEnumerable<SupplierZoneGroupTemplate> GetSupplierZoneGroupTemplates()
        {
            var templateConfigManager = new ExtensionConfigurationManager();
            return templateConfigManager.GetExtensionConfigurations<SupplierZoneGroupTemplate>(SupplierZoneGroupTemplate.EXTENSION_TYPE);
        }

        public SupplierZone GetEffectiveSupplierZoneByZoneIds(IEnumerable<long> zoneIds, DateTime effectiveDate)
        {
            if(zoneIds != null)
            {
                foreach (var zoneId in zoneIds)
                {
                    var supplierZone = GetSupplierZone(zoneId);
                    if (supplierZone.IsInTimeRange(effectiveDate))
                        return supplierZone;
                }
            }

            return null;
        }

        #endregion

        #region Private Members
        private class SupplierZoneExcelExportHandler : ExcelExportHandler<SupplierZoneDetails>
        {
            public override void ConvertResultToExcelData(IConvertResultToExcelDataContext<SupplierZoneDetails> context)
            {
                ExportExcelSheet sheet = new ExportExcelSheet()
                {
                    SheetName = "Supplier Zones",
                    Header = new ExportExcelHeader { Cells = new List<ExportExcelHeaderCell>() }
                };

                sheet.Header.Cells.Add(new ExportExcelHeaderCell { Title = "ID" });
                sheet.Header.Cells.Add(new ExportExcelHeaderCell { Title = "Name" });
                sheet.Header.Cells.Add(new ExportExcelHeaderCell { Title = "Country" });
                sheet.Header.Cells.Add(new ExportExcelHeaderCell { Title = "Supplier" });
                sheet.Header.Cells.Add(new ExportExcelHeaderCell { Title = "BED", CellType = ExcelCellType.DateTime, DateTimeType = DateTimeType.Date });
                sheet.Header.Cells.Add(new ExportExcelHeaderCell { Title = "EED", CellType = ExcelCellType.DateTime, DateTimeType = DateTimeType.Date });
                sheet.Rows = new List<ExportExcelRow>();
                if (context.BigResult != null && context.BigResult.Data != null)
                {
                    foreach (var record in context.BigResult.Data)
                    {
                        if (record.Entity != null)
                        {
                            var row = new ExportExcelRow { Cells = new List<ExportExcelCell>() };
                            sheet.Rows.Add(row);
                            row.Cells.Add(new ExportExcelCell { Value = record.Entity.SupplierZoneId });
                            row.Cells.Add(new ExportExcelCell { Value = record.Entity.Name });
                            row.Cells.Add(new ExportExcelCell { Value = record.CountryName });
                            row.Cells.Add(new ExportExcelCell { Value = record.SupplierName });
                            row.Cells.Add(new ExportExcelCell { Value = record.Entity.BED });
                            row.Cells.Add(new ExportExcelCell { Value = record.Entity.EED });
                        }
                    }
                }
                context.MainSheet = sheet;
            }
        }
        private class CacheManager : Vanrise.Caching.BaseCacheManager
        {
            ISupplierZoneDataManager _dataManager = BEDataManagerFactory.GetDataManager<ISupplierZoneDataManager>();
            object _updateHandle;

            public override T GetOrCreateObject<T>(object cacheName, Func<T> createObject)
            {
                return GetOrCreateObject(cacheName, SupplierZonesCacheExpirationChecker.Instance, createObject);
            }

            public override Vanrise.Caching.CacheObjectSize ApproximateObjectSize
            {
                get
                {
                    return Vanrise.Caching.CacheObjectSize.ExtraLarge;
                }
            }

            protected override bool ShouldSetCacheExpired(object parameter)
            {
                return _dataManager.AreSupplierZonesUpdated(ref _updateHandle);
            }
        }

        private SupplierZoneInfo SupplierZoneInfoMapper(SupplierZone supplierZone)
        {
            return new SupplierZoneInfo()
            {
                SupplierZoneId = supplierZone.SupplierZoneId,
                Name = supplierZone.Name,
            };
        }
        private SupplierZone SupplierZoneIdsMapper(SupplierZone supplierZone)
        {
            return supplierZone;

        }
        private SupplierZoneDetails SupplierZoneDetailMapper(SupplierZone supplierZone)
        {
            SupplierZoneDetails supplierZoneDetail = new SupplierZoneDetails();

            supplierZoneDetail.Entity = supplierZone;

            CountryManager countryManager = new CountryManager();
            CarrierAccountManager caManager = new CarrierAccountManager();
            var country = countryManager.GetCountry(supplierZone.CountryId);
            if (country != null)
                supplierZoneDetail.CountryName = country.Name;

            int supplierId = supplierZone.SupplierId;
            supplierZoneDetail.SupplierName = caManager.GetCarrierAccountName(supplierId);
            return supplierZoneDetail;
        }

        #endregion

        #region Private Methods
        private IEnumerable<SupplierZone> GetSupplierZonesBySupplier(int supplierId)
        {
            IEnumerable<SupplierZone> supplierZones = GetCachedSupplierZones().Values;
            return supplierZones.FindAllRecords(item => item.SupplierId == supplierId);
        }

        #endregion
        private class SupplierZoneLoggableEntity : VRLoggableEntityBase
        {
            public static SupplierZoneLoggableEntity Instance = new SupplierZoneLoggableEntity();

            private SupplierZoneLoggableEntity()
            {

            }



            public override string EntityUniqueName
            {
                get { return "WhS_BusinessEntity_SupplierZone"; }
            }

            public override string ModuleName
            {
                get { return "Business Entity"; }
            }

            public override string EntityDisplayName
            {
                get { return "Supplier Zone"; }
            }

            public override string ViewHistoryItemClientActionName
            {
                get { return "WhS_BusinessEntity_SupplierZone_ViewHistoryItem"; }
            }

            public override object GetObjectId(IVRLoggableEntityGetObjectIdContext context)
            {
                SupplierZone supplierZone = context.Object.CastWithValidate<SupplierZone>("context.Object");
                return supplierZone.SupplierZoneId;
            }

            public override string GetObjectName(IVRLoggableEntityGetObjectNameContext context)
            {
                return null;
            }
        }
        #region IBusinessEntityManager

        public dynamic GetEntity(IBusinessEntityGetByIdContext context)
        {
            return GetSupplierZone(context.EntityId);
        }

        public List<dynamic> GetAllEntities(IBusinessEntityGetAllContext context)
        {
            var allZones = GetCachedSupplierZones();
            if (allZones == null)
                return null;
            else
                return allZones.Values.Select(itm => itm as dynamic).ToList();
        }

        public string GetEntityDescription(IBusinessEntityDescriptionContext context)
        {
            return GetSupplierZoneName(Convert.ToInt64(context.EntityId));
        }

        public dynamic GetEntityId(IBusinessEntityIdContext context)
        {
            var supplierZone = context.Entity as SupplierZone;
            return supplierZone.SupplierZoneId;
        }

        public bool IsCacheExpired(IBusinessEntityIsCacheExpiredContext context, ref DateTime? lastCheckTime)
        {
            return Vanrise.Caching.CacheManagerFactory.GetCacheManager<CacheManager>().IsCacheExpired(ref lastCheckTime);
        }

        public IEnumerable<dynamic> GetIdsByParentEntityId(IBusinessEntityGetIdsByParentEntityIdContext context)
        {
            Func<SupplierZone, bool> filter;
            switch (context.ParentEntityDefinition.Name)
            {
                case Vanrise.Entities.Country.BUSINESSENTITY_DEFINITION_NAME: filter = (zone) => zone.CountryId == context.ParentEntityId; break;
                default: throw new NotImplementedException(String.Format("Business Entity Definition Name '{0}'", context.ParentEntityDefinition.Name));
            }
            return GetCachedSupplierZones().FindAllRecords(filter).MapRecords(zone => zone.SupplierZoneId as dynamic);
        }

        public dynamic GetParentEntityId(IBusinessEntityGetParentEntityIdContext context)
        {
            var supplierZone = context.Entity as SupplierZone;
            if (supplierZone == null)
                throw new NullReferenceException("supplierZone");
            switch (context.ParentEntityDefinition.Name)
            {
                case Vanrise.Entities.Country.BUSINESSENTITY_DEFINITION_NAME: return supplierZone.CountryId;
                default: throw new NotImplementedException(String.Format("Business Entity Definition Name '{0}'", context.ParentEntityDefinition.Name));
            }
        }

        public dynamic MapEntityToInfo(IBusinessEntityMapToInfoContext context)
        {
            throw new NotImplementedException();
        }

        #endregion
    }
}