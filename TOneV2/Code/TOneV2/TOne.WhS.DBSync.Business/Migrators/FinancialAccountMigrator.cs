using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TOne.WhS.BusinessEntity.Entities;
using TOne.WhS.BusinessEntity.MainExtensions.FinancialAccountTypes.CustomerPostpaid;
using TOne.WhS.BusinessEntity.MainExtensions.FinancialAccountTypes.CustomerPrepaid;
using TOne.WhS.BusinessEntity.MainExtensions.FinancialAccountTypes.Netting;
using TOne.WhS.BusinessEntity.MainExtensions.FinancialAccountTypes.SupplierPostpaid;
using TOne.WhS.BusinessEntity.MainExtensions.FinancialAccountTypes.SupplierPrepaid;
using TOne.WhS.DBSync.Data.SQL;
using TOne.WhS.DBSync.Entities;
using Vanrise.Common;

namespace TOne.WhS.DBSync.Business
{
    public class FinancialAccountMigrator : Migrator<SourceCarrierProfileWithAccounts, WHSFinancialAccount>
    {
        readonly Dictionary<string, CarrierAccount> _allCarrierAccounts;
        readonly Dictionary<string, CarrierProfile> _allCarrierProfiles;
        FinancialAccountDBSyncDataManager _dbSynchDataManager;
        public FinancialAccountMigrator(MigrationContext context)
            : base(context)
        {
            var dbTableCarrierAccount = Context.DBTables[DBTableName.CarrierAccount];
            _allCarrierAccounts = (Dictionary<string, CarrierAccount>)dbTableCarrierAccount.Records;

            var dbTableCarrierProfile = Context.DBTables[DBTableName.CarrierProfile];
            _allCarrierProfiles = (Dictionary<string, CarrierProfile>)dbTableCarrierProfile.Records;

            _dbSynchDataManager = new FinancialAccountDBSyncDataManager(Context.UseTempTables);
            TableName = _dbSynchDataManager.GetTableName();
        }

        public override void FillTableInfo(bool useTempTables)
        {

        }

