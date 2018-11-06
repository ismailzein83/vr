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
            return GetCachedClientWhSSupplierZonesInfo(userId).GetRecord(filter.SupplierId).FindAllRecords(filterExpression);
        }

        public List<ClientSupplierZoneInfo> GetClientSupplierZoneInfo(int supplierId)
        {
            var userId = SecurityContext.Current.GetLoggedInUserId();
            return GetCachedClientWhSSupplierZonesInfo(userId).GetRecord(supplierId);
        }
        public string GetSupplierZoneName(int supplierId, long supplierZoneId)
        {
            var supplierZones = GetClientSupplierZoneInfo(supplierId);
            return supplierZones.FindRecord(x => x.SupplierZoneId == supplierZoneId).Name;
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
        private Dictionary<int, List<ClientSupplierZoneInfo>> GetCachedClientWhSSupplierZonesInfo(int userId)
        {
            return Vanrise.Caching.CacheManagerFactory.GetCacheManager<CacheManager>().GetOrCreateObject(string.Format("GetCachedClientWhSSupplierZonesInfo_{0}", userId),
              () =>
              {
                  PortalConnectionManager connectionManager = new PortalConnectionManager();
                  var connectionSettings = connectionManager.GetWhSConnectionSettings();
                  WhSCarrierAccountBEManager whSCarrierAccountBEManager = new WhSCarrierAccountBEManager();
                  IEnumerable<ClientAccountInfo> clientAccountsInfo = whSCarrierAccountBEManager.GetRemoteCarrierAccountsInfo(new ClientAccountInfoFilter() { GetSuppliers = true });
                  IEnumerable<ClientSupplierZoneInfo> clientSupplierZonesInfo = connectionSettings.Get<IEnumerable<ClientSupplierZoneInfo>>(string.Format("/api/WhS_BE/SupplierZone/GetClientSupplierZonesInfo?supplierIds={0}", clientAccountsInfo.Select<ClientAccountInfo,int>(x=>x.AccountId)));
                  return clientSupplierZonesInfo == null ? null : clientSupplierZonesInfo.ToDictionary(x => x.SupplierId, zones => clientSupplierZonesInfo.ToList());
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
