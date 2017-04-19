using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TOne.WhS.AccountBalance.Data;
using TOne.WhS.AccountBalance.Entities;
using TOne.WhS.BusinessEntity.Business;
using TOne.WhS.BusinessEntity.Entities;
using Vanrise.AccountBalance.Business;
using Vanrise.AccountBalance.Entities;
using Vanrise.Common;
using Vanrise.Common.Business;
using Vanrise.Entities;
using Vanrise.GenericData.Entities;

namespace TOne.WhS.AccountBalance.Business
{
    public class FinancialAccountManager : IBusinessEntityManager
    {
        #region Fields

        private CarrierProfileManager _carrierProfileManager = new CarrierProfileManager();

        private CarrierAccountManager _carrierAccountManager = new CarrierAccountManager();

        #endregion

        #region Public Methods

        public IDataRetrievalResult<FinancialAccountDetail> GetFilteredFinancialAccounts(Vanrise.Entities.DataRetrievalInput<FinancialAccountQuery> input)
        {
            var allFinancialAccounts = GetCachedFinancialAccounts();

            Func<FinancialAccount, bool> filterExpression = (prod) =>
                {
                    if (input.Query.CarrierAccountId.HasValue)
                    {
                        if (!prod.CarrierAccountId.HasValue)
                            return false;
                        if (input.Query.CarrierAccountId.Value != prod.CarrierAccountId.Value)
                            return false;
                    }
                    if (input.Query.CarrierProfileId.HasValue)
                    {
                        if (!prod.CarrierProfileId.HasValue)
                            return false;
                        if (input.Query.CarrierProfileId.Value != prod.CarrierProfileId.Value)
                            return false;
                    }
                    return true;
                };
            return Vanrise.Common.DataRetrievalManager.Instance.ProcessResult(input, allFinancialAccounts.ToBigResult(input, filterExpression, FinancialAccountDetailMapper));
        }
        public Vanrise.Entities.InsertOperationOutput<FinancialAccountDetail> AddFinancialAccount(FinancialAccount financialAccount)
        {
            Vanrise.Entities.InsertOperationOutput<FinancialAccountDetail> insertOperationOutput = new Vanrise.Entities.InsertOperationOutput<FinancialAccountDetail>();

            insertOperationOutput.Result = Vanrise.Entities.InsertOperationResult.Failed;
            insertOperationOutput.InsertedObject = null;
            string message = null;
            if (CheckIsAllowToAddFinancialAccount(financialAccount, false, out message))
            {
                int financialAccountId = -1;

                IFinancialAccountDataManager dataManager = AccountBalanceManagerFactory.GetDataManager<IFinancialAccountDataManager>();
                bool insertActionSucc = dataManager.Insert(financialAccount, out financialAccountId);
                if (insertActionSucc)
                {
                    Vanrise.Caching.CacheManagerFactory.GetCacheManager<CacheManager>().SetCacheExpired();
                    insertOperationOutput.Result = Vanrise.Entities.InsertOperationResult.Succeeded;
                    financialAccount.FinancialAccountId = financialAccountId;
                    insertOperationOutput.InsertedObject = FinancialAccountDetailMapper(financialAccount);
                }
                else
                {
                    insertOperationOutput.Result = Vanrise.Entities.InsertOperationResult.SameExists;
                }
            }
            insertOperationOutput.Message = message;
            return insertOperationOutput;
        }
        public Vanrise.Entities.UpdateOperationOutput<FinancialAccountDetail> UpdateFinancialAccount(FinancialAccount financialAccount)
        {
            IFinancialAccountDataManager dataManager = AccountBalanceManagerFactory.GetDataManager<IFinancialAccountDataManager>();

            Vanrise.Entities.UpdateOperationOutput<FinancialAccountDetail> updateOperationOutput = new Vanrise.Entities.UpdateOperationOutput<FinancialAccountDetail>();

            updateOperationOutput.Result = Vanrise.Entities.UpdateOperationResult.Failed;
            updateOperationOutput.UpdatedObject = null;
            string message = null;

            if (CheckIsAllowToAddFinancialAccount(financialAccount, true, out message))
            {
                bool updateActionSucc = dataManager.Update(financialAccount);
                if (updateActionSucc)
                {
                    Vanrise.Caching.CacheManagerFactory.GetCacheManager<CacheManager>().SetCacheExpired();
                    updateOperationOutput.Result = Vanrise.Entities.UpdateOperationResult.Succeeded;
                    updateOperationOutput.UpdatedObject = FinancialAccountDetailMapper(financialAccount);
                }
                else
                {
                    updateOperationOutput.Result = Vanrise.Entities.UpdateOperationResult.SameExists;
                }
            }
            updateOperationOutput.Message = message;
            return updateOperationOutput;
        }
        public FinancialAccount GetFinancialAccount(int financialAccountId)
        {
            var allFinancialAccounts = GetCachedFinancialAccounts();
            return allFinancialAccounts.GetRecord(financialAccountId);
        }
        public FinancialAccountEditorRuntime GetFinancialAccountEditorRuntime(int financialAccountId)
        {
            var allFinancialAccounts = GetCachedFinancialAccounts();
            var financialAccount = allFinancialAccounts.GetRecord(financialAccountId);
            bool hasFinancialTransactions = new AccountBalanceManager().CheckFinancialAccountTranasactions(financialAccount.Settings.AccountTypeId, financialAccount.FinancialAccountId);
            return new FinancialAccountEditorRuntime
            {
                FinancialAccount = financialAccount,
                HasFinancialTransactions = hasFinancialTransactions
            };
        }
        public CarrierFinancialAccountData GetCustCarrierFinancialByFinAccId(int financialAccountId)
        {
            CarrierFinancialAccountData carrierFinancialAccountData = GetCachedCustCarrierFinancialsByFinAccId().GetRecord(financialAccountId);
            carrierFinancialAccountData.ThrowIfNull("carrierFinancialAccountData", financialAccountId);
            return carrierFinancialAccountData;
        }
        public CarrierFinancialAccountData GetSuppCarrierFinancialByFinAccId(int financialAccountId)
        {
            CarrierFinancialAccountData carrierFinancialAccountData = GetCachedSuppCarrierFinancialsByFinAccId().GetRecord(financialAccountId);
            carrierFinancialAccountData.ThrowIfNull("carrierFinancialAccountData", financialAccountId);
            return carrierFinancialAccountData;
        }
        public bool TryGetCustAccFinancialAccountData(int customerAccountId, DateTime effectiveOn, out CarrierFinancialAccountData financialAccountData)
        {
            IOrderedEnumerable<CarrierFinancialAccountData> carrierFinancialAccounts = GetCachedCustCarrierFinancialsByCarrAccId().GetRecord(customerAccountId);
            if (carrierFinancialAccounts != null)
            {
                foreach (var acc in carrierFinancialAccounts)
                {
                    if (acc.BED <= effectiveOn && acc.EED.VRGreaterThan(effectiveOn))
                    {
                        financialAccountData = acc;
                        return true;
                    }
                }
            }
            financialAccountData = null;
            return false;
        }
        public bool TryGetSuppAccFinancialAccountData(int supplierAccountId, DateTime effectiveOn, out CarrierFinancialAccountData financialAccountData)
        {
            IOrderedEnumerable<CarrierFinancialAccountData> carrierFinancialAccounts = GetCachedSuppCarrierFinancialsByCarrAccId().GetRecord(supplierAccountId);
            if (carrierFinancialAccounts != null)
            {
                foreach (var acc in carrierFinancialAccounts)
                {
                    if (acc.BED <= effectiveOn && acc.EED.VRGreaterThan(effectiveOn))
                    {
                        financialAccountData = acc;
                        return true;
                    }
                }
            }
            financialAccountData = null;
            return false;
        }
        public IEnumerable<FinancialAccount> GetFinancialAccountsByCarrierAccountId(int carrierAccountId)
        {
            var financialAccounts = GetCachedFinancialAccounts();
            return financialAccounts.Values.FindAllRecords(x => x.CarrierAccountId.HasValue && x.CarrierAccountId.Value == carrierAccountId);
        }

