﻿using Retail.BusinessEntity.Data;
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
    public class AccountTypeManager
    {
        #region Public Methods

        public Vanrise.Entities.IDataRetrievalResult<AccountTypeDetail> GetFilteredAccountTypes(Vanrise.Entities.DataRetrievalInput<AccountTypeQuery> input)
        {
            Dictionary<int, AccountType> cachedAccountTypes = this.GetCachedAccountTypes();

            Func<AccountType, bool> filterExpression = (accountType) =>
                (input.Query.Name == null || accountType.Name.ToLower().Contains(input.Query.Name.ToLower()));

            return Vanrise.Common.DataRetrievalManager.Instance.ProcessResult(input, cachedAccountTypes.ToBigResult(input, filterExpression, AccountTypeDetailMapper));
        }

        public AccountType GetAccountType(int accountTypeId)
        {
            return this.GetCachedAccountTypes().GetRecord(accountTypeId);
        }

        public string GetAccountTypeName(int accountTypeId)
        {
            AccountType accountType = this.GetAccountType(accountTypeId);
            return (accountType != null) ? accountType.Title : null;
        }

        public IEnumerable<AccountTypeInfo> GetAccountTypesInfo(AccountTypeFilter filter)
        {
            Func<AccountType, bool> filterExpression = null;

            if (filter != null)
            {
                if (filter.ParentAccountId.HasValue)
                {
                    var accountManager = new AccountManager();
                    Account parentAccount = accountManager.GetAccount(filter.ParentAccountId.Value);
                    if (parentAccount == null)
                        throw new NullReferenceException("parentAccount");

                    filterExpression = (accountType) =>
                    (
                        accountType.Settings != null &&
                        accountType.Settings.SupportedParentAccountTypeIds != null &&
                        accountType.Settings.SupportedParentAccountTypeIds.Contains(parentAccount.TypeId)
                    );
                }
                else
                {
                    filterExpression = (accountType) =>
                        (accountType.Settings != null && accountType.Settings.CanBeRootAccount == true);
                }
            }

            return this.GetCachedAccountTypes().MapRecords(AccountTypeInfoMapper, filterExpression).OrderBy(x => x.Title);
        }

        public IEnumerable<int> GetSupportedParentAccountTypeIds(long parentAccountId)
        {
            var accountManager = new AccountManager();
            Account parentAccount = accountManager.GetAccount(parentAccountId);
            if (parentAccount == null)
                throw new NullReferenceException("parentAccount");
            AccountType parentAccountType = this.GetAccountType(parentAccount.TypeId);
            if (parentAccountType == null)
                throw new NullReferenceException("parentAccountType");
            if (parentAccountType.Settings == null)
                throw new NullReferenceException("parentAccountType.Settings");
            return parentAccountType.Settings.SupportedParentAccountTypeIds;
        }

        public IEnumerable<AccountPartDefinitionConfig> GetAccountTypePartDefinitionExtensionConfigs()
        {
            ExtensionConfigurationManager manager = new ExtensionConfigurationManager();
            return manager.GetExtensionConfigurations<AccountPartDefinitionConfig>(AccountPartDefinitionConfig.EXTENSION_TYPE);
        }

        public Vanrise.Entities.InsertOperationOutput<AccountTypeDetail> AddAccountType(AccountType accountType)
        {
            ValidateAccountTypeToAdd(accountType);

            var insertOperationOutput = new Vanrise.Entities.InsertOperationOutput<AccountTypeDetail>();

            insertOperationOutput.Result = Vanrise.Entities.InsertOperationResult.Failed;
            insertOperationOutput.InsertedObject = null;

            IAccountTypeDataManager dataManager = BEDataManagerFactory.GetDataManager<IAccountTypeDataManager>();
            int accountTypeId = -1;

            if (dataManager.Insert(accountType, out accountTypeId))
            {
                Vanrise.Caching.CacheManagerFactory.GetCacheManager<CacheManager>().SetCacheExpired();
                insertOperationOutput.Result = Vanrise.Entities.InsertOperationResult.Succeeded;
                accountType.AccountTypeId = accountTypeId;
                insertOperationOutput.InsertedObject = AccountTypeDetailMapper(accountType);
            }
            else
            {
                insertOperationOutput.Result = Vanrise.Entities.InsertOperationResult.SameExists;
            }

            return insertOperationOutput;
        }

        public Vanrise.Entities.UpdateOperationOutput<AccountTypeDetail> UpdateAccountType(AccountTypeToEdit accountType)
        {
            ValidateAccountTypeToEdit(accountType);

            var updateOperationOutput = new Vanrise.Entities.UpdateOperationOutput<AccountTypeDetail>();

            updateOperationOutput.Result = Vanrise.Entities.UpdateOperationResult.Failed;
            updateOperationOutput.UpdatedObject = null;

            IAccountTypeDataManager dataManager = BEDataManagerFactory.GetDataManager<IAccountTypeDataManager>();

            if (dataManager.Update(accountType))
            {
                Vanrise.Caching.CacheManagerFactory.GetCacheManager<CacheManager>().SetCacheExpired();
                updateOperationOutput.Result = Vanrise.Entities.UpdateOperationResult.Succeeded;
                updateOperationOutput.UpdatedObject = AccountTypeDetailMapper(this.GetAccountType(accountType.AccountTypeId));
            }
            else
            {
                updateOperationOutput.Result = Vanrise.Entities.UpdateOperationResult.SameExists;
            }

            return updateOperationOutput;
        }

        #endregion

        #region Validation Methods

        private void ValidateAccountTypeToAdd(AccountType accountType)
        {
            ValidateAccountType(accountType.Name, accountType.Title, accountType.Settings);
        }

        private void ValidateAccountTypeToEdit(AccountTypeToEdit accountType)
        {
            AccountType accountTypeEntity = this.GetAccountType(accountType.AccountTypeId);
            if (accountTypeEntity == null)
                throw new DataIntegrityValidationException(String.Format("AccountType '{0}' does not exist", accountType.AccountTypeId));
            ValidateAccountType(accountType.Name, accountType.Title, accountType.Settings);
        }

        private void ValidateAccountType(string name, string title, AccountTypeSettings settings)
        {
            if (String.IsNullOrWhiteSpace(name))
                throw new MissingArgumentValidationException("AccountType.Name");
            if (String.IsNullOrWhiteSpace(title))
                throw new MissingArgumentValidationException("AccountType.Title");
            // Validate settings
        }

        #endregion

        #region Private Classes

        private class CacheManager : Vanrise.Caching.BaseCacheManager
        {
            IAccountTypeDataManager _dataManager = BEDataManagerFactory.GetDataManager<IAccountTypeDataManager>();
            object _updateHandle;

            protected override bool ShouldSetCacheExpired(object parameter)
            {
                return _dataManager.AreAccountTypesUpdated(ref _updateHandle);
            }
        }

        #endregion

        #region Private Methods

        Dictionary<int, AccountType> GetCachedAccountTypes()
        {
            return CacheManagerFactory.GetCacheManager<CacheManager>().GetOrCreateObject("GetAccountTypes", () =>
            {
                IAccountTypeDataManager dataManager = BEDataManagerFactory.GetDataManager<IAccountTypeDataManager>();
                IEnumerable<AccountType> accountTypes = dataManager.GetAccountTypes();
                return accountTypes.ToDictionary(kvp => kvp.AccountTypeId, kvp => kvp);
            });
        }

        #endregion

        #region Mappers

        private AccountTypeDetail AccountTypeDetailMapper(AccountType accountType)
        {
            return new AccountTypeDetail()
            {
                Entity = accountType
            };
        }

        private AccountTypeInfo AccountTypeInfoMapper(AccountType accountType)
        {
            return new AccountTypeInfo
            {
                AccountTypeId = accountType.AccountTypeId,
                Title = accountType.Title
            };
        }

        #endregion
    }
}
