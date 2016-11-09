using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Vanrise.Common;

namespace Vanrise.Caching
{
    internal static class CacheCleaner
    {
        static System.Timers.Timer s_timer;

        static CacheCleaner()
        {
            s_timer = new System.Timers.Timer(5000);
            s_timer.Elapsed += s_timer_Elapsed;            
        }

        internal static void Start()
        {
            s_timer.Start();
        }

        static bool s_isRunning;
        static Object s_lockObj = new object();
        static void s_timer_Elapsed(object sender, System.Timers.ElapsedEventArgs e)
        {
            if (s_isRunning)
                return;
            lock(s_lockObj)
            {
                if (s_isRunning)
                    return;
                s_isRunning = true;
            }

            try
            {

                List<CachedObject> copyOfCleanableCacheObjects = new List<CachedObject>();
                copyOfCleanableCacheObjects.AddRange(s_cleanableCacheObjects);

                bool anyCacheCleaned = false;
                foreach (var cacheObject in copyOfCleanableCacheObjects)
                {
                    CacheExpirationCheckerContext context = new CacheExpirationCheckerContext { CachedObject = cacheObject };
                    if (cacheObject.CacheExpirationChecker.IsCacheExpired(context))
                    {
                        cacheObject.CacheManager.RemoveObjectFromCache(cacheObject);
                        lock (s_cleanableCacheObjects)
                        {
                            s_cleanableCacheObjects.Remove(cacheObject);
                        }
                        anyCacheCleaned = true;
                    }
                }
                if (anyCacheCleaned)
                {
                    GC.Collect();
                }
            }
            catch(Exception ex)
            {
                LoggerFactory.GetExceptionLogger().WriteException(ex);
            }

            lock(s_lockObj)
            {
                s_isRunning = false;
            }
        }

        static List<CachedObject> s_cleanableCacheObjects = new List<CachedObject>();
        internal static void SetCacheObjectCleanable(CachedObject cachedObject)
        {
            if (cachedObject == null)
                throw new ArgumentNullException("cachedObject");
            if (cachedObject.CacheExpirationChecker == null)
                throw new NullReferenceException("cachedObject.CacheExpirationChecker");
            if (cachedObject.CacheName == null)
                throw new NullReferenceException("cachedObject.CacheName");
            if (cachedObject.CacheManager == null)
                throw new NullReferenceException("cachedObject.CacheManager");
            lock(s_cleanableCacheObjects)
            {
                s_cleanableCacheObjects.Add(cachedObject);
            }
        }
    }

    //internal static class CacheCleaner
    //{
    //    static DateTime s_lastCleanCacheTime;
    //    static Object s_cleanCacheLockObject = new object();

    //    static TimeSpan s_cleanCacheInterval;
    //    static long s_cleanCacheMemoryThreshold;
    //    static int s_cleanCacheSizePriorityFactor;
    //    static int s_cleanCacheAgePriorityFactor;

    //    static CacheCleaner()
    //    {
    //        if(!TimeSpan.TryParse(ConfigurationManager.AppSettings["VanriseCache_CleanInterval"], out s_cleanCacheInterval))
    //            s_cleanCacheInterval = new TimeSpan(0, 1, 0);
    //        if (!long.TryParse(ConfigurationManager.AppSettings["VanriseCache_CleanMemoryThreshold"], out s_cleanCacheMemoryThreshold))
    //            s_cleanCacheMemoryThreshold = 2000000000;
    //        if (!int.TryParse(ConfigurationManager.AppSettings["VanriseCache_CleanSizePriorityFactor"], out s_cleanCacheSizePriorityFactor))
    //            s_cleanCacheSizePriorityFactor = 10;
    //        if (!int.TryParse(ConfigurationManager.AppSettings["VanriseCache_CleanAgePriorityFactor"], out s_cleanCacheAgePriorityFactor))
    //            s_cleanCacheAgePriorityFactor = 1;
    //    }




    //    internal static void CleanCacheIfNeeded()
    //    {
    //        if ((VRClock.Now - s_lastCleanCacheTime) <= s_cleanCacheInterval)
    //            return;

    //        if (s_isCleaningTask)
    //            return;

    //        CleanCacheAsync();
    //    }

    //    static bool s_isCleaningTask;
    //    private static void CleanCacheAsync()
    //    {
    //        System.Threading.Tasks.Task task = new System.Threading.Tasks.Task(() =>
    //        {
    //            lock (s_cleanCacheLockObject)
    //            {
    //                if (s_isCleaningTask)
    //                    return;
    //                s_isCleaningTask = true;
    //            }
    //            try
    //            {
    //                long memorySize = GC.GetTotalMemory(true);
    //                if (memorySize < s_cleanCacheMemoryThreshold)
    //                    return;
    //                DateTime startTime = DateTime.Now;
    //                List<ICacheManager> allCacheManagers = GetAllCacheManagers();

