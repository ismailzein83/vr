using System;
using System.Collections.Generic;
using System.Linq;
using TOne.WhS.BusinessEntity.Entities;
using Vanrise.GenericData.Entities;
using Vanrise.Common;

namespace TOne.WhS.BusinessEntity.Business
{
    public class WHSFinancialAccountStaticBEManager : BaseBusinessEntityManager
    {
        WHSFinancialAccountManager whsFinancialAccountManager = new WHSFinancialAccountManager();


        #region IBusinessEntityManager

        public override List<dynamic> GetAllEntities(IBusinessEntityGetAllContext context)
        {
            Dictionary<int, WHSFinancialAccount> cachedWHSFinancialAccounts = whsFinancialAccountManager.GetCachedFinancialAccounts();
            return cachedWHSFinancialAccounts.Values.Select(itm => itm as dynamic).ToList();
        }

        public override dynamic GetEntity(IBusinessEntityGetByIdContext context)
        {
            return whsFinancialAccountManager.GetFinancialAccount(context.EntityId);
        }

        public override string GetEntityDescription(IBusinessEntityDescriptionContext context)
        {
            WHSFinancialAccount whsFinancialAccount = whsFinancialAccountManager.GetFinancialAccount(Convert.ToInt32(context.EntityId));
            return whsFinancialAccountManager.GetDescription(whsFinancialAccount);
        }

        public override dynamic GetEntityId(IBusinessEntityIdContext context)
        {
            var whsFinancialAccount = context.Entity as WHSFinancialAccount;
            return whsFinancialAccount.FinancialAccountId;
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
