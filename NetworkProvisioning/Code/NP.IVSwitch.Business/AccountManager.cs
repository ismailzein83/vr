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

            AccountType accountType;
            if (customer == true)
                accountType = AccountType.Customer;
            else
                accountType = AccountType.Vendor;

            Account account = new Account();

            account.FirstName = carrierProfile.Settings.Company;
            account.LastName = carrierProfile.Settings.Company;
            account.CompanyName = carrierProfile.Settings.Company;
            account.ContactDisplay = carrierProfile.Settings.Company;
            account.LogAlias = carrierProfile.Settings.Company;
            account.WebSite = carrierProfile.Settings.Website;
            account.Email = Guid.NewGuid().ToString() + "@guid.com";   // carrierProfile.Settings.Contacts.Find(x => x.Type.ToString().Equals("TechnicalEmail")).Description; //technical contact
            account.TypeId = accountType;




            return account;
        }
      

        public int AddAccount(Account accountItem)
        {
            IAccountDataManager dataManager = IVSwitchDataManagerFactory.GetDataManager<IAccountDataManager>();

            int accountId = -1; 
            dataManager.Insert(accountItem,out  accountId);

            return accountId;
        }

        

        #endregion

      
    }
}
