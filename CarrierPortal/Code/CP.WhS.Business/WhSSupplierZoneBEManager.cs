using CP.WhS.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;
using TOne.WhS.BusinessEntity.APIEntities;
using Vanrise.GenericData.Business;
using Vanrise.GenericData.Entities;
using Vanrise.Security.Business;
using Vanrise.Common;
using TOne.WhS.BusinessEntity.Entities;

namespace CP.WhS.Business
{
    public class WhSSupplierZoneBEManager : BaseBusinessEntityManager
    {
        #region Public Methods
        public IEnumerable<ClientSupplierZoneInfo> GetRemoteSupplierZonesInfo(ClientSupplierZoneInfoFilter filter)
        {
            Func<ClientSupplierZoneInfo, bool> filterExpression = (supplierZoneInfo) =>
            {
                return true;
            };
            var userId = SecurityContext.Current.GetLoggedInUserId();
            return GetCachedClientWhSSupplierZonesInfoBySupplierId(userId).GetRecord(filter.SupplierId).FindAllRecords(filterExpression);
        }

        public ClientSupplierZoneInfo GetClientSupplierZoneInfo(long supplierZoneId)
        {
            var userId = SecurityContext.Current.GetLoggedInUserId();
            return GetCachedClientWhSSupplierZonesInfoBySupplierZoneId(userId).GetRecord(supplierZoneId);
        }
        public string GetSupplierZoneName(long supplierZoneId)
        {
            var supplierZone = GetClientSupplierZoneInfo(supplierZoneId);
            return supplierZone!=null ? supplierZone.Name : null;
        }

        public int GetSupplierIdBySupplierZoneIds(IEnumerable<long> supplierZoneIds)
        {
            var userId = SecurityContext.Current.GetLoggedInUserId();
            var supplierZones = this.GetCachedClientWhSSupplierZonesInfoBySupplierZoneId(userId);
            return supplierZones.FindRecord(x => supplierZoneIds.Contains(x.SupplierZoneId)).SupplierId;
        }
        public IEnumerable<ClientSupplierZoneInfo> GetSupplierZoneInfoByIds(List<long> selectedIds)
        {
            var userId = SecurityContext.Current.GetLoggedInUserId();
            List<ClientSupplierZoneInfo> supplierZones = new List<ClientSupplierZoneInfo>();
            var supplierZonesInfo = GetCachedClientWhSSupplierZonesInfoBySupplierZoneId(userId);
            foreach (var id in selectedIds)
            {
                ClientSupplierZoneInfo supplierZone;
                if (supplierZonesInfo.TryGetValue(id, out supplierZone))
                {
                    supplierZones.Add(supplierZone);
                }
            }
            return supplierZones;
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
        private Dictionary<int, List<ClientSupplierZoneInfo>> GetCachedClientWhSSupplierZonesInfoBySupplierId(int userId)
        {
            return Vanrise.Caching.CacheManagerFactory.GetCacheManager<CacheManager>().GetOrCreateObject(string.Format("GetCachedClientWhSSupplierZonesInfoBySupplierId_{0}", userId),
              () =>
              {
                  PortalConnectionManager connectionManager = new PortalConnectionManager();
                  var connectionSettings = connectionManager.GetWhSConnectionSettings();
                  WhSCarrierAccountBEManager whSCarrierAccountBEManager = new WhSCarrierAccountBEManager();
                  IEnumerable<ClientAccountInfo> clientAccountsInfo = whSCarrierAccountBEManager.GetRemoteCarrierAccountsInfo(new ClientAccountInfoFilter() { GetSuppliers = true });
                  IEnumerable<int> supplierIds = clientAccountsInfo.Select<ClientAccountInfo, int>(x => x.AccountId);
                  if(supplierIds!=null && supplierIds.Count() > 0)
                  {
                      IEnumerable<ClientSupplierZoneInfo> clientSupplierZonesInfo = connectionSettings.Post<ClientSupplierZonesInfoInput, IEnumerable<ClientSupplierZoneInfo>>("/api/WhS_BE/SupplierZone/GetClientSupplierZonesInfo", new ClientSupplierZonesInfoInput() { SupplierIds = supplierIds.ToList() });
                      Dictionary<int, List<ClientSupplierZoneInfo>> dic = new Dictionary<int, List<ClientSupplierZoneInfo>>();
                      if(clientSupplierZonesInfo!=null && clientSupplierZonesInfo.Count() > 0)
                      {
                          foreach (var supplierId in supplierIds)
                          {
                              dic.Add(supplierId, clientSupplierZonesInfo.FindAllRecords(x => x.SupplierId == supplierId).ToList());
                          }
                          return dic;
                      }
                  }
                  return null;
              });
        }
        private Dictionary<long, ClientSupplierZoneInfo> GetCachedClientWhSSupplierZonesInfoBySupplierZoneId(int userId)
        {
            return Vanrise.Caching.CacheManagerFactory.GetCacheManager<CacheManager>().GetOrCreateObject(string.Format("GetCachedClientWhSSupplierZonesInfoBySupplierZoneId_{0}", userId),
              () =>
              {
                  PortalConnectionManager connectionManager = new PortalConnectionManager();
                  var connectionSettings = connectionManager.GetWhSConnectionSettings();
                  WhSCarrierAccountBEManager whSCarrierAccountBEManager = new WhSCarrierAccountBEManager();
                  IEnumerable<ClientAccountInfo> clientAccountsInfo = whSCarrierAccountBEManager.GetRemoteCarrierAccountsInfo(new ClientAccountInfoFilter() { GetSuppliers = true });
                  IEnumerable<int> supplierIds = clientAccountsInfo.Select<ClientAccountInfo, int>(x => x.AccountId);
                  if (supplierIds != null && supplierIds.Count() > 0)
                  {
                      IEnumerable<ClientSupplierZoneInfo> clientSupplierZonesInfo = connectionSettings.Post<ClientSupplierZonesInfoInput, IEnumerable<ClientSupplierZoneInfo>>("/api/WhS_BE/SupplierZone/GetClientSupplierZonesInfo", new ClientSupplierZonesInfoInput() { SupplierIds = supplierIds.ToList() });
                      return clientSupplierZonesInfo == null ? null : clientSupplierZonesInfo.ToDictionary(x => x.SupplierZoneId, zones => zones);
                  }
                  return null;
              });
        }
        private WhSSupplierZonesBEDefinition GetSupplierZoneBEDefinition(Guid businessEntityDefinitionId)
        {
            BusinessEntityDefinitionManager businessEntityDefinitionManager = new BusinessEntityDefinitionManager();
            var businessEntityDefinition = businessEntityDefinitionManager.GetBusinessEntityDefinition(businessEntityDefinitionId);
            if (businessEntityDefinition != null)
            {
                return businessEntityDefinition.Settings as WhSSupplierZonesBEDefinition;
            }
            return null;
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
            return GetSupplierZoneName(Convert.ToInt64(context.EntityId));
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
