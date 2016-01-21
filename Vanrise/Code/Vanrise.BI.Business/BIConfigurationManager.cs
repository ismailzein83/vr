using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Vanrise.BI.Data;
using Vanrise.BI.Entities;
using Vanrise.Caching;
using Vanrise.Common;
namespace Vanrise.BI.Business
{
    public class BIConfigurationManager
    {

        #region Public Methods
        public List<BIConfiguration<BIConfigurationMeasure>> GetMeasures()
        {
            return Vanrise.Caching.CacheManagerFactory.GetCacheManager<CacheManager>().GetOrCreateObject("GetMeasures",
            () =>
            {
                IBIConfigurationDataManager dataManager = BIDataManagerFactory.GetDataManager<IBIConfigurationDataManager>();
                List<BIConfiguration<BIConfigurationMeasure>> measures = dataManager.GetMeasures();
                foreach (BIConfiguration<BIConfigurationMeasure> measure in measures)
                {
                    if (measure.Configuration.Unit == "Currency")
                        measure.Configuration.Unit = System.Configuration.ConfigurationManager.AppSettings["Currency"];
                }
                return measures;
            });

        }
        public List<BIConfiguration<BIConfigurationEntity>> GetEntities()
        {
            return Vanrise.Caching.CacheManagerFactory.GetCacheManager<CacheManager>().GetOrCreateObject("GetEntities",
              () =>
              {
                  IBIConfigurationDataManager dataManager = BIDataManagerFactory.GetDataManager<IBIConfigurationDataManager>();
                  return dataManager.GetEntities();
              });

        }

        public List<BIConfiguration<BIConfigurationTimeEntity>> GetTimeEntities()
        {
            return Vanrise.Caching.CacheManagerFactory.GetCacheManager<CacheManager>().GetOrCreateObject("GetTimeEntities",
              () =>
              {
                  IBIConfigurationDataManager dataManager = BIDataManagerFactory.GetDataManager<IBIConfigurationDataManager>();
                  return dataManager.GetTimeEntities();
              });
        }

        public IEnumerable<BIMeasureInfo> GetMeasuresInfo()
        {
            return GetMeasures().MapRecords(MeasureInfoMapper);

        }
        public IEnumerable<BIEntityInfo<BIConfigurationEntity>> GetEntitiesInfo()
        {
            return GetEntities().MapRecords(EntityInfoMapper);

        }

        public IEnumerable<BIEntityInfo<BIConfigurationTimeEntity>> GetTimeEntitiesInfo()
        {
            return GetTimeEntities().MapRecords(EntityTimeInfoMapper);
        }

        #endregion

        #region Private Classes
        private class CacheManager : Vanrise.Caching.BaseCacheManager
        {
            IBIConfigurationDataManager _dataManager = BIDataManagerFactory.GetDataManager<IBIConfigurationDataManager>();
            object _updateHandle;

            protected override bool ShouldSetCacheExpired(object parameter)
            {
                return _dataManager.AreBIConfigurationUpdated(ref _updateHandle);
            }
        }
        #endregion

        #region Mapper
        BIMeasureInfo MeasureInfoMapper(BIConfiguration<BIConfigurationMeasure> entity)
        {
            return new BIMeasureInfo
            {
                Id=entity.Id,
                RequiredPermissions=entity.Configuration.RequiredPermissions,
                Unit= entity.Configuration.Unit,
                DisplayName = entity.DisplayName,
                Name = entity.Name
            };
        }
        BIEntityInfo<BIConfigurationEntity> EntityInfoMapper(BIConfiguration<BIConfigurationEntity> entity)
        {
            return new BIEntityInfo<BIConfigurationEntity>
            {
                Id = entity.Id,
                Configuration = entity.Configuration,
                DisplayName = entity.DisplayName,
                Name = entity.Name
            };
        }
        BIEntityInfo<BIConfigurationTimeEntity> EntityTimeInfoMapper(BIConfiguration<BIConfigurationTimeEntity> entity)
        {
            return new BIEntityInfo<BIConfigurationTimeEntity>
            {
                Id = entity.Id,
                Configuration = entity.Configuration,
                DisplayName = entity.DisplayName,
                Name = entity.Name
            };
        }
        #endregion
    }
}
