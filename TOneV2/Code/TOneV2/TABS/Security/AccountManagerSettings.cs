using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Xml.Linq;

namespace TABS.Security
{
    public class AccountManagerSettings : Interfaces.ICachedCollectionContainer
    {
        /// <summary>
        /// Needed for marker interface to be called by reflection
        /// </summary>
        public static void ClearCachedCollections()
        {
            __All = null;
            _All = null;
            TABS.Components.CacheProvider.Clear(typeof(AccountManagerSettings).FullName);
        }

        //New
        public List<AMU_Customer> DirectCustomers { get; set; }
        public List<AMU_Customer> InDirectCustomers
        {
            get
            {
                return GetIndirectCustomers();
            }
        }
        //End New

        protected List<AMU_Customer> GetIndirectCustomers()
        {
            List<AMU_Customer> customers = new List<AMU_Customer>();
            foreach (var amu in SubAccountManagers)
            {
                var accSettings = new AccountManagerSettings(amu, "AccountManagerSettings");
                foreach (var customer in accSettings.DirectCustomers)
                    customers.Add(customer);
                List<AMU_Customer> _customers = accSettings.GetIndirectCustomers();
                foreach (var c in _customers)
                    customers.Add(c);
            }
            return customers;
        }

        public SecurityEssentials.User User { get; set; }

        public Dictionary<TABS.CarrierAccount, DateTime> Customers
        {
            get { return DirectCustomers.Union(InDirectCustomers).ToDictionary(c => c.Customer, m => m.AssignDate); }
        }
        public Dictionary<TABS.CarrierAccount, DateTime> Suppliers { get; set; }
        public List<SecurityEssentials.User> SubAccountManagers { get; set; }
        //test account manager
        public SecurityEssentials.User AccountManager { get; set; }
        //end test account manager
        //public int PricingTemplate { get; set; }
        public PricingTemplate PricingTemplate { get; set; }

        public int Level { get; set; }

        public TABS.ActionType ActionType { get; set; }

        public AccountManagerSettings(SecurityEssentials.User user, string settingTypeType)
        {
            User = user;
            string AccountManagerSettingsContent = user[settingTypeType].ToString();
            if (string.IsNullOrEmpty(AccountManagerSettingsContent)) user[settingTypeType] = DefaultXml;

            XDocument document = XDocument.Parse(user[settingTypeType].ToString(), LoadOptions.None);

            var elements = document.Elements().Elements().ToList();

            //Customers = new Dictionary<CarrierAccount, DateTime>();
            Suppliers = new Dictionary<CarrierAccount, DateTime>();

            DirectCustomers = new List<AMU_Customer>();
            //InDirectCustomers = new List<AMU_Customer>();

            SubAccountManagers = new List<SecurityEssentials.User>();

            elements[0].Elements().ToList().ForEach(e =>
            {
                string[] values = e.Value.Split(',');
                if (TABS.CarrierAccount.All.ContainsKey(values[0]))
                    //Customers.Add(TABS.CarrierAccount.All[values[0]], DateTime.Parse(values[1]));
                    DirectCustomers.Add(new AMU_Customer
                    {
                        Customer = TABS.CarrierAccount.All[values[0]],
                        AssignDate = DateTime.Parse(values[1]),
                        MCT = MCT.Get(int.Parse(values[2])),
                        Direct_AMU = this
                    });
            });

            elements[1].Elements().ToList().ForEach(e =>
            {
                string[] values = e.Value.Split(',');
                if (TABS.CarrierAccount.All.ContainsKey(values[0]))
                    Suppliers.Add(TABS.CarrierAccount.All[values[0]], DateTime.Parse(values[1]));
            });

            var usersByID = Security.User.All.ToDictionary(u => u.ID);

            elements[2].Elements().ToList().ForEach(e =>
            {
                if (usersByID.ContainsKey(int.Parse(e.Value)))
                    SubAccountManagers.Add(usersByID[int.Parse(e.Value)]);
            });

            if (elements.Count > 3)
            {
                if (elements[3].Value != "-1")
                    PricingTemplate = PricingTemplate.All[int.Parse(elements[3].Value)];
                else
                    PricingTemplate = null;
            }
            //test account manager
            if (elements.Count > 4)
                if (usersByID.ContainsKey(int.Parse(elements[4].Value)))
                    AccountManager = usersByID[int.Parse(elements[4].Value)];
            //end test account manager

            //test level
            if (elements.Count > 5)
                Level = int.Parse(elements[5].Value);
            //end test level

            //test Action type
            if (elements.Count > 6)
                ActionType = (TABS.ActionType)short.Parse(elements[6].Value);

        }

