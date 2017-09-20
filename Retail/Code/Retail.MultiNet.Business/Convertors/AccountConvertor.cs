using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Retail.BusinessEntity.Business;
using Retail.BusinessEntity.Entities;
using Retail.BusinessEntity.MainExtensions.AccountParts;
using Vanrise.BEBridge.Entities;
using Vanrise.Common;
using Vanrise.Common.Business;
using Vanrise.Entities;

namespace Retail.MultiNet.Business.Convertors
{
    public class AccountConvertor : TargetBEConvertor
    {
        #region Properties

        static AccountBEManager s_accountBEManager = new AccountBEManager();
        static FinancialAccountManager s_financialAccountManager = new FinancialAccountManager();

        public override string Name
        {
            get
            {
                return "MultiNet GP Branch Convertor";
            }
        }
        public Guid AccountBEDefinitionId { get; set; }
        public Guid AccountTypeId { get; set; }
        public Guid CompanyProfilePartDefinitionId { get; set; }
        public Guid AccountInfoPartDefinitionId { get; set; }

        #region Account Columns
        public string AccountIdColumnName { get; set; }
        public string CustomerIdColumnName { get; set; }
        public string AccountNameColumnName { get; set; }
        public string AccountStatusColumnName { get; set; }
        public string RegistrationColumnName { get; set; }
        public string CNICColumnName { get; set; }
        public string CNICExpiryDateColumnName { get; set; }
        public string CurrencyIdColumnName { get; set; }
        public string NTNColumnName { get; set; }
        public string DueDateColumnName { get; set; }
        public string BillingPeriodColumnName { get; set; }
        public string AccountContractID { get; set; }


        #endregion

        #region Address Columns
        public string CountryColumnName { get; set; }
        public string CityColumnName { get; set; }
        public string StreetColumnName { get; set; }
        public string TownColumnName { get; set; }
        public string WebsiteColumnName { get; set; }
        public string AddressColumnName { get; set; }
        public string POBoxColumnName { get; set; }

        #endregion

        #region Contact Columns
        public string PhoneColumnName { get; set; }
        public string MobileColumnName { get; set; }
        public string FaxColumnName { get; set; }

        public string MainContactSalutaionColumnName { get; set; }
        public string MainContactNameTitleColumnName { get; set; }
        public string MainContactNameColumnName { get; set; }
        public string MainContactEmailColumnName { get; set; }
        public string MainContactPhoneColumnName { get; set; }
        public string MainContactReligionColumnName { get; set; }

        public string FinanceContactSalutaionColumnName { get; set; }
        public string FinanceContactNameTitleColumnName { get; set; }
        public string FinanceContactNameColumnName { get; set; }
        public string FinanceContactEmailColumnName { get; set; }
        public string FinanceContactPhoneColumnName { get; set; }
        public string FinanceContactReligionColumnName { get; set; }

        public string TechnicalContactSalutaionColumnName { get; set; }
        public string TechnicalContactNameTitleColumnName { get; set; }
        public string TechnicalContactNameColumnName { get; set; }
        public string TechnicalContactEmailColumnName { get; set; }
        public string TechnicalContactPhoneColumnName { get; set; }
        public string TechnicalContactReligionColumnName { get; set; }

        #endregion

        public Guid FinancialPartDefinitionId { get; set; }
        public Guid FinancialAccountDefinitionId { get; set; }
        public int CreditClassId { get; set; }

        #endregion

        #region Cstr
        public AccountConvertor()
        {
            this.FinancialAccountDefinitionId = new Guid("f7bf25bd-1f11-404e-94ac-fd41817f8607");
            this.CreditClassId = 1;
        }

        #endregion

