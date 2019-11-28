using System;
using System.Collections.Generic;
using System.Linq;
using Vanrise.Analytic.Data;
using Vanrise.Analytic.Entities;
using Vanrise.Caching;
using Vanrise.Common;
using Vanrise.Common.Business;
using Vanrise.Entities;
using Vanrise.Security.Business;

namespace Vanrise.Analytic.Business
{
    public class AnalyticTableManager : IAnalyticTableManager
    {
        VRDevProjectManager vrDevProjectManager = new VRDevProjectManager();

        #region Public Methods
        public Vanrise.Entities.IDataRetrievalResult<AnalyticTableDetail> GetFilteredAnalyticTables(Vanrise.Entities.DataRetrievalInput<AnalyticTableQuery> input)
        {
            var analyticTables = GetCachedAnalyticTables();

            Func<AnalyticTable, bool> filterExpression = (prod) =>
            {
                if (Utilities.ShouldHideItemHavingDevProjectId(prod.DevProjectId))
                    return false;

                if (input.Query.Name != null && !prod.Name.ToLower().Contains(input.Query.Name.ToLower()))
                    return false;
                if (input.Query.DevProjectIds != null && (!prod.DevProjectId.HasValue || !input.Query.DevProjectIds.Contains(prod.DevProjectId.Value)))
                    return false;
                return true;
            };
            VRActionLogger.Current.LogGetFilteredAction(AnalyticTableLoggableEntity.Instance, input);
            return Vanrise.Common.DataRetrievalManager.Instance.ProcessResult(input, analyticTables.ToBigResult(input, filterExpression, AnalyticTableDetailMapper));
        }
        public IEnumerable<AnalyticTableClientDetail> GetClientAnalyitTableInfo()
        {
            List<AnalyticTableClientDetail> analyticTableClientDetails = new List<AnalyticTableClientDetail>();
            var analyticTables = GetCachedAnalyticTables();
            foreach (var analyticTable in analyticTables.Values)
            {
                var analyticTableClientDetail = new AnalyticTableClientDetail()
                {
                    AnalyticTableId = analyticTable.AnalyticTableId,
                    Name = analyticTable.Name
                };
                analyticTableClientDetails.Add(analyticTableClientDetail);
            }

            return analyticTableClientDetails;

        }
        public MeasureStyleRuleEditorRuntime GetAnalyticTableMergedMeasureStylesEditorRuntime(Guid analyticTbaleId)
        {
            MeasureStyleRuleManager measureStyleRuleManager = new MeasureStyleRuleManager();
            var mergedMeasureStyles = GetMergedMeasureStyles(analyticTbaleId);
            return measureStyleRuleManager.GetMeasureStyleRuleEditorRuntime(mergedMeasureStyles, analyticTbaleId);
        }
        public List<MeasureStyleRule> GetMergedMeasureStyles (Guid analyticTableId)
        {
            ConfigManager configManager = new ConfigManager();
            List<MeasureStyleRule> mergedMeasureStyles = new List<MeasureStyleRule>();
            var analyticTable = GetAnalyticTableById(analyticTableId);
            analyticTable.ThrowIfNull("analyticTable");
            var analyticTableMeasureStyles = analyticTable.MeasureStyles;
            var analyticTableKpiSettings = configManager.GetAnalytictableKPISettings(analyticTableId);
            if (analyticTableKpiSettings != null)
            {
                var analyticTableKPIMeasureStyles = analyticTableKpiSettings.MeasureStyleRules;

                if (analyticTableKPIMeasureStyles != null && analyticTableKPIMeasureStyles.Count() > 0)
                    mergedMeasureStyles.AddRange(analyticTableKPIMeasureStyles);
            }

            if (analyticTableMeasureStyles != null && analyticTableMeasureStyles.MeasureStyleRules != null && analyticTableMeasureStyles.MeasureStyleRules.Count() > 0)
            {
                foreach (var measureStyleRule in analyticTableMeasureStyles.MeasureStyleRules)
                {
                    if (!mergedMeasureStyles.Any(x => x.MeasureName.Equals(measureStyleRule.MeasureName)))
                        mergedMeasureStyles.Add(measureStyleRule);
                }
            }
            return mergedMeasureStyles;
        }
        public Vanrise.Entities.UpdateOperationOutput<AnalyticTableDetail> SaveAnalyticTableMeasureStyles(AnalyticTableMeasureStyles measureStyles, Guid analyticTableId)
        {
            UpdateOperationOutput<AnalyticTableDetail> updateOperationOutput = new Vanrise.Entities.UpdateOperationOutput<AnalyticTableDetail>();
            updateOperationOutput.Result = Vanrise.Entities.UpdateOperationResult.Failed;
            updateOperationOutput.UpdatedObject = null;

            IAnalyticTableDataManager dataManager = AnalyticDataManagerFactory.GetDataManager<IAnalyticTableDataManager>();
            bool updateActionSucc = dataManager.SaveAnalyticTableMeasureStyles(measureStyles, analyticTableId);

            if (updateActionSucc)
            {
                CacheManagerFactory.GetCacheManager<CacheManager>().SetCacheExpired();
                var analyticTable = GetAnalyticTableById(analyticTableId);
                VRActionLogger.Current.LogObjectCustomAction(AnalyticTableLoggableEntity.Instance, "Update", true, analyticTable, "Measure Style");
                updateOperationOutput.Result = Vanrise.Entities.UpdateOperationResult.Succeeded;
                updateOperationOutput.UpdatedObject = AnalyticTableDetailMapper(analyticTable);
            }

            return updateOperationOutput;
        }

