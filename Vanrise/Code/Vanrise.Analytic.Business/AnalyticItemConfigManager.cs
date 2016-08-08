using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Vanrise.Analytic.Data;
using Vanrise.Analytic.Entities;
using Vanrise.Caching;
using Vanrise.Common;
using Vanrise.Entities;
using Vanrise.GenericData.Business;
using Vanrise.GenericData.Entities;
using Vanrise.Security.Business;
namespace Vanrise.Analytic.Business
{
    public class AnalyticItemConfigManager : IAnalyticItemConfigManager
    {
        #region Public Methods

        public AnalyticDimensionEditorRuntime GetAnalyticDimensionEditorRuntime(AnalyticDimensionEditorInput input)
        {
            AnalyticTableManager analyticTableManager = new AnalyticTableManager();
            var analyticTable = analyticTableManager.GetAnalyticTableById(input.TableId);
            AnalyticDimensionEditorRuntime analyticDimensionEditorRuntime = new AnalyticDimensionEditorRuntime();

            if (analyticTable.Settings != null && analyticTable.Settings.DataRecordTypeIds != null)
            {
                DataRecordTypeInfoFilter filter = new DataRecordTypeInfoFilter { RecordTypeIds = analyticTable.Settings.DataRecordTypeIds };
                DataRecordTypeManager dataRecordTypeManager = new DataRecordTypeManager();
                analyticDimensionEditorRuntime.DataRecordTypeInfo = dataRecordTypeManager.GetDataRecordTypeInfo(filter);
            }
            return analyticDimensionEditorRuntime;
        }
        public Dictionary<string, AnalyticDimension> GetDimensions(int tableId)
        {
            string cacheName = String.Format("GetDimensions_{0}", tableId);
            return Vanrise.Caching.CacheManagerFactory.GetCacheManager<CacheManager>().GetOrCreateObject(cacheName,
                () =>
                {
                    var dimensionConfigs = GetCachedAnalyticItemConfigs<AnalyticDimensionConfig>(tableId, AnalyticItemType.Dimension);
                    Dictionary<string, AnalyticDimension> analyticDimensions = new Dictionary<string, AnalyticDimension>();
                    foreach (var itemConfig in dimensionConfigs)
                    {
                        AnalyticDimensionConfig dimensionConfig = itemConfig.Config;
                        if (dimensionConfig == null)
                            throw new NullReferenceException("dimensionConfig");
                        AnalyticDimension dimension = new AnalyticDimension
                        {
                            AnalyticDimensionConfigId = itemConfig.AnalyticItemConfigId,
                            Name = itemConfig.Name,
                            Config = dimensionConfig
                        };
                        analyticDimensions.Add(itemConfig.Name, dimension);
                    }
                    DynamicTypeGenerator.BuildDimensionEvaluators(tableId, analyticDimensions.Values);
                    return analyticDimensions;
                });
        }

        public Dictionary<string, AnalyticAggregate> GetAggregates(int tableId)
        {
            string cacheName = String.Format("GetAggregates_{0}", tableId);
            return Vanrise.Caching.CacheManagerFactory.GetCacheManager<CacheManager>().GetOrCreateObject(cacheName,
                () =>
                {
                    var aggregateConfigs = GetCachedAnalyticItemConfigs<AnalyticAggregateConfig>(tableId, AnalyticItemType.Aggregate);
                    Dictionary<string, AnalyticAggregate> analyticAggregates = new Dictionary<string, AnalyticAggregate>();
                    foreach (var itemConfig in aggregateConfigs)
                    {
                        AnalyticAggregateConfig aggregateConfig = itemConfig.Config;
                        if (aggregateConfig == null)
                            throw new NullReferenceException("aggregateConfig");
                        AnalyticAggregate measure = new AnalyticAggregate
                        {
                            AnalyticAggregateConfigId = itemConfig.AnalyticItemConfigId,
                            Config = aggregateConfig
                        };
                        analyticAggregates.Add(itemConfig.Name, measure);
                    }
                    return analyticAggregates;
                });

        }