        public override void AddItems(List<WHSFinancialAccount> itemsToAdd)
        {
            _dbSynchDataManager.ApplyFinancialAccountsToTemp(itemsToAdd, 1);
        }
        public override IEnumerable<SourceCarrierProfileWithAccounts> GetSourceItems()
        {
            SourceCarrierProfileDataManager dataManager = new SourceCarrierProfileDataManager(Context.ConnectionString);
            return dataManager.GetSourceCarrierProfilesWithFinancialData();
        }
        public override WHSFinancialAccount BuildItemFromSource(SourceCarrierProfileWithAccounts sourceItem)
        {
            throw new NotImplementedException();
        }
        public override List<WHSFinancialAccount> BuildAllItemsFromSource(IEnumerable<SourceCarrierProfileWithAccounts> sourceItems)
        {
            List<WHSFinancialAccount> result = new List<WHSFinancialAccount>();

            Dictionary<string, SourceCarrierProfileWithAccounts> profilesBySourceId = GetGroupedSourceCarrierProfiles(sourceItems);

            foreach (var profileItem in profilesBySourceId)
            {
                bool hasCustomerFinancialAccount = false;
                bool hasSupplierFinancialAccount = false;
                string profileId = profileItem.Key;
                SourceCarrierProfileWithAccounts profile = profileItem.Value;

                CarrierProfile carrierProfile = _allCarrierProfiles[profileId];

                if (!profile.IsNettingEnabled)
                {
                    HashSet<string> nettingAccounts = new HashSet<string>();

                    if (profile.CustomerPaymentType == SourcePaymentType.Undefined)
                    {
                        #region Customers Financial Accounts

                        foreach (var customer in profile.Customers)
                        {
                            CarrierAccount carrierAccount = _allCarrierAccounts[customer.SourceId];
                            if (carrierAccount != null)
                            {
                                WHSFinancialAccount customerFinancialAccount = GetFinancialAccount(customer.CustomerActivateDate, customer.CustomerDeactivateDate, carrierAccount.CarrierAccountId, AccountType.Carrier);

                                if (customer.CustomerPaymentType == SourcePaymentType.Defined_By_Profile)
                                    continue;
                                hasCustomerFinancialAccount = true;
                                if (customer.IsNettingEnabled)
                                {
                                    hasSupplierFinancialAccount = true;
                                    customerFinancialAccount.Settings.ExtendedSettings = new NettingSettings
                                     {
                                         CustomerCreditLimit = GetCreditLimit(customer.CustomerCreditLimit),
                                         SupplierCreditLimit = GetCreditLimit(customer.SupplierCreditLimit)
                                     };
                                    customerFinancialAccount.FinancialAccountDefinitionId = new Guid("16661e6b-f227-4a8a-a5f5-50ccc52cc15b");
                                    result.Add(customerFinancialAccount);
                                    nettingAccounts.Add(customer.SourceId);
                                }
                                else
                                {
                                    UpdateFinancialSettings(customerFinancialAccount, SourceAccountType.Client, customer.CustomerPaymentType, customer.CustomerCreditLimit);
                                    result.Add(customerFinancialAccount);
                                }
                            }
                        }
                    }
                    else if (profile.Customers.Count > 0)
                    {
                        hasCustomerFinancialAccount = true;
                        WHSFinancialAccount customerFinancialAccount = GetFinancialAccount(profile.CustomerActivateDate, profile.CustomerDeactivateDate, carrierProfile.CarrierProfileId, AccountType.Profile);
                        UpdateFinancialSettings(customerFinancialAccount, SourceAccountType.Client, profile.CustomerPaymentType, profile.CustomerCreditLimit);
                        result.Add(customerFinancialAccount);
                    }

                        #endregion

                    #region Suppliers Financial Accounts

                    if (profile.SupplierPaymentType == SourcePaymentType.Undefined)
                    {
                        foreach (var supplier in profile.Suppliers)
                        {
                            CarrierAccount carrierAccount = _allCarrierAccounts[supplier.SourceId];
                            if (carrierAccount != null)
                            {


                                if (supplier.SupplierPaymentType == SourcePaymentType.Defined_By_Profile || nettingAccounts.Contains(supplier.SourceId))
                                    continue;

                                hasSupplierFinancialAccount = true;
                                WHSFinancialAccount supplierFinancialAccount = GetFinancialAccount(supplier.SupplierActivateDate, supplier.SupplierDeactivateDate, carrierAccount.CarrierAccountId, AccountType.Carrier);
                                UpdateFinancialSettings(supplierFinancialAccount, SourceAccountType.Termination, supplier.SupplierPaymentType, supplier.SupplierCreditLimit);
                                result.Add(supplierFinancialAccount);
                            }
                        }
                    }
                    else if (profile.Suppliers.Count > 0)
                    {
                        hasSupplierFinancialAccount = true;
                        WHSFinancialAccount supplierFinancialAccount = GetFinancialAccount(profile.SupplierActivateDate, profile.SupplierDeactivateDate, carrierProfile.CarrierProfileId, AccountType.Profile);
                        UpdateFinancialSettings(supplierFinancialAccount, SourceAccountType.Termination, profile.SupplierPaymentType, profile.SupplierCreditLimit);
                        result.Add(supplierFinancialAccount);
                    }
                    #endregion
                }

                #region Netting Accounts
                else
                {
                    WHSFinancialAccount financialAccount = new WHSFinancialAccount
                    {
                        BED = GetDate(profile.CustomerActivateDate),
                        CarrierProfileId = carrierProfile.CarrierProfileId,
                        FinancialAccountDefinitionId = new Guid("16661e6b-f227-4a8a-a5f5-50ccc52cc15b"),
                        EED = profile.CustomerDeactivateDate,
                        Settings = new WHSFinancialAccountSettings
                        {
                            ExtendedSettings = new NettingSettings
                            {
                                CustomerCreditLimit = GetCreditLimit(profile.CustomerCreditLimit),
                                SupplierCreditLimit = GetCreditLimit(profile.SupplierCreditLimit)
                            }
                        }
                    };
                    result.Add(financialAccount);
                    hasSupplierFinancialAccount = true;
                    hasCustomerFinancialAccount = true;

                }
                #endregion

                #region No Financial Account Found. Create by Checking Profile By Invoice Param

                if (!hasCustomerFinancialAccount && profile.Customers.Count > 0)
                {
                    if (profile.InvoiceByProfile)
                    {
                        WHSFinancialAccount customerFinancialAccount = GetFinancialAccount(profile.CustomerActivateDate, profile.CustomerDeactivateDate, carrierProfile.CarrierProfileId, AccountType.Profile);
                        UpdateFinancialSettings(customerFinancialAccount, SourceAccountType.Client, profile.CustomerPaymentType, profile.CustomerCreditLimit);
                        result.Add(customerFinancialAccount);
                    }
                    else
                    {
                        foreach (var customer in profile.Customers)
                        {
                            CarrierAccount carrierAccount = _allCarrierAccounts[customer.SourceId];
                            WHSFinancialAccount customerFinancialAccount = GetFinancialAccount(customer.CustomerActivateDate, customer.CustomerDeactivateDate, carrierAccount.CarrierAccountId, AccountType.Carrier);
                            UpdateFinancialSettings(customerFinancialAccount, SourceAccountType.Client, customer.CustomerPaymentType, customer.CustomerCreditLimit);
                            result.Add(customerFinancialAccount);
                        }
                    }
                }

                if (!hasSupplierFinancialAccount && profile.Suppliers.Count > 0)
                {
                    if (profile.InvoiceByProfile)
                    {
                        WHSFinancialAccount supplierFinancialAccount = GetFinancialAccount(profile.SupplierActivateDate, profile.SupplierDeactivateDate, carrierProfile.CarrierProfileId, AccountType.Profile);
                        UpdateFinancialSettings(supplierFinancialAccount, SourceAccountType.Termination, profile.SupplierPaymentType, profile.SupplierCreditLimit);
                        result.Add(supplierFinancialAccount);
                    }
                    else
                    {
                        foreach (var supplier in profile.Suppliers)
                        {
                            CarrierAccount carrierAccount = _allCarrierAccounts[supplier.SourceId];
                            WHSFinancialAccount supplierFinancialAccount = GetFinancialAccount(supplier.CustomerActivateDate, supplier.CustomerDeactivateDate, carrierAccount.CarrierAccountId, AccountType.Carrier);
                            UpdateFinancialSettings(supplierFinancialAccount, SourceAccountType.Client, supplier.CustomerPaymentType, supplier.CustomerCreditLimit);
                            result.Add(supplierFinancialAccount);
                        }
                    }
                }

                #endregion
            }

            TotalRowsSuccess += result.Count;

            return result;
        }

