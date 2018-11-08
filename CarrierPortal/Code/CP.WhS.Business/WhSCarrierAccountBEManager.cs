using CP.WhS.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;
using TOne.WhS.BusinessEntity.APIEntities;
using TOne.WhS.BusinessEntity.Entities;
using Vanrise.Common;
using Vanrise.Entities;
using Vanrise.GenericData.Business;
using Vanrise.GenericData.Entities;
using Vanrise.Security.Business;

namespace CP.WhS.Business
{
    public class WhSCarrierAccountBEManager : BaseBusinessEntityManager
    {
        #region Public Methods
        public IEnumerable<ClientAccountInfo> GetRemoteCarrierAccountsInfo(ClientAccountInfoFilter filter)
        {
            int userId = SecurityContext.Current.GetLoggedInUserId();
            Func<ClientAccountInfo, bool> filterExpression = (carrierAccountInfo) =>
            {
                if(filter != null)
                {
                    if(filter.BusinessEntityDefinitionId.HasValue)
                    {
                        var whSCarrierAccountBEDefition = GetWhSCarrierAccountsBEDefinition(filter.BusinessEntityDefinitionId.Value);
                        if(whSCarrierAccountBEDefition != null)
                        {
                            if (whSCarrierAccountBEDefition.GetCustomers && carrierAccountInfo.CarrierAccountType == ClientAccountType.Supplier)
                                return false;
                            if (whSCarrierAccountBEDefition.GetSuppliers && carrierAccountInfo.CarrierAccountType == ClientAccountType.Customer)
                                return false;
                        }
                    }
                    else
                    {
                        if (filter.GetCustomers && carrierAccountInfo.CarrierAccountType == ClientAccountType.Supplier)
                            return false;
                        if (filter.GetSuppliers && carrierAccountInfo.CarrierAccountType == ClientAccountType.Customer)
                            return false;
                    }
                }
                return true;
            };
            return GetCachedClientWhSAccountsInfo(userId).FindAllRecords(filterExpression);

        }

        public ClientAccountInfo GetCarrierAccountInfo(int carrierAccountId)
        {
            int userId = SecurityContext.Current.GetLoggedInUserId();
            return GetCachedClientWhSAccountsInfo(userId).GetRecord(carrierAccountId);
        }
        public string GetCarrierAccountName(int carrierAccountId)
        {
            var carrierAccount = GetCarrierAccountInfo(carrierAccountId);
            return carrierAccount.Name;
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
        private Dictionary<int, ClientAccountInfo> GetCachedClientWhSAccountsInfo(int userId)
        {
            return Vanrise.Caching.CacheManagerFactory.GetCacheManager<CacheManager>().GetOrCreateObject(string.Format("GetCachedClientWhSAccountsInfo_{0}", userId),
              () =>
              {
                  var filter = new CarrierAccountInfoFilter
                  {
                      GetSuppliers = true,
                      GetCustomers = true,
                      UserId = userId
                  };
                  PortalConnectionManager connectionManager = new PortalConnectionManager();
                  var connectionSettings = connectionManager.GetWhSConnectionSettings();
                  IEnumerable<ClientAccountInfo> clientAccountsInfo = connectionSettings.Get<IEnumerable<ClientAccountInfo>>(string.Format("/api/WhS_BE/CarrierAccount/GetClientAccountsInfo?serializedFilter={0}", Vanrise.Common.Serializer.Serialize(filter)));
                  return clientAccountsInfo == null ? null : clientAccountsInfo.ToDictionary(acc => acc.AccountId, acc => acc);
              });
        }
        private WhSCarrierAccountsBEDefinition GetWhSCarrierAccountsBEDefinition(Guid businessEntityDefinitionId)
        {
            BusinessEntityDefinitionManager businessEntityDefinitionManager = new BusinessEntityDefinitionManager();
            var businessEntityDefinition = businessEntityDefinitionManager.GetBusinessEntityDefinition(businessEntityDefinitionId);
            if (businessEntityDefinition != null)
            {
                return businessEntityDefinition.Settings as WhSCarrierAccountsBEDefinition;
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
            return GetCarrierAccountName(Convert.ToInt32(context.EntityId));
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