        public Dictionary<string, AnalyticMeasure> GetMeasures(int tableId)
        {
            string cacheName = String.Format("GetMeasures_{0}", tableId);
            return Vanrise.Caching.CacheManagerFactory.GetCacheManager<CacheManager>().GetOrCreateObject(cacheName,
                () =>
                {
                    var measureConfigs = GetCachedAnalyticItemConfigs<AnalyticMeasureConfig>(tableId, AnalyticItemType.Measure);
                    Dictionary<string, AnalyticMeasure> analyticMeasures = new Dictionary<string, AnalyticMeasure>();
                    foreach (var itemConfig in measureConfigs)
                    {
                        AnalyticMeasureConfig measureConfig = itemConfig.Config;
                        if (measureConfig == null)
                            throw new NullReferenceException("measureConfig");
                        AnalyticMeasure measure = new AnalyticMeasure
                        {
                            AnalyticMeasureConfigId = itemConfig.AnalyticItemConfigId,
                            Config = measureConfig
                        };
                        analyticMeasures.Add(itemConfig.Name, measure);
                    }
                    DynamicTypeGenerator.BuildMeasureEvaluators(tableId, analyticMeasures.Values);
                    return analyticMeasures;
                });

        }

        public Dictionary<string, AnalyticJoin> GetJoins(int tableId)
        {
            var joinConfigs = GetCachedAnalyticItemConfigs<AnalyticJoinConfig>(tableId, AnalyticItemType.Join);
            Dictionary<string, AnalyticJoin> analyticJoins = new Dictionary<string, AnalyticJoin>();
            foreach (var itemConfig in joinConfigs)
            {
                AnalyticJoinConfig dimensionConfig = itemConfig.Config;
                if (dimensionConfig == null)
                    throw new NullReferenceException("joinConfig");
                AnalyticJoin join = new AnalyticJoin
                {
                    Config = dimensionConfig
                };
                analyticJoins.Add(itemConfig.Name, join);
            }
            return analyticJoins;
        }
        public IEnumerable<AnalyticDimensionConfigInfo> GetDimensionsInfo(AnalyticDimensionConfigInfoFilter filter)
        {
            if (filter == null || filter.TableIds == null || filter.TableIds.Count == 0)
                throw new NullReferenceException("AnalyticDimensionConfigInfoFilter");
            List<AnalyticDimensionConfigInfo> dimensionConfigs = new List<AnalyticDimensionConfigInfo>();
            foreach (var tableId in filter.TableIds)
            {
                var dimensions = GetCachedAnalyticItemConfigs<AnalyticDimensionConfig>(tableId, AnalyticItemType.Dimension);
                dimensionConfigs.AddRange(dimensions.MapRecords(AnalyticDimensionConfigInfoMapper, x => (filter.HideIsRequiredFromParent == false || (filter.HideIsRequiredFromParent == true && x.Config.RequiredParentDimension == null))));
            }
            return dimensionConfigs;
        }
        public IEnumerable<AnalyticMeasureConfigInfo> GetMeasuresInfo(AnalyticMeasureConfigInfoFilter filter)
        {
            if (filter == null || filter.TableIds == null || filter.TableIds.Count == 0)
                throw new NullReferenceException("filter");
            List<AnalyticMeasureConfigInfo> measureConfigs = new List<AnalyticMeasureConfigInfo>();
            foreach (var tableId in filter.TableIds)
            {
                var measures = GetCachedAnalyticItemConfigs<AnalyticMeasureConfig>(tableId, AnalyticItemType.Measure);
                measureConfigs.AddRange(measures.MapRecords(AnalyticMeasureConfigInfoMapper));
            }
            return measureConfigs;
        }
        public IEnumerable<AnalyticJoinConfigInfo> GetJoinsInfo(AnalyticJoinConfigInfoFilter filter)
        {
            if (filter == null || filter.TableIds == null || filter.TableIds.Count == 0)
                throw new NullReferenceException("filter");
            List<AnalyticJoinConfigInfo> JoinConfigs = new List<AnalyticJoinConfigInfo>();
            foreach (var tableId in filter.TableIds)
            {
                var joins = GetCachedAnalyticItemConfigs<AnalyticJoinConfig>(tableId, AnalyticItemType.Join);
                JoinConfigs.AddRange(joins.MapRecords(AnalyticJoinConfigInfoMapper));
            }
            return JoinConfigs;
        }
        public IEnumerable<AnalyticAggregateConfigInfo> GetAggregatesInfo(AnalyticAggregateConfigInfoFilter filter)
        {
            if (filter == null || filter.TableIds == null || filter.TableIds.Count == 0)
                throw new NullReferenceException("filter");
            List<AnalyticAggregateConfigInfo> AggregateConfigs = new List<AnalyticAggregateConfigInfo>();
            foreach (var tableId in filter.TableIds)
            {
                var aggregates = GetCachedAnalyticItemConfigs<AnalyticAggregateConfig>(tableId, AnalyticItemType.Aggregate);
                AggregateConfigs.AddRange(aggregates.MapRecords(AnalyticAggregateConfigInfoMapper));
            }
            return AggregateConfigs;
        }

