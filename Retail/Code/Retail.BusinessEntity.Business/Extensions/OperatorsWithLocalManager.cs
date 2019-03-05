using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Vanrise.GenericData.Business;
using Vanrise.GenericData.Entities;
using Vanrise.Common;
using Retail.BusinessEntity.Entities;
using Vanrise.Common.Business;

namespace Retail.BusinessEntity.Business
{
    public class OperatorsWithLocalManager : BaseBusinessEntityManager
    {
        static BusinessEntityDefinitionManager s_businessEntityDefinitionManager = new BusinessEntityDefinitionManager();
        static AccountBEManager s_accountBEManager = new AccountBEManager();

        #region BaseBusinessEntityManager Members

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
            long accountId = Convert.ToInt64(context.EntityId);

            var interconnectSettings = new SettingManager().GetSetting<InterconnectSettings>(InterconnectSettings.SETTING_TYPE);
            interconnectSettings.ThrowIfNull("interconnectSettings");
            if (accountId == interconnectSettings.LocalOperatorID)
                return interconnectSettings.LocalOperatorName;

            var operatorWithLocalDefinition = context.EntityDefinition.Settings.CastWithValidate<OperatorsWithLocalDefinitionSettings>("context.EntityDefinition.Settings");

            return s_accountBEManager.GetAccountName(operatorWithLocalDefinition.AccountBEDefintionId, accountId);
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

        #region Public Methods

        public List<AccountInfo> GetOperatorsWithLocal(Guid businessEntityDefinitionId)
        {
            var operatorWithLocalDefinition = this.GetICXOperatorBusinessEntityDefintionSettings(businessEntityDefinitionId);

            IEnumerable<AccountInfo> icxOperators = s_accountBEManager.GetAccountsInfo(operatorWithLocalDefinition.AccountBEDefintionId, null, null);

            var interconnectSettings = new SettingManager().GetSetting<InterconnectSettings>(InterconnectSettings.SETTING_TYPE);
            interconnectSettings.ThrowIfNull("interconnectSettings");
            AccountInfo localAccount = new AccountInfo();
            localAccount.AccountId = interconnectSettings.LocalOperatorID;
            localAccount.Name = interconnectSettings.LocalOperatorName;

            List<AccountInfo> accountsWithLocal = new List<AccountInfo>();
            accountsWithLocal.Add(localAccount);
            accountsWithLocal.AddRange(icxOperators);

            return accountsWithLocal;
        }

        public long GetLocalOperatorId()
        {
            var interconnectSettings = new SettingManager().GetSetting<InterconnectSettings>(InterconnectSettings.SETTING_TYPE);
            interconnectSettings.ThrowIfNull("interconnectSettings");
            return interconnectSettings.LocalOperatorID;
        }

        #endregion

        #region Private Methods

        private OperatorsWithLocalDefinitionSettings GetICXOperatorBusinessEntityDefintionSettings(Guid businessEntityDefinitionId)
        {
            BusinessEntityDefinition businessEntityDefinition = s_businessEntityDefinitionManager.GetBusinessEntityDefinition(businessEntityDefinitionId);

            if (businessEntityDefinition == null)
                throw new NullReferenceException(string.Format("businessEntityDefinition Id : {0}", businessEntityDefinitionId));

            if (businessEntityDefinition.Settings == null)
                throw new NullReferenceException(string.Format("businessEntityDefinition.Settings Id : {0}", businessEntityDefinitionId));

            return businessEntityDefinition.Settings as OperatorsWithLocalDefinitionSettings;
        }

        #endregion
    }
}