        public IEnumerable<FinancialAccount> GetFinancialAccountsByCarrierAccountTypeId(Guid accountTypeId)
        {
            var financialAccountsByAccTypeId = GetCachedFinancialAccountsByAccBalTypeId();
            return financialAccountsByAccTypeId.GetRecord(accountTypeId);
        }

        public IEnumerable<FinancialAccount> GetCarrierProfileFinancialAccounts(int carrierProfileId)
        {
            var financialAccounts = GetCachedFinancialAccounts();
            return financialAccounts.Values.FindAllRecords(x => x.CarrierProfileId.HasValue && x.CarrierProfileId.Value == carrierProfileId);
        }
        public string GetAccountCurrencyName(int? carrierProfileId, int? carrierAccountId)
        {
            int currencyId = -1;
            if (carrierProfileId.HasValue)
                currencyId = new CarrierProfileManager().GetCarrierProfileCurrencyId(carrierProfileId.Value);
            else
                currencyId = new CarrierAccountManager().GetCarrierAccountCurrencyId(carrierAccountId.Value);
            CurrencyManager currencyManager = new CurrencyManager();
            return currencyManager.GetCurrencyName(currencyId);
        }

        #region Financial Accounts Validation
        public bool CheckFinancialCarrierAccountValidation(Guid accountTypeId, AccountBalanceSettings accountBalanceSetting, CarrierAccount carrierAccount, IEnumerable<FinancialAccountData> profileFinancialAccounts, IEnumerable<FinancialAccountData> carrierFinancialAccounts, bool isEditMode)
        {
            if (carrierAccount.IsDeleted || carrierAccount.CarrierAccountSettings.ActivationStatus == ActivationStatus.Inactive)
                return false;

            Func<FinancialAccountData, bool> filterExpression = GetFinancialAccountFilterExpression(accountTypeId, carrierAccount.AccountType, accountBalanceSetting, isEditMode);

            switch (carrierAccount.AccountType)
            {
                case BusinessEntity.Entities.CarrierAccountType.Customer:
                    if (!CheckApplicableAccountTypes(accountBalanceSetting, true, false))
                        return false;
                    break;
                case BusinessEntity.Entities.CarrierAccountType.Supplier:
                    if (!CheckApplicableAccountTypes(accountBalanceSetting, false, true))
                        return false;
                    break;
                case BusinessEntity.Entities.CarrierAccountType.Exchange:
                    if (CheckApplicableAccountTypes(accountBalanceSetting, false, false))
                        return false;
                    break;
            }
            if (carrierFinancialAccounts.Any(x => !isEditMode && x.FinancialAccount.Settings.AccountTypeId == accountTypeId && !x.FinancialAccount.EED.HasValue))
                return false;
            if (profileFinancialAccounts.Any(x => !filterExpression(x)))
                return false;
            if (carrierFinancialAccounts.Any(x => !filterExpression(x)))
                return false;
            return true;
        }
        public bool CheckFinancialCarrierProfileValidation(Guid accountTypeId, AccountBalanceSettings accountBalanceSetting, IEnumerable<CarrierAccount> carrierAccounts, IEnumerable<FinancialAccountData> profileFinancialAccounts, Dictionary<int, IEnumerable<FinancialAccountData>> financialAccountsByAccount, bool isEditMode)
        {
            bool hasCustomers = false;
            bool hasSuppliers = false;
            bool areCustomersActive = false;
            bool areSuppliersActive = false;

            foreach (var account in carrierAccounts)
            {

                if (account.AccountType == CarrierAccountType.Customer)
                {
                    hasCustomers = true;
                    if (!account.IsDeleted && account.CarrierAccountSettings.ActivationStatus != ActivationStatus.Inactive)
                        areCustomersActive = true;
                }
                else if (account.AccountType == CarrierAccountType.Supplier)
                {
                    hasSuppliers = true;
                    if (!account.IsDeleted && account.CarrierAccountSettings.ActivationStatus != ActivationStatus.Inactive)
                        areSuppliersActive = true;
                }
                else if (account.AccountType == CarrierAccountType.Exchange)
                {
                    hasSuppliers = true;
                    hasCustomers = true;
                    if (!account.IsDeleted && account.CarrierAccountSettings.ActivationStatus != ActivationStatus.Inactive)
                    {
                        areCustomersActive = true;
                        areSuppliersActive = true;
                    }

                }
            }
            Func<FinancialAccountData, bool> filterExpression = null;
            if (CheckApplicableAccountTypes(accountBalanceSetting, true, false))
            {
                if (!areCustomersActive)
                    return false;

                if (!hasCustomers)
                    return false;

                filterExpression = GetFinancialAccountFilterExpression(accountTypeId, CarrierAccountType.Customer, accountBalanceSetting, isEditMode);

            }
            else if (CheckApplicableAccountTypes(accountBalanceSetting, false, true))
            {
                if (!areSuppliersActive)
                    return false;
                if (!hasSuppliers)
                    return false;
                filterExpression = GetFinancialAccountFilterExpression(accountTypeId, CarrierAccountType.Supplier, accountBalanceSetting, isEditMode);
            }
            else if (!CheckApplicableAccountTypes(accountBalanceSetting, false, false))
            {
                if (!areCustomersActive || !areSuppliersActive)
                    return false;
                if (!hasCustomers || !hasSuppliers)
                {
                    return false;
                }
                filterExpression = GetFinancialAccountFilterExpression(accountTypeId, CarrierAccountType.Exchange, accountBalanceSetting, isEditMode);
            }


            if (profileFinancialAccounts.Any(x => !filterExpression(x)))
                return false;

            if (financialAccountsByAccount.Values.Any(x => x.Any(y => !filterExpression(y))))
                return false;

            return true;
        }
        public bool CheckCarrierAllowAddFinancialAccounts(int? carrierProfileId, int? carrierAccountId)
        {
            FinancialValidationData financialValidationData = LoadFinancialValidationData(carrierProfileId, carrierAccountId, 0);
            Func<AccountType, bool> filterExpression = (financialAccountType) =>
            {
                var accountBalanceSettings = financialAccountType.Settings.ExtendedSettings as AccountBalanceSettings;
                if (!CheckAccountBalanceSettingsActivationForCarrier(accountBalanceSettings, carrierAccountId, carrierProfileId))
                    return false;
                if (carrierProfileId.HasValue)
                {
                    if (!CheckFinancialCarrierProfileValidation(financialAccountType.VRComponentTypeId, accountBalanceSettings, financialValidationData.FinancialCarrierProfile.ProfileCarrierAccounts, financialValidationData.ProfileFinancialAccounts, financialValidationData.FinancialCarrierProfile.FinancialAccountsByAccount, false))
                        return false;
                }
                else
                {

                    if (!CheckFinancialCarrierAccountValidation(financialAccountType.VRComponentTypeId, accountBalanceSettings, financialValidationData.FinancialCarrierAccount.CarrierAccount, financialValidationData.ProfileFinancialAccounts, financialValidationData.FinancialCarrierAccount.FinancialAccounts, false))
                        return false;
                }
                return true;
            };
            var applicableFinancialAccountTypes = financialValidationData.FinancialAccountTypes.FindAllRecords(filterExpression);
            return applicableFinancialAccountTypes.Count() > 0;
        }

