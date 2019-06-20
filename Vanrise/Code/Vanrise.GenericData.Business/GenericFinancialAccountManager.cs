using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Vanrise.GenericData.Entities;
using Vanrise.Common;
using Vanrise.Common.Business;
using Vanrise.Entities;

namespace Vanrise.GenericData.Business
{
    public class GenericFinancialAccountManager
    {
        GenericFinancialAccountConfiguration _configuration;
        public GenericFinancialAccountManager(GenericFinancialAccountConfiguration configuration)
        {
            configuration.ThrowIfNull("financialAccountMappingFieldsConfigration");
            _configuration = configuration;
        }
        public GenericFinancialAccount GetFinancialAccount(string accountId)
        {
            var entity = new GenericBusinessEntityManager().GetGenericBusinessEntity(accountId, _configuration.FinancialAccountBEDefinitionId);
            if (entity == null)
                return null;
            return GenericFinancialAccountMapper(entity);
        }

        public IEnumerable<GenericFinancialAccount> GetFinancialAccounts()
        {
            var entities = new GenericBusinessEntityManager().GetAllGenericBusinessEntities(_configuration.FinancialAccountBEDefinitionId);
            if (entities == null)
                return null;
            List<GenericFinancialAccount> financialAccounts = new List<GenericFinancialAccount>();

            return entities.Select(item => GenericFinancialAccountMapper(item));
        }

        private GenericFinancialAccount GenericFinancialAccountMapper(GenericBusinessEntity genericBusinessEntity)
        {

            GenericFinancialAccount genericFinancialAccount = new GenericFinancialAccount();

            if (_configuration.BEDFieldName != null)
            {
                genericFinancialAccount.BED = (DateTime?)genericBusinessEntity.FieldValues.GetRecord(_configuration.BEDFieldName);
            }
            if (_configuration.EEDFieldName != null)
            {
                genericFinancialAccount.EED = (DateTime?)genericBusinessEntity.FieldValues.GetRecord(_configuration.EEDFieldName);
            }
            if (_configuration.FinancialAccountIdFieldName != null)
            {
                genericFinancialAccount.Name = genericBusinessEntity.FieldValues.GetRecord(_configuration.AccountNameFieldName) as string;
            }
            if (_configuration.FinancialAccountIdFieldName != null)
            {
                genericFinancialAccount.FinancialAccountId = genericBusinessEntity.FieldValues.GetRecord(_configuration.FinancialAccountIdFieldName) as string;
            }
            if (_configuration.CurrencyIdFieldName != null)
            {
                genericFinancialAccount.CurrencyId = (int)genericBusinessEntity.FieldValues.GetRecord(_configuration.CurrencyIdFieldName);
            }
            if (_configuration.StatusIdFieldName != null)
            {
                var statusDefinitionId = (Guid)genericBusinessEntity.FieldValues.GetRecord(_configuration.StatusIdFieldName);
                var statusDefinition = new StatusDefinitionManager().GetStatusDefinition(statusDefinitionId);
                statusDefinition.ThrowIfNull("statusDefinition", statusDefinitionId);
                statusDefinition.Settings.ThrowIfNull("statusDefinition.Settings", statusDefinitionId);
                genericFinancialAccount.Status = statusDefinition.Settings.IsActive ? VRAccountStatus.Active : VRAccountStatus.InActive;
            }
            return genericFinancialAccount;
        }
    }
}
