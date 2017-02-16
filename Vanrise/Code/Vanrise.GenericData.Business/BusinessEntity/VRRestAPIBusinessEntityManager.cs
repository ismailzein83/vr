using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Vanrise.Common.Business;
using Vanrise.GenericData.Entities;

namespace Vanrise.GenericData.Business
{
    public class VRRestAPIBusinessEntityManager
    {
        #region Public Methods

        //public IEnumerable<BusinessEntityInfo> GetBusinessEntitiesInfo(Guid businessEntityDefinitionId)
        //{
        //    var businessEntityDefinition = new BusinessEntityDefinitionManager().GetBusinessEntityDefinition(businessEntityDefinitionId);
        //    if(businessEntityDefinition == null)
        //        throw new NullReferenceException(string.Format("businessEntityDefinition {0}", businessEntityDefinitionId));

        //    var vrRestAPIBEDefinitionSettings = GetVRRestAPIBEDefinitionSettings(businessEntityDefinition);
        //    VRInterAppRestConnection connectionSettings = GetVRInterAppRestConnection(vrRestAPIBEDefinitionSettings.ConnectionId);

        //    return connectionSettings.Get<IEnumerable<BusinessEntityInfo>>(string.Format("/api/VR_GenericData/BusinessEntity/GetBusinessEntitiesInfo?businessEntityDefinitionId={0}", vrRestAPIBEDefinitionSettings.RemoteBEDefinitionId));
        //}

        #endregion

        #region Private Methods

        //private VRRestAPIBEDefinitionSettings GetVRRestAPIBEDefinitionSettings(BusinessEntityDefinition beDefinition)
        //{
        //    var businessEntityDefinitionSettings = beDefinition.Settings;
        //    if (businessEntityDefinitionSettings == null)
        //        throw new NullReferenceException(string.Format("businessEntityDefinition.Settings {0}", beDefinition.));

        //    var vrRestAPIBEDefinitionSettings = businessEntityDefinitionSettings as VRRestAPIBEDefinitionSettings;
        //    if (vrRestAPIBEDefinitionSettings == null)
        //        throw new NullReferenceException("businessEntityDefinition should be of type VRRestAPIBEDefinitionSettings");

        //    return vrRestAPIBEDefinitionSettings;
        //}

        //private VRInterAppRestConnection GetVRInterAppRestConnection(Guid connectionId)
        //{
        //    VRConnectionManager connectionManager = new VRConnectionManager();
        //    var vrConnection = connectionManager.GetVRConnection<VRInterAppRestConnection>(connectionId);
        //    return vrConnection.Settings as VRInterAppRestConnection;
        //}

        #endregion

        #region IBusinessEntityManager

        //public List<dynamic> GetAllEntities(IBusinessEntityGetAllContext context)
        //{
        //    var connectionId = GetConnectionId(context.EntityDefinition);
        //    VRInterAppRestConnection connectionSettings = GetVRInterAppRestConnection(connectionId);
        //    return connectionSettings.Get<List<dynamic>>(string.Format("/api/VR_GenericData/BusinessEntity/GetAllEntities?businessEntityDefinitionId={0}", context.EntityDefinitionId));
        //}

        //public dynamic GetEntity(IBusinessEntityGetByIdContext context)
        //{
        //    return GetAccount(context.EntityDefinitionId, context.EntityId);
        //}

        //public string GetEntityDescription(IBusinessEntityDescriptionContext context)
        //{
        //    return GetAccountName(context.EntityDefinition.BusinessEntityDefinitionId, Convert.ToInt64(context.EntityId));
        //}

        //public bool IsCacheExpired(IBusinessEntityIsCacheExpiredContext context, ref DateTime? lastCheckTime)
        //{
        //    return Vanrise.Caching.CacheManagerFactory.GetCacheManager<CacheManager>().IsCacheExpired(context.EntityDefinitionId, ref lastCheckTime);
        //}

        //public IEnumerable<dynamic> GetIdsByParentEntityId(IBusinessEntityGetIdsByParentEntityIdContext context)
        //{
        //    throw new NotImplementedException();
        //}

        //public dynamic GetParentEntityId(IBusinessEntityGetParentEntityIdContext context)
        //{
        //    throw new NotImplementedException();
        //}

        //public dynamic MapEntityToInfo(IBusinessEntityMapToInfoContext context)
        //{
        //    switch (context.InfoType)
        //    {
        //        case Vanrise.AccountBalance.Entities.AccountInfo.BEInfoType:
        //            {
        //                var account = context.Entity as Account;
        //                StatusDefinitionManager statusDefinitionManager = new Business.StatusDefinitionManager();
        //                var statusDesciption = statusDefinitionManager.GetStatusDefinitionName(account.StatusId);
        //                Vanrise.AccountBalance.Entities.AccountInfo accountInfo = new Vanrise.AccountBalance.Entities.AccountInfo
        //                {
        //                    Name = account.Name,
        //                    StatusDescription = statusDesciption,
        //                };
        //                var currency = GetCurrencyId(account.Settings.Parts.Values);
        //                if (currency.HasValue)
        //                {
        //                    accountInfo.CurrencyId = currency.Value;
        //                }
        //                else
        //                {
        //                    throw new Exception(string.Format("Account {0} does not have currency", accountInfo.Name));
        //                }
        //                return accountInfo;
        //            }
        //        default: return null;
        //    }
        //}

        #endregion
    }
}