        #region LoadFinancialValidationData
        public FinancialValidationData LoadFinancialValidationData(int? carrierProfileId, int? carrierAccountId, int financialAccountId)
        {

            FinancialValidationData financialValidationData = new FinancialValidationData();

            FinancialAccountManager financialAccountManager = new FinancialAccountManager();
            financialValidationData.FinancialAccountTypes = new FinancialAccountDefinitionManager().GetFinancialAccountDefinitions();


            CarrierAccountManager carrierAccountManager = new CarrierAccountManager();
            int carrierProfileID = -1;
            if (carrierAccountId.HasValue)
            {
                var carrierAccount = carrierAccountManager.GetCarrierAccount(carrierAccountId.Value);
                carrierProfileID = carrierAccount.CarrierProfileId;

                financialValidationData.FinancialCarrierAccount = new FinancialCarrierAccount
                {
                    CarrierAccount = carrierAccount
                };
                var carrierFinancialAccounts = GetFinancialAccountsByCarrierAccountId(carrierAccountId.Value);
                financialValidationData.FinancialCarrierAccount.FinancialAccounts = LoadFinancialAccountsForCarrierAccount(carrierAccountId.Value, financialValidationData.FinancialAccountTypes, financialAccountId);
            }
            else if (carrierProfileId.HasValue)
            {
                carrierProfileID = carrierProfileId.Value;

                var profileCarrierAccounts = carrierAccountManager.GetCarriersByProfileId(carrierProfileId.Value, true, true);
                financialValidationData.FinancialCarrierProfile = new FinancialCarrierProfile
                {
                    ProfileCarrierAccounts = profileCarrierAccounts,
                    FinancialAccountsByAccount = new Dictionary<int, IEnumerable<FinancialAccountData>>(),
                };
                foreach (var carrierAccount in profileCarrierAccounts)
                {
                    var financialAccounts = LoadFinancialAccountsForCarrierAccount(carrierAccount.CarrierAccountId, financialValidationData.FinancialAccountTypes, financialAccountId);
                    if (financialAccounts != null && financialAccounts.Count() > 0)
                    {
                        financialValidationData.FinancialCarrierProfile.FinancialAccountsByAccount.Add(carrierAccount.CarrierAccountId, financialAccounts);
                    }
                }
            }
            financialValidationData.ProfileFinancialAccounts = LoadProfileFinancialAccounts(carrierProfileID, financialValidationData.FinancialAccountTypes, financialAccountId);
            return financialValidationData;
        }
        #endregion

        #endregion

        public IEnumerable<FinancialAccountInfo> GetFinancialAccountsInfo(FinancialAccountInfoFilter filter)
        {
            Dictionary<int, FinancialAccount> allFinancialAccounts = GetCachedFinancialAccounts();

            Func<FinancialAccount, bool> filterFunc = null;

            if (filter != null)
            {
                filterFunc = (financialAccount) =>
                {
                    if (financialAccount.Settings.AccountTypeId != filter.AccountBalanceTypeId)
                        return false;

                    return true;
                };
            }

            return allFinancialAccounts.MapRecords(FinancialAccountInfoMapper, filterFunc);
        }
        #endregion

