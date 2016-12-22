using NP.IVSwitch.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using NP.IVSwitch.Data;
using Vanrise.Entities;
using Vanrise.Common;

namespace NP.IVSwitch.Business
{
    public class FirewallManager
    {
        #region public
        public IDataRetrievalResult<FirewallDetail> GetFilteredFirewalls(DataRetrievalInput<FirewallQuery> input)
        {
            var allFirewalls = this.GetCachedFirewalls();
            Func<Firewall, bool> filterExpression = x => (input.Query.Host == null || x.Host.Contains(input.Query.Host));
            return DataRetrievalManager.Instance.ProcessResult(input, allFirewalls.ToBigResult(input, filterExpression, FirewallDetailMapper));
        }
        #endregion


        #region Private Classes

        private class CacheManager : Vanrise.Caching.BaseCacheManager
        {
            IFirewallDataManager _dataManager = IVSwitchDataManagerFactory.GetDataManager<IFirewallDataManager>();
            public DateTime lastCheckTime { get; set; }
            protected override bool IsTimeExpirable { get { return true; } }
        }
        #endregion

        #region Private Methods

        Dictionary<int, Firewall> GetCachedFirewalls()
        {
            return Vanrise.Caching.CacheManagerFactory.GetCacheManager<CacheManager>().GetOrCreateObject("GetCachedFirewalls",
               () =>
               {
                   IFirewallDataManager dataManager = IVSwitchDataManagerFactory.GetDataManager<IFirewallDataManager>();
                   Helper.SetSwitchConfig(dataManager);
                   return dataManager.GetFirewalls().ToDictionary(x => x.RecId, x => x);
               });
        }

        #endregion
        #region mapper

        FirewallDetail FirewallDetailMapper(Firewall firewall)
        {
            return new FirewallDetail
            {
                Entity = firewall
            };
        }
        #endregion
    }
}
