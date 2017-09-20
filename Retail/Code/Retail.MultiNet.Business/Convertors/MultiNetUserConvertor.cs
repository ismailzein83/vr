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

namespace Retail.MultiNet.Business.Convertors
{
    public class MultiNetUserConvertor : TargetBEConvertor
    {
        public override string Name
        {
            get
            {
                return "MultiNet GP User Convertor";
            }
        }
        public Guid AccountBEDefinitionId { get; set; }
        public Guid AccountTypeId { get; set; }
        public Guid InitialStatusId { get; set; }
        public Guid UserProfilePartDefinitionId { get; set; }
        public string AccountIdColumnName { get; set; }
        public string UserIdColumnName { get; set; }
        public string NameColumnName { get; set; }

        public override void Initialize(ITargetBEConvertorInitializeContext context)
        {
            context.InitializationData = new AccountBEManager().GetCachedAccountsBySourceId(this.AccountBEDefinitionId);
        }
        public override void ConvertSourceBEs(ITargetBEConvertorConvertSourceBEsContext context)
        {
            SqlSourceBatch sourceBatch = context.SourceBEBatch as SqlSourceBatch;
            Dictionary<string, ITargetBE> maultiNetAccounts = new Dictionary<string, ITargetBE>();
            var accounts = context.InitializationData as Dictionary<string, Account>;
            sourceBatch.Data.DefaultView.Sort = AccountIdColumnName;
            foreach (DataRow row in sourceBatch.Data.Rows)
            {
                ITargetBE targetMultiNetAccount;
                var sourceId = (row[UserIdColumnName] as string);
                var parentId = string.Format("Account_{0}", (row[AccountIdColumnName] as string).Trim());
                string accountName = row[NameColumnName] as string;
                if (!maultiNetAccounts.TryGetValue(sourceId, out targetMultiNetAccount))
                {
                    try
                    {
                        SourceAccountData accountData = new SourceAccountData
                        {
                            Account = new Account()
                        };

                        Account parentAccount;
                        if (accounts.TryGetValue(parentId, out parentAccount))
                        {
                            accountData.Account.ParentAccountId = parentAccount.AccountId;
                        }

                        accountData.Account.Name = accountName;
                        accountData.Account.SourceId = string.Format("GPUser_{0}", sourceId);
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
                        var finalException = Utilities.WrapException(ex, String.Format("Failed to import User (Id: '{0}' Name: '{1}') due to conversion error", sourceId, accountName));
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

            accountData.Account.Settings.Parts.Add(this.UserProfilePartDefinitionId, new AccountPart
            {
                Settings = new AccountPartPersonalInfo
                {
                    FirstName = (row[NameColumnName] as string).Trim()
                }
            });
        }
        Account GetAccount(Account newAccount, Account existingAccount)
        {
            Account account = Serializer.Deserialize<Account>(Serializer.Serialize(existingAccount));
            account.SourceId = newAccount.SourceId;


            return account;
        }
    }
}