        public Vanrise.Entities.UpdateOperationOutput<AnalyticTableDetail> SaveAnalyticTablePermanentFilter(AnalyticTablePermanentFilter permanentFilter, Guid analyticTableId)
        {
            UpdateOperationOutput<AnalyticTableDetail> updateOperationOutput = new Vanrise.Entities.UpdateOperationOutput<AnalyticTableDetail>();
            updateOperationOutput.Result = Vanrise.Entities.UpdateOperationResult.Failed;
            updateOperationOutput.UpdatedObject = null;

            IAnalyticTableDataManager dataManager = AnalyticDataManagerFactory.GetDataManager<IAnalyticTableDataManager>();
            bool updateActionSucc = dataManager.SaveAnalyticTablePermanentFilter(permanentFilter, analyticTableId);

            if (updateActionSucc)
            {
                CacheManagerFactory.GetCacheManager<CacheManager>().SetCacheExpired();

                var analyticTable = GetAnalyticTableById(analyticTableId);
                VRActionLogger.Current.LogObjectCustomAction(AnalyticTableLoggableEntity.Instance, "Update", true, analyticTable, "Permanent Filter");
                updateOperationOutput.Result = Vanrise.Entities.UpdateOperationResult.Succeeded;
                updateOperationOutput.UpdatedObject = AnalyticTableDetailMapper(analyticTable);
            }

            return updateOperationOutput;
        }
        public IEnumerable<AnalyticTablePermanentFilterSettingsConfig> GetPermanentFilterSettingsConfigs()
        {
            var extensionConfiguration = new ExtensionConfigurationManager();
            return extensionConfiguration.GetExtensionConfigurations<AnalyticTablePermanentFilterSettingsConfig>(AnalyticTablePermanentFilterSettingsConfig.EXTENSION_TYPE);
        }
        public IEnumerable<AnalyticTableInfo> GetAnalyticTablesInfo(AnalyticTableInfoFilter filter)
        {
            var analyticTables = GetCachedAnalyticTables();
            
            Func<AnalyticTable, bool> filterExpression = (item) =>
            {
                if (Utilities.ShouldHideItemHavingDevProjectId(item.DevProjectId))
                    return false;

                if (filter != null)
                {
                    if (filter.OnlySelectedIds != null && !filter.OnlySelectedIds.Contains(item.AnalyticTableId))
                        return false;
                }

                return true;
            };

            return analyticTables.MapRecords(AnalyticTableInfoMapper, filterExpression);
        }

        public IEnumerable<AnalyticTableInfo> GetRemoteAnalyticTablesInfo(Guid connectionId, string serializedFilter)
        {
            VRConnectionManager connectionManager = new VRConnectionManager();
            var vrConnection = connectionManager.GetVRConnection<VRInterAppRestConnection>(connectionId);
            VRInterAppRestConnection connectionSettings = vrConnection.Settings as VRInterAppRestConnection;

            return connectionSettings.Get<IEnumerable<AnalyticTableInfo>>(string.Format("/api/VR_Analytic/AnalyticTable/GetAnalyticTablesInfo?filter={0}", serializedFilter));
        }