        void UpdateFinancialSettings(WHSFinancialAccount financialAccount, SourceAccountType accountType, SourcePaymentType paymentType, int? creditLimit)
        {
            WHSFinancialAccountExtendedSettings extendedSettings = null;

            switch (accountType)
            {
                case SourceAccountType.Client:
                    switch (paymentType)
                    {
                        case SourcePaymentType.Undefined:
                        case SourcePaymentType.Postpaid:
                            financialAccount.FinancialAccountDefinitionId = new Guid("89bc46ef-28f0-43ac-9f4a-d3f9c2ea2ef1");
                            extendedSettings = new CustomerPostpaidSettings
                            {
                                CreditLimit = GetCreditLimit(creditLimit)
                            };
                            break;
                        case SourcePaymentType.Prepaid:
                            financialAccount.FinancialAccountDefinitionId = new Guid("ca290901-8259-4a2d-82af-1b5fefb5e40d");
                            extendedSettings = new CustomerPrepaidSettings();
                            break;
                    }
                    break;
                case SourceAccountType.Termination:
                    switch (paymentType)
                    {
                        case SourcePaymentType.Undefined:
                        case SourcePaymentType.Postpaid:
                            financialAccount.FinancialAccountDefinitionId = new Guid("b3d286b2-c5a3-4a57-898d-1c1f95a30a25");
                            extendedSettings = new SupplierPostpaidSettings
                            {
                                CreditLimit = GetCreditLimit(creditLimit)
                            };
                            break;
                        case SourcePaymentType.Prepaid:
                            financialAccount.FinancialAccountDefinitionId = new Guid("1ef3ba16-149e-49eb-95cb-908f0d8a26ca");
                            extendedSettings = new SupplierPrepaidSettings();
                            break;
                    }
                    break;
            }

            financialAccount.Settings.ExtendedSettings = extendedSettings;
        }
        Dictionary<string, SourceCarrierProfileWithAccounts> GetGroupedSourceCarrierProfiles(IEnumerable<SourceCarrierProfileWithAccounts> sourceItems)
        {
            Dictionary<string, SourceCarrierProfileWithAccounts> profilesBySourceId = new Dictionary<string, SourceCarrierProfileWithAccounts>();

            SourceCarrierAccountDataManager dataManager = new SourceCarrierAccountDataManager(Context.ConnectionString);
            IEnumerable<SourceCarrierAccount> carrierAccounts = dataManager.GetSourceCarrierAccounts();

            foreach (var sourceProfile in sourceItems)
            {
                CarrierProfile carrierProfile;
                if (!_allCarrierProfiles.TryGetValue(sourceProfile.SourceId, out carrierProfile))
                {
                    TotalRowsFailed++;
                    continue;
                }

                SourceCarrierProfileWithAccounts carrierProfileWithAccounts = profilesBySourceId.GetOrCreateItem(sourceProfile.SourceId, () => sourceProfile);

                ///Get Accounts Without Deleted ones
                IEnumerable<SourceCarrierAccount> accounts = carrierAccounts.FindAllRecords(c => c.ProfileId.ToString() == sourceProfile.SourceId && !c.IsDeleted);

                foreach (var account in accounts)
                {
                    CarrierAccount carrierAccount = GetCarrierAccount(account);

                    if (carrierAccount != null && account.ActivationStatus != SourceActivationStatus.Inactive)
                    {

                        switch (account.AccountType)
                        {
                            case SourceAccountType.Client:
                                carrierProfileWithAccounts.Customers.Add(account);
                                break;
                            case SourceAccountType.Exchange:
                                carrierProfileWithAccounts.Suppliers.Add(account);
                                carrierProfileWithAccounts.Customers.Add(account);
                                break;
                            case SourceAccountType.Termination:
                                carrierProfileWithAccounts.Suppliers.Add(account);
                                break;
                        }
                    }
                }
            }
            return profilesBySourceId;
        }

