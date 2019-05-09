using Retail.BusinessEntity.Data;
using Retail.BusinessEntity.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using Vanrise.Caching;
using Vanrise.Entities;
using Vanrise.Runtime;

namespace Retail.BusinessEntity.Business
{
    public class PackageUsageVolumeCombinationManager
    {
        #region Public Methods

        public Dictionary<string, int> InsertAndGetCombinationIds(Dictionary<string, BasePackageUsageVolumeCombination> packageCombinationsDict)
        {
            int maxLockRetryCount = Int32.MaxValue;
            TimeSpan lockRetryInterval = new TimeSpan(0, 0, 1);
            string transactionLockName = "PackageUsageVolumeCombination";

            Dictionary<string, int> results = new Dictionary<string, int>();
            IPackageUsageVolumeCombinationDataManager dataManager = BEDataManagerFactory.GetDataManager<IPackageUsageVolumeCombinationDataManager>();

            Dictionary<string, PackageUsageVolumeCombination> cachedPackageUsageVolumeCombinations = this.GetCachedPackageUsageVolumeCombinations();

            int maxCombinationId = 0;
            if (cachedPackageUsageVolumeCombinations != null && cachedPackageUsageVolumeCombinations.Count > 0)
                maxCombinationId = cachedPackageUsageVolumeCombinations.Select(itm => itm.Value.PackageUsageVolumeCombinationId).Max();

            int retryCount = 0;
            while (retryCount < maxLockRetryCount)
            {
                if (TransactionLocker.Instance.TryLock(transactionLockName, () =>
                {
                    Dictionary<int, PackageUsageVolumeCombination> newPackageVolumeCombinationsById = dataManager.GetPackageUsageVolumeCombinationAfterID(maxCombinationId);

                    int combinationId = maxCombinationId;
                    if (newPackageVolumeCombinationsById != null && newPackageVolumeCombinationsById.Count > 0)
                    {
                        foreach (var newPackageVolumeCombinationKvp in newPackageVolumeCombinationsById)
                        {
                            var packageUsageVolumeCombinationId = newPackageVolumeCombinationKvp.Key;
                            var packageUsageVolumeCombination = newPackageVolumeCombinationKvp.Value;
                            combinationId = Math.Max(combinationId, packageUsageVolumeCombination.PackageUsageVolumeCombinationId);
                            cachedPackageUsageVolumeCombinations.Add(Helper.SerializePackageCombinations(packageUsageVolumeCombination.PackageItemsByPackageId), packageUsageVolumeCombination);
                        }
                    }
                    combinationId++;

                    Dictionary<string, PackageUsageVolumeCombination> packageUsageVolumeCombinationsToAddToCache = new Dictionary<string, PackageUsageVolumeCombination>();

                    Object dbApplyStream = dataManager.InitialiazeStreamForDBApply();
                    foreach (var packageCombinationsKvp in packageCombinationsDict)
                    {
                        string packageCombinations = packageCombinationsKvp.Key;
                        BasePackageUsageVolumeCombination basePackageUsageVolumeCombination = packageCombinationsKvp.Value;

                        PackageUsageVolumeCombination packageUsageVolumeCombination;
                        if (cachedPackageUsageVolumeCombinations == null || !cachedPackageUsageVolumeCombinations.TryGetValue(packageCombinations, out packageUsageVolumeCombination))
                        {
                            packageUsageVolumeCombination = new PackageUsageVolumeCombination
                            {
                                PackageUsageVolumeCombinationId = combinationId,
                                PackageItemsByPackageId = basePackageUsageVolumeCombination.PackageItemsByPackageId
                            };
                            packageUsageVolumeCombinationsToAddToCache.Add(packageCombinations, packageUsageVolumeCombination);
                            results.Add(packageCombinations, combinationId);
                            dataManager.WriteRecordToStream(packageUsageVolumeCombination, dbApplyStream);
                            combinationId++;
                        }
                        else
                        {
                            results.Add(packageCombinations, packageUsageVolumeCombination.PackageUsageVolumeCombinationId);
                        }
                    }
                    object obj = dataManager.FinishDBApplyStream(dbApplyStream);
                    dataManager.ApplyPackageUsageVolumeCombinationForDB(obj);

                    if (packageUsageVolumeCombinationsToAddToCache.Count > 0)
                    {
                        if (cachedPackageUsageVolumeCombinations == null)
                            cachedPackageUsageVolumeCombinations = new Dictionary<string, PackageUsageVolumeCombination>();

                        foreach (var packageUsageVolumeCombinationToAddToCache in packageUsageVolumeCombinationsToAddToCache)
                            cachedPackageUsageVolumeCombinations.Add(packageUsageVolumeCombinationToAddToCache.Key, packageUsageVolumeCombinationToAddToCache.Value);
                    }
                }))
                {
                    return results;
                }
                else
                {
                    Thread.Sleep(lockRetryInterval);
                    retryCount++;
                }
            }

            throw new Exception("Cannot Lock Retail_BE.PackageUsageVolumeCombination");
        }