        public bool IsDynamicCodeSupported(Guid analyticTableId)
        {
            AnalyticTable analyticTable = GetAnalyticTableById(analyticTableId);
            analyticTable.ThrowIfNull("analyticTable", analyticTableId);
            analyticTable.Settings.ThrowIfNull("analyticTable.Settings", analyticTableId);
            analyticTable.Settings.DataProvider.ThrowIfNull("analyticTable.Settings.DataProvider", analyticTableId);
            return analyticTable.Settings.DataProvider.IsDynamicCodeSupported();
        }
        public AnalyticTable GetAnalyticTableById(Guid analyticTableId)
        {
            var analyticTables = GetCachedAnalyticTables();

            return analyticTables.GetRecord(analyticTableId); ;
        }

        public bool IsAnalyticTableExist(Guid analyticTableId)
        {
            return GetAnalyticTableById(analyticTableId) != null;
        }

        public string GetAnalyticTableName(Guid analyticTableId)
        {
            var analyticTable = GetAnalyticTableById(analyticTableId);
            return analyticTable != null ? GetAnalyticTableName(analyticTable) : null;
        }
        public string GetAnalyticTableName(AnalyticTable analyticTable)
        {
            if (analyticTable != null)
                return analyticTable.Name;
            return null;
        }
        public Vanrise.Entities.InsertOperationOutput<AnalyticTableDetail> AddAnalyticTable(AnalyticTable analyticTable)
        {
            InsertOperationOutput<AnalyticTableDetail> insertOperationOutput = new Vanrise.Entities.InsertOperationOutput<AnalyticTableDetail>();

            insertOperationOutput.Result = Vanrise.Entities.InsertOperationResult.Failed;
            insertOperationOutput.InsertedObject = null;
            analyticTable.AnalyticTableId = Guid.NewGuid();
            IAnalyticTableDataManager dataManager = AnalyticDataManagerFactory.GetDataManager<IAnalyticTableDataManager>();
            bool insertActionSucc = dataManager.AddAnalyticTable(analyticTable);

            if (insertActionSucc)
            {
                CacheManagerFactory.GetCacheManager<CacheManager>().SetCacheExpired();
                VRActionLogger.Current.TrackAndLogObjectAdded(AnalyticTableLoggableEntity.Instance, analyticTable);
                insertOperationOutput.Result = Vanrise.Entities.InsertOperationResult.Succeeded;
                insertOperationOutput.InsertedObject = AnalyticTableDetailMapper(analyticTable);
            }
            else
            {
                insertOperationOutput.Result = Vanrise.Entities.InsertOperationResult.SameExists;
            }

            return insertOperationOutput;
        }
        public Vanrise.Entities.UpdateOperationOutput<AnalyticTableDetail> UpdateAnalyticTable(AnalyticTable analyticTable)
        {
            IAnalyticTableDataManager dataManager = AnalyticDataManagerFactory.GetDataManager<IAnalyticTableDataManager>();
            bool updateActionSucc = dataManager.UpdateAnalyticTable(analyticTable);
            UpdateOperationOutput<AnalyticTableDetail> updateOperationOutput = new Vanrise.Entities.UpdateOperationOutput<AnalyticTableDetail>();

            updateOperationOutput.Result = Vanrise.Entities.UpdateOperationResult.Failed;
            updateOperationOutput.UpdatedObject = null;

            if (updateActionSucc)
            {
                CacheManagerFactory.GetCacheManager<CacheManager>().SetCacheExpired();
                VRActionLogger.Current.TrackAndLogObjectUpdated(AnalyticTableLoggableEntity.Instance, analyticTable);
                updateOperationOutput.Result = Vanrise.Entities.UpdateOperationResult.Succeeded;
                updateOperationOutput.UpdatedObject = AnalyticTableDetailMapper(analyticTable);
            }
            else
            {
                updateOperationOutput.Result = Vanrise.Entities.UpdateOperationResult.SameExists;
            }

            return updateOperationOutput;
        }

