using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Vanrise.Analytic.Data;
using Vanrise.Analytic.Entities;
using Vanrise.Common;
using Vanrise.Caching;

namespace Vanrise.Analytic.Business
{
    public class AnalyticConfigurationManager
    {
        public AnalyticConfigurationManager()
        {

        }
        
        public IEnumerable<AnalyticConfiguration<DimensionConfiguration>> GetDimensions()
        {
            return Vanrise.Caching.CacheManagerFactory.GetCacheManager<CacheManager>().GetOrCreateObject("GetAnalyticDimensions",
                   () =>
                   {
                       IAnalyticConfigurationDataManager dataManager = AnalyticDataManagerFactory.GetDataManager<IAnalyticConfigurationDataManager>();
                       return dataManager.GetDimensions();
                   });
        }
        public IEnumerable<AnalyticConfiguration<MeasureConfiguration>> GetMeasures()
        {
            return Vanrise.Caching.CacheManagerFactory.GetCacheManager<CacheManager>().GetOrCreateObject("GetAnalyticMeasures",
                   () =>
                   {
                       IAnalyticConfigurationDataManager dataManager = AnalyticDataManagerFactory.GetDataManager<IAnalyticConfigurationDataManager>();
                       return dataManager.GetMeasures();
                   });
        }
        public IEnumerable<MeasureInfo> GetMeasuresInfo()
        {
            return GetMeasures().MapRecords(MeasureInfoMapper);

        }
        public IEnumerable<DimensionInfo<DimensionConfiguration>> GetDimensionsInfo()
        {
            return GetDimensions().MapRecords(DimensionInfoMapper);

        }
        MeasureInfo MeasureInfoMapper(AnalyticConfiguration<MeasureConfiguration> measure)
        {
            return new MeasureInfo
            {
                Id = measure.Id,
                Name = measure.Name
            };
        }
        DimensionInfo<DimensionConfiguration> DimensionInfoMapper(AnalyticConfiguration<DimensionConfiguration> dimension)
        {
            return new DimensionInfo<DimensionConfiguration>
            {
                Id = dimension.Id,
                Configuration = dimension.Configuration,
                Name = dimension.Name
            };
        }

        #region Private Classes
        private class CacheManager : Vanrise.Caching.BaseCacheManager
        {
            IAnalyticConfigurationDataManager _dataManager = AnalyticDataManagerFactory.GetDataManager<IAnalyticConfigurationDataManager>();
            object _updateHandle;

            protected override bool ShouldSetCacheExpired(object parameter)
            {
                return _dataManager.AreAnalyticConfigurationUpdated(ref _updateHandle);
            }
        }
        #endregion
    }
}
