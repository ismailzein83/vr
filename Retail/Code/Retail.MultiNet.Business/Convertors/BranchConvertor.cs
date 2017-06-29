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
        public override string Name
        {
            get
            {
                return "MultiNet Branch Convertor";
            }
        }
        static AccountBEManager s_accountBEManager = new AccountBEManager();

        public Guid AccountBEDefinitionId { get; set; }
        public Guid AccountTypeId { get; set; }
        public Guid InitialStatusId { get; set; }
        public Guid CompanyProfilePartDefinitionId { get; set; }
        public Guid BranchInfoPartDefinitionId { get; set; }
        public string BranchIdColumnName { get; set; }
        public string CompanyIdColumnName { get; set; }

        public Guid FinancialPartDefinitionId { get; set; }

        public Guid FinancialAccountDefinitionId { get; set; }

        public int CreditClassId { get; set; }

        public BranchConvertor()
        {
            this.FinancialAccountDefinitionId = new Guid("f7bf25bd-1f11-404e-94ac-fd41817f8607");
            this.CreditClassId = 1;
        }

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
            sourceBatch.Data.DefaultView.Sort = BranchIdColumnName;
            foreach (DataRow row in sourceBatch.Data.Rows)
            {
                ITargetBE targetMultiNetAccount;
                var sourceId = (Int64)row[BranchIdColumnName];
                var parentId = string.Format("Company_{0}", (Int64)row[CompanyIdColumnName]);
                string accountName = row["AC_ACCTHOLDERNAME"] as string;
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
                        int state = (int)row["AS_ACCTSTATEID"];
                        accountData.Account.Name = accountName;
                        accountData.Account.CreatedTime = row["SU_INSERTDATE"] != DBNull.Value ? (DateTime)row["SU_INSERTDATE"] : default(DateTime);
                        accountData.Account.SourceId = string.Format("Branch_{0}", sourceId);
                        accountData.Account.TypeId = this.AccountTypeId;
                        Guid statusId = accountsInitializationData.Statuses.GetOrCreateItem(state, () => this.InitialStatusId);
                        accountData.Account.StatusId = statusId;

                        accountData.Account.Settings = new AccountSettings
                        {
                            Parts = new AccountPartCollection()
                        };

                        FillBranchInfo(accountData, row);
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
        static FinancialAccountManager s_financialAccountManager = new FinancialAccountManager();
        private void CreateFinancialAccount(Account account, DataRow row)
        {
            if (row["AC_ACTIVATIONDATE"] != DBNull.Value)
            {
                DateTime bed = (DateTime)row["AC_ACTIVATIONDATE"];
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

        void FillBranchInfo(SourceAccountData accountData, DataRow row)
        {
            CityManager cityManager = new CityManager();
            City city = cityManager.GetCityBySourceId(((int)row["CI_CITYID"]).ToString());

            accountData.Account.Settings.Parts.Add(this.CompanyProfilePartDefinitionId, new AccountPart
            {
                Settings = new AccountPartCompanyProfile
                {
                    Contacts = GetContactsList(row),
                    CityId = city != null ? city.CityId : (int?)null,
                    CountryId = city != null ? city.CountryId : (int?)null
                }
            });

            accountData.Account.Settings.Parts.Add(this.BranchInfoPartDefinitionId, new AccountPart
            {
                Settings = new MultiNetBranchExtendedInfo
                {
                    BranchCode = row["AC_BRANCHCODE"] as string,
                    ContractReferenceNumber = row["AC_CONTRACTREFNO"] as string
                }
            });

            FillFinancialInfo(accountData, row);
        }

        void FillFinancialInfo(SourceAccountData accountData, DataRow row)
        {
            CurrencyManager currencyManager = new CurrencyManager();
            Currency currency = currencyManager.GetCurrencyBySourceId(((int)row["C_CURRENCYID"]).ToString());

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
                Email = row["AC_EMAIL"] as string,
            });

            return contacts;
        }
        Account GetAccount(Account newAccount, Account existingAccount)
        {
            Account account = Serializer.Deserialize<Account>(Serializer.Serialize(existingAccount));
            account.SourceId = newAccount.SourceId;


            return account;
        }
    }

    public class AccountsInitializationData
    {
        public Dictionary<int, Guid> Statuses { get; set; }
        public Dictionary<string, Account> Accounts { get; set; }
    }
}