        public bool DoesUserHaveAccess(int userId, Guid analyticTableId)
        {
            AnalyticTable table = GetAnalyticTableById(analyticTableId);
            SecurityManager secManager = new SecurityManager();
            if (table.Settings.RequiredPermission != null && !secManager.IsAllowed(table.Settings.RequiredPermission, userId))
            {
                return false;
            }
            return true;
        }

        public Guid? GetAnalyticTableConnectionId(List<Guid> analyticTableIds)
        {
            if (analyticTableIds == null || analyticTableIds.Count == 0)
                return null;
            var analyticTableId = analyticTableIds.First();
            var analyticTable = GetAnalyticTableById(analyticTableId);
            analyticTable.ThrowIfNull("analyticTable", analyticTableId);
            analyticTable.Settings.ThrowIfNull("analyticTable.Settings", analyticTableId);
            analyticTable.Settings.DataProvider.ThrowIfNull("analyticTable.Settings.DataProvider", analyticTableId);
            return analyticTable.Settings.DataProvider.ConnectionId;
        }

        #endregion

        #region Private Methods

        Dictionary<Guid, AnalyticTable> GetCachedAnalyticTables()
        {
            return CacheManagerFactory.GetCacheManager<CacheManager>().GetOrCreateObject("GetCachedAnalyticTables",
                () =>
                {
                    IAnalyticTableDataManager dataManager = AnalyticDataManagerFactory.GetDataManager<IAnalyticTableDataManager>();
                    IEnumerable<AnalyticTable> analyticTables = dataManager.GetAnalyticTables();
                    return analyticTables.ToDictionary(x => x.AnalyticTableId, analyticTable => analyticTable);
                });
        }
        #endregion

        #region Private Classes

        public class CacheManager : Vanrise.Caching.BaseCacheManager
        {

            IAnalyticTableDataManager _dataManager = AnalyticDataManagerFactory.GetDataManager<IAnalyticTableDataManager>();
            object _updateHandle;

            protected override bool ShouldSetCacheExpired(object parameter)
            {
                return _dataManager.AreAnalyticTableUpdated(ref _updateHandle);
            }
        }

        private class AnalyticTableLoggableEntity : VRLoggableEntityBase
        {
            public static AnalyticTableLoggableEntity Instance = new AnalyticTableLoggableEntity();

            private AnalyticTableLoggableEntity()
            {

            }

            static AnalyticTableManager s_AnalyticTableManager = new AnalyticTableManager();

            public override string EntityUniqueName
            {
                get { return "VR_Analytic_AnalyticTable"; }
            }

            public override string ModuleName
            {
                get { return "Analytic"; }
            }

            public override string EntityDisplayName
            {
                get { return "Analytic Table"; }
            }

            public override string ViewHistoryItemClientActionName
            {
                get { return "VR_Analytic_AnalyticTable_ViewHistoryItem"; }
            }

            public override object GetObjectId(IVRLoggableEntityGetObjectIdContext context)
            {
                AnalyticTable analyticTable = context.Object.CastWithValidate<AnalyticTable>("context.Object");
                return analyticTable.AnalyticTableId;
            }

            public override string GetObjectName(IVRLoggableEntityGetObjectNameContext context)
            {
                AnalyticTable analyticTable = context.Object.CastWithValidate<AnalyticTable>("context.Object");
                return s_AnalyticTableManager.GetAnalyticTableName(analyticTable);
            }
        }

        #endregion

        #region Mappers

        AnalyticTableDetail AnalyticTableDetailMapper(AnalyticTable analyticTable)
        {
            string devProjectName = null;
            if (analyticTable.DevProjectId.HasValue)
            {
                devProjectName = vrDevProjectManager.GetVRDevProjectName(analyticTable.DevProjectId.Value);
            }
            return new AnalyticTableDetail()
            {
                Entity = analyticTable,
                DevProjectName= devProjectName
            };

        }
        AnalyticTableInfo AnalyticTableInfoMapper(AnalyticTable analyticTable)
        {
            return new AnalyticTableInfo()
            {
                AnalyticTableId = analyticTable.AnalyticTableId,
                Name = vrDevProjectManager.ConcatenateTitleAndDevProjectName(analyticTable.DevProjectId, analyticTable.Name),
                StatusBEDefinitionId = analyticTable.Settings.StatusDefinitionBEId

            };
        }
        #endregion
    } 
}