        CarrierAccount GetCarrierAccount(SourceCarrierAccount supplier)
        {
            CarrierAccount carrierAccount;
            if (!_allCarrierAccounts.TryGetValue(supplier.SourceId, out carrierAccount))
            {
                TotalRowsFailed++;
                Context.WriteWarning(string.Format("Financial Account not created for Carrier Account, Source Id {0}", supplier.SourceId));

            }
            return carrierAccount;
        }

        WHSFinancialAccount GetFinancialAccount(DateTime? activationDate, DateTime? deactivationDate, int carrierId, AccountType accountType)
        {
            WHSFinancialAccount financialAccount = new WHSFinancialAccount
               {

                   BED = GetDate(activationDate),
                   EED = deactivationDate,
                   Settings = new WHSFinancialAccountSettings()
               };
            switch (accountType)
            {
                case AccountType.Profile:
                    financialAccount.CarrierProfileId = carrierId;
                    break;
                case AccountType.Carrier:
                    financialAccount.CarrierAccountId = carrierId;
                    break;
            }
            return financialAccount;
        }

        WHSFinancialAccount GetNettingAccount(SourceCarrierAccount customer, CarrierAccount carrierAccount)
        {
            WHSFinancialAccount nettingAccount = new WHSFinancialAccount
            {
                BED = GetDate(customer.CustomerActivateDate),
                EED = customer.CustomerDeactivateDate,
                CarrierAccountId = carrierAccount.CarrierAccountId,
                Settings = new WHSFinancialAccountSettings
                {
                    ExtendedSettings = new NettingSettings
                    {
                        CustomerCreditLimit = GetCreditLimit(customer.CustomerCreditLimit),
                        SupplierCreditLimit = GetCreditLimit(customer.SupplierCreditLimit)
                    }
                }
            };
            return nettingAccount;
        }

        decimal GetCreditLimit(int? creditLimit)
        {
            return creditLimit.HasValue ? creditLimit.Value : 0;
        }

        DateTime GetDate(DateTime? date)
        {
            return date.HasValue ? date.Value : new DateTime(2000, 1, 1);
        }
        public override bool IsBuildAllItemsOnce
        {
            get
            {
                return true;
            }
        }
    }

    enum AccountType
    {
        Profile,
        Carrier
    }
}