    //                List<CachedObjectWithCacheManager> allCachedObjects = new List<CachedObjectWithCacheManager>();
    //                Dictionary<CacheObjectSize, CacheObjectSizeSummary> cacheObjectSizeSummaries = new Dictionary<CacheObjectSize, CacheObjectSizeSummary>();
    //                foreach (var cacheManager in allCacheManagers)
    //                {
    //                    var cachedObjects = cacheManager.GetAllCachedObjects();
    //                    if (cachedObjects != null)
    //                    {
    //                        foreach (var obj in cachedObjects)
    //                        {
    //                            allCachedObjects.Add(new CachedObjectWithCacheManager
    //                            {
    //                                CachedObject = obj,
    //                                ParentCacheManager = cacheManager
    //                            });
    //                            CacheObjectSizeSummary cacheObjectSizeSummary = cacheObjectSizeSummaries.GetOrCreateItem(obj.ApproximateSize);
    //                            cacheObjectSizeSummary.NumberOfItems++;
    //                        }
    //                    }
    //                }
    //                List<CachedObjectWithCacheManager> orderedCachedObjects =
    //                    allCachedObjects.OrderByDescending(itm => ((int)itm.CachedObject.ApproximateSize * s_cleanCacheSizePriorityFactor + (DateTime.Now - itm.CachedObject.LastAccessedTime).TotalSeconds * s_cleanCacheAgePriorityFactor)).ToList();
    //                allCachedObjects.Clear();
    //                int itemsToRemove = (int)(((double)(memorySize - s_cleanCacheMemoryThreshold) / s_cleanCacheMemoryThreshold) * orderedCachedObjects.Count);
    //                int removedItems = 0;

    //                DateTime olderCacheAccessTime = DateTime.MaxValue;
    //                DateTime newerCacheAccessTime = DateTime.MinValue;
    //                while (orderedCachedObjects.Count > 0 && removedItems < itemsToRemove)
    //                {
    //                    var firstCachedObject = orderedCachedObjects[0];
    //                    firstCachedObject.ParentCacheManager.RemoveObjectFromCache(firstCachedObject.CachedObject);
    //                    orderedCachedObjects.RemoveAt(0);
    //                    cacheObjectSizeSummaries[firstCachedObject.CachedObject.ApproximateSize].RemovedItems++;
    //                    if (firstCachedObject.CachedObject.LastAccessedTime < olderCacheAccessTime)
    //                        olderCacheAccessTime = firstCachedObject.CachedObject.LastAccessedTime;
    //                    if (firstCachedObject.CachedObject.LastAccessedTime > newerCacheAccessTime)
    //                        newerCacheAccessTime = firstCachedObject.CachedObject.LastAccessedTime;
    //                    removedItems++;
    //                }
    //                if (removedItems > 0)
    //                {
    //                    GC.Collect();
    //                    //GC.WaitForPendingFinalizers();
    //                    LogWarning(memorySize, (DateTime.Now - startTime).TotalSeconds, cacheObjectSizeSummaries, orderedCachedObjects.Count, removedItems, olderCacheAccessTime, newerCacheAccessTime);
    //                    //CleanCacheAsync();
    //                }
    //            }
    //            catch (Exception ex)
    //            {
    //                Common.LoggerFactory.GetExceptionLogger().WriteException(ex);
    //            }
    //            finally
    //            {
    //                lock (s_cleanCacheLockObject)
    //                {
    //                    s_lastCleanCacheTime = DateTime.Now;
    //                    s_isCleaningTask = false;
    //                }
    //            }

    //        });
    //        task.Start();
    //    }

    //    private static List<ICacheManager> GetAllCacheManagers()
    //    {
    //        List<ICacheManager> allCacheManagers = new List<ICacheManager>();
    //        allCacheManagers.AddRange(CacheManagerFactory.s_defaultCacheManagers.Values);
    //        CacheManagerForManagers cacheManagerForManagers = CacheManagerFactory.GetCacheManager<CacheManagerForManagers>();
    //        var cachedCacheManagers = cacheManagerForManagers.GetAllCachedObjects();
    //        if (cachedCacheManagers != null)
    //            allCacheManagers.AddRange(cachedCacheManagers.Select(itm => itm.Object as ICacheManager));
    //        return allCacheManagers;
    //    }

    //    private static void LogWarning(long memorySize, double takenTime, Dictionary<CacheObjectSize, CacheObjectSizeSummary> cacheObjectSizeSummaries, int remainingItemsInCache, int removedItems, DateTime olderCacheAccessTime, DateTime newerCacheAccessTime)
    //    {
    //        StringBuilder msgBuilder = new StringBuilder();
    //        msgBuilder.AppendLine(String.Format("Cache Cleaned. Taken Time {0} sec", takenTime));
    //        msgBuilder.AppendLine(String.Format("memory size '{0}'", memorySize));
    //        msgBuilder.AppendLine(String.Format("removed items from cache '{0}'", removedItems));
    //        msgBuilder.AppendLine(String.Format("remaining items in cache '{0}'", remainingItemsInCache));
    //        msgBuilder.AppendLine("Removed Items Information:");
    //        msgBuilder.AppendLine(String.Format("Older Cache Object: {0} sec. Newer Cache Object: {1} sec", (DateTime.Now - olderCacheAccessTime).TotalSeconds, (DateTime.Now - newerCacheAccessTime).TotalSeconds));
    //        foreach (var itm in cacheObjectSizeSummaries)
    //        {
    //            msgBuilder.AppendLine(String.Format("{0}/{1} removed '{2}' size", itm.Value.RemovedItems, itm.Value.NumberOfItems, itm.Key));
    //        }
    //        msgBuilder.AppendLine();
    //        Common.LoggerFactory.GetLogger().WriteWarning(msgBuilder.ToString());
    //    }

    //    private class CachedObjectWithCacheManager
    //    {
    //        public CachedObject CachedObject { get; set; }

    //        public ICacheManager ParentCacheManager { get; set; }
    //    }

    //    private class CacheObjectSizeSummary
    //    {
    //        public int NumberOfItems { get; set; }

    //        public int RemovedItems { get; set; }
    //    }
    //}
}
