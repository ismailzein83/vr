using NP.IVSwitch.Data;
using NP.IVSwitch.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Vanrise.Common;

namespace NP.IVSwitch.Business
{
    public class DomainManager
    {
        #region Public Methods
       
        public IEnumerable<DomainInfo> GetDomainsInfo(DomainFilter filter)
        {
            Func<Domain, bool> filterExpression = null;

            return this.GetCachedDomain().MapRecords(DomainInfoMapper, filterExpression).OrderBy(x => x.Description);
        }     
 
         

        #endregion

        #region Private Classes

        private class CacheManager : Vanrise.Caching.BaseCacheManager
        {
            IDomainDataManager _dataManager = IVSwitchDataManagerFactory.GetDataManager<IDomainDataManager>();
            protected override bool IsTimeExpirable { get { return true; } }

        }
        #endregion

        #region Private Methods

        Dictionary<Int16, Domain> GetCachedDomain()
        {
            return Vanrise.Caching.CacheManagerFactory.GetCacheManager<CacheManager>().GetOrCreateObject("GetDomain",
                () =>
                {
                    IDomainDataManager dataManager = IVSwitchDataManagerFactory.GetDataManager<IDomainDataManager>();
                    Helper.SetSwitchConfig(dataManager);
                    return dataManager.GetDomains().ToDictionary(x => x.DomainId, x => x);
                });
        }

        #endregion

        #region Mappers

        public DomainInfo DomainInfoMapper(Domain domain)
        {
            DomainInfo domainInfo = new DomainInfo()
            {
                DomainId = domain.DomainId,
                Description = domain.Description,

            };
            return domainInfo;
        }

        #endregion
    }
}
