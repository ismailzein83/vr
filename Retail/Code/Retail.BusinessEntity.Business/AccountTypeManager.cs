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
using Vanrise.GenericData.Entities;

namespace Retail.BusinessEntity.Business
{
    public class AccountTypeManager : IBusinessEntityManager
    {
        #region Public Methods

        public Vanrise.Entities.IDataRetrievalResult<AccountTypeDetail> GetFilteredAccountTypes(Vanrise.Entities.DataRetrievalInput<AccountTypeQuery> input)
        {
            Dictionary<Guid, AccountType> cachedAccountTypes = this.GetCachedAccountTypesWithHidden();

            Func<AccountType, bool> filterExpression = (accountType) =>
                (input.Query.Name == null || accountType.Name.ToLower().Contains(input.Query.Name.ToLower()));

            return Vanrise.Common.DataRetrievalManager.Instance.ProcessResult(input, cachedAccountTypes.ToBigResult(input, filterExpression, AccountTypeDetailMapper));
        }

        public AccountType GetAccountType(Guid accountTypeId)
        {
            return this.GetCachedAccountTypesWithHidden().GetRecord(accountTypeId);
        }

        public string GetAccountTypeName(Guid accountTypeId)
        {
            AccountType accountType = this.GetAccountType(accountTypeId);
            return (accountType != null) ? accountType.Title : null;
        }

        public IEnumerable<AccountTypeInfo> GetAccountTypesInfo(AccountTypeFilter filter)
        {
            Dictionary<Guid, AccountType> cachedAccountTypes = null;

            Func<AccountType, bool> filterExpression = null;
            if (filter != null)
            {
                if (filter.IncludeHiddenAccountTypes)
                    cachedAccountTypes = this.GetCachedAccountTypesWithHidden();

                filterExpression = (accountType) =>
                {
                    if (filter.AccountBEDefinitionId.HasValue && filter.AccountBEDefinitionId.Value != accountType.AccountBEDefinitionId)
                        return false;

                    if (filter.AccountBEDefinitionId.HasValue && filter.ParentAccountId.HasValue)
                    {
                        var accountBEManager = new AccountBEManager();
                        Account parentAccount = accountBEManager.GetAccount(filter.AccountBEDefinitionId.Value, filter.ParentAccountId.Value);
                        if (parentAccount == null)
                            throw new NullReferenceException("parentAccount");

                        if (accountType.Settings == null || accountType.Settings.SupportedParentAccountTypeIds == null || !accountType.Settings.SupportedParentAccountTypeIds.Contains(parentAccount.TypeId))
                            return false;
                    }

                    if (filter.RootAccountTypeOnly && (accountType.Settings == null || !accountType.Settings.CanBeRootAccount))
                        return false;

                    return true;
                };
            }

            if (cachedAccountTypes == null)
                cachedAccountTypes = this.GetCachedAccountTypes();

            return cachedAccountTypes.MapRecords(AccountTypeInfoMapper, filterExpression).OrderBy(x => x.Title);
        }

