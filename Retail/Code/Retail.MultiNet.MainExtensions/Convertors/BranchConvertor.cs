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

namespace Retail.MultiNet.MainExtensions.Convertors
{
    public class BranchConvertor : TargetBEConvertor
    {
        public Guid AccountBEDefinitionId { get; set; }
        public Guid AccountTypeId { get; set; }
        public Guid InitialStatusId { get; set; }
        public Guid CompanyProfilePartDefinitionId { get; set; }
        public string BrachIdColumnName { get; set; }
        public string CompanyIdColumnName { get; set; }

        public override void Initialize(ITargetBEConvertorInitializeContext context)
        {
            context.InitializationData = new AccountBEManager().GetCachedAccountsBySourceId(this.AccountBEDefinitionId);
        }
        public override void ConvertSourceBEs(ITargetBEConvertorConvertSourceBEsContext context)
        {
            SqlSourceBatch sourceBatch = context.SourceBEBatch as SqlSourceBatch;
            Dictionary<Int64, ITargetBE> maultiNetAccounts = new Dictionary<Int64, ITargetBE>();
            var accounts = context.InitializationData as Dictionary<string, Account>;
            sourceBatch.Data.DefaultView.Sort = BrachIdColumnName;
            foreach (DataRow row in sourceBatch.Data.Rows)
            {
                ITargetBE targetMultiNetAccount;
                var sourceId = (Int64)row[BrachIdColumnName];
                var parentId = (Int64)row[CompanyIdColumnName];
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
                        if (accounts.TryGetValue(parentId.ToString(), out parentAccount))
                        {
                            accountData.Account.ParentAccountId = parentAccount.AccountId;
                        }

                        accountData.Account.Name = accountName;
                        accountData.Account.CreatedTime = row["SU_INSERTDATE"] != DBNull.Value ? (DateTime)row["SU_INSERTDATE"] : default(DateTime);
                        accountData.Account.SourceId = sourceId.ToString();
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

        void FillCompanyProfile(SourceAccountData accountData, DataRow row)
        {

            accountData.Account.Settings.Parts.Add(this.CompanyProfilePartDefinitionId, new AccountPart
            {
                Settings = new AccountPartCompanyProfile
                {
                    Contacts = GetContactsList(row)
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
}