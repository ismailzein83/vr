using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Retail.BusinessEntity.Entities;
using Retail.BusinessEntity.MainExtensions.AccountParts;
using Vanrise.BEBridge.Entities;
using Vanrise.Common;
using Vanrise.Common.Business;
using Vanrise.Entities;

namespace Retail.MultiNet.Business.Convertors
{
    public class CompanyConvertor : TargetBEConvertor
    {
        public override string Name
        {
            get
            {
                return "MultiNet Company Convertor";
            }
        }
        public Guid AccountBEDefinitionId { get; set; }
        public Guid AccountTypeId { get; set; }
        public Guid InitialStatusId { get; set; }
        public Guid CompanyProfilePartDefinitionId { get; set; }
        public Guid FinancialPartDefinitionId { get; set; }
        public string AccountIdColumnName { get; set; }

        public override void ConvertSourceBEs(ITargetBEConvertorConvertSourceBEsContext context)
        {
            SqlSourceBatch sourceBatch = context.SourceBEBatch as SqlSourceBatch;
            Dictionary<Int64, ITargetBE> maultiNetAccounts = new Dictionary<Int64, ITargetBE>();

            sourceBatch.Data.DefaultView.Sort = "CUS_CUSTOMERID";
            foreach (DataRow row in sourceBatch.Data.Rows)
            {
                ITargetBE targetMultiNetAccount;
                var sourceId = (Int64)row["CUS_CUSTOMERID"];
                string accountName = row["CUS_NAME"] as string;
                if (!maultiNetAccounts.TryGetValue(sourceId, out targetMultiNetAccount))
                {
                    try
                    {
                        SourceAccountData accountData = new SourceAccountData
                        {
                            Account = new Account()
                        };

                        accountData.Account.Name = accountName;
                        accountData.Account.CreatedTime = row["SU_INSERTDATE"] != DBNull.Value ? (DateTime)row["SU_INSERTDATE"] : default(DateTime);
                        accountData.Account.SourceId = string.Format("Company_{0}", sourceId);
                        accountData.Account.TypeId = this.AccountTypeId;
                        accountData.Account.StatusId = this.InitialStatusId;

                        accountData.Account.Settings = new AccountSettings
                        {
                            Parts = new AccountPartCollection()
                        };

                        FillCompanyProfile(accountData, row);
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

        void FillCompanyExtendedInfo(SourceAccountData accountData, DataRow row)
        {
            accountData.Account.Settings.Parts.Add(new Guid("ee7fee18-ebfc-4f85-955b-95ea98519abb"), new AccountPart
            {
                Settings = new MultiNetCompanyExtendedInfo
                {

                }
            });
        }
        void FillCompanyProfile(SourceAccountData accountData, DataRow row)
        {

            CityManager cityManager = new CityManager();
            City city = cityManager.GetCityBySourceId(((int)row["CI_CITYID"]).ToString());

            accountData.Account.Settings.Parts.Add(this.CompanyProfilePartDefinitionId, new AccountPart
            {
                Settings = new AccountPartCompanyProfile
                {
                    Contacts = GetContactsList(row),
                    Website = row["CUS_WEB"] as string,
                    CityId = city != null ? city.CityId : (int?)null,
                    CountryId = city != null ? city.CountryId : (int?)null
                }
            });
            FillFinancialInfo(accountData, row);
            FillCompanyExtendedInfo(accountData, row);
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
                Email = row["CUS_EMAIL"] as string
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
}