        public IEnumerable<Guid> GetSupportedParentAccountTypeIds(Guid accountBEDefinitionId, long parentAccountId)
        {
            var accountBEManager = new AccountBEManager();
            Account parentAccount = accountBEManager.GetAccount(accountBEDefinitionId, parentAccountId);
            if (parentAccount == null)
                throw new NullReferenceException("parentAccount");
            AccountType parentAccountType = this.GetAccountType(parentAccount.TypeId);
            if (parentAccountType == null)
                throw new NullReferenceException("parentAccountType");
            if (parentAccountType.Settings == null)
                throw new NullReferenceException("parentAccountType.Settings");
            return parentAccountType.Settings.SupportedParentAccountTypeIds;
        }
        public IEnumerable<Guid> GetSupportedParentAccountTypeIds(Guid accountTypeId)
        {
            AccountType accountType = this.GetAccountType(accountTypeId);
            if (accountType == null)
                throw new NullReferenceException(string.Format("accountType of Id: {0}", accountTypeId));

            if (accountType.Settings == null)
                throw new NullReferenceException(string.Format("accountType.Settings of accountTypeId: {0}", accountTypeId));

            return accountType.Settings.SupportedParentAccountTypeIds;
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
            accountType.AccountTypeId = Guid.NewGuid();
            if (dataManager.Insert(accountType))
            {
                Vanrise.Caching.CacheManagerFactory.GetCacheManager<CacheManager>().SetCacheExpired();
                insertOperationOutput.Result = Vanrise.Entities.InsertOperationResult.Succeeded;
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

        private struct GetAccountGenericFieldsCacheName
        {
            public Guid? AccountBEDefinitionId { get; set; }
        }
        public Dictionary<string, AccountGenericField> GetAccountGenericFields(Guid? accountBEDefinitionId)
        {
            var cacheName = new GetAccountGenericFieldsCacheName { AccountBEDefinitionId = accountBEDefinitionId };
            return Vanrise.Caching.CacheManagerFactory.GetCacheManager<CacheManager>().GetOrCreateObject(cacheName,
                () =>
                {
                    List<AccountGenericField> fields = new List<AccountGenericField>();
                    FillAccountCommonGenericFields(accountBEDefinitionId, fields);

                    if (accountBEDefinitionId.HasValue)
                    {
                        IEnumerable<AccountType> accountTypes = this.GetCachedAccountTypes().Values.Where(itm => itm.AccountBEDefinitionId == accountBEDefinitionId.Value);

                        List<Guid> accountPartDefinitionIds = new List<Guid>();
                        foreach (var accountType in accountTypes)
                        {
                            if (accountType != null && accountType.Settings != null && accountType.Settings.PartDefinitionSettings != null)
                                accountPartDefinitionIds.AddRange(accountType.Settings.PartDefinitionSettings.Select(itm => itm.PartDefinitionId));
                        }

                        var accountPartDefinitions = new AccountPartDefinitionManager().GetAccountPartDefinitions();
                        if (accountPartDefinitions != null)
                        {
                            foreach (var partDefinition in accountPartDefinitions)
                            {
                                if (!accountPartDefinitionIds.Contains(partDefinition.AccountPartDefinitionId))
                                    continue;

                                var partFieldDefinitions = partDefinition.Settings.GetFieldDefinitions();
                                if (partFieldDefinitions != null)
                                {
                                    fields.AddRange(partFieldDefinitions.Select(partFieldDefinition => new AccountPartGenericField(accountBEDefinitionId.Value, partDefinition, partFieldDefinition)));
                                }
                            }
                        }
                    }
                    return fields.ToDictionary(fld => fld.Name, fld => fld);
                });
        }
        public Dictionary<string, object> GetAccountGenericFieldValues(Guid accountBEDefinitionId, long accountId, List<string> accountGenericFieldNames)
        {
            if (accountGenericFieldNames == null)
                return null;

            Dictionary<string, object> results = new Dictionary<string, object>();
            Dictionary<string, AccountGenericField> accountGenericFields = this.GetAccountGenericFields(accountBEDefinitionId);
            Account account = new AccountBEManager().GetAccount(accountBEDefinitionId, accountId);
            AccountGenericFieldContext accountGenericFieldContext = new AccountGenericFieldContext(account);

            AccountGenericField accountGenericField;
            foreach (var name in accountGenericFieldNames)
            {
                if (accountGenericFields.TryGetValue(name, out accountGenericField))
                {
                    if(!results.ContainsKey(name))
                        results.Add(name, accountGenericField.GetValue(accountGenericFieldContext));
                }
            }

            return results;
        }

        public AccountGenericField GetAccountGenericField(Guid accountBEDefinitionId, string fieldName)
        {
            return GetAccountGenericFields(accountBEDefinitionId).GetRecord(fieldName);
        }

        public IEnumerable<GenericFieldDefinitionInfo> GetGenericFieldDefinitionsInfo(Guid? accountBEDefinitionId)
        {
            Dictionary<string, AccountGenericField> accountGenericFields = GetAccountGenericFields(accountBEDefinitionId);
            if (accountGenericFields == null || accountGenericFields.Values.Count() == 0)
                return null;
            return accountGenericFields.Values.Select(itm => new GenericFieldDefinitionInfo() { Name = itm.Name, Title = itm.Title, FieldType = itm.FieldType });
        }

        public bool CanHaveSubAccounts(Account account)
        {
            if (account != null)
            {
                foreach (var itm in this.GetCachedAccountTypes())
                {
                    IEnumerable<Guid> supportedParentAccountTypeIds = this.GetSupportedParentAccountTypeIds(itm.Key);
                    if (supportedParentAccountTypeIds != null && supportedParentAccountTypeIds.Contains(account.TypeId))
                        return true;
                }
            }
            return false;
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

        #region Private Methods

        Dictionary<Guid, AccountType> GetCachedAccountTypesWithHidden()
        {
            return CacheManagerFactory.GetCacheManager<CacheManager>().GetOrCreateObject("GetCachedAccountTypesWithHidden", () =>
            {
                IAccountTypeDataManager dataManager = BEDataManagerFactory.GetDataManager<IAccountTypeDataManager>();
                IEnumerable<AccountType> accountTypes = dataManager.GetAccountTypes();
                return accountTypes.ToDictionary(kvp => kvp.AccountTypeId, kvp => kvp);
            });
        }

        Dictionary<Guid, AccountType> GetCachedAccountTypes()
        {
            return CacheManagerFactory.GetCacheManager<CacheManager>().GetOrCreateObject("GetAccountTypes", () =>
            {
                List<AccountType> includedAccountTypes = new List<AccountType>();
                VRRetailBEVisibilityManager retailBEVisibilityManager = new VRRetailBEVisibilityManager();
                Dictionary<Guid, VRRetailBEVisibilityAccountDefinitionAccountType> visibleAccountTypesById;

                IEnumerable<AccountType> allAccountTypes = this.GetCachedAccountTypesWithHidden().Values;

                if (retailBEVisibilityManager.ShouldApplyAccountTypesVisibility(out visibleAccountTypesById))
                {
                    foreach (var itm in allAccountTypes)
                    {
                        if (visibleAccountTypesById.ContainsKey(itm.AccountTypeId))
                            includedAccountTypes.Add(itm);
                    }
                }
                else
                {
                    includedAccountTypes = allAccountTypes.ToList();
                }

                return includedAccountTypes.ToDictionary(kvp => kvp.AccountTypeId, kvp => kvp);
            });
        }

        void FillAccountCommonGenericFields(Guid? accountBEDefinitionId, List<AccountGenericField> fields)
        {
            fields.Add(new AccountNameGenericField());
            fields.Add(new AccountSourceIdGenericField());

            if (accountBEDefinitionId.HasValue)
            {
                fields.Add(new AccountTypeGenericField(accountBEDefinitionId.Value));
                fields.Add(new AccountStatusGenericField(accountBEDefinitionId.Value));
            }
        }

        #endregion

        #region Private Classes

        private class CacheManager : Vanrise.Caching.BaseCacheManager
        {
            IAccountTypeDataManager _dataManager = BEDataManagerFactory.GetDataManager<IAccountTypeDataManager>();
            object _updateHandle;

            DateTime? _accountPartDefinitionCacheLastCheck;

            protected override bool ShouldSetCacheExpired(object parameter)
            {
                return _dataManager.AreAccountTypesUpdated(ref _updateHandle)
                            |
                        Vanrise.Caching.CacheManagerFactory.GetCacheManager<AccountPartDefinitionManager.CacheManager>().IsCacheExpired(ref _accountPartDefinitionCacheLastCheck);
            }
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

        #region IBusinessEntityManager

        public List<dynamic> GetAllEntities(IBusinessEntityGetAllContext context)
        {
            return GetCachedAccountTypes().Select(itm => itm as dynamic).ToList();
        }

        public dynamic GetEntity(IBusinessEntityGetByIdContext context)
        {
            return GetAccountType(context.EntityId);
        }

        public string GetEntityDescription(IBusinessEntityDescriptionContext context)
        {
            return GetAccountTypeName(Guid.Parse(context.EntityId.ToString()));
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
