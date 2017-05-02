using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Vanrise.Entities;
using TOne.WhS.BusinessEntity.Entities;
using Vanrise.Caching;
using TOne.WhS.BusinessEntity.Data;
using Vanrise.Common;
using Vanrise.Common.Business;

namespace TOne.WhS.BusinessEntity.Business
{
    public class ZoneServiceConfigManager
    {

        #region ctor/Local Variables
        #endregion

        #region Public Methods

        public IEnumerable<ZoneServiceConfig> GetAllServices()
        {
            var allServices = GetCachedZoneServiceConfigs();
            if (allServices == null)
                return null;

            return allServices.Values;
        }


        public IDataRetrievalResult<ZoneServiceConfigDetail> GetFilteredZoneServiceConfigs(DataRetrievalInput<ZoneServiceConfigQuery> input)
        {
            var allZoneServiceConfigs = GetCachedZoneServiceConfigs();

            Func<ZoneServiceConfig, bool> filterExpression = (x) =>
            {
                if (!string.IsNullOrEmpty(input.Query.Name) && !x.Settings.Name.ToLower().Contains(input.Query.Name.ToLower()))
                    return false;
                return true;
            };

            ResultProcessingHandler<ZoneServiceConfigDetail> handler = new ResultProcessingHandler<ZoneServiceConfigDetail>()
            {
                ExportExcelHandler = new ZoneServiceConfigExcelExportHandler()
            };
            VRActionLogger.Current.LogGetFilteredAction(ZoneServiceConfigLoggableEntity.Instance, input);
            return Vanrise.Common.DataRetrievalManager.Instance.ProcessResult(input, allZoneServiceConfigs.ToBigResult(input, filterExpression, ZoneServiceConfigDetailMapper), handler);
        }

        public Dictionary<int, ZoneServiceConfig> GetCachedZoneServiceConfigs()
        {
            return Vanrise.Caching.CacheManagerFactory.GetCacheManager<CacheManager>().GetOrCreateObject("GetZoneServiceConfigs",
               () =>
               {
                   IZoneServiceConfigDataManager dataManager = BEDataManagerFactory.GetDataManager<IZoneServiceConfigDataManager>();
                   IEnumerable<ZoneServiceConfig> rateTypes = dataManager.GetZoneServiceConfigs();
                   return rateTypes.ToDictionary(x => x.ZoneServiceConfigId, x => x);
               });
        }

        public string GetZoneServiceConfigName(ZoneServiceConfig zoneServiceConfig)
        {
            return (zoneServiceConfig != null) ? zoneServiceConfig.Settings.Name : null;
        }

        public IEnumerable<ZoneServiceConfigInfo> GetAllZoneServiceConfigs(ZoneServiceConfigFilter filter)
        {
            var allZoneServiceConfigs = GetCachedZoneServiceConfigs();
            if (allZoneServiceConfigs == null)
                return null;

            Func<ZoneServiceConfig, bool> filterExpression = null;

            if (filter != null)
            {
                List<int> childrenServiceIds = new List<int>();
                if (filter.AssinableToServiceId != null)
                    this.GetChildServices(filter.AssinableToServiceId.Value, childrenServiceIds);

                filterExpression = (prod) =>
                {
                    if (filter.AssinableToServiceId != null && filter.AssinableToServiceId == prod.ZoneServiceConfigId)
                        return false;
                    if (childrenServiceIds.Count > 0 && childrenServiceIds.Contains(prod.ZoneServiceConfigId))
                        return false;

                    return true;
                };
            }
            return allZoneServiceConfigs.FindAllRecords(filterExpression).OrderBy(it => it.Settings.Weight).MapRecords(ZoneServiceConfigInfoMapper);
        }

        public IEnumerable<ZoneServiceConfig> GetAllZoneServices()
        {
            var allZoneServiceConfigs = GetCachedZoneServiceConfigs();
            if (allZoneServiceConfigs == null)
                return null;
            return allZoneServiceConfigs.Values;
        }

