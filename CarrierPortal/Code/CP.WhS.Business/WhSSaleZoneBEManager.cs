using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;
using TOne.WhS.BusinessEntity.APIEntities;
using Vanrise.GenericData.Entities;
using Vanrise.Security.Business;
using Vanrise.Common;
using CP.WhS.Entities;
using TOne.WhS.BusinessEntity.Entities;

namespace CP.WhS.Business
{
    public class WhSSaleZoneBEManager : BaseBusinessEntityManager
    {
        #region Public Methods
        public IEnumerable<ClientSaleZoneInfo> GetRemoteSaleZonesInfo(ClientSaleZoneInfoFilter filter)
        {
            Func<ClientSaleZoneInfo, bool> filterExpression = (saleZoneInfo) =>
            {
                return true;
            };
            var userId = SecurityContext.Current.GetLoggedInUserId();
            return GetCachedClientWhSSaleZonesInfoBySaleZoneId(userId).FindAllRecords(filterExpression);
        }
        public ClientSaleZoneInfo GetClientSaleZoneInfo(long saleZoneId)
        {
            var userId = SecurityContext.Current.GetLoggedInUserId();
            return GetCachedClientWhSSaleZonesInfoBySaleZoneId(userId).GetRecord(saleZoneId);
        }
        public string GetSaleZoneName(long saleZoneId)
        {
            var saleZone = GetClientSaleZoneInfo(saleZoneId);
            return saleZone.Name;
        }

        public IEnumerable<ClientSaleZoneInfo> GetSaleZoneInfoByIds(List<long> selectedIds)
        {
            var userId = SecurityContext.Current.GetLoggedInUserId();
            List<ClientSaleZoneInfo> saleZones = new List<ClientSaleZoneInfo>();
            var saleZonesInfo = GetCachedClientWhSSaleZonesInfoBySaleZoneId(userId);
            foreach (var id in selectedIds)
            {
                ClientSaleZoneInfo saleZone;
                if (saleZonesInfo.TryGetValue(id, out saleZone))
                {
                    saleZones.Add(saleZone);
                }
            }
            return saleZones;
        }
        #endregion

        #region Private Classes
        private class CacheManager : Vanrise.Caching.BaseCacheManager
        {
            protected override bool IsTimeExpirable
            {
                get { return true; }
            }
        }

        #endregion

        #region Private Methods
        private Dictionary<long, ClientSaleZoneInfo> GetCachedClientWhSSaleZonesInfoBySaleZoneId(int userId)
        {
            return Vanrise.Caching.CacheManagerFactory.GetCacheManager<CacheManager>().GetOrCreateObject(string.Format("GetCachedClientWhSSaleZonesInfoBySaleZoneId_{0}", userId),
              () =>
              {
                  PortalConnectionManager connectionManager = new PortalConnectionManager();
                  var connectionSettings = connectionManager.GetWhSConnectionSettings();
                  var saleZones = connectionSettings.Get<IEnumerable<ClientSaleZoneInfo>>(string.Format("/api/WhS_BE/SaleZone/GetClientSaleZonesInfo?serializedFilter={0}", Serializer.Serialize(new SaleZoneInfoFilter())));
                  return saleZones!=null ? saleZones.ToDictionary(x=>x.SaleZoneId, x=>x) : null;
              });
        }
        #endregion
        #region BaseBusinessEntityManager
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
            return GetSaleZoneName(Convert.ToInt64(context.EntityId));
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
