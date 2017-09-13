using System;
using System.Collections.Generic;
using System.Linq;
using TOne.WhS.BusinessEntity.Entities;
using Vanrise.GenericData.Entities;
using Vanrise.Common;

namespace TOne.WhS.BusinessEntity.Business
{
    public class WHSFinancialAccountStaticBEManager : IBusinessEntityManager
    {
        WHSFinancialAccountManager whsFinancialAccountManager = new WHSFinancialAccountManager();


        #region IBusinessEntityManager

        public List<dynamic> GetAllEntities(IBusinessEntityGetAllContext context)
        {
            Dictionary<int, WHSFinancialAccount> cachedWHSFinancialAccounts = whsFinancialAccountManager.GetCachedFinancialAccounts();
            return cachedWHSFinancialAccounts.Values.Select(itm => itm as dynamic).ToList();
        }

        public dynamic GetEntity(IBusinessEntityGetByIdContext context)
        {
            return whsFinancialAccountManager.GetFinancialAccount(context.EntityId);
        }

        public string GetEntityDescription(IBusinessEntityDescriptionContext context)
        {
            WHSFinancialAccount whsFinancialAccount = whsFinancialAccountManager.GetFinancialAccount(Convert.ToInt32(context.EntityId));
            return whsFinancialAccountManager.GetDescription(whsFinancialAccount);
        }

        public dynamic GetEntityId(IBusinessEntityIdContext context)
        {
            var whsFinancialAccount = context.Entity as WHSFinancialAccount;
            return whsFinancialAccount.FinancialAccountId;
        }

        public IEnumerable<dynamic> GetIdsByParentEntityId(IBusinessEntityGetIdsByParentEntityIdContext context)
        {
            throw new NotImplementedException();
        }

        public dynamic GetParentEntityId(IBusinessEntityGetParentEntityIdContext context)
        {
            throw new NotImplementedException();
        }

        public bool IsCacheExpired(IBusinessEntityIsCacheExpiredContext context, ref DateTime? lastCheckTime)
        {
            throw new NotImplementedException();
        }

        public dynamic MapEntityToInfo(IBusinessEntityMapToInfoContext context)
        {
            throw new NotImplementedException();
        }

        #endregion
    }
}