        public IEnumerable<Object> GetAnalyticItemConfigs(List<int> tableIds, AnalyticItemType itemType)
        {
            return GetCachedAnalyticItemConfigsByItemType(tableIds, itemType);
        }
        public Vanrise.Entities.IDataRetrievalResult<Object> GetFilteredAnalyticItemConfigs(Vanrise.Entities.DataRetrievalInput<AnalyticItemConfigQuery> input)
        {
            if (input.Query == null)
            {
                throw new ArgumentNullException("input.Query");
            }
            var itemConfigs = GetCachedAnalyticItemConfigsDetailByItemType(input.Query.TableId, input.Query.ItemType);

            Func<Object, bool> filterExpression = (prod) =>
                 (true);

            return Vanrise.Common.DataRetrievalManager.Instance.ProcessResult(input, itemConfigs.ToBigResult(input, filterExpression));
        }
        public Object GetAnalyticItemConfigsById(int tableId, AnalyticItemType itemType, int analyticItemConfigId)
        {
            return GetCachedAnalyticItemConfigByItemType(tableId, itemType, analyticItemConfigId);
        }
        public Vanrise.Entities.InsertOperationOutput<AnalyticItemConfigDetail<T>> AddAnalyticItemConfig<T>(AnalyticItemConfig<T> analyticItemConfig) where T : class
        {
            InsertOperationOutput<AnalyticItemConfigDetail<T>> insertOperationOutput = new Vanrise.Entities.InsertOperationOutput<AnalyticItemConfigDetail<T>>();

            insertOperationOutput.Result = Vanrise.Entities.InsertOperationResult.Failed;
            insertOperationOutput.InsertedObject = null;
            int analyticItemConfigId = -1;

            IAnalyticItemConfigDataManager dataManager = AnalyticDataManagerFactory.GetDataManager<IAnalyticItemConfigDataManager>();
            bool insertActionSucc = false;
            insertActionSucc = dataManager.AddAnalyticItemConfig(analyticItemConfig, out analyticItemConfigId);

            if (insertActionSucc)
            {
                CacheManagerFactory.GetCacheManager<CacheManager>().SetCacheExpired();
                insertOperationOutput.Result = Vanrise.Entities.InsertOperationResult.Succeeded;
                analyticItemConfig.AnalyticItemConfigId = analyticItemConfigId;
                insertOperationOutput.InsertedObject = AnalyticConfigDetailMapper(analyticItemConfig);
            }
            else
            {
                insertOperationOutput.Result = Vanrise.Entities.InsertOperationResult.SameExists;
            }

            return insertOperationOutput;
        }
        public Vanrise.Entities.UpdateOperationOutput<AnalyticItemConfigDetail<T>> UpdateAnalyticItemConfig<T>(AnalyticItemConfig<T> analyticItemConfig) where T : class
        {
            IAnalyticItemConfigDataManager dataManager = AnalyticDataManagerFactory.GetDataManager<IAnalyticItemConfigDataManager>();

            bool updateActionSucc = false;
            UpdateOperationOutput<AnalyticItemConfigDetail<T>> updateOperationOutput = new Vanrise.Entities.UpdateOperationOutput<AnalyticItemConfigDetail<T>>();

            updateOperationOutput.Result = Vanrise.Entities.UpdateOperationResult.Failed;

            updateOperationOutput.UpdatedObject = null;
            updateActionSucc = dataManager.UpdateAnalyticItemConfig(analyticItemConfig);
            if (updateActionSucc)
            {
                CacheManagerFactory.GetCacheManager<CacheManager>().SetCacheExpired();
                updateOperationOutput.Result = Vanrise.Entities.UpdateOperationResult.Succeeded;
                updateOperationOutput.UpdatedObject = AnalyticConfigDetailMapper(analyticItemConfig);
            }
            else
            {
                updateOperationOutput.Result = Vanrise.Entities.UpdateOperationResult.SameExists;
            }

            return updateOperationOutput;
        }


        public bool DoesUserHaveAccess(int userId, int analyticTableId, List<string> measureNames)
        {
            var filterdMeasures = this.GetMeasures(analyticTableId).Where(k => measureNames.Contains(k.Key)).Select(v => v.Value);
            SecurityManager secManager = new SecurityManager();

            try
            {
                foreach (var m in filterdMeasures)
                {
                    if (m.Config != null && m.Config.RequiredPermission != null && !secManager.IsAllowed(m.Config.RequiredPermission, userId))
                    {
                        return false;
                    }
                }
            }
            catch (Exception e)
            {

            }
            return true;

        }

