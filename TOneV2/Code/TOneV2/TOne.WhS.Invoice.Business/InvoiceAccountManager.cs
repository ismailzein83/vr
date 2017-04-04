using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TOne.WhS.BusinessEntity.Business;
using TOne.WhS.BusinessEntity.Entities;
using TOne.WhS.Invoice.Data;
using TOne.WhS.Invoice.Entities;
using Vanrise.Common;
using Vanrise.Entities;
using Vanrise.Invoice.Business;
using Vanrise.Invoice.Entities;

namespace TOne.WhS.Invoice.Business
{
    public class InvoiceAccountManager
    {
       
        #region Fields

        private CarrierProfileManager _carrierProfileManager = new CarrierProfileManager();

        private CarrierAccountManager _carrierAccountManager = new CarrierAccountManager();

        #endregion

        #region Public Methods

        public IDataRetrievalResult<InvoiceAccountDetail> GetFilteredInvoiceAccounts(Vanrise.Entities.DataRetrievalInput<InvoiceAccountQuery> input)
        {
            var allInvoiceAccounts = GetCachedInvoiceAccounts();

            Func<InvoiceAccount, bool> filterExpression = (prod) =>
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
            return Vanrise.Common.DataRetrievalManager.Instance.ProcessResult(input, allInvoiceAccounts.ToBigResult(input, filterExpression, InvoiceAccountDetailMapper));
        }
        public Vanrise.Entities.InsertOperationOutput<InvoiceAccountDetail> AddInvoiceAccount(InvoiceAccount invoiceAccount)
        {
            Vanrise.Entities.InsertOperationOutput<InvoiceAccountDetail> insertOperationOutput = new Vanrise.Entities.InsertOperationOutput<InvoiceAccountDetail>();

            insertOperationOutput.Result = Vanrise.Entities.InsertOperationResult.Failed;
            insertOperationOutput.InsertedObject = null;
            string message = null;
            if (CheckIsAllowToAddInvoiceAccount(invoiceAccount, false, out message))
            {
                int invoiceAccountId = -1;

                IInvoiceAccountDataManager dataManager = InvoiceManagerFactory.GetDataManager<IInvoiceAccountDataManager>();
                bool insertActionSucc = dataManager.Insert(invoiceAccount, out invoiceAccountId);
                if (insertActionSucc)
                {
                    Vanrise.Caching.CacheManagerFactory.GetCacheManager<CacheManager>().SetCacheExpired();
                    insertOperationOutput.Result = Vanrise.Entities.InsertOperationResult.Succeeded;
                    invoiceAccount.InvoiceAccountId = invoiceAccountId;
                    insertOperationOutput.InsertedObject = InvoiceAccountDetailMapper(invoiceAccount);
                }
                else
                {
                    insertOperationOutput.Result = Vanrise.Entities.InsertOperationResult.SameExists;
                }
            }
            insertOperationOutput.Message = message;
            return insertOperationOutput;
        }
        public Vanrise.Entities.UpdateOperationOutput<InvoiceAccountDetail> UpdateInvoiceAccount(InvoiceAccount invoiceAccount)
        {
            IInvoiceAccountDataManager dataManager = InvoiceManagerFactory.GetDataManager<IInvoiceAccountDataManager>();

            Vanrise.Entities.UpdateOperationOutput<InvoiceAccountDetail> updateOperationOutput = new Vanrise.Entities.UpdateOperationOutput<InvoiceAccountDetail>();

            updateOperationOutput.Result = Vanrise.Entities.UpdateOperationResult.Failed;
            updateOperationOutput.UpdatedObject = null;
            string message = null;

            if (CheckIsAllowToAddInvoiceAccount(invoiceAccount, true, out message))
            {
                bool updateActionSucc = dataManager.Update(invoiceAccount);
                if (updateActionSucc)
                {
                    Vanrise.Caching.CacheManagerFactory.GetCacheManager<CacheManager>().SetCacheExpired();
                    updateOperationOutput.Result = Vanrise.Entities.UpdateOperationResult.Succeeded;
                    updateOperationOutput.UpdatedObject = InvoiceAccountDetailMapper(invoiceAccount);
                }
                else
                {
                    updateOperationOutput.Result = Vanrise.Entities.UpdateOperationResult.SameExists;
                }
            }
            updateOperationOutput.Message = message;
            return updateOperationOutput;
        }
        public InvoiceAccount GetInvoiceAccount(int invoiceAccountId)
        {
            var allInvoiceAccounts = GetCachedInvoiceAccounts();
            return allInvoiceAccounts.GetRecord(invoiceAccountId);
        }
        public CarrierInvoiceAccountData GetCustCarrierInvoiceByFinAccId(int invoiceAccountId)
        {
            CarrierInvoiceAccountData carrierInvoiceAccountData = GetCachedCustCarrierInvoicesByFinAccId().GetRecord(invoiceAccountId);
            carrierInvoiceAccountData.ThrowIfNull("carrierInvoiceAccountData", invoiceAccountId);
            return carrierInvoiceAccountData;
        }
        public CarrierInvoiceAccountData GetSuppCarrierInvoiceByFinAccId(int invoiceAccountId)
        {
            CarrierInvoiceAccountData carrierInvoiceAccountData = GetCachedSuppCarrierInvoicesByFinAccId().GetRecord(invoiceAccountId);
            carrierInvoiceAccountData.ThrowIfNull("carrierInvoiceAccountData", invoiceAccountId);
            return carrierInvoiceAccountData;
        }
        public bool TryGetCustAccInvoiceAccountData(int customerAccountId, DateTime effectiveOn, out CarrierInvoiceAccountData invoiceAccountData)
        {
            IOrderedEnumerable<CarrierInvoiceAccountData> carrierInvoiceAccounts = GetCachedCustCarrierInvoicesByCarrAccId().GetRecord(customerAccountId);
            if (carrierInvoiceAccounts != null)
            {
                foreach (var acc in carrierInvoiceAccounts)
                {
                    if (acc.BED <= effectiveOn && acc.EED.VRGreaterThan(effectiveOn))
                    {
                        invoiceAccountData = acc;
                        return true;
                    }
                }
            }
            invoiceAccountData = null;
            return false;
        }
        public bool TryGetCustProfInvoiceAccountData(int customerProfileId, DateTime effectiveOn, out List<CarrierInvoiceAccountData> invoiceAccountsData)
        {
            throw new NotImplementedException();
        }
        public bool TryGetSuppAccInvoiceAccountData(int supplierAccountId, DateTime effectiveOn, out CarrierInvoiceAccountData invoiceAccountData)
        {
            IOrderedEnumerable<CarrierInvoiceAccountData> carrierInvoiceAccounts = GetCachedSuppCarrierInvoicesByCarrAccId().GetRecord(supplierAccountId);
            if (carrierInvoiceAccounts != null)
            {
                foreach (var acc in carrierInvoiceAccounts)
                {
                    if (acc.BED <= effectiveOn && acc.EED.VRGreaterThan(effectiveOn))
                    {
                        invoiceAccountData = acc;
                        return true;
                    }
                }
            }
            invoiceAccountData = null;
            return false;
        }
        public bool TryGetSuppProfInvoiceAccountData(int supplierProfileId, DateTime effectiveOn, out List<CarrierInvoiceAccountData> invoiceAccountsData)
        {
            throw new NotImplementedException();
        }
        public InvoiceAccountEditorRuntime GetInvoiceAccountEditorRuntime(int invoiceAccountId)
        {
            var allInvoiceAccounts = GetCachedInvoiceAccounts();
            var invoiceAccount = allInvoiceAccounts.GetRecord(invoiceAccountId);
            return new InvoiceAccountEditorRuntime
            {
                InvoiceAccount = invoiceAccount,
            };
        }