        public IEnumerable<ZoneServiceConfig> GetAllZoneServicesByIds(HashSet<int> serviceIds)
        {
            if (serviceIds == null)
                return null;

            var allZoneServiceConfigs = GetCachedZoneServiceConfigs();
            if (allZoneServiceConfigs == null)
                return null;

            Func<ZoneServiceConfig, bool> filterExpression = (itm) => serviceIds.Contains(itm.ZoneServiceConfigId);
            return allZoneServiceConfigs.FindAllRecords(filterExpression);
        }

        public string GetZoneServicesNames(List<int> services)
        {
            var allZoneServiceConfigs = GetCachedZoneServiceConfigs().FindAllRecords(x => services.Contains(x.Key));
            if (allZoneServiceConfigs == null)
                return null;
            return String.Join(",", allZoneServiceConfigs.Select(x => x.Value.Symbol).ToList());
        }

        public Dictionary<int, string> GetZoneServicesNamesDict(IEnumerable<int> services)
        {
            var allZoneServiceConfigs = GetCachedZoneServiceConfigs().FindAllRecords(x => services.Contains(x.Key));
            if (allZoneServiceConfigs == null)
                return null;

            return allZoneServiceConfigs.ToDictionary(x => x.Key, x => x.Value.Symbol);
        }

        public ZoneServiceConfig GetZoneServiceConfig(int zoneServiceConfigId, bool isViewedFromUI)
        {
            var allZoneServiceConfigs = GetCachedZoneServiceConfigs();
            var zoneServiceConfigItem = allZoneServiceConfigs.GetRecord(zoneServiceConfigId);
            if (zoneServiceConfigItem != null && isViewedFromUI)
                VRActionLogger.Current.LogObjectViewed(ZoneServiceConfigLoggableEntity.Instance, zoneServiceConfigItem);
            return zoneServiceConfigItem;
        }
        public ZoneServiceConfig GetZoneServiceConfig(int zoneServiceConfigId)
        {
            return GetZoneServiceConfig(zoneServiceConfigId, false);
        }
        public string GetServiceSymbol(int serviceId)
        {
            ZoneServiceConfig zoneServiceConfig = GetZoneServiceConfig(serviceId);

            if (zoneServiceConfig == null)
                throw new NullReferenceException(string.Format("zoneServiceConfig Id: {0} does not exist", serviceId));

            return zoneServiceConfig.Symbol;
        }

        public IEnumerable<ZoneServiceConfig> GetDistinctZoneServiceConfigsWithChildren(IEnumerable<int> zoneServiceIds, bool getOnlyDirectChildren = false)
        {
            if (zoneServiceIds == null || zoneServiceIds.Count() == 0)
                return null;

            Dictionary<int, Dictionary<int, ZoneServiceConfig>> result = GetAllZoneServiceConfigsWithChildren(getOnlyDirectChildren);
            Dictionary<int, ZoneServiceConfig> zoneServiceConfigs = new Dictionary<int, ZoneServiceConfig>();
            ZoneServiceConfig tempZoneServiceConfig;

            foreach (int zoneServiceId in zoneServiceIds)
            {
                Dictionary<int, ZoneServiceConfig> zoneServiceChildren = result.GetRecord(zoneServiceId);
                if (zoneServiceChildren == null || zoneServiceChildren.Count == 0)
                    continue;
                foreach (KeyValuePair<int, ZoneServiceConfig> zoneServiceChild in zoneServiceChildren)
                {
                    if (!zoneServiceConfigs.TryGetValue(zoneServiceChild.Key, out tempZoneServiceConfig))
                    {
                        zoneServiceConfigs.Add(zoneServiceChild.Key, zoneServiceChild.Value);
                    }
                }
            }
            return zoneServiceConfigs.Values;
        }

