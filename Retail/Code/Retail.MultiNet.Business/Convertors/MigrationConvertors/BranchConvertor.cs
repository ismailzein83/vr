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
    public class BranchConvertor : TargetBEConvertor
    {
        #region Properties

        static AccountBEManager s_accountBEManager = new AccountBEManager();
        static FinancialAccountManager s_financialAccountManager = new FinancialAccountManager();

        public override string Name
        {
            get
            {
                return "MultiNet Branch Convertor";
            }
        }
        public Guid AccountBEDefinitionId { get; set; }
        public Guid AccountTypeId { get; set; }
        public Guid InitialStatusId { get; set; }
        public Guid CompanyProfilePartDefinitionId { get; set; }
        public Guid BranchInfoPartDefinitionId { get; set; }

        #region Branche Columns 
        public string BranchIdColumnName { get; set; }
        public string CompanyIdColumnName { get; set; }
        public string AccountHolderColumnName { get; set; }
        public string AccountStateColumnName { get; set; }
        public string InsertDateColumnName { get; set; }
        public string BranchCodeColumnName { get; set; }
        public string ContractRefNoColumnName { get; set;}
        public string CurrencyIdColumnName { get; set; }
        public string EmailColumnName { get; set; }
        public string ActivationDateColumnName { get; set; }

        #endregion

        #region Address Columns
        public string SmaOwnerIdColumnName { get; set; }
        public string SmaAddressColumnName { get; set; }
        public string AtTypeIdColumnName { get; set; }

        #endregion


        #region Phone Numbers Columns
        public string SmpOwnerIdColumnName { get; set; }
        public string PhoneTypeColumnName { get; set; }
        public string SmpPhoneNumberColumnName { get; set; }
       
        #endregion

        #region Identity Columns
        public string IdentityIdColumnName { get; set; }
        public string SmniOwnerIdColumnName { get; set; }
        public string SmniValueColumnName { get; set; }

        #endregion

        public Guid FinancialPartDefinitionId { get; set; }
        public Guid FinancialAccountDefinitionId { get; set; }
        public int CreditClassId { get; set; }

        #endregion

        #region Cstr
        public BranchConvertor()
        {
            this.FinancialAccountDefinitionId = new Guid("f7bf25bd-1f11-404e-94ac-fd41817f8607");
            this.CreditClassId = 1;
        }

        #endregion

        #region Public Methods
        public override void Initialize(ITargetBEConvertorInitializeContext context)
        {
            Dictionary<int, Guid> statuses = new Dictionary<int, Guid>();

            statuses.Add(15, new Guid("80b7dc84-5c43-47fc-b921-2ea717c9bdbf"));
            statuses.Add(2, new Guid("dadc2977-a348-4504-89c9-c92f8f9008dd"));
            statuses.Add(4, new Guid("f8388386-0011-4133-a64b-5247a480ef5e"));
            statuses.Add(13, new Guid("931d3708-1911-4ff1-8e83-feef8115930f"));
            statuses.Add(0, new Guid("7378cdbb-f5b4-452c-8b61-e09997e4dca1"));

            context.InitializationData = new AccountsInitializationData()
            {
                Accounts = new AccountBEManager().GetCachedAccountsBySourceId(this.AccountBEDefinitionId),
                Statuses = statuses
            };
        }
        public override void ConvertSourceBEs(ITargetBEConvertorConvertSourceBEsContext context)
        {
            SqlSourceBatch sourceBatch = context.SourceBEBatch as SqlSourceBatch;
            Dictionary<Int64, ITargetBE> maultiNetAccounts = new Dictionary<Int64, ITargetBE>();
            var accountsInitializationData = context.InitializationData as AccountsInitializationData;
            DataSet accountsDataSet = sourceBatch.DataSet;

            sourceBatch.DataSet.Tables[0].DefaultView.Sort = BranchIdColumnName;

            Dictionary<long, List<BranchAddress>> addresses = GetAddresses(accountsDataSet.Tables[1]);
            Dictionary<long, Dictionary<NumberType, List<AccountNumber>>> numbers = GetNumbers(accountsDataSet.Tables[2]);
            Dictionary<long, List<Identity>> identities = GetIdentities(accountsDataSet.Tables[3]);

            foreach (DataRow row in accountsDataSet.Tables[0].Rows)
            {
                ITargetBE targetMultiNetAccount;
                var sourceId = (Int64)row[BranchIdColumnName];
                var parentId = string.Format("Company_{0}", (Int64)row[CompanyIdColumnName]);
                string accountName = row[AccountHolderColumnName] as string;
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
                        int state = (int)row[AccountStateColumnName];
                        accountData.Account.Name = accountName;
                        accountData.Account.CreatedTime = row[InsertDateColumnName] != DBNull.Value ? (DateTime)row[InsertDateColumnName] : default(DateTime);
                        accountData.Account.SourceId = string.Format("Branch_{0}", sourceId);
                        accountData.Account.TypeId = this.AccountTypeId;
                        Guid statusId = accountsInitializationData.Statuses.GetOrCreateItem(state, () => this.InitialStatusId);
                        accountData.Account.StatusId = statusId;

                        accountData.Account.Settings = new AccountSettings
                        {
                            Parts = new AccountPartCollection()
                        };

                        List<BranchAddress> lstAddresses;
                        addresses.TryGetValue(sourceId, out lstAddresses);

                        Dictionary<NumberType, List<AccountNumber>> dicNumbers;
                        numbers.TryGetValue(sourceId, out dicNumbers);

                        List<Identity> dicIdentities;
                        identities.TryGetValue(sourceId, out dicIdentities);

                        FillBranchInfo(accountData, row, dicNumbers);
                        FillExtendedInfo(accountData, row, lstAddresses, dicIdentities);
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
        void FillBranchInfo(SourceAccountData accountData, DataRow row, Dictionary<NumberType, List<AccountNumber>> numbers)
        {
            CityManager cityManager = new CityManager();
            City city = cityManager.GetCityBySourceId(((int)row["CI_CITYID"]).ToString());
            AccountPartCompanyProfile settings = new AccountPartCompanyProfile
                {
                    Contacts = GetContactsList(row),
                    CityId = city != null ? city.CityId : (int?)null,
                    CountryId = city != null ? city.CountryId : (int?)null,
                };
            FillBranchInfoSettingNumbers(numbers, settings);

            AccountPart part = new AccountPart
            {
                Settings = settings
            };

            accountData.Account.Settings.Parts.Add(this.CompanyProfilePartDefinitionId, part);
        }
        void FillExtendedInfo(SourceAccountData accountData, DataRow row, List<BranchAddress> addresses, List<Identity> identities)
        {
            MultiNetBranchExtendedInfo settings = new MultiNetBranchExtendedInfo
            {
                BranchCode = row[BranchCodeColumnName] as string,
                ContractReferenceNumber = row[ContractRefNoColumnName] as string
            };

            FillBranchExtendedSettingAddresses(addresses, settings);
            FillBranchExtendedInfoIdentities(identities, settings);

            AccountPart part = new AccountPart
            {
                Settings = settings
            };

            accountData.Account.Settings.Parts.Add(this.BranchInfoPartDefinitionId, part);
        }
        void FillFinancialInfo(SourceAccountData accountData, DataRow row)
        {
            CurrencyManager currencyManager = new CurrencyManager();
            Currency currency = currencyManager.GetCurrencyBySourceId(((int)row[CurrencyIdColumnName]).ToString());

            accountData.Account.Settings.Parts.Add(this.FinancialPartDefinitionId, new AccountPart
            {
                Settings = new AccountPartFinancial
                {
                    CurrencyId = currency != null ? currency.CurrencyId : 0
                }
            });
        }
        void FillBranchExtendedInfoIdentities(List<Identity> dicIdentities, MultiNetBranchExtendedInfo settings)
        {
            if (dicIdentities != null)
            {
                foreach (var identity in dicIdentities)
                {
                    switch (identity.Type)
                    {
                        case IdentityType.CNIC:
                            settings.CNIC = identity.Value;
                            break;
                        case IdentityType.RegNo:
                            settings.RegistrationNumber = identity.Value;
                            break;
                        case IdentityType.NTN:
                            settings.NTN = identity.Value;
                            break;
                        case IdentityType.STR:
                            break;
                        case IdentityType.Passport:
                            settings.PassportNumber = identity.Value;
                            break;
                        case IdentityType.NTN1:
                            break;
                        case IdentityType.PIN:
                            settings.PIN = identity.Value;
                            break;
                        case IdentityType.NUMBER:
                            break;
                        case IdentityType.REFNO:
                            settings.RefNumber = identity.Value;
                            break;
                    }
                }
            }
        }
        void FillBranchExtendedSettingAddresses(List<BranchAddress> addresses, MultiNetBranchExtendedInfo settings)
        {
            if (addresses != null)
            {
                foreach (var address in addresses)
                {
                    switch (address.Type)
                    {
                        case AddressType.Office:
                            settings.OfficeAddress = address.Address;
                            break;
                        case AddressType.Technical:
                            settings.TechnicalAddress = address.Address;
                            break;
                        case AddressType.Home:
                            settings.HomeAddress = address.Address;
                            break;
                        case AddressType.Billing:
                            settings.BillingAddress = address.Address;
                            break;
                    }
                }
            }
        }
        void FillBranchInfoSettingNumbers(Dictionary<NumberType, List<AccountNumber>> numbers, AccountPartCompanyProfile settings)
        {

            if (numbers != null)
            {
                foreach (var number in numbers)
                {
                    switch (number.Key)
                    {
                        case NumberType.LandLine:
                            settings.PhoneNumbers = number.Value.Select(n => n.Number).ToList();
                            break;
                        case NumberType.Mobile:
                            settings.MobileNumbers = number.Value.Select(n => n.Number).ToList();
                            break;
                        case NumberType.Fax:
                            settings.Faxes = number.Value.Select(n => n.Number).ToList();
                            break;
                    }
                }
            }
        }
        Dictionary<string, AccountCompanyContact> GetContactsList(DataRow row)
        {
            Dictionary<string, AccountCompanyContact> contacts = new Dictionary<string, AccountCompanyContact>();

            contacts.Add("Main", new AccountCompanyContact
            {
                Email = row[EmailColumnName] as string,
            });

            return contacts;
        }
        Account GetAccount(Account newAccount, Account existingAccount)
        {
            Account account = Serializer.Deserialize<Account>(Serializer.Serialize(existingAccount));
            account.SourceId = newAccount.SourceId;


            return account;
        }
        Dictionary<long, Dictionary<NumberType, List<AccountNumber>>> GetNumbers(DataTable dataTable)
        {
            Dictionary<long, Dictionary<NumberType, List<AccountNumber>>> numbers = new Dictionary<long, Dictionary<NumberType, List<AccountNumber>>>();

            foreach (DataRow row in dataTable.Rows)
            {
                int branchId = (int)row[SmpOwnerIdColumnName];
                NumberType numberType = (NumberType)row[PhoneTypeColumnName];
                Dictionary<NumberType, List<AccountNumber>> dicNumbers = numbers.GetOrCreateItem(branchId, () => new Dictionary<NumberType, List<AccountNumber>>());
                List<AccountNumber> lstnumbers = dicNumbers.GetOrCreateItem(numberType, () => new List<AccountNumber>());
                lstnumbers.Add(new AccountNumber
                {
                    Number = row[SmpPhoneNumberColumnName] as string,
                    BranchId = branchId,
                    Type = numberType
                });
            }
            return numbers;
        }
        Dictionary<long, List<BranchAddress>> GetAddresses(DataTable dataTable)
        {
            Dictionary<long, List<BranchAddress>> addresses = new Dictionary<long, List<BranchAddress>>();
            foreach (DataRow row in dataTable.Rows)
            {
                int branchId = (int)row[SmaOwnerIdColumnName];
                List<BranchAddress> lstAddresses = addresses.GetOrCreateItem(branchId, () => new List<BranchAddress>());
                lstAddresses.Add(new BranchAddress
                {
                    Address = row[SmaAddressColumnName] as string,
                    BranchId = branchId,
                    Type = (AddressType)row[AtTypeIdColumnName],
                    CityId = row["CI_CITYID"] == DBNull.Value ? null : (int?)row["CI_CITYID"]
                });
            }

            return addresses;
        }
        Dictionary<long, List<Identity>> GetIdentities(DataTable dataTable)
        {
            Dictionary<long, List<Identity>> identities = new Dictionary<long, List<Identity>>();

            foreach (DataRow row in dataTable.Rows)
            {
                IdentityType identityType = (IdentityType)row[IdentityIdColumnName];
                int branchId = (int)row[SmniOwnerIdColumnName];
                List<Identity> lstIdentities = identities.GetOrCreateItem(branchId, () => new List<Identity>());

                lstIdentities.Add(new Identity
                {
                    Value = row[SmniValueColumnName] as string,
                    BranchId = branchId,
                    Type = identityType
                });
            }

            return identities;
        }
        void CreateFinancialAccount(Account account, DataRow row)
        {
            if (row[ActivationDateColumnName] != DBNull.Value)
            {
                DateTime bed = (DateTime)row[ActivationDateColumnName];
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

    #region Classes

    public class AccountsInitializationData
    {
        public Dictionary<int, Guid> Statuses { get; set; }
        public Dictionary<string, Account> Accounts { get; set; }
    }

    public class BranchAddress
    {
        public long BranchId { get; set; }
        public string Address { get; set; }
        public int? CityId { get; set; }
        public AddressType Type { get; set; }

    }

    public class AccountNumber
    {
        public long BranchId { get; set; }
        public NumberType Type { get; set; }
        public string Number { get; set; }
    }

    public class Identity
    {
        public long BranchId { get; set; }
        public string Value { get; set; }
        public IdentityType Type { get; set; }

    }

    public enum AddressType
    {
        Office = 1,
        Technical = 24,
        Home = 43,
        Billing = 21
    }

    public enum NumberType
    {
        LandLine = 1,
        Mobile = 2,
        Fax = 3
    }

    public enum IdentityType
    {
        CNIC = 1,
        RegNo = 2,
        NTN = 3,
        STR = 21,
        Passport = 41,
        NTN1 = 61,
        PIN = 62,
        NUMBER = 102,
        REFNO = 103
    }
    #endregion

}