        public IEnumerable<InvoiceAccount> GetInvoiceAccountsByCarrierAccountId(int carrierAccountId)
        {
            var invoiceAccounts = GetCachedInvoiceAccounts();
            return invoiceAccounts.Values.FindAllRecords(x => x.CarrierAccountId.HasValue && x.CarrierAccountId.Value == carrierAccountId);
        }
        public IEnumerable<InvoiceAccount> GetCarrierProfileInvoiceAccounts(int carrierProfileId)
        {
            var invoiceAccounts = GetCachedInvoiceAccounts();
            return invoiceAccounts.Values.FindAllRecords(x => x.CarrierProfileId.HasValue && x.CarrierProfileId.Value == carrierProfileId);
        }

        #region LoadInvoiceValidationData
        public InvoiceValidationData LoadInvoiceValidationData(int? carrierProfileId, int? carrierAccountId, int invoiceAccountId)
        {

            InvoiceValidationData invoiceValidationData = new InvoiceValidationData();

            InvoiceAccountManager invoiceAccountManager = new InvoiceAccountManager();
            invoiceValidationData.InvoiceTypes = new InvoiceTypeManager().GetInvoiceTypes();


            CarrierAccountManager carrierAccountManager = new CarrierAccountManager();
            int carrierProfileID = -1;
            if (carrierAccountId.HasValue)
            {
                var carrierAccount = carrierAccountManager.GetCarrierAccount(carrierAccountId.Value);
                carrierProfileID = carrierAccount.CarrierProfileId;

                invoiceValidationData.InvoiceCarrierAccount = new InvoiceCarrierAccount
                {
                    CarrierAccount = carrierAccount
                };
                var carrierInvoiceAccounts = GetInvoiceAccountsByCarrierAccountId(carrierAccountId.Value);
                invoiceValidationData.InvoiceCarrierAccount.InvoiceAccounts = LoadInvoiceAccountsForCarrierAccount(carrierAccountId.Value, invoiceValidationData.InvoiceTypes, invoiceAccountId);
            }
            else if (carrierProfileId.HasValue)
            {
                carrierProfileID = carrierProfileId.Value;

                var profileCarrierAccounts = carrierAccountManager.GetCarriersByProfileId(carrierProfileId.Value, true, true);
                invoiceValidationData.InvoiceCarrierProfile = new InvoiceCarrierProfile
                {
                    ProfileCarrierAccounts = profileCarrierAccounts,
                    InvoiceAccountsByAccount = new Dictionary<int, IEnumerable<InvoiceAccountData>>(),
                };
                foreach (var carrierAccount in profileCarrierAccounts)
                {
                    var invoiceAccounts = LoadInvoiceAccountsForCarrierAccount(carrierAccount.CarrierAccountId, invoiceValidationData.InvoiceTypes, invoiceAccountId);
                    if (invoiceAccounts != null && invoiceAccounts.Count() > 0)
                    {
                        invoiceValidationData.InvoiceCarrierProfile.InvoiceAccountsByAccount.Add(carrierAccount.CarrierAccountId, invoiceAccounts);
                    }
                }
            }
            invoiceValidationData.ProfileInvoiceAccounts = LoadProfileInvoiceAccounts(carrierProfileID, invoiceValidationData.InvoiceTypes, invoiceAccountId);
            return invoiceValidationData;
        }
        #endregion