        private Dictionary<int, Dictionary<int, ZoneServiceConfig>> GetAllZoneServiceConfigsWithChildren(bool getOnlyDirectChildren)
        {
            string cacheName;
            if (getOnlyDirectChildren)
                cacheName = "GetAllZoneServicesWithDirectChildren";
            else
                cacheName = "GetAllZoneServicesWithAllChildren";

            return Vanrise.Caching.CacheManagerFactory.GetCacheManager<CacheManager>().GetOrCreateObject(cacheName,
               () =>
               {
                   Dictionary<int, Dictionary<int, ZoneServiceConfig>> result = new Dictionary<int, Dictionary<int, ZoneServiceConfig>>();
                   IEnumerable<ZoneServiceConfig> allServices = GetAllServices();
                   foreach (ZoneServiceConfig zoneServiceConfig in allServices)
                   {
                       Dictionary<int, ZoneServiceConfig> zoneServiceConfigs = new Dictionary<int, ZoneServiceConfig>();
                       zoneServiceConfigs.Add(zoneServiceConfig.ZoneServiceConfigId, zoneServiceConfig);
                       result.Add(zoneServiceConfig.ZoneServiceConfigId, zoneServiceConfigs);
                       BuildZoneServicesWithChildren(new List<int>() { zoneServiceConfig.ZoneServiceConfigId }, zoneServiceConfigs, allServices, getOnlyDirectChildren);
                   }

                   return result;
               });
        }

        public InsertOperationOutput<ZoneServiceConfigDetail> AddZoneServiceConfig(ZoneServiceConfig zoneServiceConfig)
        {
            InsertOperationOutput<ZoneServiceConfigDetail> insertOperationOutput = new InsertOperationOutput<ZoneServiceConfigDetail>();

            insertOperationOutput.Result = Vanrise.Entities.InsertOperationResult.Failed;
            if (GetCachedZoneServiceConfigs().FindRecord(x => x.Value.Settings.Color.ToLower() == zoneServiceConfig.Settings.Color.ToLower()).Value != null)
            {
                insertOperationOutput.Result = Vanrise.Entities.InsertOperationResult.SameExists;
                return insertOperationOutput;
            }

            insertOperationOutput.InsertedObject = null;
            int ZoneServiceConfigtId = -1;

            IZoneServiceConfigDataManager dataManager = BEDataManagerFactory.GetDataManager<IZoneServiceConfigDataManager>();
            bool insertActionSucc = dataManager.Insert(zoneServiceConfig, out ZoneServiceConfigtId);
            if (insertActionSucc)
            {
                Vanrise.Caching.CacheManagerFactory.GetCacheManager<CacheManager>().SetCacheExpired();
                zoneServiceConfig.ZoneServiceConfigId = ZoneServiceConfigtId;
                VRActionLogger.Current.TrackAndLogObjectAdded(ZoneServiceConfigLoggableEntity.Instance, zoneServiceConfig);
                insertOperationOutput.Result = Vanrise.Entities.InsertOperationResult.Succeeded;
                insertOperationOutput.InsertedObject = ZoneServiceConfigDetailMapper(zoneServiceConfig);
            }
            else
            {
                insertOperationOutput.Result = Vanrise.Entities.InsertOperationResult.SameExists;
            }

            return insertOperationOutput;
        }
        public UpdateOperationOutput<ZoneServiceConfigDetail> UpdateZoneServiceConfig(ZoneServiceConfig zoneServiceConfig)
        {
            IZoneServiceConfigDataManager dataManager = BEDataManagerFactory.GetDataManager<IZoneServiceConfigDataManager>();

            UpdateOperationOutput<ZoneServiceConfigDetail> updateOperationOutput = new UpdateOperationOutput<ZoneServiceConfigDetail>();

            updateOperationOutput.Result = Vanrise.Entities.UpdateOperationResult.Failed;
            if (GetCachedZoneServiceConfigs().FindRecord(x => x.Value.Settings.Color.ToLower() == zoneServiceConfig.Settings.Color.ToLower() && x.Key != zoneServiceConfig.ZoneServiceConfigId).Value != null)
            {
                updateOperationOutput.Result = Vanrise.Entities.UpdateOperationResult.SameExists;
                return updateOperationOutput;
            }

            bool updateActionSucc = dataManager.Update(zoneServiceConfig);
            updateOperationOutput.UpdatedObject = null;

            if (updateActionSucc)
            {
                Vanrise.Caching.CacheManagerFactory.GetCacheManager<CacheManager>().SetCacheExpired();
                VRActionLogger.Current.TrackAndLogObjectUpdated(ZoneServiceConfigLoggableEntity.Instance, zoneServiceConfig);
                updateOperationOutput.Result = Vanrise.Entities.UpdateOperationResult.Succeeded;
                updateOperationOutput.UpdatedObject = ZoneServiceConfigDetailMapper(zoneServiceConfig);
            }
            else
            {
                updateOperationOutput.Result = Vanrise.Entities.UpdateOperationResult.SameExists;
            }

            return updateOperationOutput;
        }
        #endregion

