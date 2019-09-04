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
    public class ZooManager
    {
        #region Public Methods

        public IDataRetrievalResult<ZooDetail> GetFilteredZoos(DataRetrievalInput<ZooQuery> input)
        {
            var allZoos = GetCachedZoos();
            Func<Zoo, bool> filterExpression = (zoo) =>
            {
                if (!string.IsNullOrEmpty(input.Query.Name) && !zoo.Name.ToLower().Contains(input.Query.Name.ToLower()))
                    return false;

                if (input.Query.Sizes != null && !input.Query.Sizes.Contains(zoo.Size))
                    return false;

                return true;
            };

            return DataRetrievalManager.Instance.ProcessResult(input, allZoos.ToBigResult(input, filterExpression, ZooDetailMapper));
        }

        public IEnumerable<ZooInfo> GetZoosInfo(ZooInfoFilter zooInfoFilter)
        {
            var allZoos = GetCachedZoos();

            if (allZoos == null)
                return null;

            Func<Zoo, bool> filterFunc = (zoo) =>
            {
                if (zooInfoFilter != null)
                {
                    if (zooInfoFilter.Filters != null)
                    {
                        var context = new ZooInfoFilterContext { ZooId = zoo.ZooId };

                        foreach (var filter in zooInfoFilter.Filters)
                        {
                            if (!filter.IsMatch(context))
                                return false;
                        }
                    }
                }

                return true;
            };

            return allZoos.MapRecords(ZooInfoMapper, filterFunc).OrderBy(zoo => zoo.Name);
        }

        public Zoo GetZooById(long zooId)
        {
            var allZoos = GetCachedZoos();
            return allZoos != null ? allZoos.GetRecord(zooId) : null;
        }

        public string GetZooNameById(long zooId)
        {
            var allZoos = GetCachedZoos();

            if (allZoos == null)
                return null;

            var zoo = allZoos.GetRecord(zooId);

            return zoo != null ? zoo.Name : null;
        }

        public InsertOperationOutput<ZooDetail> AddZoo(Zoo zoo)
        {
            InsertOperationOutput<ZooDetail> insertOperationOutput = new InsertOperationOutput<ZooDetail>();
            insertOperationOutput.Result = InsertOperationResult.Failed;
            insertOperationOutput.InsertedObject = null;
            long zooId = -1;

            IZooDataManager zooDataManager = DemoModuleFactory.GetDataManager<IZooDataManager>();
            bool insertActionSuccess = zooDataManager.Insert(zoo, out zooId);
            if (insertActionSuccess)
            {
                CacheManagerFactory.GetCacheManager<CacheManager>().SetCacheExpired();
                zoo.ZooId = zooId;
                insertOperationOutput.Result = InsertOperationResult.Succeeded;
                insertOperationOutput.InsertedObject = this.ZooDetailMapper(zoo);
            }
            else
            {
                insertOperationOutput.Result = InsertOperationResult.SameExists;
            }

            return insertOperationOutput;
        }

        public UpdateOperationOutput<ZooDetail> UpdateZoo(Zoo zoo)
        {
            UpdateOperationOutput<ZooDetail> updateOperationOutput = new UpdateOperationOutput<ZooDetail>();
            updateOperationOutput.Result = UpdateOperationResult.Failed;
            updateOperationOutput.UpdatedObject = null;

            IZooDataManager zooDataManager = DemoModuleFactory.GetDataManager<IZooDataManager>();
            bool updateActionSuccess = zooDataManager.Update(zoo);
            if (updateActionSuccess)
            {
                CacheManagerFactory.GetCacheManager<CacheManager>().SetCacheExpired();
                updateOperationOutput.Result = UpdateOperationResult.Succeeded;
                updateOperationOutput.UpdatedObject = this.ZooDetailMapper(zoo);
            }
            else
            {
                updateOperationOutput.Result = UpdateOperationResult.SameExists;
            }

            return updateOperationOutput;
        }

        #endregion

        #region Private Methods

        private Dictionary<long, Zoo> GetCachedZoos()
        {
            return CacheManagerFactory.GetCacheManager<CacheManager>().GetOrCreateObject("GetCachedZoos", () =>
            {
                IZooDataManager zooDataManager = DemoModuleFactory.GetDataManager<IZooDataManager>();
                List<Zoo> zoos = zooDataManager.GetZoos();
                return zoos != null && zoos.Count > 0 ? zoos.ToDictionary(zoo => zoo.ZooId, zoo => zoo) : null;
            });
        }

        #endregion

        #region Private Class

        private class CacheManager: BaseCacheManager
        {
            IZooDataManager zooDataManager = DemoModuleFactory.GetDataManager<IZooDataManager>();
            object _updateHandle;

            protected override bool ShouldSetCacheExpired()
            {
                return zooDataManager.AreZoosUpdated(ref _updateHandle);
            }
        }

        #endregion


        #region Mappers

        private ZooDetail ZooDetailMapper(Zoo zoo)
        {
            return new ZooDetail()
            {
                ZooId = zoo.ZooId,
                Name = zoo.Name,
                City = zoo.City,
                Size = zoo.Size
            };
        }

        private ZooInfo ZooInfoMapper(Zoo zoo)
        {
            return new ZooInfo()
            {
                ZooId = zoo.ZooId,
                Name = zoo.Name,
            };
        }

        #endregion
    }
}
