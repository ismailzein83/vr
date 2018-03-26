using Retail.BusinessEntity.Business;
using Retail.BusinessEntity.Entities;
using Retail.Teles.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Vanrise.Common.Business;
using Vanrise.Entities;
using Vanrise.GenericData.Entities;
using Vanrise.Common;
using Retail.Teles.Business.AccountBEActionTypes;
namespace Retail.Teles.Business
{
    public class TelesGatewayManager : BaseBusinessEntityManager
    {
        #region Public Methods
        AccountBEManager _accountBEManager = new AccountBEManager();
        public IEnumerable<TelesGatewayInfo> GetGatewaysInfo(Guid vrConnectionId, string siteId, TelesGatewayFilter filter)
        {
            var cachedSites = GetGatewaysInfoBySiteId(vrConnectionId, siteId);
            Func<TelesGatewayInfo, bool> filterFunc = (telesGatewayInfo) =>
            {
                return true;
            };
            return cachedSites.FindAllRecords(filterFunc).OrderBy(x => x.Name);

        }
        public IEnumerable<dynamic> GetGateways(Guid vrConnectionId, string siteId)
        {
            var actionPath = string.Format("/domain/{0}/gateway", siteId);
            TelesRestConnection telesRestConnection = GetTelesRestConnection(vrConnectionId);
            List<dynamic> sites = telesRestConnection.Get<List<dynamic>>(actionPath);
            return sites;
        }
        public dynamic GetGateway(Guid vrConnectionId, string userId)
        {
            var actionPath = string.Format("/gateway/{0}", userId);
            TelesRestConnection telesRestConnection = GetTelesRestConnection(vrConnectionId);

            return telesRestConnection.Get<dynamic>(actionPath);
        }
        public static void SetCacheExpired()
        {
            Vanrise.Caching.CacheManagerFactory.GetCacheManager<CacheManager>().SetCacheExpired();
        }
        public string GetGatewayeName(Guid vrConnectionId, dynamic telesGatewayId)
        {
            var user = GetGateway(vrConnectionId, telesGatewayId);
            if (user == null)
                return null;
            return user.name;
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
        
        #endregion

        #region Private Methods
        private Dictionary<string, TelesGatewayInfo> GetGatewaysInfoBySiteId(Guid vrConnectionId, string siteId)
        {
            var gateways = GetGateways(vrConnectionId, siteId);
            List<TelesGatewayInfo> telesGatewaysInfo = new List<TelesGatewayInfo>();
            if (gateways != null)
            {
                foreach (var gateway in gateways)
                {
                    telesGatewaysInfo.Add(new TelesGatewayInfo
                    {
                        Name = gateway.name,
                        TelesGatewayId = gateway.id.Value.ToString()
                    });
                }
            }
            return telesGatewaysInfo.ToDictionary(x => x.TelesGatewayId, x => x);
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
