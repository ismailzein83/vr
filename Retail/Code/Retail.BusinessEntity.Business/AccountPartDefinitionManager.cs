using Retail.BusinessEntity.Data;
using Retail.BusinessEntity.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Vanrise.Caching;
using Vanrise.Common;
using Vanrise.Common.Business;
using Vanrise.Entities;
namespace Retail.BusinessEntity.Business
{
    public class AccountPartDefinitionManager
    {
        #region Public Methods

        public Vanrise.Entities.IDataRetrievalResult<AccountPartDefinitionDetail> GetFilteredAccountPartDefinitions(Vanrise.Entities.DataRetrievalInput<AccountPartDefinitionQuery> input)
        {
            Dictionary<Guid, AccountPartDefinition> cachedAccountPartDefinitions = this.GetCachedAccountPartDefinitions();

            Func<AccountPartDefinition, bool> filterExpression = (accountPartDefinition) =>
                (input.Query.Name == null || accountPartDefinition.Name.ToLower().Contains(input.Query.Name.ToLower()));

            return Vanrise.Common.DataRetrievalManager.Instance.ProcessResult(input, cachedAccountPartDefinitions.ToBigResult(input, filterExpression, AccountPartDefinitionDetailMapper));
        }

        public AccountPartDefinition GetAccountPartDefinition(Guid accountPartDefinitionId)
        {
            return this.GetCachedAccountPartDefinitions().GetRecord(accountPartDefinitionId);
        }

        public IEnumerable<AccountPartDefinitionInfo> GetAccountPartDefinitionsInfo()
        {
            return this.GetCachedAccountPartDefinitions().MapRecords(AccountPartDefinitionInfoMapper).OrderBy(x => x.Title);
        }

        public IEnumerable<AccountPartDefinitionConfig> GetAccountPartDefinitionExtensionConfigs()
        {
            ExtensionConfigurationManager manager = new ExtensionConfigurationManager();
            return manager.GetExtensionConfigurations<AccountPartDefinitionConfig>(AccountPartDefinitionConfig.EXTENSION_TYPE);
        }

        public Vanrise.Entities.InsertOperationOutput<AccountPartDefinitionDetail> AddAccountPartDefinition(AccountPartDefinition accountPartDefinition)
        {
            ValidateAccountPartDefinitionToAdd(accountPartDefinition);

            var insertOperationOutput = new Vanrise.Entities.InsertOperationOutput<AccountPartDefinitionDetail>();

            insertOperationOutput.Result = Vanrise.Entities.InsertOperationResult.Failed;
            insertOperationOutput.InsertedObject = null;

            IAccountPartDefinitionDataManager dataManager = BEDataManagerFactory.GetDataManager<IAccountPartDefinitionDataManager>();
            accountPartDefinition.AccountPartDefinitionId = Guid.NewGuid();

            if (dataManager.Insert(accountPartDefinition))
            {
                Vanrise.Caching.CacheManagerFactory.GetCacheManager<CacheManager>().SetCacheExpired();
                insertOperationOutput.Result = Vanrise.Entities.InsertOperationResult.Succeeded;
                insertOperationOutput.InsertedObject = AccountPartDefinitionDetailMapper(accountPartDefinition);
            }
            else
            {
                insertOperationOutput.Result = Vanrise.Entities.InsertOperationResult.SameExists;
            }

            return insertOperationOutput;
        }