        public bool CheckInvoiceCarrierAccountValidation(Guid invoiceTypeId, InvoiceSettings invoiceSettings, CarrierAccount carrierAccount, IEnumerable<InvoiceAccountData> profileinvoiceAccounts, IEnumerable<InvoiceAccountData> carrierInvoiceAccounts, bool isEditMode)
        {
            if (carrierAccount.IsDeleted || carrierAccount.CarrierAccountSettings.ActivationStatus == ActivationStatus.Inactive)
                return false;

            Func<InvoiceAccountData, bool> filterExpression = GetInvoiceAccountFilterExpression(invoiceTypeId, carrierAccount.AccountType, invoiceSettings, isEditMode);

            switch (carrierAccount.AccountType)
            {
                case BusinessEntity.Entities.CarrierAccountType.Customer:
                    if (!CheckApplicableInvoiceTypes(invoiceSettings, true, false))
                        return false;
                    break;
                case BusinessEntity.Entities.CarrierAccountType.Supplier:
                    if (!CheckApplicableInvoiceTypes(invoiceSettings, false, true))
                        return false;
                    break;
                case BusinessEntity.Entities.CarrierAccountType.Exchange:
                    if (CheckApplicableInvoiceTypes(invoiceSettings, false, false))
                        return false;
                    break;
            }
            if (carrierInvoiceAccounts.Any(x => !isEditMode && x.InvoiceAccount.Settings.InvoiceTypeId == invoiceTypeId && !x.InvoiceAccount.EED.HasValue))
                return false;
            if (carrierInvoiceAccounts.Any(x => !filterExpression(x)))
                return false;
            if (carrierInvoiceAccounts.Any(x => !filterExpression(x)))
                return false;
            return true;
        }
        public bool CheckInvoiceCarrierProfileValidation(Guid accountTypeId, InvoiceSettings invoiceSettings, IEnumerable<CarrierAccount> carrierAccounts, IEnumerable<InvoiceAccountData> profileInvoiceAccounts, Dictionary<int, IEnumerable<InvoiceAccountData>> invoiceAccountsByAccount, bool isEditMode)
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
            Func<InvoiceAccountData, bool> filterExpression = null;
            if (CheckApplicableInvoiceTypes(invoiceSettings, true, false))
            {
                if (!areCustomersActive)
                    return false;

                if (!hasCustomers)
                    return false;

                filterExpression = GetInvoiceAccountFilterExpression(accountTypeId, CarrierAccountType.Customer, invoiceSettings, isEditMode);

            }
            else if (CheckApplicableInvoiceTypes(invoiceSettings, false, true))
            {
                if (!areSuppliersActive)
                    return false;
                if (!hasSuppliers)
                    return false;
                filterExpression = GetInvoiceAccountFilterExpression(accountTypeId, CarrierAccountType.Supplier, invoiceSettings, isEditMode);
            }
            else if (!CheckApplicableInvoiceTypes(invoiceSettings, false, false))
            {
                if (!areCustomersActive || !areSuppliersActive)
                    return false;
                if (!hasCustomers || !hasSuppliers)
                {
                    return false;
                }
                filterExpression = GetInvoiceAccountFilterExpression(accountTypeId, CarrierAccountType.Exchange, invoiceSettings, isEditMode);
            }


            if (profileInvoiceAccounts.Any(x => !filterExpression(x)))
                return false;

            if (invoiceAccountsByAccount.Values.Any(x => x.Any(y => !filterExpression(y))))
                return false;

            return true;
        }
        public bool CheckCarrierAllowAddInvoiceAccounts(int? carrierProfileId, int? carrierAccountId)
        {
            InvoiceValidationData invoiceValidationData = LoadInvoiceValidationData(carrierProfileId, carrierAccountId, 0);
            Func<InvoiceType, bool> filterExpression = (invoiceType) =>
            {
                var invoiceSettings = invoiceType.Settings.ExtendedSettings as InvoiceSettings;
                if (invoiceSettings == null)
                    return false;
                if (!CheckInvoiceTypeSettingsActivationForCarrier(invoiceSettings, carrierAccountId, carrierProfileId))
                    return false;
                if (carrierProfileId.HasValue)
                {
                    if (!CheckInvoiceCarrierProfileValidation(invoiceType.InvoiceTypeId, invoiceSettings, invoiceValidationData.InvoiceCarrierProfile.ProfileCarrierAccounts, invoiceValidationData.ProfileInvoiceAccounts, invoiceValidationData.InvoiceCarrierProfile.InvoiceAccountsByAccount, false))
                        return false;
                }
                else
                {

                    if (!CheckInvoiceCarrierAccountValidation(invoiceType.InvoiceTypeId, invoiceSettings, invoiceValidationData.InvoiceCarrierAccount.CarrierAccount, invoiceValidationData.ProfileInvoiceAccounts, invoiceValidationData.InvoiceCarrierAccount.InvoiceAccounts, false))
                        return false;
                }
                return true;
            };
            var applicableInvoiceAccountTypes = invoiceValidationData.InvoiceTypes.FindAllRecords(filterExpression);
            return applicableInvoiceAccountTypes.Count() > 0;
        }

        public IEnumerable<InvoiceAccountInfo> GetInvoiceAccountsInfo(InvoiceAccountInfoFilter filter)
        {
            Dictionary<int, InvoiceAccount> allInvoiceAccounts = GetCachedInvoiceAccounts();

            Func<InvoiceAccount, bool> filterFunc = null;

            if (filter != null)
            {
                filterFunc = (invoiceAccount) =>
                {
                    if (invoiceAccount.Settings.InvoiceTypeId != filter.InvoiceTypeId)
                        return false;

                    return true;
                };
            }
            return allInvoiceAccounts.MapRecords(InvoiceAccountInfoMapper, filterFunc);
        }

        #endregion

