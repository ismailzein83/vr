﻿using NP.IVSwitch.Data;
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
