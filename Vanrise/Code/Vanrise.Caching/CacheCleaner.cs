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
        static DateTime s_lastCleanCacheTime;
        static Object s_cleanCacheLockObject = new object();

        static TimeSpan s_cleanCacheInterval;
        static long s_cleanCacheMemoryThreshold;
        static int s_cleanCacheSizePriorityFactor;
        static int s_cleanCacheAgePriorityFactor;

        static CacheCleaner()
        {
            if(!TimeSpan.TryParse(ConfigurationManager.AppSettings["CleanCacheInterval"], out s_cleanCacheInterval))
                s_cleanCacheInterval = new TimeSpan(0, 0, 2);
            if (!long.TryParse(ConfigurationManager.AppSettings["CleanCacheMemoryThreshold"], out s_cleanCacheMemoryThreshold))
                s_cleanCacheMemoryThreshold = 1000000000;
            if (!int.TryParse(ConfigurationManager.AppSettings["CleanCacheSizePriorityFactor"], out s_cleanCacheSizePriorityFactor))
                s_cleanCacheSizePriorityFactor = 10;
            if (!int.TryParse(ConfigurationManager.AppSettings["CleanCacheAgePriorityFactor"], out s_cleanCacheAgePriorityFactor))
                s_cleanCacheAgePriorityFactor = 1;
        }


        internal static void CleanCacheIfNeeded()
        {
            if ((DateTime.Now - s_lastCleanCacheTime) <= s_cleanCacheInterval)
                return;

            if (s_isCleaningTask)
                return;

            CleanCacheAsync();
        }

        static bool s_isCleaningTask;
        private static void CleanCacheAsync()
        {
            System.Threading.Tasks.Task task = new System.Threading.Tasks.Task(() =>
            {
                lock (s_cleanCacheLockObject)
                {
                    if (s_isCleaningTask)
                        return;
                    s_isCleaningTask = true;
                }
                try
                {
                    long memorySize = GC.GetTotalMemory(true);
                    if (memorySize < s_cleanCacheMemoryThreshold)
                        return;
                    DateTime startTime = DateTime.Now;
                    List<ICacheManager> allCacheManagers = new List<ICacheManager>();
                    allCacheManagers.AddRange(CacheManagerFactory.s_defaultCacheManagers.Values);
                    CacheManagerForManagers cacheManagerForManagers = CacheManagerFactory.GetCacheManager<CacheManagerForManagers>();
                    var cachedCacheManagers = cacheManagerForManagers.GetAllCachedObjects();
                    if (cachedCacheManagers != null)
                        allCacheManagers.AddRange(cachedCacheManagers.Select(itm => itm.Object as ICacheManager));

                    List<CachedObjectWithCacheManager> allCachedObjects = new List<CachedObjectWithCacheManager>();
                    Dictionary<CacheObjectSize, CacheObjectSizeSummary> cacheObjectSizeSummaries = new Dictionary<CacheObjectSize, CacheObjectSizeSummary>();
                    foreach (var cacheManager in allCacheManagers)
                    {
                        var cachedObjects = cacheManager.GetAllCachedObjects();
                        if (cachedObjects != null)
                        {
                            foreach (var obj in cachedObjects)
                            {
                                allCachedObjects.Add(new CachedObjectWithCacheManager
                                {
                                    CachedObject = obj,
                                    ParentCacheManager = cacheManager
                                });
                                CacheObjectSizeSummary cacheObjectSizeSummary = cacheObjectSizeSummaries.GetOrCreateItem(obj.ApproximateSize);
                                cacheObjectSizeSummary.NumberOfItems++;
                            }
                        }
                    }
                    List<CachedObjectWithCacheManager> orderedCachedObjects =
                        allCachedObjects.OrderByDescending(itm => ((int)itm.CachedObject.ApproximateSize * s_cleanCacheSizePriorityFactor + (DateTime.Now - itm.CachedObject.LastAccessedTime).TotalSeconds * s_cleanCacheAgePriorityFactor)).ToList();
                    allCachedObjects.Clear();
                    int itemsToRemove = (int)(((double)(memorySize - s_cleanCacheMemoryThreshold) / s_cleanCacheMemoryThreshold) * orderedCachedObjects.Count);
                    int removedItems = 0;

                    DateTime olderCacheAccessTime = DateTime.MaxValue;
                    DateTime newerCacheAccessTime = DateTime.MinValue;
                    while (orderedCachedObjects.Count > 0 && removedItems < itemsToRemove)
                    {
                        var firstCachedObject = orderedCachedObjects[0];
                        firstCachedObject.ParentCacheManager.RemoveObjectFromCache(firstCachedObject.CachedObject);
                        orderedCachedObjects.RemoveAt(0);
                        cacheObjectSizeSummaries[firstCachedObject.CachedObject.ApproximateSize].RemovedItems++;
                        if (firstCachedObject.CachedObject.LastAccessedTime < olderCacheAccessTime)
                            olderCacheAccessTime = firstCachedObject.CachedObject.LastAccessedTime;
                        if (firstCachedObject.CachedObject.LastAccessedTime > newerCacheAccessTime)
                            newerCacheAccessTime = firstCachedObject.CachedObject.LastAccessedTime;
                        removedItems++;
                    }
                    if (removedItems > 0)
                    {
                        GC.Collect();
                        //GC.WaitForPendingFinalizers();
                        StringBuilder msgBuilder = new StringBuilder();
                        msgBuilder.AppendLine(String.Format("Cache Cleaned. Taken Time {0} sec", (DateTime.Now - startTime).TotalSeconds));
                        msgBuilder.AppendLine(String.Format("memory size '{0}'", memorySize));
                        msgBuilder.AppendLine(String.Format("removed items from cache '{0}'", removedItems));
                        msgBuilder.AppendLine(String.Format("remaining items in cache '{0}'", orderedCachedObjects.Count));
                        msgBuilder.AppendLine("Removed Items Information:");
                        msgBuilder.AppendLine(String.Format("Older Cache Object: {0} sec. Newer Cache Object: {1} sec", (DateTime.Now - olderCacheAccessTime).TotalSeconds, (DateTime.Now - newerCacheAccessTime).TotalSeconds));
                        foreach (var itm in cacheObjectSizeSummaries)
                        {
                            msgBuilder.AppendLine(String.Format("{0}/{1} removed '{2}' size", itm.Value.RemovedItems, itm.Value.NumberOfItems, itm.Key));
                        }
                        msgBuilder.AppendLine();
                        Common.LoggerFactory.GetLogger().WriteWarning(msgBuilder.ToString());
                        //CleanCacheAsync();
                    }
                }
                catch (Exception ex)
                {
                    Common.LoggerFactory.GetExceptionLogger().WriteException(ex);
                }
                finally
                {
                    lock (s_cleanCacheLockObject)
                    {
                        s_lastCleanCacheTime = DateTime.Now;
                        s_isCleaningTask = false;
                    }
                }

            });
            task.Start();
        }

        private class CachedObjectWithCacheManager
        {
            public CachedObject CachedObject { get; set; }

            public ICacheManager ParentCacheManager { get; set; }
        }

        private class CacheObjectSizeSummary
        {
            public int NumberOfItems { get; set; }

            public int RemovedItems { get; set; }
        }
    }
}
