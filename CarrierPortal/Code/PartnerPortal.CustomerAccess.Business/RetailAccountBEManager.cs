using Retail.BusinessEntity.APIEntities;
using System;
using System.Collections.Generic;
using Vanrise.Common;
using Vanrise.GenericData.Business;
using Vanrise.GenericData.Entities;

namespace PartnerPortal.CustomerAccess.Business
{
    public class RetailAccountBEManager : BaseBusinessEntityManager
    { 
        #region Ctor/Fields

        static BusinessEntityDefinitionManager s_businessEntityDefinitionManager = new BusinessEntityDefinitionManager();

        #endregion

        #region Public Methods

        public IEnumerable<ClientAccountInfo> GetClientChildAccountsInfo(Guid businessEntityDefinitionId)
        {
            var retailBusinessEntityDefinitionSettings = GetRetailUserSubaccountsBEDefinition(businessEntityDefinitionId);
            retailBusinessEntityDefinitionSettings.ThrowIfNull("retailBusinessEntityDefinitionSettings");
            RetailAccountUserManager retailAccountUserManager = new Business.RetailAccountUserManager();
            var clientChildAccountsInfo = retailAccountUserManager.GetClientRetailAccountsInfo(retailBusinessEntityDefinitionSettings.VRConnectionId);
            Func<ClientAccountInfo, bool> filterExpression = (clientChildAccountInfo) =>
            {
                if (retailBusinessEntityDefinitionSettings.AccountTypeIds != null && !retailBusinessEntityDefinitionSettings.AccountTypeIds.Contains(clientChildAccountInfo.TypeId))
                    return false;
                return true;
            };

            return clientChildAccountsInfo != null ? clientChildAccountsInfo.FindAllRecords(filterExpression) : null; ;
        }

        public RetailUserSubaccountsBEDefinition GetRetailUserSubaccountsBEDefinition(Guid businessEntityDefinitionId)
        {
            var businessEntityDefinition = s_businessEntityDefinitionManager.GetBusinessEntityDefinition(businessEntityDefinitionId);
            businessEntityDefinition.ThrowIfNull("businessEntityDefinition", businessEntityDefinitionId);
            return businessEntityDefinition.Settings.CastWithValidate<RetailUserSubaccountsBEDefinition>("businessEntityDefinition.Settings", businessEntityDefinitionId);
        }

        public ClientAccountInfo GetClientAccountInfo(Guid businessEntityDefinitionId, long accountId)
        {
            var clientAccountsInfo = GetClientChildAccountsInfo(businessEntityDefinitionId);
            return clientAccountsInfo == null ? null : clientAccountsInfo.FindRecord(x => x.AccountId == accountId);
        }

        public string GetClientAccountInfoName(Guid businessEntityDefinitionId, long accountId)
        {
            var clientAccountInfo = GetClientAccountInfo(businessEntityDefinitionId, accountId);
            if (clientAccountInfo != null)
                return clientAccountInfo.Name;
            else
                return null;
        }

        #endregion

        #region IBusinessEntityManager

        public override List<dynamic> GetAllEntities(IBusinessEntityGetAllContext context)
        {
            throw new NotImplementedException();
        }

        public override dynamic GetEntity(IBusinessEntityGetByIdContext context)
        {
            return GetClientAccountInfo(context.EntityDefinitionId, Convert.ToInt64(context.EntityId));
        }

        public override string GetEntityDescription(IBusinessEntityDescriptionContext context)
        {
            return GetClientAccountInfoName(context.EntityDefinition.BusinessEntityDefinitionId, Convert.ToInt64(context.EntityId));
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