        #endregion

        #region Private Methods

        private Object GetCachedAnalyticItemConfigByItemType(int tableId, AnalyticItemType itemType, int analyticItemConfigId)
        {
            Object data = null;
            switch (itemType)
            {
                case AnalyticItemType.Dimension: data = GetCachedAnalyticItemConfigs<AnalyticDimensionConfig>(tableId, itemType).FindRecord(x => x.AnalyticItemConfigId == analyticItemConfigId); break;
                case AnalyticItemType.Join: data = GetCachedAnalyticItemConfigs<AnalyticJoinConfig>(tableId, itemType).FindRecord(x => x.AnalyticItemConfigId == analyticItemConfigId); break;
                case AnalyticItemType.Measure: data = GetCachedAnalyticItemConfigs<AnalyticMeasureConfig>(tableId, itemType).FindRecord(x => x.AnalyticItemConfigId == analyticItemConfigId); break;
                case AnalyticItemType.Aggregate: data = GetCachedAnalyticItemConfigs<AnalyticAggregateConfig>(tableId, itemType).FindRecord(x => x.AnalyticItemConfigId == analyticItemConfigId); break;
            }
            return data;
        }

        private IEnumerable<Object> GetCachedAnalyticItemConfigsByItemType(List<int> tableIds, AnalyticItemType itemType)
        {
            IEnumerable<Object> data = null;
            switch (itemType)
            {
                case AnalyticItemType.Dimension:
                    List<AnalyticItemConfig<AnalyticDimensionConfig>> dimensions = new List<AnalyticItemConfig<AnalyticDimensionConfig>>();

                    foreach (int tableId in tableIds)
                    {
                        var obj = GetCachedAnalyticItemConfigs<AnalyticDimensionConfig>(tableId, itemType).ToList();
                        RemoveCommonItemsInList(obj, dimensions);
                        dimensions.AddRange(obj);
                    }
                    data = dimensions;
                    break;
                case AnalyticItemType.Join:
                    List<AnalyticItemConfig<AnalyticJoinConfig>> joins = new List<AnalyticItemConfig<AnalyticJoinConfig>>();
                    foreach (int tableId in tableIds)
                    {
                        var obj = GetCachedAnalyticItemConfigs<AnalyticJoinConfig>(tableId, itemType).ToList();
                        RemoveCommonItemsInList(obj, joins);
                        joins.AddRange(obj);
                    }
                    data = joins;
                    break;
                case AnalyticItemType.Measure:
                    List<AnalyticItemConfig<AnalyticMeasureConfig>> measures = new List<AnalyticItemConfig<AnalyticMeasureConfig>>();
                    foreach (int tableId in tableIds)
                    {
                        var obj = GetCachedAnalyticItemConfigs<AnalyticMeasureConfig>(tableId, itemType).ToList();
                        RemoveCommonItemsInList(obj, measures);
                        measures.AddRange(obj);
                    }
                    data = measures;
                    break;
                case AnalyticItemType.Aggregate:
                    List<AnalyticItemConfig<AnalyticAggregateConfig>> aggregates = new List<AnalyticItemConfig<AnalyticAggregateConfig>>();
                    foreach (int tableId in tableIds)
                    {
                        var obj = GetCachedAnalyticItemConfigs<AnalyticAggregateConfig>(tableId, itemType).ToList();
                        RemoveCommonItemsInList(obj, aggregates);
                        aggregates.AddRange(obj);
                    }
                    data = aggregates;
                    break;


            }
            return data;
        }
        private IEnumerable<Object> GetCachedAnalyticItemConfigsDetailByItemType(int tableId, AnalyticItemType itemType)
        {
            IEnumerable<Object> data = null;
            switch (itemType)
            {
                case AnalyticItemType.Dimension: data = GetCachedAnalyticItemConfigs<AnalyticDimensionConfig>(tableId, itemType).MapRecords(AnalyticConfigDetailMapper<AnalyticDimensionConfig>); break;
                case AnalyticItemType.Join: data = GetCachedAnalyticItemConfigs<AnalyticJoinConfig>(tableId, itemType).MapRecords(AnalyticConfigDetailMapper<AnalyticJoinConfig>); break;
                case AnalyticItemType.Measure: data = GetCachedAnalyticItemConfigs<AnalyticMeasureConfig>(tableId, itemType).MapRecords(AnalyticConfigDetailMapper<AnalyticMeasureConfig>); break;
                case AnalyticItemType.Aggregate: data = GetCachedAnalyticItemConfigs<AnalyticAggregateConfig>(tableId, itemType).MapRecords(AnalyticConfigDetailMapper<AnalyticAggregateConfig>); break;

            }
            return data;
        }
        AnalyticItemConfigDetail<T> AnalyticConfigDetailMapper<T>(AnalyticItemConfig<T> analyticItemConfig) where T : class
        {
            return new AnalyticItemConfigDetail<T>
            {
                AnalyticItemConfigId = analyticItemConfig.AnalyticItemConfigId,
                Name = analyticItemConfig.Name,
                Entity = analyticItemConfig,
                Title = analyticItemConfig.Title
            };
        }
        private IEnumerable<AnalyticItemConfig<T>> GetCachedAnalyticItemConfigs<T>(int tableId, AnalyticItemType itemType) where T : class
        {
            return CacheManagerFactory.GetCacheManager<CacheManager>().GetOrCreateObject(String.Format("GetCachedAnalyticItemConfigs_{0}_{1}", tableId, itemType),
               () =>
               {
                   IAnalyticItemConfigDataManager dataManager = AnalyticDataManagerFactory.GetDataManager<IAnalyticItemConfigDataManager>();
                   return dataManager.GetItemConfigs<T>(tableId, itemType);
               });
        }