        public PackageUsageVolumeCombination GetPackageUsageVolumeCombination(int combinationId)
        {
            Dictionary<int, PackageUsageVolumeCombination> cachedPackageUsageVolumeCombinationsById = this.GetCachedPackageUsageVolumeCombinationsById();

            int maxCombinationId = 0;
            if (cachedPackageUsageVolumeCombinationsById != null && cachedPackageUsageVolumeCombinationsById.Count > 0)
                maxCombinationId = cachedPackageUsageVolumeCombinationsById.Select(itm => itm.Value.PackageUsageVolumeCombinationId).Max();

            if (cachedPackageUsageVolumeCombinationsById == null)
                cachedPackageUsageVolumeCombinationsById = new Dictionary<int, PackageUsageVolumeCombination>();

            IPackageUsageVolumeCombinationDataManager dataManager = BEDataManagerFactory.GetDataManager<IPackageUsageVolumeCombinationDataManager>();
            Dictionary<int, PackageUsageVolumeCombination> newPackageVolumeCombinationsById = dataManager.GetPackageUsageVolumeCombinationAfterID(maxCombinationId);

            if (newPackageVolumeCombinationsById != null && newPackageVolumeCombinationsById.Count > 0)
            {
                foreach (var newPackageVolumeCombinationKvp in newPackageVolumeCombinationsById)
                    cachedPackageUsageVolumeCombinationsById.Add(newPackageVolumeCombinationKvp.Key, newPackageVolumeCombinationKvp.Value);
            }

            PackageUsageVolumeCombination packageUsageVolumeCombination;
            if (!cachedPackageUsageVolumeCombinationsById.TryGetValue(combinationId, out packageUsageVolumeCombination))
                throw new VRBusinessException($"cachedPackageUsageVolumeCombinationsById does not contain combinationId: '{combinationId}'");

            return packageUsageVolumeCombination;
        }

        #endregion

        #region Private Methods

        private Dictionary<string, PackageUsageVolumeCombination> GetCachedPackageUsageVolumeCombinations()
        {
            var cacheManager = Vanrise.Caching.CacheManagerFactory.GetCacheManager<PackageUsageVolumeCombinationCacheManager>();

            return cacheManager.GetOrCreateObject("GetCachedPackageUsageVolumeCombinations", PackageUsageVolumeCombinationCacheExpirationChecker.Instance, () =>
            {
                Dictionary<string, PackageUsageVolumeCombination> results = new Dictionary<string, PackageUsageVolumeCombination>();

                var packageUsageVolumeCombinationsById = GetCachedPackageUsageVolumeCombinationsById();
                if (packageUsageVolumeCombinationsById != null)
                {
                    foreach (var packageUsageVolumeCombinationKvp in packageUsageVolumeCombinationsById)
                        results.Add(Helper.SerializePackageCombinations(packageUsageVolumeCombinationKvp.Value.PackageItemsByPackageId), packageUsageVolumeCombinationKvp.Value);
                }

                return results.Count > 0 ? results : null;
            });
        }

        private Dictionary<int, PackageUsageVolumeCombination> GetCachedPackageUsageVolumeCombinationsById()
        {
            var cacheManager = Vanrise.Caching.CacheManagerFactory.GetCacheManager<PackageUsageVolumeCombinationCacheManager>();

            return cacheManager.GetOrCreateObject("GetCachedPackageUsageVolumeCombinationsById", PackageUsageVolumeCombinationCacheExpirationChecker.Instance, () =>
            {
                Dictionary<int, PackageUsageVolumeCombination> results = new Dictionary<int, PackageUsageVolumeCombination>();

                IPackageUsageVolumeCombinationDataManager dataManager = BEDataManagerFactory.GetDataManager<IPackageUsageVolumeCombinationDataManager>();
                List<PackageUsageVolumeCombination> packageUsageVolumeCombinations = dataManager.GetAllPackageUsageVolumeCombinations();
                if (packageUsageVolumeCombinations != null)
                {
                    foreach (PackageUsageVolumeCombination packageUsageVolumeCombination in packageUsageVolumeCombinations)
                        results.Add(packageUsageVolumeCombination.PackageUsageVolumeCombinationId, packageUsageVolumeCombination);
                }

                return results.Count > 0 ? results : null;
            });
        }

        #endregion

        #region Private Classes

        private class PackageUsageVolumeCombinationCacheManager : BaseCacheManager
        {

        }

        private class PackageUsageVolumeCombinationCacheExpirationChecker : CacheExpirationChecker
        {
            static PackageUsageVolumeCombinationCacheExpirationChecker s_instance = new PackageUsageVolumeCombinationCacheExpirationChecker();
            public static PackageUsageVolumeCombinationCacheExpirationChecker Instance { get { return s_instance; } }

            public override bool IsCacheExpired(Vanrise.Caching.ICacheExpirationCheckerContext context)
            {
                TimeSpan entitiesTimeSpan = TimeSpan.FromMinutes(15);
                SlidingWindowCacheExpirationChecker slidingWindowCacheExpirationChecker = new SlidingWindowCacheExpirationChecker(entitiesTimeSpan);
                return slidingWindowCacheExpirationChecker.IsCacheExpired(context);
            }
        }

        #endregion
    }
}