        #region Public Methods
        public override void Initialize(ITargetBEConvertorInitializeContext context)
        {

            context.InitializationData = new AccountsInitializationData()
            {
                Accounts = new AccountBEManager().GetCachedAccountsBySourceId(this.AccountBEDefinitionId)
            };
        }
        public override void ConvertSourceBEs(ITargetBEConvertorConvertSourceBEsContext context)
        {
            SqlSourceBatch sourceBatch = context.SourceBEBatch as SqlSourceBatch;
            Dictionary<string, ITargetBE> maultiNetAccounts = new Dictionary<string, ITargetBE>();
            var accountsInitializationData = context.InitializationData as AccountsInitializationData;
            DataTable accountsDataTable = sourceBatch.Data;

            sourceBatch.Data.DefaultView.Sort = AccountIdColumnName;

            foreach (DataRow row in accountsDataTable.Rows)
            {
                ITargetBE targetMultiNetAccount;
                var sourceId = (row[AccountIdColumnName] as string).Trim();
                var parentId = string.Format("Customer_{0}", (row[CustomerIdColumnName] as string).Trim());
                string accountName = row[AccountNameColumnName] as string;
                if (!maultiNetAccounts.TryGetValue(sourceId, out targetMultiNetAccount))
                {
                    try
                    {
                        SourceAccountData accountData = new SourceAccountData
                        {
                            Account = new Account()
                        };

                        Account parentAccount;
                        if (accountsInitializationData.Accounts.TryGetValue(parentId, out parentAccount))
                        {
                            accountData.Account.ParentAccountId = parentAccount.AccountId;
                        }
                        bool isActive = (bool)row[AccountStatusColumnName];
                        accountData.Account.Name = accountName;
                        accountData.Account.SourceId = string.Format("Account_{0}", sourceId);
                        accountData.Account.TypeId = this.AccountTypeId;
                        Guid statusId = new Guid("dadc2977-a348-4504-89c9-c92f8f9008dd");// isActive ? new Guid("dadc2977-a348-4504-89c9-c92f8f9008dd") : new Guid("80b7dc84-5c43-47fc-b921-2ea717c9bdbf");
                        accountData.Account.StatusId = statusId;

                        accountData.Account.Settings = new AccountSettings
                        {
                            Parts = new AccountPartCollection()
                        };


                        FillBranchInfo(accountData, row);
                        FillExtendedInfo(accountData, row, sourceId);
                        FillFinancialInfo(accountData, row);

                        CreateFinancialAccount(accountData.Account, row);

                        maultiNetAccounts.Add(sourceId, accountData);
                    }
                    catch (Exception ex)
                    {
                        var finalException = Utilities.WrapException(ex, String.Format("Failed to import Account (Id: '{0}' Name: '{1}') due to conversion error", sourceId, accountName));
                        context.WriteBusinessHandledException(finalException);
                    }
                }
            }
            context.TargetBEs = maultiNetAccounts.Values.ToList();
        }
        public override void MergeTargetBEs(ITargetBEConvertorMergeTargetBEsContext context)
        {
            SourceAccountData existingBe = context.ExistingBE as SourceAccountData;
            SourceAccountData newBe = context.NewBE as SourceAccountData;

            SourceAccountData finalBe = new SourceAccountData
            {
                Account = GetAccount(newBe.Account, existingBe.Account)
            };

            context.FinalBE = finalBe;
        }

        #endregion