        private void RemoveCommonItemsInList<T>(List<AnalyticItemConfig<T>> mainItems, List<AnalyticItemConfig<T>> comparedList)
        {
            var commonItems = mainItems.Where(x => comparedList.Any(y => y.Name == x.Name));
            if (commonItems != null)
            {
                foreach (var item in commonItems)
                {
                    mainItems.Remove(item);
                }
            }
        }

        #endregion

        #region Private Classes

        public class CacheManager : Vanrise.Caching.BaseCacheManager
        {
            IAnalyticItemConfigDataManager _dataManager = AnalyticDataManagerFactory.GetDataManager<IAnalyticItemConfigDataManager>();
            object _updateHandle;

            protected override bool ShouldSetCacheExpired(object parameter)
            {
                return _dataManager.AreAnalyticItemConfigUpdated(ref _updateHandle);
            }
        }

        #endregion

        #region Mapper
        AnalyticDimensionConfigInfo AnalyticDimensionConfigInfoMapper(AnalyticItemConfig<AnalyticDimensionConfig> analyticItemConfig)
        {
            return new AnalyticDimensionConfigInfo
            {
                AnalyticItemConfigId = analyticItemConfig.AnalyticItemConfigId,
                Name = analyticItemConfig.Name,
                Title = analyticItemConfig.Title,
                RequiredParentDimension = analyticItemConfig.Config.RequiredParentDimension,
                Parents = analyticItemConfig.Config.Parents,
                Attribute = analyticItemConfig.Config.FieldType.GetGridColumnAttribute()
            };
        }
        AnalyticMeasureConfigInfo AnalyticMeasureConfigInfoMapper(AnalyticItemConfig<AnalyticMeasureConfig> analyticItemConfig)
        {
            return new AnalyticMeasureConfigInfo
            {
                AnalyticItemConfigId = analyticItemConfig.AnalyticItemConfigId,
                Name = analyticItemConfig.Name,
                Title = analyticItemConfig.Title,
                Attribute = analyticItemConfig.Config.FieldType.GetGridColumnAttribute(),
                FieldType = analyticItemConfig.Config.FieldType

            };
        }
        AnalyticJoinConfigInfo AnalyticJoinConfigInfoMapper(AnalyticItemConfig<AnalyticJoinConfig> analyticItemConfig)
        {
            return new AnalyticJoinConfigInfo
            {
                AnalyticItemConfigId = analyticItemConfig.AnalyticItemConfigId,
                Name = analyticItemConfig.Name,
                Title = analyticItemConfig.Title
            };
        }
        AnalyticAggregateConfigInfo AnalyticAggregateConfigInfoMapper(AnalyticItemConfig<AnalyticAggregateConfig> analyticItemConfig)
        {
            return new AnalyticAggregateConfigInfo
            {
                AnalyticItemConfigId = analyticItemConfig.AnalyticItemConfigId,
                Name = analyticItemConfig.Name,
                Title = analyticItemConfig.Title,

            };
        }
        #endregion

    }
}