        #region Private Classes

        private class CacheManager : Vanrise.Caching.BaseCacheManager
        {
            IFinancialAccountDataManager _dataManager = AccountBalanceManagerFactory.GetDataManager<IFinancialAccountDataManager>();
            object _updateHandle;

            protected override bool ShouldSetCacheExpired(object parameter)
            {
                return _dataManager.AreFinancialAccountsUpdated(ref _updateHandle);
            }
        }

        #endregion

        #region Private Methods

        Dictionary<int, FinancialAccount> GetCachedFinancialAccounts()
        {
            return Vanrise.Caching.CacheManagerFactory.GetCacheManager<CacheManager>().GetOrCreateObject("GetCachedFinancialAccounts",
               () =>
               {
                   IFinancialAccountDataManager dataManager = AccountBalanceManagerFactory.GetDataManager<IFinancialAccountDataManager>();
                   IEnumerable<FinancialAccount> financialAccounts = dataManager.GetFinancialAccounts();
                   return financialAccounts.ToDictionary(fa => fa.FinancialAccountId, fa => fa);
               });
        }

        Dictionary<Guid, List<FinancialAccount>> GetCachedFinancialAccountsByAccBalTypeId()
        {
            return Vanrise.Caching.CacheManagerFactory.GetCacheManager<CacheManager>().GetOrCreateObject("GetCachedFinancialAccountsByAccBalTypeId",
                 () =>
                 {
                     var financialAccountsByType = new Dictionary<Guid, List<FinancialAccount>>();

                     Dictionary<int, FinancialAccount> cachedFinancialAccounts = GetCachedFinancialAccounts();

                     if (cachedFinancialAccounts != null)
                     {
                         foreach (FinancialAccount financialAccount in cachedFinancialAccounts.Values)
                         {
                             List<FinancialAccount> financialAccounts;

                             if (!financialAccountsByType.TryGetValue(financialAccount.Settings.AccountTypeId, out financialAccounts))
                             {
                                 financialAccounts = new List<FinancialAccount>();
                                 financialAccountsByType.Add(financialAccount.Settings.AccountTypeId, financialAccounts);
                             }

                             financialAccounts.Add(financialAccount);
                         }
                     }

                     return financialAccountsByType;
                 });
        }

        Dictionary<int, CarrierFinancialAccountData> GetCachedCustCarrierFinancialsByFinAccId()
        {
            return Vanrise.Caching.CacheManagerFactory.GetCacheManager<CacheManager>().GetOrCreateObject("GetCachedCustCarrierFinancialsByFinAccId",
             () =>
             {
                 var carrierDataByFinancialAccountId = new Dictionary<int, CarrierFinancialAccountData>();

                 Dictionary<int, FinancialAccount> cachedFinancialAccounts = GetCachedFinancialAccounts();
                 if (cachedFinancialAccounts != null)
                 {
                     foreach (var financialAccount in cachedFinancialAccounts.Values)
                     {
                         if (financialAccount.Settings == null)
                             throw new NullReferenceException(string.Format("financialAccount.Settings for financial Account Id: {0}", financialAccount.FinancialAccountId));

                         if (financialAccount.Settings.ExtendedSettings == null)
                             throw new NullReferenceException(string.Format("financialAccount.Settings.ExtendedSettings for financial Account Id: {0}", financialAccount.FinancialAccountId));

                         FinancialAccountIsCustomerAccountContext context = new FinancialAccountIsCustomerAccountContext() { AccountTypeId = financialAccount.Settings.AccountTypeId };

                         if (financialAccount.Settings.ExtendedSettings.IsCustomerAccount(context))// IsCustomerAccount will set CreditLimit on context
                         {
                             int currencyId;
                             if (financialAccount.CarrierAccountId.HasValue)
                                 currencyId = _carrierAccountManager.GetCarrierAccountCurrencyId(financialAccount.CarrierAccountId.Value);
                             else  // so financialAccount.CarrierProfileId.HasValue = true
                                 currencyId = _carrierProfileManager.GetCarrierProfileCurrencyId(financialAccount.CarrierProfileId.Value);

                             carrierDataByFinancialAccountId.GetOrCreateItem(financialAccount.FinancialAccountId, () =>
                             {
                                 return CreateCarrierFinancialAccountData(financialAccount, context.UsageTransactionTypeId, context.CreditLimit, currencyId);
                             });
                         }
                     }
                 }
                 return carrierDataByFinancialAccountId;
             });
        }

        Dictionary<int, CarrierFinancialAccountData> GetCachedSuppCarrierFinancialsByFinAccId()
        {
            return Vanrise.Caching.CacheManagerFactory.GetCacheManager<CacheManager>().GetOrCreateObject("GetCachedSuppCarrierFinancialsByFinAccId",
              () =>
              {
                  var carrierDataByFinancialAccountId = new Dictionary<int, CarrierFinancialAccountData>();
                  Dictionary<int, FinancialAccount> cachedFinancialAccounts = GetCachedFinancialAccounts();
                  if (cachedFinancialAccounts != null)
                  {
                      foreach (var financialAccount in cachedFinancialAccounts.Values)
                      {
                          if (financialAccount.Settings == null)
                              throw new NullReferenceException(string.Format("financialAccount.Settings for financial Account Id: {0}", financialAccount.FinancialAccountId));

                          if (financialAccount.Settings.ExtendedSettings == null)
                              throw new NullReferenceException(string.Format("financialAccount.Settings.ExtendedSettings for financial Account Id: {0}", financialAccount.FinancialAccountId));

                          FinancialAccountIsSupplierAccountContext context = new FinancialAccountIsSupplierAccountContext() { AccountTypeId = financialAccount.Settings.AccountTypeId };

                          if (financialAccount.Settings.ExtendedSettings.IsSupplierAccount(context))// IsSupplierAccount will set CreditLimit on context
                          {
                              int currencyId;
                              if (financialAccount.CarrierAccountId.HasValue)
                                  currencyId = _carrierAccountManager.GetCarrierAccountCurrencyId(financialAccount.CarrierAccountId.Value);
                              else  // so financialAccount.CarrierProfileId.HasValue = true
                                  currencyId = _carrierProfileManager.GetCarrierProfileCurrencyId(financialAccount.CarrierProfileId.Value);

                              carrierDataByFinancialAccountId.GetOrCreateItem(financialAccount.FinancialAccountId, () =>
                              {
                                  return CreateCarrierFinancialAccountData(financialAccount, context.UsageTransactionTypeId, context.CreditLimit, currencyId);
                              });
                          }
                      }
                  }
                  return carrierDataByFinancialAccountId;
              });
        }

