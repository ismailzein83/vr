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
                        throw new NullReferenceException(string.Format("invoiceAccount.CarrierAccountId & invoiceAccount.CarrierProfileId for financial Account Id: {0}", invoiceAccount.InvoiceAccountId));

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
                      throw new NullReferenceException(string.Format("invoiceAccount.CarrierAccountId & invoiceAccount.CarrierProfileId for financial Account Id: {0}", invoiceAccount.InvoiceAccountId));

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
        private bool CheckIsAllowToAddInvoiceAccount(InvoiceAccount invoiceAccount, bool isEditMode, out string message)
        {
            message = null;
            if (!CheckInvoiceAccountActivation(invoiceAccount))
            {
                message = "Financial account is inactive.";
                return false;
            }
            return true;
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
        private bool CheckInvoiceAccountActivation(InvoiceAccount invoiceAccount)
        {
            return CheckActivationForCarrier(invoiceAccount.CarrierAccountId, invoiceAccount.CarrierProfileId);
        }
        private bool CheckActivationForCarrier(int? carrierAccountId, int? carrierProfileId)
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
               isActive = CheckProfileCarrierAccountsActivation(carrierProfileId.Value, true, true);
            }
            return isActive;
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

        #endregion
    }
}
