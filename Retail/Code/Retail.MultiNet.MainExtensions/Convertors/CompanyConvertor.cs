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

namespace Retail.MultiNet.MainExtensions.Convertors
{
    public class CompanyConvertor : TargetBEConvertor
    {
        public Guid AccountBEDefinitionId { get; set; }
        public Guid AccountTypeId { get; set; }
        public Guid InitialStatusId { get; set; }
        public Guid CompanyProfilePartDefinitionId { get; set; }
        public string AccountIdColumnName { get; set; }

        public override void ConvertSourceBEs(ITargetBEConvertorConvertSourceBEsContext context)
        {
            SqlSourceBatch sourceBatch = context.SourceBEBatch as SqlSourceBatch;
            Dictionary<string, ITargetBE> maultiNetAccounts = new Dictionary<string, ITargetBE>();

            sourceBatch.Data.DefaultView.Sort = "CUS_CUSTOMERID";
            foreach (DataRow row in sourceBatch.Data.Rows)
            {
                ITargetBE targetMultiNetAccount;
                var sourceId = row["CUS_CUSTOMERID"] as string;
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
                        accountData.Account.SourceId = sourceId;
                        accountData.Account.TypeId = this.AccountTypeId;
                        accountData.Account.StatusId = this.InitialStatusId;
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

        void FillCompanyProfile(SourceAccountData accountData, DataRow row)
        {

            accountData.Account.Settings.Parts.Add(this.CompanyProfilePartDefinitionId, new AccountPart
            {
                Settings = new AccountPartCompanyProfile
                {
                    Contacts = GetContactsList(row),
                    Website = row["CUS_WEB"] as string
                }
            });
        }
        Dictionary<string, AccountCompanyContact> GetContactsList(DataRow row)
        {
            Dictionary<string, AccountCompanyContact> contacts = new Dictionary<string, AccountCompanyContact>();

            contacts.Add("Main", new AccountCompanyContact
            {
                Email = row["CUS_EMAIL"] as string,
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