        /// <summary>
        /// should return a list of applicable Financial Account Data for each customer account ordered by BED desc
        /// </summary>
        /// <returns></returns>
        Dictionary<int, IOrderedEnumerable<CarrierFinancialAccountData>> GetCachedCustCarrierFinancialsByCarrAccId()
        {
            return Vanrise.Caching.CacheManagerFactory.GetCacheManager<CacheManager>().GetOrCreateObject("GetCachedCustCarrierFinancialsByCarrAccId",
            () =>
            {
                int currencyId;
                List<CarrierFinancialAccountData> customerFinancialAccountsData;
                Dictionary<int, List<CarrierFinancialAccountData>> customerFinancialAccountsDataDict = new Dictionary<int, List<CarrierFinancialAccountData>>();
                Dictionary<int, FinancialAccount> financialAccounts = GetCachedFinancialAccounts();

                foreach (var financialAccount in financialAccounts.Values)
                {
                    if (financialAccount.Settings == null)
                        throw new NullReferenceException(string.Format("financialAccount.Settings for financial Account Id: {0}", financialAccount.FinancialAccountId));

                    if (financialAccount.Settings.ExtendedSettings == null)
                        throw new NullReferenceException(string.Format("financialAccount.Settings.ExtendedSettings for financial Account Id: {0}", financialAccount.FinancialAccountId));

                    FinancialAccountIsCustomerAccountContext context = new FinancialAccountIsCustomerAccountContext() { AccountTypeId = financialAccount.Settings.AccountTypeId };

                    if (!financialAccount.Settings.ExtendedSettings.IsCustomerAccount(context))// IsCustomerAccount will set UsageTransactionTypeId on context
                        continue;

                    if (!financialAccount.CarrierAccountId.HasValue && !financialAccount.CarrierProfileId.HasValue)
                        throw new NullReferenceException(string.Format("financialAccount.CarrierAccountId & financialAccount.CarrierProfileId for financial Account Id: {0}", financialAccount.FinancialAccountId));

                    if (financialAccount.CarrierAccountId.HasValue)
                    {
                        currencyId = _carrierAccountManager.GetCarrierAccountCurrencyId(financialAccount.CarrierAccountId.Value);
                        customerFinancialAccountsData = customerFinancialAccountsDataDict.GetOrCreateItem(financialAccount.CarrierAccountId.Value);
                        customerFinancialAccountsData.Add(CreateCarrierFinancialAccountData(financialAccount, context.UsageTransactionTypeId, context.CreditLimit, currencyId));
                    }
                    else // so financialAccount.CarrierProfileId.HasValue = true
                    {
                        var customerAccounts = new CarrierAccountManager().GetCarriersByProfileId(financialAccount.CarrierProfileId.Value, true, false);
                        if (customerAccounts != null)
                        {
                            currencyId = _carrierProfileManager.GetCarrierProfileCurrencyId(financialAccount.CarrierProfileId.Value);
                            var carrierFinancialAccountData = CreateCarrierFinancialAccountData(financialAccount, context.UsageTransactionTypeId, context.CreditLimit, currencyId);
                            foreach (var customerAccount in customerAccounts)
                            {
                                customerFinancialAccountsData = customerFinancialAccountsDataDict.GetOrCreateItem(customerAccount.CarrierAccountId);
                                customerFinancialAccountsData.Add(carrierFinancialAccountData);
                            }
                        }
                    }
                }

                return customerFinancialAccountsDataDict.ToDictionary(itm => itm.Key, itm => itm.Value.OrderByDescending(financialAccount => financialAccount.BED));
            });
        }

        /// <summary>
        /// should return a list of applicable Financial Account Data for each supplier account ordered by BED desc
        /// </summary>
        /// <returns></returns>
        Dictionary<int, IOrderedEnumerable<CarrierFinancialAccountData>> GetCachedSuppCarrierFinancialsByCarrAccId()
        {
            return Vanrise.Caching.CacheManagerFactory.GetCacheManager<CacheManager>().GetOrCreateObject("GetCachedSuppCarrierFinancialsByCarrAccId",
            () =>
            {
                int currencyId;
                List<CarrierFinancialAccountData> supplierFinancialAccountsData;
                Dictionary<int, List<CarrierFinancialAccountData>> supplierFinancialAccountsDataDict = new Dictionary<int, List<CarrierFinancialAccountData>>();
                Dictionary<int, FinancialAccount> financialAccounts = GetCachedFinancialAccounts();

                foreach (var financialAccount in financialAccounts.Values)
                {
                    if (financialAccount.Settings == null)
                        throw new NullReferenceException(string.Format("financialAccount.Settings for financial Account Id: {0}", financialAccount.FinancialAccountId));

                    if (financialAccount.Settings.ExtendedSettings == null)
                        throw new NullReferenceException(string.Format("financialAccount.Settings.ExtendedSettings for financial Account Id: {0}", financialAccount.FinancialAccountId));

                    FinancialAccountIsSupplierAccountContext context = new FinancialAccountIsSupplierAccountContext() { AccountTypeId = financialAccount.Settings.AccountTypeId };

                    if (!financialAccount.Settings.ExtendedSettings.IsSupplierAccount(context))// IsSupplierAccount will set UsageTransactionTypeId on context
                        continue;

                    if (!financialAccount.CarrierAccountId.HasValue && !financialAccount.CarrierProfileId.HasValue)
                        throw new NullReferenceException(string.Format("financialAccount.CarrierAccountId & financialAccount.CarrierProfileId for financial Account Id: {0}", financialAccount.FinancialAccountId));

                    if (financialAccount.CarrierAccountId.HasValue)
                    {
                        currencyId = _carrierAccountManager.GetCarrierAccountCurrencyId(financialAccount.CarrierAccountId.Value);
                        supplierFinancialAccountsData = supplierFinancialAccountsDataDict.GetOrCreateItem(financialAccount.CarrierAccountId.Value);
                        supplierFinancialAccountsData.Add(CreateCarrierFinancialAccountData(financialAccount, context.UsageTransactionTypeId, context.CreditLimit, currencyId));
                    }
                    else // so financialAccount.CarrierProfileId.HasValue = true
                    {
                        currencyId = _carrierProfileManager.GetCarrierProfileCurrencyId(financialAccount.CarrierProfileId.Value);
                        var supplierAccounts = new CarrierAccountManager().GetCarriersByProfileId(financialAccount.CarrierProfileId.Value, false, true);
                        var carrierFinancialAccountData = CreateCarrierFinancialAccountData(financialAccount, context.UsageTransactionTypeId, context.CreditLimit, currencyId);

                        foreach (var supplierAccount in supplierAccounts)
                        {
                            supplierFinancialAccountsData = supplierFinancialAccountsDataDict.GetOrCreateItem(supplierAccount.CarrierAccountId);
                            supplierFinancialAccountsData.Add(carrierFinancialAccountData);
                        }
                    }
                }

                return supplierFinancialAccountsDataDict.ToDictionary(itm => itm.Key, itm => itm.Value.OrderByDescending(financialAccount => financialAccount.BED));
            });
        }

