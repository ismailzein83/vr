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
namespace Vanrise.Analytic.Business
{
    public class AnalyticItemConfigManager
    {
        #region Public Methods
        public Dictionary<string, AnalyticDimension> GetDimensions(int tableId)
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
                    Config = dimensionConfig
                };
                analyticDimensions.Add(itemConfig.Name, dimension);
            }
            return analyticDimensions;
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
                            Config = measureConfig,
                            Evaluator = DynamicTypeGenerator.GetMeasureEvaluator(itemConfig.AnalyticItemConfigId, measureConfig)
                        };
                        analyticMeasures.Add(itemConfig.Name, measure);
                    }
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
                dimensionConfigs.AddRange(dimensions.MapRecords(AnalyticDimensionConfigInfoMapper, x => (filter.HideIsRequiredFromParent== false || x.Config.IsRequiredFromParent != filter.HideIsRequiredFromParent)));
            }
            return dimensionConfigs;
        }
        public IEnumerable<AnalyticMeasureConfigInfo> GetMeasuresInfo(AnalyticMeasureConfigInfoFilter filter)
        {
            if (filter == null || filter.TableIds == null || filter.TableIds.Count == 0)
                throw new NullReferenceException("AnalyticMeasureConfigInfoFilter");
            List<AnalyticMeasureConfigInfo> measureConfigs = new List<AnalyticMeasureConfigInfo>();
            foreach (var tableId in filter.TableIds)
            {
                var measures = GetCachedAnalyticItemConfigs<AnalyticMeasureConfig>(tableId, AnalyticItemType.Measure);
                measureConfigs.AddRange(measures.MapRecords(AnalyticMeasureConfigInfoMapper));
            }
            return measureConfigs;
        }


        public Vanrise.Entities.IDataRetrievalResult<AnalyticDimensionConfigDetail> GetFilteredDimensions(Vanrise.Entities.DataRetrievalInput<AnalyticDimensionConfigQuery> input)
        {
            if(input.Query == null)
            {
                throw new NullReferenceException("GetFilteredDimensions");
            }
            var dimensions = GetCachedAnalyticItemConfigs<AnalyticDimensionConfig>(input.Query.TableId, AnalyticItemType.Dimension);

            Func<AnalyticItemConfig<AnalyticDimensionConfig>, bool> filterExpression = (prod) =>
                 (true);

            return Vanrise.Common.DataRetrievalManager.Instance.ProcessResult(input, dimensions.ToBigResult(input, filterExpression, AnalyticDimensionConfigDetailMapper));
        }
        public Vanrise.Entities.IDataRetrievalResult<AnalyticMeasureConfigDetail> GetFilteredMeasures(Vanrise.Entities.DataRetrievalInput<AnalyticMeasureConfigQuery> input)
        {
            if (input.Query == null)
            {
                throw new NullReferenceException("GetFilteredMeasures");
            }
            var measures = GetCachedAnalyticItemConfigs<AnalyticMeasureConfig>(input.Query.TableId, AnalyticItemType.Measure);

            Func<AnalyticItemConfig<AnalyticMeasureConfig>, bool> filterExpression = (prod) =>
                 (true);
            return Vanrise.Common.DataRetrievalManager.Instance.ProcessResult(input, measures.ToBigResult(input, filterExpression, AnalyticMeasureConfigDetailMapper));
        }
        public Vanrise.Entities.IDataRetrievalResult<AnalyticJoinConfigDetail> GetFilteredJoins(Vanrise.Entities.DataRetrievalInput<AnalyticJoinConfigQuery> input)
        {
            if (input.Query == null)
            {
                throw new NullReferenceException("GetFilteredJoins");
            }
            var joins = GetCachedAnalyticItemConfigs<AnalyticJoinConfig>(input.Query.TableId, AnalyticItemType.Join);
            Func<AnalyticItemConfig<AnalyticJoinConfig>, bool> filterExpression = (prod) =>
                 (true);

            return Vanrise.Common.DataRetrievalManager.Instance.ProcessResult(input, joins.ToBigResult(input, filterExpression, AnalyticJoinConfigDetailMapper));
        }
        #endregion

        #region Private Methods

        private IEnumerable<AnalyticItemConfig<T>> GetCachedAnalyticItemConfigs<T>(int tableId, AnalyticItemType itemType) where T : class
        {
            return CacheManagerFactory.GetCacheManager<CacheManager>().GetOrCreateObject(String.Format("GetCachedAnalyticItemConfigs_{0}_{1}", tableId, itemType),
               () =>
               {
                   IAnalyticItemConfigDataManager dataManager = AnalyticDataManagerFactory.GetDataManager<IAnalyticItemConfigDataManager>();
                   return dataManager.GetItemConfigs<T>(tableId, itemType);
               });
        }
        private Object GetAnalyticConfigByType(AnalyticItemType itemType)
        {
            switch (itemType)
            {
                case AnalyticItemType.Dimension: return typeof(AnalyticDimensionConfig);
                case AnalyticItemType.Measure: return typeof(AnalyticMeasureConfig);
                case AnalyticItemType.Join: return typeof(AnalyticJoinConfig);
            }
            return null;
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
                IsRequiredFromParent = analyticItemConfig.Config.IsRequiredFromParent,
                ParentDimension = analyticItemConfig.Config.ParentDimension,
            };
        }
        AnalyticMeasureConfigInfo AnalyticMeasureConfigInfoMapper(AnalyticItemConfig<AnalyticMeasureConfig> analyticItemConfig)
        {
            return new AnalyticMeasureConfigInfo
            {
                AnalyticItemConfigId = analyticItemConfig.AnalyticItemConfigId,
                Name = analyticItemConfig.Name,
                Title = analyticItemConfig.Title
            };
        }
        AnalyticDimensionConfigDetail AnalyticDimensionConfigDetailMapper(AnalyticItemConfig<AnalyticDimensionConfig> analyticItemConfig)
        {
            return new AnalyticDimensionConfigDetail
            {
                AnalyticItemConfigId = analyticItemConfig.AnalyticItemConfigId,
                Name = analyticItemConfig.Name,
                Title = analyticItemConfig.Title,
                Entity = analyticItemConfig.Config
            };
        }
        AnalyticMeasureConfigDetail AnalyticMeasureConfigDetailMapper(AnalyticItemConfig<AnalyticMeasureConfig> analyticItemConfig)
        {
            return new AnalyticMeasureConfigDetail
            {
                AnalyticItemConfigId = analyticItemConfig.AnalyticItemConfigId,
                Name = analyticItemConfig.Name,
                Title = analyticItemConfig.Title,
                Entity = analyticItemConfig.Config
            };
        }
        AnalyticJoinConfigDetail AnalyticJoinConfigDetailMapper(AnalyticItemConfig<AnalyticJoinConfig> analyticItemConfig)
        {
            return new AnalyticJoinConfigDetail
            {
                AnalyticItemConfigId = analyticItemConfig.AnalyticItemConfigId,
                Name = analyticItemConfig.Name,
                Title = analyticItemConfig.Title,
                Entity = analyticItemConfig.Config
            };
        }
        #endregion

    }
}
