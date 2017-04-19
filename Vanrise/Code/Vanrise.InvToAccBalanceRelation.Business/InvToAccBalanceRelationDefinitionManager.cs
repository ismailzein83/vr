using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Vanrise.InvToAccBalanceRelation.Entities;
using Vanrise.Common;
using Vanrise.Common.Business;
using Vanrise.AccountBalance.Business;

namespace Vanrise.InvToAccBalanceRelation.Business
{
    public class InvToAccBalanceRelationDefinitionManager
    {
        #region Ctor/Properties

        Vanrise.Common.Business.VRComponentTypeManager _vrComponentTypeManager = new Common.Business.VRComponentTypeManager();

        #endregion

        #region Public Methods

        public List<InvoiceAccountInfo> GetBalanceInvoiceAccounts(Guid accountTypeId, string accountId, DateTime effectiveOn)
        {
            var relationId = GetRelationIdByAccountTypeID(accountTypeId);
            InvToAccBalanceRelationDefinitionExtendedSettings relationSettings = GetRelationExtendedSettings(relationId);
            relationSettings.ThrowIfNull("relationSettings", relationId);
            var context = new InvToAccBalanceRelGetBalanceInvoiceAccountsContext { AccountTypeId = accountTypeId, AccountId = accountId, EffectiveOn = effectiveOn };
            return relationSettings.GetBalanceInvoiceAccounts(context);
        }

        public List<BalanceAccountInfo> GetInvoiceBalanceAccounts(Guid invoiceTypeId, string PartnerId, DateTime effectiveOn)
        {
            var relationId = GetRelationIdByInvoiceTypeID(invoiceTypeId);
            InvToAccBalanceRelationDefinitionExtendedSettings relationSettings = GetRelationExtendedSettings(relationId);
            relationSettings.ThrowIfNull("relationSettings", relationId);
            var context = new InvToAccBalanceRelGetInvoiceBalanceAccountsContext { InvoiceTypeId = invoiceTypeId, PartnerId = PartnerId, EffectiveOn = effectiveOn };
            return relationSettings.GetInvoiceBalanceAccounts(context);
        }

        public InvToAccBalanceRelationDefinitionExtendedSettings GetRelationExtendedSettings(Guid relationId)
        {
            var relationSetting =  _vrComponentTypeManager.GetComponentTypeSettings<InvToAccBalanceRelationDefinitionSettings>(relationId);
            if (relationSetting == null)
                return null;
            return relationSetting.ExtendedSettings;
        }
        public IEnumerable<RelationDefinitionExtendedSettingsConfig> GetRelationDefinitionExtendedSettingsConfigs()
        {
            var extensionConfiguration = new ExtensionConfigurationManager();
            return extensionConfiguration.GetExtensionConfigurations<RelationDefinitionExtendedSettingsConfig>(RelationDefinitionExtendedSettingsConfig.EXTENSION_TYPE);
        }
        public IEnumerable<InvToAccBalanceRelationDefinitionInfo> GetRelationDefinitionInfos(InvToAccBalanceRelationDefinitionInfoFilter filter)
        {
            Func<InvToAccBalanceRelationDefinition, bool> filterExpression = null;
            if (filter != null)
            {
            }
            return _vrComponentTypeManager.GetComponentTypes<InvToAccBalanceRelationDefinitionSettings, InvToAccBalanceRelationDefinition>().MapRecords(RelationDefinitionInfoMapper, filterExpression);
        }
        #endregion

        #region Private Classes

      


        #endregion

        #region Private Methods
        private Guid GetRelationIdByAccountTypeID(Guid accountTypeId)
        {
            AccountTypeManager accountTypeManager = new AccountTypeManager();
            var relationId = accountTypeManager.GetInvToAccBalanceRelationId(accountTypeId);
            if (!relationId.HasValue)
                throw new NullReferenceException("relationId");
            return relationId.Value;
        }
        private Guid GetRelationIdByInvoiceTypeID(Guid invoiceTypeId)
        {
            throw new NotImplementedException();
        }
        #endregion


        #region Mappers

        private InvToAccBalanceRelationDefinitionInfo RelationDefinitionInfoMapper(InvToAccBalanceRelationDefinition relationDefinition)
        {
            return new InvToAccBalanceRelationDefinitionInfo
            {
                InvToAccBalanceRelationDefinitionId = relationDefinition.VRComponentTypeId,
                Name = relationDefinition.Name,
            };
        }

        #endregion
    }
}