        private CarrierFinancialAccountData CreateCarrierFinancialAccountData(FinancialAccount financialAccount, Guid usageTransactionTypeId, Decimal? creditLimit, int currencyId)
        {
            return new CarrierFinancialAccountData()
            {
                AccountTypeId = financialAccount.Settings.AccountTypeId,
                FinancialAccountId = financialAccount.FinancialAccountId,
                CreditLimit = creditLimit,
                CarrierCurrencyId = currencyId,
                UsageTransactionTypeId = usageTransactionTypeId,
                BED = financialAccount.BED,
                EED = financialAccount.EED
            };
        }

        #endregion

        #region Private Validation Methods
        private bool CheckIsAllowToAddFinancialAccount(FinancialAccount financialAccount, bool isEditMode, out string message)
        {
            message = null;
            if (!CheckFinancialAccountActivation(financialAccount))
            {
                message = "Financial account is inactive.";
                return false;
            }
            if (financialAccount.EED.HasValue && financialAccount.EED.Value < new DateTime())
            {
                message = "EED must not be less than today.";
                return false;
            }
            FinancialValidationData financialValidationData = LoadFinancialValidationData(financialAccount.CarrierProfileId, financialAccount.CarrierAccountId, financialAccount.FinancialAccountId);
            var financialAccountType = financialValidationData.FinancialAccountTypes.FirstOrDefault(x => x.VRComponentTypeId == financialAccount.Settings.AccountTypeId);
            var accountBalanceSettings = financialAccountType.Settings.ExtendedSettings as AccountBalanceSettings;
            if (financialAccount.CarrierProfileId.HasValue)
            {
                if (!CheckFinancialCarrierProfileValidation(financialAccountType.VRComponentTypeId, accountBalanceSettings, financialValidationData.FinancialCarrierProfile.ProfileCarrierAccounts, financialValidationData.ProfileFinancialAccounts, financialValidationData.FinancialCarrierProfile.FinancialAccountsByAccount, isEditMode))
                    return false;
            }
            else
            {
                if (!CheckFinancialCarrierAccountValidation(financialAccountType.VRComponentTypeId, accountBalanceSettings, financialValidationData.FinancialCarrierAccount.CarrierAccount, financialValidationData.ProfileFinancialAccounts, financialValidationData.FinancialCarrierAccount.FinancialAccounts, isEditMode))
                    return false;

            }
            ValidateFinancialAccount(financialAccount, financialValidationData, accountBalanceSettings, out message);
            if (message != null)
                return false;
            return true;
        }
        private bool CheckProfileCarrierAccountsActivation(int carrierProfileId, bool isCustomer, bool isSupplier)
        {
            var carrierAccounts = _carrierAccountManager.GetCarriersByProfileId(carrierProfileId, isCustomer, isSupplier);
            foreach (var carrierAccount in carrierAccounts)
            {
                if (carrierAccount.CarrierAccountSettings.ActivationStatus != ActivationStatus.Inactive)
                {
                    return true;
                }
            }
            return false;
        }
        private bool CheckFinancialAccountActivation(FinancialAccount financialAccount)
        {
            var accountBalanceSettings = new FinancialAccountDefinitionManager().GetFinancialAccountDefinitionExtendedSettings<AccountBalanceSettings>(financialAccount.Settings.AccountTypeId);
            return CheckAccountBalanceSettingsActivationForCarrier(accountBalanceSettings, financialAccount.CarrierAccountId, financialAccount.CarrierProfileId);
        }
        private bool CheckAccountBalanceSettingsActivationForCarrier(AccountBalanceSettings accountBalanceSettings, int? carrierAccountId, int? carrierProfileId)
        {
            var isActive = false;
            if (carrierAccountId.HasValue)
            {
                var carrierAccount = _carrierAccountManager.GetCarrierAccount(carrierAccountId.Value);
                if (carrierAccount.CarrierAccountSettings.ActivationStatus != ActivationStatus.Inactive)
                    isActive = true;
            }
            else
            {
                if (CheckApplicableAccountTypes(accountBalanceSettings, true, false))
                {
                    isActive = CheckProfileCarrierAccountsActivation(carrierProfileId.Value, true, false);
                }
                else if (CheckApplicableAccountTypes(accountBalanceSettings, false, true))
                {
                    isActive = CheckProfileCarrierAccountsActivation(carrierProfileId.Value, false, true);
                }
                else if (CheckApplicableAccountTypes(accountBalanceSettings, true, true))
                {
                    isActive = CheckProfileCarrierAccountsActivation(carrierProfileId.Value, true, true);
                }

            }
            return isActive;
        }
        private bool ValidateFinancialAccount(FinancialAccount financialAccount, FinancialValidationData financialValidationData, AccountBalanceSettings accountBalanceSettings, out string message)
        {
            bool result = true;
            if (financialAccount.CarrierAccountId.HasValue)
            {
                ValidateFinancialAccountforCarrierAccount(financialAccount.Settings.AccountTypeId, financialAccount.FinancialAccountId, financialAccount.BED, financialAccount.EED, financialValidationData.FinancialCarrierAccount.FinancialAccounts, accountBalanceSettings, out message, out result);
                if (result)
                {
                    CheckFinancialAccountProfileOverlapping(financialAccount.Settings.AccountTypeId, financialAccount.FinancialAccountId, financialAccount.BED, financialAccount.EED, financialValidationData.ProfileFinancialAccounts, accountBalanceSettings, out  message, out result);
                }
            }
            else
            {
                ValidateFinancialAccountforCarrierProfile(financialAccount.Settings.AccountTypeId, financialAccount.FinancialAccountId, financialAccount.BED, financialAccount.EED, financialValidationData.ProfileFinancialAccounts, financialValidationData.FinancialCarrierProfile.FinancialAccountsByAccount, accountBalanceSettings, out  message, out result);
            }
            return result;
        }
        private void ValidateFinancialAccountforCarrierProfile(Guid accountTypeId, int financialAccountId, DateTime bed, DateTime? eed, IEnumerable<FinancialAccountData> profileFinancialAccounts, Dictionary<int, IEnumerable<FinancialAccountData>> financialAccountsByAccount, AccountBalanceSettings accountBalanceSettings, out string message, out bool result)
        {
            CheckFinancialAccountProfileOverlapping(accountTypeId, financialAccountId, bed, eed, profileFinancialAccounts, accountBalanceSettings, out message, out result);
            if (!result)
                return;
            foreach (var carrierFinancialAccounts in financialAccountsByAccount.Values)
            {
                ValidateFinancialAccountforCarrierAccount(accountTypeId, financialAccountId, bed, eed, carrierFinancialAccounts, accountBalanceSettings, out message, out result);
                if (!result)
                    return;
            }
        }
        private bool CheckApplicableAccountTypes(AccountBalanceSettings accountBalanceSettings, bool isApplicableToCustomer, bool isApplicableToSupplier)
        {
            if (accountBalanceSettings.IsApplicableToSupplier == isApplicableToSupplier && accountBalanceSettings.IsApplicableToCustomer == isApplicableToCustomer)
                return true;
            return false;
        }
        private void CheckFinancialAccountOverlaping(Guid accountTypeId, int financialAccountId, DateTime bed, DateTime? eed, IEnumerable<FinancialAccountData> financialAccounts, AccountBalanceSettings accountBalanceSettings, out string message, out bool result)
        {
            foreach (var financialAccount in financialAccounts)
            {
                if (financialAccount.FinancialAccount.FinancialAccountId != financialAccountId && accountBalanceSettings.IsApplicableToCustomer == financialAccount.IsApplicableToCustomer && accountBalanceSettings.IsApplicableToSupplier == financialAccount.IsApplicableToSupplier)
                {
                    if (eed.VRGreaterThan(financialAccount.FinancialAccount.BED) && financialAccount.FinancialAccount.EED.VRGreaterThan(bed))
                    {
                        message = string.Format("Financial account must not overlap.");
                        result = false;
                        return;
                    }
                }
            }
            message = null;
            result = true;
        }
        private void CheckFinancialAccountProfileOverlapping(Guid accountTypeId, int financialAccountId, DateTime bed, DateTime? eed, IEnumerable<FinancialAccountData> financialCarrierProfiles, AccountBalanceSettings accountBalanceSettings, out string message, out bool result)
        {
            CheckFinancialAccountOverlaping(accountTypeId, financialAccountId, bed, eed, financialCarrierProfiles, accountBalanceSettings, out message, out result);
        }
        private void ValidateFinancialAccountforCarrierAccount(Guid accountTypeId, int financialAccountId, DateTime bed, DateTime? eed, IEnumerable<FinancialAccountData> carrierFinancialAccounts, AccountBalanceSettings accountBalanceSettings, out string message, out bool result)
        {
            CheckFinancialAccountOverlaping(accountTypeId, financialAccountId, bed, eed, carrierFinancialAccounts, accountBalanceSettings, out message, out result);
        }
        private Func<FinancialAccountData, bool> GetFinancialAccountFilterExpression(Guid accountTypeId, CarrierAccountType carrierAccountType, AccountBalanceSettings accountBalanceSetting, bool isEditMode)
        {
            Func<FinancialAccountData, bool> filterExpression = (financialAccountData) =>
            {
                if (!isEditMode && financialAccountData.FinancialAccount.Settings.AccountTypeId == accountTypeId && !financialAccountData.FinancialAccount.EED.HasValue)
                    return false;

                switch (carrierAccountType)
                {
                    case BusinessEntity.Entities.CarrierAccountType.Customer:
                        if (!isEditMode && !financialAccountData.FinancialAccount.EED.HasValue && accountBalanceSetting.IsApplicableToCustomer == financialAccountData.IsApplicableToCustomer)
                            return false;
                        break;
                    case BusinessEntity.Entities.CarrierAccountType.Supplier:
                        if (!isEditMode && !financialAccountData.FinancialAccount.EED.HasValue && accountBalanceSetting.IsApplicableToSupplier == financialAccountData.IsApplicableToSupplier)
                            return false;
                        break;
                    case BusinessEntity.Entities.CarrierAccountType.Exchange:
                        if (!isEditMode && !financialAccountData.FinancialAccount.EED.HasValue && (accountBalanceSetting.IsApplicableToSupplier == financialAccountData.IsApplicableToSupplier || accountBalanceSetting.IsApplicableToCustomer == financialAccountData.IsApplicableToCustomer))
                            return false;
                        break;
                }
                return true;
            };

            return filterExpression;
        }