        public Vanrise.Entities.UpdateOperationOutput<AccountPartDefinitionDetail> UpdateAccountPartDefinition(AccountPartDefinitionToEdit accountPartDefinitionToEdit)
        {
            ValidateAccountPartDefinitionToEdit(accountPartDefinitionToEdit);

            var updateOperationOutput = new Vanrise.Entities.UpdateOperationOutput<AccountPartDefinitionDetail>();

            updateOperationOutput.Result = Vanrise.Entities.UpdateOperationResult.Failed;
            updateOperationOutput.UpdatedObject = null;

            IAccountPartDefinitionDataManager dataManager = BEDataManagerFactory.GetDataManager<IAccountPartDefinitionDataManager>();

            AccountPartDefinition accountPartDefinition = new AccountPartDefinition
            {
                AccountPartDefinitionId = accountPartDefinitionToEdit.AccountPartDefinitionId,
                Name = accountPartDefinitionToEdit.Name,
                Settings = accountPartDefinitionToEdit.Settings,
                Title = accountPartDefinitionToEdit.Title,
            };


            if (dataManager.Update(accountPartDefinition))
            {
                Vanrise.Caching.CacheManagerFactory.GetCacheManager<CacheManager>().SetCacheExpired();
                updateOperationOutput.Result = Vanrise.Entities.UpdateOperationResult.Succeeded;
                updateOperationOutput.UpdatedObject = AccountPartDefinitionDetailMapper(this.GetAccountPartDefinition(accountPartDefinition.AccountPartDefinitionId));
            }
            else
            {
                updateOperationOutput.Result = Vanrise.Entities.UpdateOperationResult.SameExists;
            }

            return updateOperationOutput;
        }

        public List<AccountPartDefinition> GetAccountPartDefinitions()
        {
            var cachedPartDefinitions = GetCachedAccountPartDefinitions();
            return cachedPartDefinitions != null ? cachedPartDefinitions.Values.ToList() : null;
        }

        #endregion

        #region Validation Methods

        private void ValidateAccountPartDefinitionToAdd(AccountPartDefinition accountPartDefinition)
        {
            ValidateAccountPartDefinition(accountPartDefinition.Name, accountPartDefinition.Title, accountPartDefinition.Settings);
        }

        private void ValidateAccountPartDefinitionToEdit(AccountPartDefinitionToEdit accountPartDefinition)
        {
            AccountPartDefinition accountPartDefinitionEntity = this.GetAccountPartDefinition(accountPartDefinition.AccountPartDefinitionId);
            if (accountPartDefinitionEntity == null)
                throw new DataIntegrityValidationException(String.Format("AccountPartDefinition '{0}' does not exist", accountPartDefinition.AccountPartDefinitionId));
            ValidateAccountPartDefinition(accountPartDefinition.Name, accountPartDefinition.Title, accountPartDefinition.Settings);
        }

        private void ValidateAccountPartDefinition(string name, string title, AccountPartDefinitionSettings settings)
        {
            if (String.IsNullOrWhiteSpace(name))
                throw new MissingArgumentValidationException("AccountPartDefinition.Name");
            if (String.IsNullOrWhiteSpace(title))
                throw new MissingArgumentValidationException("AccountPartDefinition.Title");
            // Validate settings
        }

        #endregion

        #region Private Classes

        public class CacheManager : Vanrise.Caching.BaseCacheManager
        {
            IAccountPartDefinitionDataManager _dataManager = BEDataManagerFactory.GetDataManager<IAccountPartDefinitionDataManager>();
            object _updateHandle;

            protected override bool ShouldSetCacheExpired(object parameter)
            {
                return _dataManager.AreAccountPartDefinitionsUpdated(ref _updateHandle);
            }
        }

        #endregion

        #region Private Methods

        Dictionary<Guid, AccountPartDefinition> GetCachedAccountPartDefinitions()
        {
            return CacheManagerFactory.GetCacheManager<CacheManager>().GetOrCreateObject("GetAccountPartDefinitions", () =>
            {
                IAccountPartDefinitionDataManager dataManager = BEDataManagerFactory.GetDataManager<IAccountPartDefinitionDataManager>();
                IEnumerable<AccountPartDefinition> accountPartDefinitions = dataManager.GetAccountPartDefinitions();
                return accountPartDefinitions.ToDictionary(kvp => kvp.AccountPartDefinitionId, kvp => kvp);
            });
        }

        #endregion

        #region Mappers

        private AccountPartDefinitionDetail AccountPartDefinitionDetailMapper(AccountPartDefinition accountPartDefinition)
        {
            return new AccountPartDefinitionDetail()
            {
                Entity = accountPartDefinition
            };
        }
        private AccountPartDefinitionInfo AccountPartDefinitionInfoMapper(AccountPartDefinition accountPartDefinition)
        {
            return new AccountPartDefinitionInfo
            {
                AccountPartDefinitionId = accountPartDefinition.AccountPartDefinitionId,
                Title = accountPartDefinition.Title
            };
        }

        #endregion
    }
}