        #region Private Methods

        private void BuildZoneServicesWithChildren(List<int> zoneServiceConfigIds, Dictionary<int, ZoneServiceConfig> zoneServiceConfigs, IEnumerable<ZoneServiceConfig> allServices, bool getOnlyDirectChildren)
        {
            if (zoneServiceConfigIds == null || zoneServiceConfigIds.Count == 0)
                return;

            List<int> childZoneServiceConfigIds = new List<int>();
            foreach (int zoneServiceConfigId in zoneServiceConfigIds)
            {
                IEnumerable<ZoneServiceConfig> childServices = allServices.FindAllRecords(x => x.Settings.ParentId != null && x.Settings.ParentId == zoneServiceConfigId);
                if (childServices != null && childServices.Count() > 0)
                {
                    childZoneServiceConfigIds.AddRange(GetAndAddNonDuplicateZoneServiceConfigIds(childServices, zoneServiceConfigs));
                }
            }
            if (getOnlyDirectChildren)
                return;

            BuildZoneServicesWithChildren(childZoneServiceConfigIds, zoneServiceConfigs, allServices, getOnlyDirectChildren);
        }

        private List<int> GetAndAddNonDuplicateZoneServiceConfigIds(IEnumerable<ZoneServiceConfig> zoneServiceConfigList, Dictionary<int, ZoneServiceConfig> zoneServiceConfigs)
        {
            List<int> newAddedServiceConfigIds = new List<int>();
            foreach (ZoneServiceConfig zoneServiceConfig in zoneServiceConfigList)
            {
                ZoneServiceConfig duplicateZoneServices = zoneServiceConfigs.GetRecord(zoneServiceConfig.ZoneServiceConfigId);
                if (duplicateZoneServices == null)
                {
                    newAddedServiceConfigIds.Add(zoneServiceConfig.ZoneServiceConfigId);
                    zoneServiceConfigs.Add(zoneServiceConfig.ZoneServiceConfigId, zoneServiceConfig);
                }
            }
            return newAddedServiceConfigIds;
        }

        private void GetChildServices(int serviceId, List<int> childrenServiceIds)
        {
            IEnumerable<ZoneServiceConfig> childServices = GetAllServices().FindAllRecords(x => x.Settings.ParentId != null && x.Settings.ParentId == serviceId);
            if (childServices == null)
                return;

            foreach (ZoneServiceConfig child in childServices)
            {
                childrenServiceIds.Add(child.ZoneServiceConfigId);
                GetChildServices(child.ZoneServiceConfigId, childrenServiceIds);
            }
        }

        private class ZoneServiceConfigExcelExportHandler : ExcelExportHandler<ZoneServiceConfigDetail>
        {
            public override void ConvertResultToExcelData(IConvertResultToExcelDataContext<ZoneServiceConfigDetail> context)
            {
                ExportExcelSheet sheet = new ExportExcelSheet()
                {
                    SheetName = "Zone Service Configuration",
                    Header = new ExportExcelHeader { Cells = new List<ExportExcelHeaderCell>() }
                };

                sheet.Header.Cells.Add(new ExportExcelHeaderCell { Title = "Id" });
                sheet.Header.Cells.Add(new ExportExcelHeaderCell { Title = "Symbol" });
                sheet.Header.Cells.Add(new ExportExcelHeaderCell { Title = "Name" });
                sheet.Header.Cells.Add(new ExportExcelHeaderCell { Title = "Weight" });
                sheet.Header.Cells.Add(new ExportExcelHeaderCell { Title = "Description", Width = 35 });
                sheet.Header.Cells.Add(new ExportExcelHeaderCell { Title = "Flag" });
                sheet.Header.Cells.Add(new ExportExcelHeaderCell { Title = "ParentName" });

                sheet.Rows = new List<ExportExcelRow>();
                if (context.BigResult != null && context.BigResult.Data != null)
                {
                    foreach (var record in context.BigResult.Data)
                    {
                        if (record.Entity != null)
                        {
                            var row = new ExportExcelRow { Cells = new List<ExportExcelCell>() };
                            sheet.Rows.Add(row);
                            row.Cells.Add(new ExportExcelCell { Value = record.Entity.ZoneServiceConfigId });
                            row.Cells.Add(new ExportExcelCell { Value = record.Entity.Symbol });
                            row.Cells.Add(new ExportExcelCell { Value = record.Entity.Settings.Name });
                            row.Cells.Add(new ExportExcelCell { Value = record.Entity.Settings.Weight });
                            row.Cells.Add(new ExportExcelCell { Value = record.Entity.Settings.Description });
                            row.Cells.Add(new ExportExcelCell { Value = record.Entity.Settings.Color });
                            row.Cells.Add(new ExportExcelCell { Value = record.ParentName });
                        }
                    }
                }
                context.MainSheet = sheet;
            }
        }