        #endregion

        #region LoadFinancialValidationData
        private List<FinancialAccountData> LoadProfileFinancialAccounts(int carrierProfileId, IEnumerable<AccountType> financialAccountTypes, int financialAccountId)
        {
            FinancialAccountManager financialAccountManager = new FinancialAccountManager();
            var profileFinancialAccounts = financialAccountManager.GetCarrierProfileFinancialAccounts(carrierProfileId);
            List<FinancialAccountData> profileFinancialAccountsData = new List<FinancialAccountData>();
            foreach (var profileFinancialAccount in profileFinancialAccounts)
            {
                if (profileFinancialAccount.FinancialAccountId != financialAccountId)
                {
                    var financialAccountType = financialAccountTypes.FindRecord(x => x.VRComponentTypeId == profileFinancialAccount.Settings.AccountTypeId);
                    AccountBalanceSettings accountBalanceSettings = financialAccountType.Settings.ExtendedSettings as AccountBalanceSettings;
                    profileFinancialAccountsData.Add(new FinancialAccountData
                    {
                        FinancialAccount = profileFinancialAccount,
                        IsApplicableToSupplier = accountBalanceSettings.IsApplicableToSupplier,
                        IsApplicableToCustomer = accountBalanceSettings.IsApplicableToCustomer
                    });
                }
            }
            return profileFinancialAccountsData;
        }
        private List<FinancialAccountData> LoadFinancialAccountsForCarrierAccount(int carrierAccountId, IEnumerable<AccountType> financialAccountTypes, int financialAccountId)
        {
            FinancialAccountManager financialAccountManager = new FinancialAccountManager();
            var financialAccounts = financialAccountManager.GetFinancialAccountsByCarrierAccountId(carrierAccountId);
            List<FinancialAccountData> financialAccountsData = new List<FinancialAccountData>();
            foreach (var financialAccount in financialAccounts)
            {
                if (financialAccount.FinancialAccountId != financialAccountId)
                {
                    var financialAccountType = financialAccountTypes.FindRecord(x => x.VRComponentTypeId == financialAccount.Settings.AccountTypeId);
                    AccountBalanceSettings accountBalanceSettings = financialAccountType.Settings.ExtendedSettings as AccountBalanceSettings;
                    financialAccountsData.Add(new FinancialAccountData
                    {
                        FinancialAccount = financialAccount,
                        IsApplicableToSupplier = accountBalanceSettings.IsApplicableToSupplier,
                        IsApplicableToCustomer = accountBalanceSettings.IsApplicableToCustomer
                    });
                }

            }
            return financialAccountsData;
        }

