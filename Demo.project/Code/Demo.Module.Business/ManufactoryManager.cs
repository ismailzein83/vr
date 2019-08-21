using Demo.Module.Data;
using Demo.Module.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using Vanrise.Caching;
using Vanrise.Common;
using Vanrise.Entities;

namespace Demo.Module.Business
{
    public class ManufactoryManager
    {
        IManufactoryDataManager _manufactoryDataManager = DemoModuleFactory.GetDataManager<IManufactoryDataManager>();

        #region Public Methods

        public IDataRetrievalResult<ManufactoryDetail> GetFilteredManufactories(DataRetrievalInput<ManufactoryQuery> input)
        {
            var manufactories = GetCachedManufactories();

            Func<Manufactory, bool> filterManufactories = (manufactory) =>
            {
                if (!string.IsNullOrEmpty(input.Query.Name) && !manufactory.Name.ToLower().Contains(input.Query.Name.ToLower()))
                    return false;

                if (!string.IsNullOrEmpty(input.Query.CountryOfOrigin) && !manufactory.CountryOfOrigin.ToLower().Contains(input.Query.CountryOfOrigin.ToLower()))
                    return false;

                return true;
            };

            return DataRetrievalManager.Instance.ProcessResult(input, manufactories.ToBigResult(input, filterManufactories, ManufactoryDetailMapper));
        }

        public Manufactory GetManufactoryById(int manufactoryId)
        {
            Dictionary<int, Manufactory> cachedManufactories = GetCachedManufactories();
            return cachedManufactories != null ? cachedManufactories.GetRecord(manufactoryId) : null;
        }

        public string GetManufactoryName(int manufactoryId)
        {
            Manufactory manufactory = GetManufactoryById(manufactoryId);
            return manufactory != null ? manufactory.Name : string.Empty;
        }

        public IEnumerable<ManufactoryInfo> GetManufactoriesInfo(ManufactoryInfoFilter manufactoryInfoFilter)
        {
            var manufactories = GetCachedManufactories();

            Func<Manufactory, bool> filterManufactories = (manufactory) =>
            {
                if (manufactoryInfoFilter != null && manufactoryInfoFilter.Filters != null)
                {
                    var context = new ManufactoryInfoFilterContext { Id = manufactory.Id };
                    foreach (var filter in manufactoryInfoFilter.Filters)
                    {
                        if (!filter.IsMatch(context))
                            return false;
                    }
                }
                return true;
            };
            return manufactories.MapRecords(ManufactoryInfoMapper, filterManufactories).OrderBy(manufactory => manufactory.Name);
        }

        public InsertOperationOutput<ManufactoryDetail> InsertManufactory(Manufactory manufactory)
        {
            InsertOperationOutput<ManufactoryDetail> insertOperationOutput = new InsertOperationOutput<ManufactoryDetail>();
            insertOperationOutput.Result = InsertOperationResult.Failed;
            insertOperationOutput.InsertedObject = null;
            int manufactoryId = -1;

            bool succeed = _manufactoryDataManager.InsertManufactory(manufactory, out manufactoryId);
            if (succeed)
            {
                manufactory.Id = manufactoryId;
                CacheManagerFactory.GetCacheManager<CacheManager>().SetCacheExpired();
                insertOperationOutput.Result = InsertOperationResult.Succeeded;
                insertOperationOutput.InsertedObject = ManufactoryDetailMapper(manufactory);
            }
            else
            {
                insertOperationOutput.Result = InsertOperationResult.SameExists;
            }

            return insertOperationOutput;
        }

        public UpdateOperationOutput<ManufactoryDetail> UpdateManufactory(Manufactory manufactory)
        {
            UpdateOperationOutput<ManufactoryDetail> updateOperationOutput = new UpdateOperationOutput<ManufactoryDetail>();
            updateOperationOutput.Result = UpdateOperationResult.Failed;
            updateOperationOutput.UpdatedObject = null;

            bool succeed = _manufactoryDataManager.UpdateManufactory(manufactory);
            if (succeed)
            {
                CacheManagerFactory.GetCacheManager<CacheManager>().SetCacheExpired();
                updateOperationOutput.Result = UpdateOperationResult.Succeeded;
                updateOperationOutput.UpdatedObject = ManufactoryDetailMapper(manufactory);
            }
            else
            {
                updateOperationOutput.Result = UpdateOperationResult.SameExists;
            }

            return updateOperationOutput;
        }

        #endregion

        #region Private Methods

        private ManufactoryDetail ManufactoryDetailMapper(Manufactory manufactory)
        {
            return new ManufactoryDetail()
            {
                Id = manufactory.Id,
                Name = manufactory.Name,
                CountryOfOrigin = manufactory.CountryOfOrigin
            };
        }

        private ManufactoryInfo ManufactoryInfoMapper(Manufactory manufactory)
        {
            return new ManufactoryInfo()
            {
                Id = manufactory.Id,
                Name = manufactory.Name,
            };
        }

        private Dictionary<int, Manufactory> GetCachedManufactories()
        {
            return Vanrise.Caching.CacheManagerFactory.GetCacheManager<CacheManager>().GetOrCreateObject("GetCachedManufactories", () =>
            {
                IManufactoryDataManager manufactoryDataManager = DemoModuleFactory.GetDataManager<IManufactoryDataManager>();
                List<Manufactory> manufactories = manufactoryDataManager.GetManufactories();
                return manufactories.ToDictionary(manufactory => manufactory.Id, manufactory => manufactory);
            });
        }

        #endregion

        #region Private Classes

        private class CacheManager : Vanrise.Caching.BaseCacheManager
        {
            IManufactoryDataManager manufactoryDataManager = DemoModuleFactory.GetDataManager<IManufactoryDataManager>();
            object _updateHandle;

            protected override bool ShouldSetCacheExpired(object parameter)
            {
                return manufactoryDataManager.AreManufactoriesUpdated(ref _updateHandle);
            }
        }

        #endregion
    }
}