        /// <summary>
        /// intansiate a user settings  from user option and user
        /// </summary>
        /// <param name="settings">user option account manager settings</param>
        /// <param name="user">the user (account manager)</param>
        public AccountManagerSettings(SecurityEssentials.User user)
        {
            User = user;
            string AccountManagerSettingsContent = user["AccountManagerSettings"].ToString();
            if (string.IsNullOrEmpty(AccountManagerSettingsContent)) user["AccountManagerSettings"] = DefaultXml;

            XDocument document = XDocument.Parse(user["AccountManagerSettings"].ToString(), LoadOptions.None);

            var elements = document.Elements().Elements().ToList();

            //Customers = new Dictionary<CarrierAccount, DateTime>();
            Suppliers = new Dictionary<CarrierAccount, DateTime>();

            DirectCustomers = new List<AMU_Customer>();
            //InDirectCustomers = new List<AMU_Customer>();

            SubAccountManagers = new List<SecurityEssentials.User>();

            elements[0].Elements().ToList().ForEach(e =>
            {
                string[] values = e.Value.Split(',');
                if (TABS.CarrierAccount.All.ContainsKey(values[0]))
                {
                    //Customers.Add(TABS.CarrierAccount.All[values[0]], DateTime.Parse(values[1]));
                    DirectCustomers.Add(new AMU_Customer
                    {
                        Customer = TABS.CarrierAccount.All[values[0]],
                        AssignDate = DateTime.Parse(values[1]),
                        MCT = MCT.Get(int.Parse(values[2])),
                        Direct_AMU = this
                    });
                }
            });

            elements[1].Elements().ToList().ForEach(e =>
            {
                string[] values = e.Value.Split(',');
                if (TABS.CarrierAccount.All.ContainsKey(values[0]))
                    Suppliers.Add(TABS.CarrierAccount.All[values[0]], DateTime.Parse(values[1]));
            });

            var usersByID = Security.User.All.ToDictionary(u => u.ID);

            elements[2].Elements().ToList().ForEach(e =>
            {
                if (usersByID.ContainsKey(int.Parse(e.Value)))
                    SubAccountManagers.Add(usersByID[int.Parse(e.Value)]);
            });

            if (elements.Count > 3)
            {
                int param = -1;
                int.TryParse(elements[3].Value, out param);
                if (param != -1 && PricingTemplate.All.ContainsKey(param))
                    PricingTemplate = PricingTemplate.All[param];
                else
                    PricingTemplate = null;
            }
            //test account manager
            if (elements.Count > 4)
                if (usersByID.ContainsKey(int.Parse(elements[4].Value)))
                    AccountManager = usersByID[int.Parse(elements[4].Value)];
            //end test account manager

            //test level
            if (elements.Count > 5)
                Level = int.Parse(elements[5].Value);
            //end test level

            //test Action type
            if (elements.Count > 6)
                ActionType = (TABS.ActionType)short.Parse(elements[6].Value);

        }

        public AccountManagerSettings() { }


