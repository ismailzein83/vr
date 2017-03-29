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
                    else if (context.NeverExpires)
                    {
                        lock (s_cleanableCacheObjects)
                        {
                            s_cleanableCacheObjects.Remove(cacheObject);
                        }
                    }
                }
                if (s_cleanableCacheObjects.Count == 0)
                {
                    lock (s_cleanableCacheObjects)
                    {
                        if (s_cleanableCacheObjects.Count == 0)
                        {
                            s_timer.Enabled = false;
                        }
                    }
                }
                if (anyCacheCleaned)
                {
                    GC.Collect();
                }
            }
            catch (Exception ex)
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
                if (!s_timer.Enabled)
                    s_timer.Enabled = true;
            }
        }
    }
}
