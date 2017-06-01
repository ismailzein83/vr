using NP.IVSwitch.Data;
using NP.IVSwitch.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Vanrise.Entities;
using Vanrise.Common;
using TOne.WhS.BusinessEntity.Business;
using TOne.WhS.BusinessEntity.Entities;
using Vanrise.GenericData.Entities;



namespace NP.IVSwitch.Business
{
    public class AccountManager
    {
        #region Public Methods

        public Account GetAccountInfoFromProfile(CarrierProfile carrierProfile, bool customer)
        {
            AccountType accountType = customer ? AccountType.Customer : AccountType.Vendor;
            Account account = new Account
            {
                FirstName = carrierProfile.Settings.Company,
                LastName = carrierProfile.Settings.Company,
                CompanyName = carrierProfile.Settings.Company,
                ContactDisplay = carrierProfile.Settings.Company,
                LogAlias = carrierProfile.Settings.Company,
                WebSite = carrierProfile.Settings.Website,
                Email = Guid.NewGuid() + "@guid.com",
                Address = carrierProfile.Settings.Address,
                TypeId = accountType,
                CurrentState = State.Active,
                BillingCycle = 1,
                TaxGroupId = 1,
                PaymentTerms = 3,
                CreditLimit = -1
            };
            return account;
        }

        public void UpdateChannelLimit(int accountId)
        {
            IAccountDataManager dataManager = IVSwitchDataManagerFactory.GetDataManager<IAccountDataManager>();
            Helper.SetSwitchConfig(dataManager);
            dataManager.UpdateCustomerChannelLimit(accountId);
        }
        public int AddAccount(Account accountItem)
        {
            IAccountDataManager dataManager = IVSwitchDataManagerFactory.GetDataManager<IAccountDataManager>();
            Helper.SetSwitchConfig(dataManager);
            int accountId;
            bool succInsert = dataManager.Insert(accountItem, out  accountId);
            if (succInsert)
            {
                ICDRDataManager cdrDataManager = IVSwitchDataManagerFactory.GetDataManager<ICDRDataManager>();
                Helper.SetSwitchConfig(cdrDataManager);
                cdrDataManager.InsertHelperUser(accountId, accountItem.LogAlias);
            }

            return accountId;
        }

        public string GetEndPointSwitchAccountName(int endPointId)
        {
            return GetSwitchAccountNamesByEndPointId().GetRecord(endPointId);
        }


        public string GetRouteSwitchAccountName(int routeId)
        {
            return GetSwitchAccountNamesByRouteId().GetRecord(routeId);
        }

        public int? GetCarrierAccountSWCustomerAccountId(int carrierAccountId)
        {
            AccountCarrierProfileExtension carrierProfileExtension = GetCarrierProfileExtension(carrierAccountId);
            if (carrierProfileExtension != null)
                return carrierProfileExtension.CustomerAccountId;
            else
                return default(int?);
        }

        public int? GetCarrierAccountSWSupplierAccountId(int carrierAccountId)
        {
            AccountCarrierProfileExtension carrierProfileExtension = GetCarrierProfileExtension(carrierAccountId);
            if (carrierProfileExtension != null)
                return carrierProfileExtension.VendorAccountId;
            else
                return default(int?);
        }

        private AccountCarrierProfileExtension GetCarrierProfileExtension(int carrierAccountId)
        {
            int? carrierProfileId = new CarrierAccountManager().GetCarrierProfileId(carrierAccountId);
            if (!carrierProfileId.HasValue)
                throw new NullReferenceException(String.Format("carrierProfileId. '{0}'", carrierAccountId));
            AccountCarrierProfileExtension carrierProfileExtension = new CarrierProfileManager().GetExtendedSettings<AccountCarrierProfileExtension>(carrierProfileId.Value);
            return carrierProfileExtension;
        }

        public List<int> GetAllAssignedSWCustomerAccountIds()
        {
            List<int> assignedSWCustomerAccountIds = new List<int>();
            CarrierProfileManager carrierProfileManager = new CarrierProfileManager();
            var allProfiles = carrierProfileManager.GetCachedCarrierProfiles();
            if (allProfiles != null)
            {
                foreach(var profile in allProfiles.Values)
                {
                    AccountCarrierProfileExtension accountCarrierProfileExtension =
                carrierProfileManager.GetExtendedSettings<AccountCarrierProfileExtension>(profile);
                    if (accountCarrierProfileExtension != null && accountCarrierProfileExtension.CustomerAccountId.HasValue)
                    {
                        assignedSWCustomerAccountIds.Add(accountCarrierProfileExtension.CustomerAccountId.Value);
                    }
                }
            }
            return assignedSWCustomerAccountIds;
        }

