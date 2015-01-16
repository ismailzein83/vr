using System;
using System.Collections.Generic;
using System.Text;
using System.Collections;

namespace TABS.Components
{
	public class CacheProvider : NHibernate.Cache.ICacheProvider
	{
        protected static log4net.ILog log = log4net.LogManager.GetLogger(typeof(CacheProvider));
		protected static Dictionary<string, NHibernate.Cache.ICache> _RegionCaches = new Dictionary<string, NHibernate.Cache.ICache>();
        public static Dictionary<string, NHibernate.Cache.ICache> RegionCaches { get { return _RegionCaches; } }

        /// <summary>
        /// Clear the cache for a given region
        /// </summary>
        /// <param name="regionName"></param>
        public static void Clear(string regionName)
        {
            NHibernate.Cache.ICache cache = null;
            if (_RegionCaches.TryGetValue(regionName, out cache))
                cache.Clear();
        }

		public void Clear()
		{
            TABS.DataConfiguration.Default.SessionFactory.EvictQueries();
            foreach (NHibernate.Cache.ICache cache in _RegionCaches.Values)
                cache.Clear();
		}

		#region ICacheProvider Members

		public NHibernate.Cache.ICache BuildCache(string regionName, System.Collections.Generic.IDictionary<string,string> properties)
		{
			log.InfoFormat("Building Cache For Region {1}", DateTime.Now, regionName);
            NHibernate.Cache.ICache cache = null;
            if (!_RegionCaches.TryGetValue(regionName, out cache))
                cache = new Cache<object,object>(regionName,properties);
            return cache;
		}

		public long NextTimestamp()
		{
			return NHibernate.Cache.Timestamper.Next();
		}

        //public void Start(System.Collections.IDictionary properties)
        //{
        //    log.InfoFormat("Cache Provider Started.", DateTime.Now);
        //}
        public void Start(System.Collections.Generic.IDictionary<string, string> properties)
        {
            log.InfoFormat("Cache Provider Started.", DateTime.Now);
        }
		public void Stop()
		{
			log.InfoFormat("Cache Provider Stopped.", DateTime.Now);
		}

		#endregion
	}

    /// <summary>
    /// The Cache Region Container
    /// </summary>
    /// <typeparam name="KEY"></typeparam>
    /// <typeparam name="VALUE"></typeparam>
    public class Cache<KEY, VALUE> : Dictionary<KEY, VALUE>, NHibernate.Cache.ICache
    {
        string _regionName;

        public Cache(string regionName, System.Collections.Generic.IDictionary<string,string> properties) : base()
        {
            _regionName = regionName;
            CacheProvider.RegionCaches[regionName] = this;
        }

        public void Destroy()
        {
            base.Clear();
        }

        public object Get(object key)
        {
            VALUE value = default(VALUE);
            base.TryGetValue((KEY)key, out value);
            return value;
        }

        /// <summary>
        /// Lock the object given by the specified key
        /// </summary>
        /// <param name="key"></param>
        public void Lock(object key)
        {
            // this probably should do something if the object is lockable
        }

        public long NextTimestamp()
        {
            return NHibernate.Cache.Timestamper.Next();
        }

        public void Put(object key, object value)
        {
            base[(KEY)key] = (VALUE)value;
        }

        public string RegionName
        {
            get
            {
                return _regionName;
            }
        }

        public int Timeout
        {
            get { return int.MaxValue; }
        }

        /// <summary>
        /// Unlock the object given by the specified key
        /// </summary>
        /// <param name="key"></param>
        public void Unlock(object key)
        {
            // this probably should do something if the object is lockable
        }

        public void Remove(object key)
        {
            base.Remove((KEY)key);
        }
    }
}