        #region Private Classes
        private class CacheManager : Vanrise.Caching.BaseCacheManager
        {
            IInvoiceAccountDataManager _dataManager = InvoiceManagerFactory.GetDataManager<IInvoiceAccountDataManager>();
            object _updateHandle;

            protected override bool ShouldSetCacheExpired(object parameter)
            {
                return _dataManager.AreInvoiceAccountsUpdated(ref _updateHandle);
            }
        }
        #endregion
       
        #region Private Methods
        Dictionary<int, InvoiceAccount> GetCachedInvoiceAccounts()
        {
            return Vanrise.Caching.CacheManagerFactory.GetCacheManager<CacheManager>().GetOrCreateObject("GetCachedInvoiceAccounts",
              () =>
              {
                  IInvoiceAccountDataManager dataManager = InvoiceManagerFactory.GetDataManager<IInvoiceAccountDataManager>();
                  IEnumerable<InvoiceAccount> invoiceAccounts = dataManager.GetInvoiceAccounts();
                  return invoiceAccounts.ToDictionary(fa => fa.InvoiceAccountId, fa => fa);
              });
        }
        Dictionary<Guid, List<InvoiceAccount>> GetCachedInvoiceAccountsByInvoiceTypeId()
        {
            return Vanrise.Caching.CacheManagerFactory.GetCacheManager<CacheManager>().GetOrCreateObject("GetCachedInvoiceAccountsByInvoiceTypeId",
             () =>
             {
                 var invoiceAccountsByType = new Dictionary<Guid, List<InvoiceAccount>>();
                 Dictionary<int, InvoiceAccount> cachedInvoiceAccounts = GetCachedInvoiceAccounts();
                 if (cachedInvoiceAccounts != null)
                 {
                     foreach (InvoiceAccount invoiceAccount in cachedInvoiceAccounts.Values)
                     {
                         List<InvoiceAccount> invoiceAccounts;

                         if (!invoiceAccountsByType.TryGetValue(invoiceAccount.Settings.InvoiceTypeId, out invoiceAccounts))
                         {
                             invoiceAccounts = new List<InvoiceAccount>();
                             invoiceAccountsByType.Add(invoiceAccount.Settings.InvoiceTypeId, invoiceAccounts);
                         }
                         invoiceAccounts.Add(invoiceAccount);
                     }
                 }
                 return invoiceAccountsByType;
             });
        }
        Dictionary<int, CarrierInvoiceAccountData> GetCachedCustCarrierInvoicesByFinAccId()
        {
            return Vanrise.Caching.CacheManagerFactory.GetCacheManager<CacheManager>().GetOrCreateObject("GetCachedCustCarrierInvoicesByFinAccId",
             () =>
             {
                 var carrierDataByInvoiceAccountId = new Dictionary<int, CarrierInvoiceAccountData>();
                 Dictionary<int, InvoiceAccount> cachedInvoiceAccounts = GetCachedInvoiceAccounts();
                 if (cachedInvoiceAccounts != null)
                 {

                     foreach (var invoiceAccount in cachedInvoiceAccounts.Values)
                     {
                         if (invoiceAccount.Settings == null)
                             throw new NullReferenceException(string.Format("invoiceAccount.Settings for invoice Account Id: {0}", invoiceAccount.InvoiceAccountId));

                         if (invoiceAccount.Settings.ExtendedSettings == null)
                             throw new NullReferenceException(string.Format("invoiceAccount.Settings.ExtendedSettings for invoice Account Id: {0}", invoiceAccount.InvoiceAccountId));

                         InvoiceAccountIsCustomerAccountContext context = new InvoiceAccountIsCustomerAccountContext() { InvoiceTypeId = invoiceAccount.Settings.InvoiceTypeId };

                         if (invoiceAccount.Settings.ExtendedSettings.IsCustomerAccount(context))// IsCustomerAccount will set CreditLimit on context
                         {
                             carrierDataByInvoiceAccountId.GetOrCreateItem(invoiceAccount.InvoiceAccountId, () =>
                             {
                                 return new CarrierInvoiceAccountData
                                 {
                                     InvoiceTypeId = invoiceAccount.Settings.InvoiceTypeId,
                                     BED = invoiceAccount.BED,
                                     EED = invoiceAccount.EED,
                                     InvoiceAccountId = invoiceAccount.InvoiceAccountId,
                                 };
                             });
                         }
                     }
                 }
                 return carrierDataByInvoiceAccountId;
             });
        }
        Dictionary<int, CarrierInvoiceAccountData> GetCachedSuppCarrierInvoicesByFinAccId()
        {
            return Vanrise.Caching.CacheManagerFactory.GetCacheManager<CacheManager>().GetOrCreateObject("GetCachedSuppCarrierInvoicesByFinAccId",
            () =>
            {
                var carrierDataByInvoiceAccountId = new Dictionary<int, CarrierInvoiceAccountData>();
                Dictionary<int, InvoiceAccount> cachedInvoiceAccounts = GetCachedInvoiceAccounts();
                if (cachedInvoiceAccounts != null)
                {
                    foreach (var invoiceAccount in cachedInvoiceAccounts.Values)
                    {
                        if (invoiceAccount.Settings == null)
                            throw new NullReferenceException(string.Format("invoiceAccount.Settings for invoice Account Id: {0}", invoiceAccount.InvoiceAccountId));

                        if (invoiceAccount.Settings.ExtendedSettings == null)
                            throw new NullReferenceException(string.Format("invoiceAccount.Settings.ExtendedSettings for invoice Account Id: {0}", invoiceAccount.InvoiceAccountId));

                        InvoiceAccountIsSupplierAccountContext context = new InvoiceAccountIsSupplierAccountContext() { InvoiceTypeId = invoiceAccount.Settings.InvoiceTypeId };

                        if (invoiceAccount.Settings.ExtendedSettings.IsSupplierAccount(context))// IsSupplierAccount will set CreditLimit on context
                        {
                            carrierDataByInvoiceAccountId.GetOrCreateItem(invoiceAccount.InvoiceAccountId, () =>
                            {
                                return new CarrierInvoiceAccountData
                                {
                                    InvoiceTypeId = invoiceAccount.Settings.InvoiceTypeId,
                                    BED = invoiceAccount.BED,
                                    EED = invoiceAccount.EED,
                                    InvoiceAccountId = invoiceAccount.InvoiceAccountId,
                                };
                            });
                        }
                    }
                }
                return carrierDataByInvoiceAccountId;
            });

        }