        public List<int> GetAllAssignedSWVendorAccountIds()
        {
            List<int> assignedSWVendorAccountIds = new List<int>();
            CarrierProfileManager carrierProfileManager = new CarrierProfileManager();
            var allProfiles = carrierProfileManager.GetCachedCarrierProfiles();
            if (allProfiles != null)
            {
                foreach (var profile in allProfiles.Values)
                {
                    AccountCarrierProfileExtension accountCarrierProfileExtension =
                carrierProfileManager.GetExtendedSettings<AccountCarrierProfileExtension>(profile);
                    if (accountCarrierProfileExtension != null && accountCarrierProfileExtension.VendorAccountId.HasValue)
                    {
                        assignedSWVendorAccountIds.Add(accountCarrierProfileExtension.VendorAccountId.Value);
                    }
                }
            }
            return assignedSWVendorAccountIds;
        }

        public void TrySetSWCustomerAccountId(int carrierAccountId, int swCustomerAccountId)
        {
            CarrierAccountManager carrierAccountManager = new CarrierAccountManager();

            int? carrierProfileId = carrierAccountManager.GetCarrierProfileId(carrierAccountId);
            if (!carrierProfileId.HasValue)
                throw new NullReferenceException(String.Format("carrierProfileId. '{0}'", carrierAccountId));
            CarrierProfileManager carrierProfileManager = new CarrierProfileManager();
            AccountCarrierProfileExtension accountCarrierProfileExtension =
                carrierProfileManager.GetExtendedSettings<AccountCarrierProfileExtension>(carrierProfileId.Value);
            if (accountCarrierProfileExtension == null || !accountCarrierProfileExtension.CustomerAccountId.HasValue)
            {
                if (accountCarrierProfileExtension == null)
                    accountCarrierProfileExtension = new AccountCarrierProfileExtension();
                accountCarrierProfileExtension.CustomerAccountId = swCustomerAccountId;
                carrierProfileManager.UpdateCarrierProfileExtendedSetting(carrierProfileId.Value, accountCarrierProfileExtension);
            }
            else if (accountCarrierProfileExtension.CustomerAccountId.Value != swCustomerAccountId)
                throw new Exception(String.Format("assigned SWCustomerAccountId '{0}' is different than endpoints AccountId '{1}'", accountCarrierProfileExtension.CustomerAccountId.Value, swCustomerAccountId));
        }

        public void TrySetSWVendorAccountId(int carrierAccountId, int swVendorAccountId)
        {
            CarrierAccountManager carrierAccountManager = new CarrierAccountManager();

            int? carrierProfileId = carrierAccountManager.GetCarrierProfileId(carrierAccountId);
            if (!carrierProfileId.HasValue)
                throw new NullReferenceException(String.Format("carrierProfileId. '{0}'", carrierAccountId));
            CarrierProfileManager carrierProfileManager = new CarrierProfileManager();
            AccountCarrierProfileExtension accountCarrierProfileExtension =
                carrierProfileManager.GetExtendedSettings<AccountCarrierProfileExtension>(carrierProfileId.Value);
            if (accountCarrierProfileExtension == null || !accountCarrierProfileExtension.VendorAccountId.HasValue)
            {
                if (accountCarrierProfileExtension == null)
                    accountCarrierProfileExtension = new AccountCarrierProfileExtension();
                accountCarrierProfileExtension.VendorAccountId = swVendorAccountId;
                carrierProfileManager.UpdateCarrierProfileExtendedSetting(carrierProfileId.Value, accountCarrierProfileExtension);
            }
            else if (accountCarrierProfileExtension.VendorAccountId.Value != swVendorAccountId)
                throw new Exception(String.Format("assigned SWVendorAccountId '{0}' is different than route AccountId '{1}'", accountCarrierProfileExtension.VendorAccountId.Value, swVendorAccountId));
        }

        #endregion

        #region Private

        private class CacheManager : Vanrise.Caching.BaseCacheManager
        {
            protected override bool IsTimeExpirable { get { return true; } }
        }

        private Dictionary<int,string> GetSwitchAccountNamesByEndPointId()
        {
            return Vanrise.Caching.CacheManagerFactory.GetCacheManager<CacheManager>().GetOrCreateObject("GetSwitchAccountNamesByEndPointId", () =>
                {
                    IAccountDataManager dataManager = IVSwitchDataManagerFactory.GetDataManager<IAccountDataManager>();
                    Helper.SetSwitchConfig(dataManager);
                    return dataManager.GetSwitchAccountNamesByEndPointId();
                });
        }

        private Dictionary<int, string> GetSwitchAccountNamesByRouteId()
        {
            return Vanrise.Caching.CacheManagerFactory.GetCacheManager<CacheManager>().GetOrCreateObject("GetSwitchAccountNamesByRouteId", () =>
            {
                IAccountDataManager dataManager = IVSwitchDataManagerFactory.GetDataManager<IAccountDataManager>();
                Helper.SetSwitchConfig(dataManager);
                return dataManager.GetSwitchAccountNamesByRouteId();
            });
        }

        #endregion

    }
}