        #region Private Methods
        void FillBranchInfo(SourceAccountData accountData, DataRow row)
        {
            CountryManager countryManager = new CountryManager();
            string countryName = (row[CountryColumnName] as string);

            var country = string.IsNullOrEmpty(countryName) ? null : countryManager.GetCountry(countryName.Trim());
            City city = null;
            if (country != null)
            {
                CityManager cityManager = new CityManager();
                string cityName = (row[CityColumnName] as string);
                city = string.IsNullOrEmpty(cityName) ? null : cityManager.GetCityByName(country.CountryId, cityName);
            }
            AccountPartCompanyProfile settings = new AccountPartCompanyProfile
                {
                    Contacts = GetContactsList(row),
                    CityId = city != null ? city.CityId : (int?)null,
                    CountryId = country != null ? (int?)country.CountryId : null,
                    POBox = (row[POBoxColumnName] as string).Trim(),
                    Address = (row[AddressColumnName] as string).Trim(),
                    Street = (row[StreetColumnName] as string).Trim(),
                    Town = (row[TownColumnName] as string).Trim(),
                    Website = (row[WebsiteColumnName] as string).Trim(),
                    Faxes = GetNumbersList(row[FaxColumnName] as string),
                    MobileNumbers = GetNumbersList(row[MobileColumnName] as string),
                    PhoneNumbers = GetNumbersList(row[PhoneColumnName] as string)

                };


            AccountPart part = new AccountPart
            {
                Settings = settings
            };

            accountData.Account.Settings.Parts.Add(this.CompanyProfilePartDefinitionId, part);
        }
        private List<string> GetNumbersList(string numbers)
        {
            if (string.IsNullOrEmpty(numbers))
                return null;
            return numbers.Trim().Split(',').ToList();
        }
        void FillExtendedInfo(SourceAccountData accountData, DataRow row, string sourceId)
        {
            MultiNetBranchExtendedInfo settings = new MultiNetBranchExtendedInfo
            {
                RegistrationNumber = (row[RegistrationColumnName] as string).Trim(),
                CNIC = (row[CNICColumnName] as string).Trim(),
                CNICExpiryDate = (DateTime)row[CNICExpiryDateColumnName],
                NTN = (row[NTNColumnName] as string).Trim(),
                BillingPeriod = row[BillingPeriodColumnName] == DBNull.Value ? 0 : (int)row[BillingPeriodColumnName],
                DueDate = row[DueDateColumnName] == DBNull.Value ? default(DateTime?) : (DateTime)row[DueDateColumnName],
                GPSiteId = sourceId
            };

            AccountPart part = new AccountPart
            {
                Settings = settings
            };

            accountData.Account.Settings.Parts.Add(this.AccountInfoPartDefinitionId, part);
        }
        void FillFinancialInfo(SourceAccountData accountData, DataRow row)
        {
            string currencySourceId = (row[CurrencyIdColumnName] as string).Trim();
            CurrencyManager currencyManager = new CurrencyManager();
            Currency currency = currencyManager.GetCurrencyBySymbol(currencySourceId);
            currency.ThrowIfNull("currency", currencySourceId);

            accountData.Account.Settings.Parts.Add(this.FinancialPartDefinitionId, new AccountPart
            {
                Settings = new AccountPartFinancial
                {
                    CurrencyId = currency != null ? currency.CurrencyId : 0
                }
            });
        }
        Dictionary<string, AccountCompanyContact> GetContactsList(DataRow row)
        {
            Dictionary<string, AccountCompanyContact> contacts = new Dictionary<string, AccountCompanyContact>();

            contacts.Add("Main", new AccountCompanyContact
            {
                Email = row[MainContactEmailColumnName] as string,
                PhoneNumbers = new List<string> { row[MainContactPhoneColumnName] as string },
                ContactName = row[MainContactNameColumnName] as string,
                Notes = row[MainContactReligionColumnName] as string,
                Title = row[MainContactNameTitleColumnName] as string,
                Salutation = GetSalutation(row[MainContactSalutaionColumnName] as string)
            });

            contacts.Add("Technical", new AccountCompanyContact
            {
                Email = row[TechnicalContactEmailColumnName] as string,
                PhoneNumbers = new List<string> { row[TechnicalContactPhoneColumnName] as string },
                ContactName = row[TechnicalContactNameColumnName] as string,
                Notes = row[TechnicalContactReligionColumnName] as string,
                Title = row[TechnicalContactNameTitleColumnName] as string,
                Salutation = GetSalutation(row[TechnicalContactSalutaionColumnName] as string)
            });

            contacts.Add("Financial", new AccountCompanyContact
            {
                Email = row[FinanceContactEmailColumnName] as string,
                PhoneNumbers = new List<string> { row[FinanceContactPhoneColumnName] as string },
                ContactName = row[FinanceContactNameColumnName] as string,
                Notes = row[FinanceContactReligionColumnName] as string,
                Title = row[FinanceContactNameTitleColumnName] as string,
                Salutation = GetSalutation(row[FinanceContactSalutaionColumnName] as string)
            });
            return contacts;
        }
        private SalutationType? GetSalutation(string salutation)
        {
            switch (salutation)
            {
                case "Mr":
                    return SalutationType.Mr;
                case "Miss":
                    return SalutationType.Miss;
                case "Mrs":
                    return SalutationType.Mrs;
                case "Dr":
                    return SalutationType.Dr;
                default:
                    return null;
            }
        }
        Account GetAccount(Account newAccount, Account existingAccount)
        {
            Account account = Serializer.Deserialize<Account>(Serializer.Serialize(existingAccount));
            account.SourceId = newAccount.SourceId;


            return account;
        }
        void CreateFinancialAccount(Account account, DataRow row)
        {
            if (row[DueDateColumnName] != DBNull.Value)
            {
                DateTime bed = (DateTime)row[DueDateColumnName];
                FinancialAccount financialAccount = new FinancialAccount
                {
                    FinancialAccountDefinitionId = this.FinancialAccountDefinitionId,
                    ExtendedSettings = new Retail.BusinessEntity.MainExtensions.FinancialAccount.PostpaidFinancialAccount { CreditClassId = this.CreditClassId },
                    BED = bed
                };
                AccountBEFinancialAccountsSettings accountFinancialAccountExtSettings = s_accountBEManager.GetExtendedSettings<AccountBEFinancialAccountsSettings>(account);
                if (accountFinancialAccountExtSettings == null)
                    accountFinancialAccountExtSettings = s_financialAccountManager.CreateAccountFinancialAccountExtSettings();
                s_financialAccountManager.AddFinancialAccountToExtSettings(financialAccount, accountFinancialAccountExtSettings);
                s_accountBEManager.SetExtendedSettings(accountFinancialAccountExtSettings, account);
            }
        }

        #endregion
    }
}
