using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Retail.BusinessEntity.Entities;
using Vanrise.BEBridge.Entities;

namespace Retail.BusinessEntity.Business
{
    public class AccountSynchronizer : TargetBESynchronizer
    {
        public override void InsertBEs(ITargetBESynchronizerInsertBEsContext context)
        {
            AccountManager accountManager = new AccountManager();
            foreach (var targetAccount in context.TargetBE)
            {
                SourceAccountData accountData = targetAccount as SourceAccountData;
                accountManager.AddAccount(accountData.Account);
            }
        }

        public override bool TryGetExistingBE(ITargetBESynchronizerTryGetExistingBEContext context)
        {
            AccountManager accountManager = new AccountManager();
            Account account = accountManager.GetAccountBySourceId(context.SourceBEId as string);
            if (account != null)
            {
                context.TargetBE = new SourceAccountData
                {
                    Account = account
                };
                return true;
            }
            return false;
        }

        public override void UpdateBEs(ITargetBESynchronizerInsertBEsContext context)
        {
            AccountManager accountManager = new AccountManager();
            foreach (var target in context.TargetBE)
            {
                SourceAccountData accountData = target as SourceAccountData;
                AccountToEdit editAccount = new AccountToEdit
                {
                    Settings = accountData.Account.Settings,
                    AccountId = accountData.Account.AccountId,
                    Name = accountData.Account.Name,
                    TypeId = accountData.Account.TypeId,
                    SourceId = accountData.Account.SourceId
                };
                accountManager.UpdateAccount(editAccount);
            }
        }
    }
}
