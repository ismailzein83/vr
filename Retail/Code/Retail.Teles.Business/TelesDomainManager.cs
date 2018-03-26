using Retail.BusinessEntity.Business;
using Retail.Teles.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Vanrise.Common;
using Vanrise.Common.Business;
using Vanrise.Entities;
using Vanrise.GenericData.Entities;

namespace Retail.Teles.Business
{
    public class TelesDomainManager : BaseBusinessEntityManager
    {      
        #region Public Methods
        AccountBEManager _accountBEManager = new AccountBEManager();
        public IEnumerable<TelesDomainInfo> GetDomainsInfo(Guid vrConnectionId, TelesDomainFilter filter)
        {
            var cachedDomains = GetCachedDomains(vrConnectionId, false);
            Func<TelesDomainInfo, bool> filterFunc = null;
            if (filter != null)
            {
                filterFunc = (telesDomainInfo) =>
                {
                    return true;
                };
                return cachedDomains.FindAllRecords(filterFunc).OrderBy(x => x.Name);

            }
            return cachedDomains.Values.OrderBy(x => x.Name);
        }
        public TelesDomainInfo GetDomain(Guid vrConnectionId, string domainId)
        {
            var cachedDomains = GetCachedDomains(vrConnectionId, false);
            TelesDomainInfo domainInfo;
            cachedDomains.TryGetValue(domainId, out domainInfo);
            return domainInfo;
        }
        public string GetDomainName(Guid vrConnectionId, string domainId)
        {
            var cachedDomains = GetCachedDomains(vrConnectionId, true);
            if (cachedDomains != null)
            {
                TelesDomainInfo domainInfo;
                if (cachedDomains.TryGetValue(domainId, out domainInfo))
                    return domainInfo.Name;
                else
                    return null;
            }
            else
            {
                return string.Format("{0} (Name unavailable)", domainId);
            }
        }
        public static void SetCacheExpired()
        {
            Vanrise.Caching.CacheManagerFactory.GetCacheManager<CacheManager>().SetCacheExpired();
        }
        #endregion

        #region Private Classes

        internal class CacheManager : Vanrise.Caching.BaseCacheManager
        {
            protected override bool IsTimeExpirable
            {
                get
                {
                    return true;
                }
            }
            protected override bool UseCentralizedCacheRefresher
            {
                get
                {
                    return true;
                }
            }
            
        }

        private class CachedDomainsInfo
        {
            public bool IsValid { get; set; }

            public Dictionary<string, TelesDomainInfo> DomainInfos { get; set; }
        }

        #endregion

        #region Private Methods
        private struct GetDomainCacheName
        {
            public Guid VRConnectionId { get; set; }
        }
        private Dictionary<string, TelesDomainInfo> GetCachedDomains(Guid vrConnectionId, bool handleTelesNotAvailable)
        {
            CachedDomainsInfo domainInfos = Vanrise.Caching.CacheManagerFactory.GetCacheManager<CacheManager>().GetOrCreateObject(new GetDomainCacheName
            {
                VRConnectionId = vrConnectionId
            },
               () =>
               {
                   try
                   {
                       TelesRestConnection telesRestConnection = GetTelesRestConnection(vrConnectionId);
                       string actionPath = string.Format("/domain/search?level=SERVICE_PROVIDER");
                       List<dynamic> domains = telesRestConnection.Get<List<dynamic>>(actionPath);
                       List<TelesDomainInfo> telesDomainInfo = new List<TelesDomainInfo>();
                       if (domains != null)
                       {
                           foreach (var domain in domains)
                           {
                               telesDomainInfo.Add(new TelesDomainInfo
                               {
                                   Name = domain.name,
                                   TelesDomainId = domain.id.Value.ToString()
                               });
                           }
                       }
                       return new CachedDomainsInfo
                       {
                           DomainInfos = telesDomainInfo.ToDictionary(x => x.TelesDomainId, x => x),
                           IsValid = true
                       };
                   }
                   catch (Exception ex)//handle the case where Teles API is not available
                   {
                       LoggerFactory.GetExceptionLogger().WriteException(ex);
                       return new CachedDomainsInfo
                       {
                           IsValid = false
                       };
                   }
               });
            if (domainInfos.IsValid)
            {
                return domainInfos.DomainInfos;
            }
            else
            {
                if (handleTelesNotAvailable)
                    return null;
                else
                    throw new VRBusinessException("Cannot connect to Teles API");
            }
        }
        private TelesRestConnection GetTelesRestConnection(Guid vrConnectionId)
        {
            VRConnectionManager vrConnectionManager = new VRConnectionManager();
            VRConnection vrConnection = vrConnectionManager.GetVRConnection<TelesRestConnection>(vrConnectionId);
            return vrConnection.Settings.CastWithValidate<TelesRestConnection>("telesRestConnection", vrConnectionId);
        }  
        #endregion

        #region IBusinessEntityManager
        public override List<dynamic> GetAllEntities(IBusinessEntityGetAllContext context)
        {
            throw new NotImplementedException();

        }

        public override dynamic GetEntity(IBusinessEntityGetByIdContext context)
        {
            throw new NotImplementedException();

        }

        public override string GetEntityDescription(IBusinessEntityDescriptionContext context)
        {
            throw new NotImplementedException();

        }

        public override dynamic GetEntityId(IBusinessEntityIdContext context)
        {
            throw new NotImplementedException();

        }

        public override IEnumerable<dynamic> GetIdsByParentEntityId(IBusinessEntityGetIdsByParentEntityIdContext context)
        {
            throw new NotImplementedException();
        }

        public override dynamic GetParentEntityId(IBusinessEntityGetParentEntityIdContext context)
        {
            throw new NotImplementedException();
        }

        public override bool IsCacheExpired(IBusinessEntityIsCacheExpiredContext context, ref DateTime? lastCheckTime)
        {
            throw new NotImplementedException();
        }

        public override dynamic MapEntityToInfo(IBusinessEntityMapToInfoContext context)
        {
            throw new NotImplementedException();
        }

        #endregion  
    }
}
