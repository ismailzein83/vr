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

        #region Public Methods
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

        public string GetExtraFieldValueDescription(string fieldName, object fieldValue)
        {
            return new GenericBusinessEntityManager().GetFieldDescription(_configuration.FinancialAccountBEDefinitionId, fieldName, fieldValue);
        }

        public object GetFinancialAccountObject(string accountId)
        {
            return new GenericBusinessEntityManager().GetGenericBEObject(_configuration.FinancialAccountBEDefinitionId, accountId);
        }
        #endregion

        #region Private Methods
        private GenericFinancialAccount GenericFinancialAccountMapper(GenericBusinessEntity genericBusinessEntity)
        {

            GenericFinancialAccount genericFinancialAccount = new GenericFinancialAccount();
            foreach (var fieldValue in genericBusinessEntity.FieldValues)
            {
                if (_configuration.BEDFieldName == fieldValue.Key)
                {
                    genericFinancialAccount.BED = (DateTime?)fieldValue.Value;
                }
                else
                if (_configuration.EEDFieldName == fieldValue.Key)
                {
                    genericFinancialAccount.EED = (DateTime?)fieldValue.Value;
                }
                else
                if (_configuration.AccountNameFieldName == fieldValue.Key)
                {
                    genericFinancialAccount.Name = fieldValue.Value as string;
                }
                else
                if (_configuration.FinancialAccountIdFieldName == fieldValue.Key)
                {
                    genericFinancialAccount.FinancialAccountId = fieldValue.Value.ToString();
                }
                else
                if (_configuration.CurrencyIdFieldName == fieldValue.Key)
                {
                    genericFinancialAccount.CurrencyId = (int)fieldValue.Value;
                }
                else
                if (_configuration.StatusIdFieldName == fieldValue.Key)
                {
                    var statusDefinitionId = (Guid)fieldValue.Value;
                    var statusDefinition = new StatusDefinitionManager().GetStatusDefinition(statusDefinitionId);
                    statusDefinition.ThrowIfNull("statusDefinition", statusDefinitionId);
                    statusDefinition.Settings.ThrowIfNull("statusDefinition.Settings", statusDefinitionId);
                    genericFinancialAccount.Status = statusDefinition.Settings.IsActive ? VRAccountStatus.Active : VRAccountStatus.InActive;
                }
                else
                {
                    if (genericFinancialAccount.ExtraFields == null)
                        genericFinancialAccount.ExtraFields = new Dictionary<string, object>();

                    genericFinancialAccount.ExtraFields.Add(fieldValue.Key, fieldValue.Value);
                }
            }
            return genericFinancialAccount;
        }

        #endregion
    }
}
