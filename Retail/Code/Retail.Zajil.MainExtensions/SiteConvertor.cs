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

namespace Retail.Zajil.MainExtensions
{
    public class SiteConvertor : TargetBEConvertor
    {
        #region Properties
        public override string Name
        {
            get
            {
                return "Zajil Site Convertor";
            }
        }
        public Guid AccountBEDefinitionId { get; set; }
        public Guid SiteAccountTypeId { get; set; }
        public Guid InitialStatusId { get; set; }
        public string ParentAccountIdColumn { get; set; }
        public string SOColumn { get; set; }

        #endregion

        public override void Initialize(ITargetBEConvertorInitializeContext context)
        {
            context.InitializationData = new AccountBEManager().GetCachedAccountsBySourceId(AccountBEDefinitionId);
        }
        public override void ConvertSourceBEs(ITargetBEConvertorConvertSourceBEsContext context)
        {

            Dictionary<string, Account> accountsBySourceId = context.InitializationData as Dictionary<string, Account>;
            SqlSourceBatch sourceBatch = context.SourceBEBatch as SqlSourceBatch;
            List<ITargetBE> zajilAccounts = new List<ITargetBE>();

            sourceBatch.Data.DefaultView.Sort = ParentAccountIdColumn;
            foreach (DataRow row in sourceBatch.Data.Rows)
            {
                var parentId = row[ParentAccountIdColumn] as string;
                try
                {
                    var so = GetIntValue(row, SOColumn);
                    Account parentAccount;
                    if (accountsBySourceId.TryGetValue(parentId, out parentAccount))
                    {
                        string name = string.Format("Site {0}", so);
                        SourceAccountData accountData = new SourceAccountData
                        {
                            Account = new Account
                            {
                                TypeId = this.SiteAccountTypeId,
                                Name = name,
                                StatusId = this.InitialStatusId,
                                ParentAccountId = parentAccount.AccountId,
                                Settings = new AccountSettings(),
                                SourceId = string.Format("Site_{0}_{1}", parentId, so)
                            }
                        };



                        zajilAccounts.Add(accountData);
                    }
                    else
                    {
                        context.WriteBusinessTrackingMsg(LogEntryType.Warning, "Site Account was not created for Source Company Id {0}, Company does not exist", parentId);
                    }

                }
                catch (Exception ex)
                {
                    var finalException = Utilities.WrapException(ex, String.Format("Failed to create Site Account (Id: '{0}') due to conversion error", parentId));
                    context.WriteBusinessHandledException(finalException);
                }
            }
            context.TargetBEs = zajilAccounts;
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
        Account GetAccount(Account newAccount, Account existingAccount)
        {
            Account account = Serializer.Deserialize<Account>(Serializer.Serialize(existingAccount));
            account.SourceId = newAccount.SourceId;

            return account;
        }
        int GetIntValue(DataRow row, string columnName)
        {
            return row[columnName] == DBNull.Value ? 0 : int.Parse(row[columnName].ToString());
        }
    }
}