        /// <summary>
        /// should return a list of applicable Invoice Account Data for each customer account ordered by BED desc
        /// </summary>
        /// <returns></returns>
        Dictionary<int, IOrderedEnumerable<CarrierInvoiceAccountData>> GetCachedCustCarrierInvoicesByCarrAccId()
        {
            return Vanrise.Caching.CacheManagerFactory.GetCacheManager<CacheManager>().GetOrCreateObject("GetCachedCustCarrierInvoicesByCarrAccId",
            () =>
            {
                List<CarrierInvoiceAccountData> customerInvoiceAccountsData;
                Dictionary<int, List<CarrierInvoiceAccountData>> customerInvoiceAccountsDataDict = new Dictionary<int, List<CarrierInvoiceAccountData>>();
                Dictionary<int, InvoiceAccount> invoiceAccounts = GetCachedInvoiceAccounts();

                foreach (var invoiceAccount in invoiceAccounts.Values)
                {
                    if (invoiceAccount.Settings == null)
                        throw new NullReferenceException(string.Format("invoiceAccount.Settings for invoice Account Id: {0}", invoiceAccount.InvoiceAccountId));

                    if (invoiceAccount.Settings.ExtendedSettings == null)
                        throw new NullReferenceException(string.Format("invoiceAccount.Settings.ExtendedSettings for invoice Account Id: {0}", invoiceAccount.InvoiceAccountId));

                    InvoiceAccountIsCustomerAccountContext context = new InvoiceAccountIsCustomerAccountContext() { InvoiceTypeId = invoiceAccount.Settings.InvoiceTypeId };

                    if (!invoiceAccount.Settings.ExtendedSettings.IsCustomerAccount(context))// IsCustomerAccount will set UsageTransactionTypeId on context
                        continue;

                    if (!invoiceAccount.CarrierAccountId.HasValue && !invoiceAccount.CarrierProfileId.HasValue)
                        throw new NullReferenceException(string.Format("invoiceAccount.CarrierAccountId & invoiceAccount.CarrierProfileId for invoice Account Id: {0}", invoiceAccount.InvoiceAccountId));

                    if (invoiceAccount.CarrierAccountId.HasValue)
                    {
                        customerInvoiceAccountsData = customerInvoiceAccountsDataDict.GetOrCreateItem(invoiceAccount.CarrierAccountId.Value);
                        customerInvoiceAccountsData.Add(CreateCarrierInvoiceAccountData(invoiceAccount));
                    }
                    else // so invoiceAccount.CarrierProfileId.HasValue = true
                    {
                        var customerAccounts = new CarrierAccountManager().GetCarriersByProfileId(invoiceAccount.CarrierProfileId.Value, true, false);
                        if (customerAccounts != null)
                        {
                            foreach (var customerAccount in customerAccounts)
                            {
                                customerInvoiceAccountsData = customerInvoiceAccountsDataDict.GetOrCreateItem(customerAccount.CarrierAccountId);
                                customerInvoiceAccountsData.Add(CreateCarrierInvoiceAccountData(invoiceAccount));
                            }
                        }
                    }
                }

                return customerInvoiceAccountsDataDict.ToDictionary(itm => itm.Key, itm => itm.Value.OrderByDescending(invoiceAccount => invoiceAccount.BED));
            });
        }