        public static bool Save(AccountManagerSettings accountSettings, string settingType, out Exception ex)
        {
            ex = null;
            bool success = true;

            StringBuilder document = new StringBuilder();

            try
            {
                //appending customers
                if (accountSettings.DirectCustomers != null && accountSettings.DirectCustomers.Count > 0)
                {
                    //document.Append(
                    //new XElement("Customers",
                    //   accountSettings.Customers.Select(c =>
                    //       new XElement("CarrierAccount", string.Concat(c.Key.CarrierAccountID, ",", c.Value.ToString("yyyy-MM-dd"))))).ToString());
                    document.Append(
                        new XElement("Customers",
                            accountSettings.DirectCustomers.Select(c =>
                                new XElement("CarrierAccount", string.Concat(c.Customer.CarrierAccountID, ",", c.AssignDate.ToString("yyyy-MM-dd"), ",", c.MCT.ID)))).ToString());

                }
                else
                {
                    document.Append(@"<Customers>
                                        <CarrierAccount>$NOTHING$,$FALSEDATE$,$NOTHING$</CarrierAccount>
                                      </Customers>");
                }

                if (accountSettings.Suppliers != null && accountSettings.Suppliers.Count > 0)
                {
                    document.Append(
                    new XElement("Suppliers",
                       accountSettings.Suppliers.Select(c =>
                           new XElement("CarrierAccount", string.Concat(c.Key.CarrierAccountID, ",", c.Value.ToString("yyyy-MM-dd"))))).ToString());

                }
                else
                {
                    document.Append(@"<Suppliers>
                                        <CarrierAccount>$CarrierAccountID$,$Date$</CarrierAccount>
                                     </Suppliers>");
                }

                if (accountSettings.SubAccountManagers != null && accountSettings.SubAccountManagers.Count > 0)
                {
                    document.Append(
                    new XElement("SubAccountManagers",
                       accountSettings.SubAccountManagers.Select(s =>
                           new XElement("AccountManager", s.ID))).ToString());

                }
                else
                {
                    document.Append(@"<SubAccountManagers>
                                        <AccountManager>-1</AccountManager>
                                      </SubAccountManagers>");
                }


                if (accountSettings.PricingTemplate != null)
                    document.Append(new XElement("PricingTemplate", accountSettings.PricingTemplate.PricingTemplateId));
                else
                    document.Append(@"<PricingTemplate>-1</PricingTemplate>");


                if (accountSettings.AccountManager != null)
                    document.Append(new XElement("Manager", accountSettings.AccountManager.ID));
                else
                    document.Append(@"<Manager>-1</Manager>");


                //test Level
                if (accountSettings.Level == -1)
                {
                    if (accountSettings.AccountManager != null)
                    {
                        AccountManagerSettings managerSettings = new AccountManagerSettings(accountSettings.AccountManager);
                        accountSettings.Level = managerSettings.Level + 1;
                    }
                    else
                        accountSettings.Level = 1;
                }
                if (accountSettings.Level == 0)
                {
                    if (accountSettings.AccountManager != null)
                    {
                        AccountManagerSettings managerSettings = new AccountManagerSettings(accountSettings.AccountManager);
                        accountSettings.Level = managerSettings.Level + 1;
                    }

                }
                document.Append(new XElement("Level", accountSettings.Level));

                //end Test Level

                //test ActionType
                if (accountSettings.ActionType != 0)
                    document.Append(new XElement("ActionType", (int)accountSettings.ActionType));
                else
                    document.Append(@"<ActionType>-1</ActionType>");
                //end test ActionType

                accountSettings.User[settingType] = string.Concat("<Accounts>", document.ToString(), "</Accounts>");

                TABS.ObjectAssembler.SaveOrUpdate(accountSettings.User, out ex);

            }
            catch (Exception exception)
            {
                ex = exception;
                success = false;
            }
            //_All = null;
            return success;
        }

        /// <summary>
        /// save for a specific user
        /// </summary>
        /// <param name="accountSettings"></param>
        /// <param name="user"></param>
        /// <param name="ex"></param>
        /// <returns></returns>
        public static bool Save(AccountManagerSettings accountSettings, out Exception ex)
        {
            ex = null;
            bool success = true;

            StringBuilder document = new StringBuilder();

            try
            {
                //appending customers
                if (accountSettings.DirectCustomers != null && accountSettings.DirectCustomers.Count > 0)
                {
                    document.Append(
                    new XElement("Customers",
                       accountSettings.DirectCustomers.Select(c =>
                           new XElement("CarrierAccount", string.Concat(c.Customer.CarrierAccountID, ",", c.AssignDate.ToString("yyyy-MM-dd"))))).ToString());

                }
                else
                {
                    document.Append(@"<Customers>
                                        <CarrierAccount>$NOTHING$,$FALSEDATE$</CarrierAccount>
                                      </Customers>");
                }

                if (accountSettings.Suppliers != null && accountSettings.Suppliers.Count > 0)
                {
                    document.Append(
                    new XElement("Suppliers",
                       accountSettings.Suppliers.Select(c =>
                           new XElement("CarrierAccount", string.Concat(c.Key.CarrierAccountID, ",", c.Value.ToString("yyyy-MM-dd"))))).ToString());

                }
                else
                {
                    document.Append(@"<Suppliers>
                                        <CarrierAccount>$CarrierAccountID$,$Date$</CarrierAccount>
                                     </Suppliers>");
                }

                if (accountSettings.SubAccountManagers != null && accountSettings.SubAccountManagers.Count > 0)
                {
                    document.Append(
                    new XElement("SubAccountManagers",
                       accountSettings.SubAccountManagers.Select(s =>
                           new XElement("AccountManager", s.ID))).ToString());

                }
                else
                {
                    document.Append(@"<SubAccountManagers>
                                        <AccountManager>-1</AccountManager>
                                      </SubAccountManagers>");
                }


                if (accountSettings.PricingTemplate != null)
                    document.Append(new XElement("PricingTemplate", accountSettings.PricingTemplate.PricingTemplateId));
                else
                    document.Append(@"<PricingTemplate>-1</PricingTemplate>");


                if (accountSettings.AccountManager != null)
                    document.Append(new XElement("Manager", accountSettings.AccountManager.ID));
                else
                    document.Append(@"<Manager>-1</Manager>");


                //test Level
                if (accountSettings.Level == -1)
                {
                    if (accountSettings.AccountManager != null)
                    {
                        AccountManagerSettings managerSettings = new AccountManagerSettings(accountSettings.AccountManager);
                        accountSettings.Level = managerSettings.Level + 1;
                    }
                    else
                        accountSettings.Level = 1;
                }
                if (accountSettings.Level == 0)
                {
                    if (accountSettings.AccountManager != null)
                    {
                        AccountManagerSettings managerSettings = new AccountManagerSettings(accountSettings.AccountManager);
                        accountSettings.Level = managerSettings.Level + 1;
                    }

                }
                document.Append(new XElement("Level", accountSettings.Level));

                //end Test Level

                //test ActionType
                if (accountSettings.ActionType != 0)
                    document.Append(new XElement("ActionType", (int)accountSettings.ActionType));
                else
                    document.Append(@"<ActionType>-1</ActionType>");
                //end test ActionType

                accountSettings.User["AccountManagerSettings"] = string.Concat("<Accounts>", document.ToString(), "</Accounts>");

                TABS.ObjectAssembler.SaveOrUpdate(accountSettings.User, out ex);

            }
            catch (Exception exception)
            {
                ex = exception;
                success = false;
            }
            //_All = null;
            return success;
        }


        internal static Dictionary<SecurityEssentials.User, AccountManagerSettings> _All;
        public static Dictionary<SecurityEssentials.User, AccountManagerSettings> All_
        {
            get
            {
                if (_All == null)
                {
                    _All = new Dictionary<SecurityEssentials.User, AccountManagerSettings>();

                    foreach (SecurityEssentials.User user in TABS.Security.User.All)
                    {
                        AccountManagerSettings setting = new AccountManagerSettings(user);
                        _All.Add(user, setting);
                    }
                }
                return _All;
            }
            set { _All = value; }
        }

        internal static Dictionary<string, Dictionary<SecurityEssentials.User, AccountManagerSettings>> __All;
        public static Dictionary<string, Dictionary<SecurityEssentials.User, AccountManagerSettings>> All
        {
            get
            {
                if (__All == null)
                {
                    __All = new Dictionary<string, Dictionary<SecurityEssentials.User, AccountManagerSettings>>();

                    foreach (string settingType in Enum.GetNames(typeof(TABS.Module)))
                    {
                        var SettingTypeValues = new Dictionary<SecurityEssentials.User, AccountManagerSettings>();
                        foreach (SecurityEssentials.User user in TABS.Security.User.All)
                        {
                            AccountManagerSettings setting = new AccountManagerSettings(user, settingType);
                            SettingTypeValues.Add(user, setting);
                            //__All.Add(settingType, 
                        }
                        __All.Add(settingType, SettingTypeValues);
                    }
                }
                return __All;
            }
            set { __All = value; }
        }

        internal static Dictionary<TABS.CarrierAccount, List<SecurityEssentials.User>> _CarrierAccountManagers;
        public static Dictionary<TABS.CarrierAccount, List<SecurityEssentials.User>> CarrierAccountManagers
        {
            get
            {
                _CarrierAccountManagers = new Dictionary<CarrierAccount, List<SecurityEssentials.User>>();

                // foreach (var setting in All.Values)
                //FOR TESTING
                foreach (var setting in All.Values.SelectMany(kvp => kvp.Values))
                //ENDTESTING
                {
                    var accounts = setting.Customers.Keys.Union(setting.Suppliers.Keys).ToList();

                    foreach (var account in accounts)
                    {
                        if (!_CarrierAccountManagers.ContainsKey(account)) _CarrierAccountManagers[account] = new List<SecurityEssentials.User>();

                        _CarrierAccountManagers[account].Add(setting.User);
                    }
                }

                return _CarrierAccountManagers;
            }

            set { _CarrierAccountManagers = value; }
        }

        public static string DefaultXml
        {
            get
            {
                return @"<Accounts>
                           <Customers>
                              <CarrierAccount>$CarrierAccountID$,$Date$,$MCT$</CarrierAccount>
                           </Customers>
                           <Suppliers>
                              <CarrierAccount>$CarrierAccountID$,$Date$</CarrierAccount>
                           </Suppliers>
                           <SubAccountManagers>
                              <AccountManager>-1</AccountManager>
                           </SubAccountManagers>
                           <PricingTemplate>-1</PricingTemplate> 
                           <Manager>-1</Manager>
                           <Level>-1</Level>
                           <ActionType>-1</ActionType>
                         </Accounts>";
            }
        }

        public static AccountManagerSettings CurrentUserAccountSettings
        {
            get
            {
                if (SecurityEssentials.Web.Helper.CurrentWebUser.HasRestriction)
                    return All_[SecurityEssentials.Web.Helper.CurrentWebUser];
                //FOR TESTING
                //return All.Values.ToDictionary;
                //ENDTESTING
                return null;
            }
        }

        public bool IsEmpty
        {
            get
            {
                return (this.Customers.Count == 0 && this.Suppliers.Count == 0 && this.SubAccountManagers.Count == 0);
            }
        }

        public static void DeleteAccountManagerSettings(SecurityEssentials.User user)
        {
            try
            {
                string sql = "DELETE FROM UserOption WHERE [Option] = 'ACCOUNTMANAGERSETTINGS' AND [User] = " + user.ID;
                DataHelper.ExecuteNonQuery(sql, null);
            }
            catch { }
        }

        public static void DeleteAccountManagerSettings(SecurityEssentials.User user, string settingType)
        {
            try
            {
                string sql = "DELETE FROM UserOption WHERE [Option] like '%" + settingType + "%' AND [User] = " + user.ID;
                DataHelper.ExecuteNonQuery(sql, null);
            }
            catch { }
        }

    }

    public class AMU_Customer
    {
        public CarrierAccount Customer { get; set; }
        public MCT MCT { get; set; }
        public AccountManagerSettings Direct_AMU { get; set; }
        public DateTime AssignDate { get; set; }
    }
}
