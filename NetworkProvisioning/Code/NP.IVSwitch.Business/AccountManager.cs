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
             //CarrierAccountType { Exchange = 1, Supplier = 2, Customer = 3 }
             //   AccountType {Vendor = 1, Customer = 2}

            AccountType accountType= customer == true ? AccountType.Customer : AccountType.Vendor;

            Account account = new Account
            {
                FirstName = carrierProfile.Settings.Company,
                LastName = carrierProfile.Settings.Company,
                CompanyName = carrierProfile.Settings.Company,
                ContactDisplay = carrierProfile.Settings.Company,
                LogAlias = carrierProfile.Settings.Company,
                WebSite = carrierProfile.Settings.Website,
                Email = Guid.NewGuid().ToString() + "@guid.com",
                TypeId = accountType
            };

            // carrierProfile.Settings.Contacts.Find(x => x.Type.ToString().Equals("TechnicalEmail")).Description; //technical contact
            return account;
        }
      

        public int AddAccount(Account accountItem)
        {
            IAccountDataManager dataManager = IVSwitchDataManagerFactory.GetDataManager<IAccountDataManager>();
            Helper.SetSwitchConfig(dataManager);
            int accountId = -1; 
            dataManager.Insert(accountItem,out  accountId);

            return accountId;
        }

        

        #endregion

      
    }
}