        /// <summary>
        /// should return a list of applicable Invoice Account Data for each supplier account ordered by BED desc
        /// </summary>
        /// <returns></returns>
        Dictionary<int, IOrderedEnumerable<CarrierInvoiceAccountData>> GetCachedSuppCarrierInvoicesByCarrAccId()
        {
            return Vanrise.Caching.CacheManagerFactory.GetCacheManager<CacheManager>().GetOrCreateObject("GetCachedSuppCarrierInvoicesByCarrAccId",
          () =>
          {
              List<CarrierInvoiceAccountData> supplierInvoiceAccountsData;
              Dictionary<int, List<CarrierInvoiceAccountData>> supplierInvoiceAccountsDataDict = new Dictionary<int, List<CarrierInvoiceAccountData>>();
              Dictionary<int, InvoiceAccount> invoiceAccounts = GetCachedInvoiceAccounts();

              foreach (var invoiceAccount in invoiceAccounts.Values)
              {
                  if (invoiceAccount.Settings == null)
                      throw new NullReferenceException(string.Format("invoiceAccount.Settings for invoice Account Id: {0}", invoiceAccount.InvoiceAccountId));

                  if (invoiceAccount.Settings.ExtendedSettings == null)
                      throw new NullReferenceException(string.Format("invoiceAccount.Settings.ExtendedSettings for invoice Account Id: {0}", invoiceAccount.InvoiceAccountId));

                  InvoiceAccountIsSupplierAccountContext context = new InvoiceAccountIsSupplierAccountContext() { InvoiceTypeId = invoiceAccount.Settings.InvoiceTypeId };

                  if (!invoiceAccount.Settings.ExtendedSettings.IsSupplierAccount(context))
                      continue;

                  if (!invoiceAccount.CarrierAccountId.HasValue && !invoiceAccount.CarrierProfileId.HasValue)
                      throw new NullReferenceException(string.Format("invoiceAccount.CarrierAccountId & invoiceAccount.CarrierProfileId for invoice Account Id: {0}", invoiceAccount.InvoiceAccountId));

                  if (invoiceAccount.CarrierAccountId.HasValue)
                  {
                      supplierInvoiceAccountsData = supplierInvoiceAccountsDataDict.GetOrCreateItem(invoiceAccount.CarrierAccountId.Value);
                      supplierInvoiceAccountsData.Add(CreateCarrierInvoiceAccountData(invoiceAccount));
                  }
                  else
                  {
                      var supplierAccounts = new CarrierAccountManager().GetCarriersByProfileId(invoiceAccount.CarrierProfileId.Value, false, true);
                      foreach (var supplierAccount in supplierAccounts)
                      {
                          supplierInvoiceAccountsData = supplierInvoiceAccountsDataDict.GetOrCreateItem(supplierAccount.CarrierAccountId);
                          supplierInvoiceAccountsData.Add(CreateCarrierInvoiceAccountData(invoiceAccount));
                      }
                  }
              }

              return supplierInvoiceAccountsDataDict.ToDictionary(itm => itm.Key, itm => itm.Value.OrderByDescending(invoiceAccount => invoiceAccount.BED));
          });
        }
        private CarrierInvoiceAccountData CreateCarrierInvoiceAccountData(InvoiceAccount invoiceAccount)
        {
            return new CarrierInvoiceAccountData()
            {
                InvoiceTypeId = invoiceAccount.Settings.InvoiceTypeId,
                InvoiceAccountId = invoiceAccount.InvoiceAccountId,
                BED = invoiceAccount.BED,
                EED = invoiceAccount.EED,
            };
        }

        #endregion


        #region Mappers

        private InvoiceAccountInfo InvoiceAccountInfoMapper(InvoiceAccount InvoiceAccount)
        {
            var InvoiceAccountInfo = new InvoiceAccountInfo()
            {
                InvoiceAccountId = InvoiceAccount.InvoiceAccountId,
                EffectiveStatus = GetInvoiceAccountEffectiveStatus(InvoiceAccount.BED, InvoiceAccount.EED)
            };
            if (InvoiceAccount.CarrierProfileId.HasValue)
            {
                InvoiceAccountInfo.CarrierType = InvoiceAccountCarrierType.Profile;
                InvoiceAccountInfo.Name = _carrierProfileManager.GetCarrierProfileName(InvoiceAccount.CarrierProfileId.Value);
                string profilePrefix = Utilities.GetEnumDescription<InvoiceAccountCarrierType>(InvoiceAccountCarrierType.Profile);
                InvoiceAccountInfo.Description = string.Format("({0}) {1}", profilePrefix, InvoiceAccountInfo.Name);
            }
            else
            {
                InvoiceAccountInfo.CarrierType = InvoiceAccountCarrierType.Account;
                InvoiceAccountInfo.Name = _carrierAccountManager.GetCarrierAccountName(InvoiceAccount.CarrierAccountId.Value);
                string accountPrefix = Utilities.GetEnumDescription<InvoiceAccountCarrierType>(InvoiceAccountCarrierType.Account);
                InvoiceAccountInfo.Description = string.Format("({0}) {1}", accountPrefix, InvoiceAccountInfo.Name);
            }
            return InvoiceAccountInfo;
        }
        private InvoiceAccountEffectiveStatus GetInvoiceAccountEffectiveStatus(DateTime InvoiceAccountBED, DateTime? InvoiceAccountEED)
        {
            DateTime today = DateTime.Today;

            if (InvoiceAccountEED.HasValue)
            {
                if (InvoiceAccountBED > today)
                    return InvoiceAccountEffectiveStatus.Future;
                else if (InvoiceAccountBED == today)
                    return InvoiceAccountEffectiveStatus.Current;
                else
                {
                    return (InvoiceAccountEED.Value > today) ? InvoiceAccountEffectiveStatus.Current : InvoiceAccountEffectiveStatus.Recent;
                }
            }
            else
            {
                return (InvoiceAccountBED > today) ? InvoiceAccountEffectiveStatus.Future : InvoiceAccountEffectiveStatus.Current;
            }
        }
        private InvoiceAccountDetail InvoiceAccountDetailMapper(InvoiceAccount InvoiceAccount)
        {

            return new InvoiceAccountDetail
            {
                Entity = InvoiceAccount,
                InvoiceTypeDescription = new InvoiceTypeManager().GetInvoiceTypeName(InvoiceAccount.Settings.InvoiceTypeId),
                IsActive = CheckInvoiceAccountActivation(InvoiceAccount)
            };
        }



