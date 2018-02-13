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
    public class CustomerConvertor : TargetBEConvertor
    {
        #region Properties

        static AccountBEManager s_accountBEManager = new AccountBEManager();
        static FinancialAccountManager s_financialAccountManager = new FinancialAccountManager();
        static CountryManager s_countryManager = new CountryManager();
        static CityManager s_cityManager = new CityManager();
        static CurrencyManager s_currencyManager = new CurrencyManager();
                

        public override string Name
        {
            get
            {
                return "MultiNet GP Company Convertor";
            }
        }
        public Guid AccountBEDefinitionId { get; set; }
        public Guid AccountTypeId { get; set; }
        public Guid CompanyProfilePartDefinitionId { get; set; }
        public Guid AccountInfoPartDefinitionId { get; set; }

        #region Account Columns
        public string CustomerIdColumnName { get; set; }
        public string AccountNameColumnName { get; set; }
        public string AccountStatusColumnName { get; set; }
        public string CNICColumnName { get; set; }
        public string CNICExpiryDateColumnName { get; set; }
        public string CurrencyIdColumnName { get; set; }
        public string NTNColumnName { get; set; }
        public string DueDateColumnName { get; set; }
        public string BillingPeriodColumnName { get; set; }
        public string IsExcludedFromTaxColumnName { get; set; }
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
        public CustomerConvertor()
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

            foreach (DataRow row in accountsDataTable.Rows)
            {
                ITargetBE targetMultiNetAccount;
                var sourceId = GetStringRowValue(row, CustomerIdColumnName);

                string accountName = GetStringRowValue(row, AccountNameColumnName);
                if (!maultiNetAccounts.TryGetValue(sourceId, out targetMultiNetAccount))
                {
                    try
                    {
                        SourceAccountData accountData = new SourceAccountData
                        {
                            Account = new Account()
                        };

                        accountData.Account.Name = accountName;
                        accountData.Account.SourceId = string.Format("Company_GP_{0}", sourceId);
                        accountData.Account.TypeId = this.AccountTypeId;
                        //Guid statusId = new Guid("dadc2977-a348-4504-89c9-c92f8f9008dd"); //isActive ? new Guid("dadc2977-a348-4504-89c9-c92f8f9008dd") : new Guid("80b7dc84-5c43-47fc-b921-2ea717c9bdbf");
                        //accountData.Account.StatusId = statusId;

                        accountData.Account.Settings = new AccountSettings
                        {
                            Parts = new AccountPartCollection()
                        };


                        FillProfileInfo(accountData, row);
                        FillExtendedInfo(accountData, row, sourceId);
                        FillFinancialInfo(accountData, row);

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
        void FillProfileInfo(SourceAccountData accountData, DataRow row)
        {

            
            string countryName = GetStringRowValue(row, CountryColumnName);

            var country = string.IsNullOrEmpty(countryName) ? null : s_countryManager.GetCountry(countryName);
            City city = null;
            if (country != null)
            {
                string cityName = GetStringRowValue(row, CityColumnName);
                city = string.IsNullOrEmpty(cityName) ? null : s_cityManager.GetCityByName(country.CountryId, cityName);
            }

            AccountPartCompanyProfile settings = new AccountPartCompanyProfile
                {
                    Contacts = GetContactsList(row),
                    CityId = city != null ? city.CityId : (int?)null,
                    CountryId = country != null ? country.CountryId : (int?)null,
                    POBox = GetStringRowValue(row, POBoxColumnName),
                    Address = GetStringRowValue(row, AddressColumnName),
                    Street = GetStringRowValue(row, StreetColumnName),
                    Town = GetStringRowValue(row, TownColumnName),
                    Website = GetStringRowValue(row, WebsiteColumnName),
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
            MultiNetCompanyExtendedInfo settings = new MultiNetCompanyExtendedInfo
            {
                CNIC = GetStringRowValue(row, CNICColumnName),
                CNICExpiryDate = row[CNICExpiryDateColumnName] == DBNull.Value ? default(DateTime?) : (DateTime)row[CNICExpiryDateColumnName],
                NTN = GetStringRowValue(row, NTNColumnName),
                BillingPeriod = row[BillingPeriodColumnName] == DBNull.Value ? 0 : (int)row[BillingPeriodColumnName],
                DueDate = row[DueDateColumnName] == DBNull.Value ? default(DateTime?) : (DateTime)row[DueDateColumnName],
                GPSiteID = sourceId
            };

            AccountPart part = new AccountPart
            {
                Settings = settings
            };

            accountData.Account.Settings.Parts.Add(this.AccountInfoPartDefinitionId, part);
        }
        void FillFinancialInfo(SourceAccountData accountData, DataRow row)
        {
            string currencySourceId = GetStringRowValue(row, CurrencyIdColumnName);
            Currency currency = s_currencyManager.GetCurrencyBySymbol(currencySourceId);
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
                Email = GetStringRowValue(row, MainContactEmailColumnName),
                PhoneNumbers = new List<string> { GetStringRowValue(row, MainContactPhoneColumnName) },
                ContactName = GetStringRowValue(row, MainContactNameColumnName),
                Notes = GetStringRowValue(row, MainContactReligionColumnName),
                Title = GetStringRowValue(row, MainContactNameTitleColumnName),
                Salutation = GetSalutation(row[MainContactSalutaionColumnName] as string)
            });

            contacts.Add("Technical", new AccountCompanyContact
            {
                Email = GetStringRowValue(row, TechnicalContactEmailColumnName),
                PhoneNumbers = new List<string> { GetStringRowValue(row, TechnicalContactPhoneColumnName) },
                ContactName = GetStringRowValue(row, TechnicalContactNameColumnName),
                Notes = GetStringRowValue(row, TechnicalContactReligionColumnName),
                Title = GetStringRowValue(row, TechnicalContactNameTitleColumnName),
                Salutation = GetSalutation(row[TechnicalContactSalutaionColumnName] as string)
            });

            contacts.Add("Financial", new AccountCompanyContact
            {
                Email = GetStringRowValue(row, FinanceContactEmailColumnName),
                PhoneNumbers = new List<string> { GetStringRowValue(row, FinanceContactPhoneColumnName) },
                ContactName = GetStringRowValue(row, FinanceContactNameColumnName),
                Notes = GetStringRowValue(row, FinanceContactReligionColumnName),
                Title = GetStringRowValue(row, FinanceContactNameTitleColumnName),
                Salutation = GetSalutation(row[FinanceContactSalutaionColumnName] as string)
            });
            return contacts;
        }
        private SalutationType? GetSalutation(string salutation)
        {
            if (salutation == null)
                return null;
            switch (salutation.Trim())
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

        private string GetStringRowValue(DataRow row, string fieldName)
        {
            var value = row[fieldName] as string;
            return value != null ? value.Trim() : null;
        }

        #endregion
    }
}