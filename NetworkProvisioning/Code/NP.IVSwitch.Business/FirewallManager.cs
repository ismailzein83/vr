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
            Func<Firewall, bool> filterExpression =
                x =>
                    ((input.Query.Host == null || x.Host.Contains(input.Query.Host)) &&
                     (input.Query.Description == null ||
                      x.Description.Contains(input.Query.Description)));
            return DataRetrievalManager.Instance.ProcessResult(input, allFirewalls.ToBigResult(input, filterExpression, FirewallDetailMapper));
        }
        public Firewall GetFirewall(int firewallId)
        {
            Dictionary<int, Firewall> cachedFirewalls = GetCachedFirewalls();
            return cachedFirewalls.GetRecord(firewallId);
        }
        public InsertOperationOutput<FirewallDetail> AddFirewall(Firewall firewall)
        {
            var insertOperationOutput = new InsertOperationOutput<FirewallDetail>
            {
                Result = InsertOperationResult.Failed,
                InsertedObject = null
            };
            IFirewallDataManager dataManager = IVSwitchDataManagerFactory.GetDataManager<IFirewallDataManager>();
            Helper.SetSwitchConfig(dataManager);
            int firewallId;

            if (dataManager.Insert(firewall, out  firewallId))
            {
                Vanrise.Caching.CacheManagerFactory.GetCacheManager<CacheManager>().SetCacheExpired();
                insertOperationOutput.Result = InsertOperationResult.Succeeded;
                insertOperationOutput.InsertedObject = FirewallDetailMapper(GetFirewall(firewallId));
            }
            else
            {
                insertOperationOutput.Result = InsertOperationResult.SameExists;
            }
            return insertOperationOutput;
        }
        public UpdateOperationOutput<FirewallDetail> UpdateFirewall(Firewall firewallItem)
        {
            var updateOperationOutput = new UpdateOperationOutput<FirewallDetail>
            {
                Result = UpdateOperationResult.Failed,
                UpdatedObject = null
            };
            IFirewallDataManager dataManager = IVSwitchDataManagerFactory.GetDataManager<IFirewallDataManager>();
            Helper.SetSwitchConfig(dataManager);
            if (dataManager.Update(firewallItem))
            {
                Vanrise.Caching.CacheManagerFactory.GetCacheManager<CacheManager>().SetCacheExpired();
                updateOperationOutput.Result = UpdateOperationResult.Succeeded;
                updateOperationOutput.UpdatedObject = FirewallDetailMapper(GetFirewall(firewallItem.Id));
            }
            else
            {
                updateOperationOutput.Result = UpdateOperationResult.SameExists;
            }
            return updateOperationOutput;
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
                   return dataManager.GetFirewalls().ToDictionary(x => x.Id, x => x);
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
