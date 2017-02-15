using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Vanrise.Common.Business;
using Vanrise.GenericData.Entities;

namespace Vanrise.GenericData.Business
{
    public class VRRestAPIBEDefinitionManager
    {
        #region IBusinessEntityManager

        //public List<dynamic> GetAllEntities(IBusinessEntityGetAllContext context)
        //{
        //    var cachedAccounts = GetCachedAccounts(context.EntityDefinitionId);
        //    if (cachedAccounts != null)
        //        return cachedAccounts.Values.Select(itm => itm as dynamic).ToList();
        //    else
        //        return null;
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