        #region Private Validation Methods
        private bool CheckIsAllowToAddInvoiceAccount(InvoiceAccount invoiceAccount, bool isEditMode, out string message)
        {
            message = null;
            if (!CheckInvoiceAccountActivation(invoiceAccount))
            {
                message = "Invoice account is inactive.";
                return false;
            }
            if (invoiceAccount.EED.HasValue && invoiceAccount.EED.Value < new DateTime())
            {
                message = "EED must not be less than today.";
                return false;
            }
            InvoiceValidationData invoiceValidationData = LoadInvoiceValidationData(invoiceAccount.CarrierProfileId, invoiceAccount.CarrierAccountId, invoiceAccount.InvoiceAccountId);
            var invoiceType = invoiceValidationData.InvoiceTypes.FirstOrDefault(x => x.InvoiceTypeId == invoiceAccount.Settings.InvoiceTypeId);
            var invoiceSettings = invoiceType.Settings.ExtendedSettings as InvoiceSettings;
            if (invoiceAccount.CarrierProfileId.HasValue)
            {
                if (!CheckInvoiceCarrierProfileValidation(invoiceType.InvoiceTypeId, invoiceSettings, invoiceValidationData.InvoiceCarrierProfile.ProfileCarrierAccounts, invoiceValidationData.ProfileInvoiceAccounts, invoiceValidationData.InvoiceCarrierProfile.InvoiceAccountsByAccount, isEditMode))
                    return false;
            }
            else
            {
                if (!CheckInvoiceCarrierAccountValidation(invoiceType.InvoiceTypeId, invoiceSettings, invoiceValidationData.InvoiceCarrierAccount.CarrierAccount, invoiceValidationData.ProfileInvoiceAccounts, invoiceValidationData.InvoiceCarrierAccount.InvoiceAccounts, isEditMode))
                    return false;

            }
            ValidateInvoiceAccount(invoiceAccount, invoiceValidationData, invoiceSettings, out message);
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
        private bool CheckInvoiceAccountActivation(InvoiceAccount invoiceAccount)
        {
            var invoiceSettings = new InvoiceTypeManager().GetInvoiceTypeExtendedSettings(invoiceAccount.Settings.InvoiceTypeId) as InvoiceSettings;
            return CheckInvoiceTypeSettingsActivationForCarrier(invoiceSettings, invoiceAccount.CarrierAccountId, invoiceAccount.CarrierProfileId);
        }
        private bool CheckInvoiceTypeSettingsActivationForCarrier(InvoiceSettings invoiceSettings, int? carrierAccountId, int? carrierProfileId)
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
                if (CheckApplicableInvoiceTypes(invoiceSettings, true, false))
                {
                    isActive = CheckProfileCarrierAccountsActivation(carrierProfileId.Value, true, false);
                }
                else if (CheckApplicableInvoiceTypes(invoiceSettings, false, true))
                {
                    isActive = CheckProfileCarrierAccountsActivation(carrierProfileId.Value, false, true);
                }
                else if (CheckApplicableInvoiceTypes(invoiceSettings, true, true))
                {
                    isActive = CheckProfileCarrierAccountsActivation(carrierProfileId.Value, true, true);
                }

            }
            return isActive;
        }
        private bool ValidateInvoiceAccount(InvoiceAccount invoiceAccount, InvoiceValidationData invoiceValidationData, InvoiceSettings invoiceSettings, out string message)
        {
            bool result = true;
            if (invoiceAccount.CarrierAccountId.HasValue)
            {
                ValidateInvoiceAccountforCarrierAccount(invoiceAccount.Settings.InvoiceTypeId, invoiceAccount.InvoiceAccountId, invoiceAccount.BED, invoiceAccount.EED, invoiceValidationData.InvoiceCarrierAccount.InvoiceAccounts, invoiceSettings, out message, out result);
                if (result)
                {
                    CheckInvoiceAccountProfileOverlapping(invoiceAccount.Settings.InvoiceTypeId, invoiceAccount.InvoiceAccountId, invoiceAccount.BED, invoiceAccount.EED, invoiceValidationData.ProfileInvoiceAccounts, invoiceSettings, out  message, out result);
                }
            }
            else
            {
                ValidateInvoiceAccountforCarrierProfile(invoiceAccount.Settings.InvoiceTypeId, invoiceAccount.InvoiceAccountId, invoiceAccount.BED, invoiceAccount.EED, invoiceValidationData.ProfileInvoiceAccounts, invoiceValidationData.InvoiceCarrierProfile.InvoiceAccountsByAccount, invoiceSettings, out  message, out result);
            }
            return result;
        }
        private void ValidateInvoiceAccountforCarrierProfile(Guid invoiceTypeId, int invoiceAccountId, DateTime bed, DateTime? eed, IEnumerable<InvoiceAccountData> profileInvoiceAccounts, Dictionary<int, IEnumerable<InvoiceAccountData>> invoiceAccountsByAccount, InvoiceSettings invoiceSettings, out string message, out bool result)
        {
            CheckInvoiceAccountProfileOverlapping(invoiceTypeId, invoiceAccountId, bed, eed, profileInvoiceAccounts, invoiceSettings, out message, out result);
            if (!result)
                return;
            foreach (var carrierInvoiceAccounts in invoiceAccountsByAccount.Values)
            {
                ValidateInvoiceAccountforCarrierAccount(invoiceTypeId, invoiceAccountId, bed, eed, carrierInvoiceAccounts, invoiceSettings, out message, out result);
                if (!result)
                    return;
            }
        }
        private bool CheckApplicableInvoiceTypes(InvoiceSettings invoiceSettings, bool isApplicableToCustomer, bool isApplicableToSupplier)
        {
            if (invoiceSettings.IsApplicableToSupplier == isApplicableToSupplier && invoiceSettings.IsApplicableToCustomer == isApplicableToCustomer)
                return true;
            return false;
        }
        private void CheckInvoiceAccountOverlaping(Guid invoiceTypeId, int invoiceAccountId, DateTime bed, DateTime? eed, IEnumerable<InvoiceAccountData> invoiceAccounts, InvoiceSettings invoiceSettings, out string message, out bool result)
        {
            foreach (var invoiceAccount in invoiceAccounts)
            {
                if (invoiceAccount.InvoiceAccount.InvoiceAccountId != invoiceAccountId && invoiceSettings.IsApplicableToCustomer == invoiceAccount.IsApplicableToCustomer && invoiceSettings.IsApplicableToSupplier == invoiceAccount.IsApplicableToSupplier)
                {
                    if (eed.VRGreaterThan(invoiceAccount.InvoiceAccount.BED) && invoiceAccount.InvoiceAccount.EED.VRGreaterThan(bed))
                    {
                        message = string.Format("Invoice account must not overlap.");
                        result = false;
                        return;
                    }
                }
            }
            message = null;
            result = true;
        }
        private void CheckInvoiceAccountProfileOverlapping(Guid invoiceTypeId, int invoiceAccountId, DateTime bed, DateTime? eed, IEnumerable<InvoiceAccountData> invoiceCarrierProfiles, InvoiceSettings invoiceSettings, out string message, out bool result)
        {
            CheckInvoiceAccountOverlaping(invoiceTypeId, invoiceAccountId, bed, eed, invoiceCarrierProfiles, invoiceSettings, out message, out result);
        }
        private void ValidateInvoiceAccountforCarrierAccount(Guid invoiceTypeId, int invoiceAccountId, DateTime bed, DateTime? eed, IEnumerable<InvoiceAccountData> carrierInvoiceAccounts, InvoiceSettings invoiceSettings, out string message, out bool result)
        {
            CheckInvoiceAccountOverlaping(invoiceTypeId, invoiceAccountId, bed, eed, carrierInvoiceAccounts, invoiceSettings, out message, out result);
        }
        private Func<InvoiceAccountData, bool> GetInvoiceAccountFilterExpression(Guid invoiceTypeId, CarrierAccountType carrierAccountType, InvoiceSettings invoiceSettings, bool isEditMode)
        {
            Func<InvoiceAccountData, bool> filterExpression = (invoiceAccountData) =>
            {
                if (!isEditMode && invoiceAccountData.InvoiceAccount.Settings.InvoiceTypeId == invoiceTypeId && !invoiceAccountData.InvoiceAccount.EED.HasValue)
                    return false;

                switch (carrierAccountType)
                {
                    case BusinessEntity.Entities.CarrierAccountType.Customer:
                        if (!isEditMode && !invoiceAccountData.InvoiceAccount.EED.HasValue && invoiceSettings.IsApplicableToCustomer == invoiceAccountData.IsApplicableToCustomer)
                            return false;
                        break;
                    case BusinessEntity.Entities.CarrierAccountType.Supplier:
                        if (!isEditMode && !invoiceAccountData.InvoiceAccount.EED.HasValue && invoiceSettings.IsApplicableToSupplier == invoiceAccountData.IsApplicableToSupplier)
                            return false;
                        break;
                    case BusinessEntity.Entities.CarrierAccountType.Exchange:
                        if (!isEditMode && !invoiceAccountData.InvoiceAccount.EED.HasValue && (invoiceSettings.IsApplicableToSupplier == invoiceAccountData.IsApplicableToSupplier || invoiceSettings.IsApplicableToCustomer == invoiceAccountData.IsApplicableToCustomer))
                            return false;
                        break;
                }
                return true;
            };

            return filterExpression;
        }

        #region LoadInvoiceValidationData
        private List<InvoiceAccountData> LoadProfileInvoiceAccounts(int carrierProfileId, IEnumerable<InvoiceType> invoiceTypes, int invoiceAccountId)
        {
            var profileInvoiceAccounts = GetCarrierProfileInvoiceAccounts(carrierProfileId);
            List<InvoiceAccountData> profileInvoiceAccountsData = new List<InvoiceAccountData>();
            foreach (var profileInvoiceAccount in profileInvoiceAccounts)
            {
                if (profileInvoiceAccount.InvoiceAccountId != invoiceAccountId)
                {
                    var invoiceType = invoiceTypes.FindRecord(x => x.InvoiceTypeId == profileInvoiceAccount.Settings.InvoiceTypeId);
                    InvoiceSettings invoiceSettings = invoiceType.Settings.ExtendedSettings as InvoiceSettings;
                    profileInvoiceAccountsData.Add(new InvoiceAccountData
                    {
                        InvoiceAccount = profileInvoiceAccount,
                        IsApplicableToSupplier = invoiceSettings.IsApplicableToSupplier,
                        IsApplicableToCustomer = invoiceSettings.IsApplicableToCustomer
                    });
                }
            }
            return profileInvoiceAccountsData;
        }
        private List<InvoiceAccountData> LoadInvoiceAccountsForCarrierAccount(int carrierAccountId, IEnumerable<InvoiceType> invoiceTypes, int invoiceAccountId)
        {
            var invoiceAccounts = GetInvoiceAccountsByCarrierAccountId(carrierAccountId);
            List<InvoiceAccountData> invoiceAccountsData = new List<InvoiceAccountData>();
            foreach (var invoiceAccount in invoiceAccounts)
            {
                if (invoiceAccount.InvoiceAccountId != invoiceAccountId)
                {
                    var invoiceType = invoiceTypes.FindRecord(x => x.InvoiceTypeId == invoiceAccount.Settings.InvoiceTypeId);
                    InvoiceSettings invoiceSettings = invoiceType.Settings.ExtendedSettings as InvoiceSettings;
                    invoiceAccountsData.Add(new InvoiceAccountData
                    {
                        InvoiceAccount = invoiceAccount,
                        IsApplicableToSupplier = invoiceSettings.IsApplicableToSupplier,
                        IsApplicableToCustomer = invoiceSettings.IsApplicableToCustomer
                    });
                }

            }
            return invoiceAccountsData;
        }

        #endregion

        #endregion


       
        #endregion
    }
}