        private class CacheManager : BaseCacheManager
        {
            IZoneServiceConfigDataManager dataManager = BEDataManagerFactory.GetDataManager<IZoneServiceConfigDataManager>();

            object _updateHandle;

            protected override bool ShouldSetCacheExpired(object parameter)
            {
                return dataManager.AreZoneServiceConfigsUpdated(ref _updateHandle);
            }
        }

        #endregion

        private class ZoneServiceConfigLoggableEntity : VRLoggableEntityBase
        {
            public static ZoneServiceConfigLoggableEntity Instance = new ZoneServiceConfigLoggableEntity();

            private ZoneServiceConfigLoggableEntity()
            {

            }

            static ZoneServiceConfigManager s_zoneServiceConfigManager = new ZoneServiceConfigManager();

            public override string EntityUniqueName
            {
                get { return "WhS_BusinessEntity_ZoneServiceConfig"; }
            }

            public override string ModuleName
            {
                get { return "Business Entity"; }
            }

            public override string EntityDisplayName
            {
                get { return "Zone Service Config"; }
            }

            public override string ViewHistoryItemClientActionName
            {
                get { return "WhS_BusinessEntity_ZoneServiceConfig_ViewHistoryItem"; }
            }

            public override object GetObjectId(IVRLoggableEntityGetObjectIdContext context)
            {
                ZoneServiceConfig zoneServiceConfig = context.Object.CastWithValidate<ZoneServiceConfig>("context.Object");
                return zoneServiceConfig.ZoneServiceConfigId;
            }

            public override string GetObjectName(IVRLoggableEntityGetObjectNameContext context)
            {
                ZoneServiceConfig zoneServiceConfig = context.Object.CastWithValidate<ZoneServiceConfig>("context.Object");
                return s_zoneServiceConfigManager.GetZoneServiceConfigName(zoneServiceConfig);

            }
        }

        #region  Mappers
        private ZoneServiceConfigInfo ZoneServiceConfigInfoMapper(ZoneServiceConfig zoneServiceConfig)
        {
            return new ZoneServiceConfigInfo()
            {
                ZoneServiceConfigId = zoneServiceConfig.ZoneServiceConfigId,
                Symbol = zoneServiceConfig.Symbol
            };
        }

        private ZoneServiceConfigDetail ZoneServiceConfigDetailMapper(ZoneServiceConfig zoneServiceConfig)
        {
            ZoneServiceConfigDetail zoneServiceConfigDetail = new ZoneServiceConfigDetail()
            {
                Entity = zoneServiceConfig
            };
            if (zoneServiceConfig.Settings.ParentId.HasValue)
            {

                var parentzoneServiceConfig = GetZoneServiceConfig(zoneServiceConfig.Settings.ParentId.Value);
                if (parentzoneServiceConfig == null)
                    throw new DataIntegrityValidationException(String.Format("Parent Zone Service  '{0}' does not exist", (zoneServiceConfig.Settings.ParentId)));

                zoneServiceConfigDetail.ParentName = parentzoneServiceConfig != null ? parentzoneServiceConfig.Settings.Name : null;
            }
            return zoneServiceConfigDetail;

        }
        #endregion

    }
}