        #endregion

        #region Mappers

        private FinancialAccountInfo FinancialAccountInfoMapper(FinancialAccount financialAccount)
        {
            var financialAccountInfo = new FinancialAccountInfo()
            {
                FinancialAccountId = financialAccount.FinancialAccountId,
                EffectiveStatus = GetFinancialAccountEffectiveStatus(financialAccount.BED, financialAccount.EED)
            };
            if (financialAccount.CarrierProfileId.HasValue)
            {
                financialAccountInfo.CarrierType = FinancialAccountCarrierType.Profile;
                financialAccountInfo.Name = _carrierProfileManager.GetCarrierProfileName(financialAccount.CarrierProfileId.Value);
                string profilePrefix = Utilities.GetEnumDescription<FinancialAccountCarrierType>(FinancialAccountCarrierType.Profile);
                financialAccountInfo.Description = string.Format("({0}) {1}", profilePrefix, financialAccountInfo.Name);
            }
            else
            {
                financialAccountInfo.CarrierType = FinancialAccountCarrierType.Account;
                financialAccountInfo.Name = _carrierAccountManager.GetCarrierAccountName(financialAccount.CarrierAccountId.Value);
                string accountPrefix = Utilities.GetEnumDescription<FinancialAccountCarrierType>(FinancialAccountCarrierType.Account);
                financialAccountInfo.Description = string.Format("({0}) {1}", accountPrefix, financialAccountInfo.Name);
            }
            return financialAccountInfo;
        }
        private FinancialAccountEffectiveStatus GetFinancialAccountEffectiveStatus(DateTime financialAccountBED, DateTime? financialAccountEED)
        {
            DateTime today = DateTime.Today;

            if (financialAccountEED.HasValue)
            {
                if (financialAccountBED > today)
                    return FinancialAccountEffectiveStatus.Future;
                else if (financialAccountBED == today)
                    return FinancialAccountEffectiveStatus.Current;
                else
                {
                    return (financialAccountEED.Value > today) ? FinancialAccountEffectiveStatus.Current : FinancialAccountEffectiveStatus.Recent;
                }
            }
            else
            {
                return (financialAccountBED > today) ? FinancialAccountEffectiveStatus.Future : FinancialAccountEffectiveStatus.Current;
            }
        }
        private FinancialAccountDetail FinancialAccountDetailMapper(FinancialAccount financialAccount)
        {

            return new FinancialAccountDetail
            {
                Entity = financialAccount,
                AccountTypeDescription = new AccountTypeManager().GetAccountTypeName(financialAccount.Settings.AccountTypeId),
                IsActive = CheckFinancialAccountActivation(financialAccount)
            };
        }
        #endregion

        #region IBusinessEntityManager

        public List<dynamic> GetAllEntities(IBusinessEntityGetAllContext context)
        {
            var settings = context.EntityDefinition.Settings.CastWithValidate<FinancialAccountBESettings>("EntityDefinition.Settings", context.EntityDefinitionId);
            return this.GetFinancialAccountsByCarrierAccountTypeId(settings.AccountTypeId).Select(itm => itm as dynamic).ToList();
        }

        public dynamic GetEntity(IBusinessEntityGetByIdContext context)
        {
            throw new NotImplementedException();
        }

        public string GetEntityDescription(IBusinessEntityDescriptionContext context)
        {
            throw new NotImplementedException();
        }

        public dynamic GetEntityId(IBusinessEntityIdContext context)
        {
            throw new NotImplementedException();
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
