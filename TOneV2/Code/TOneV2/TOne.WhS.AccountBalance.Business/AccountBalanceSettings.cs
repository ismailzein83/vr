using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Vanrise.AccountBalance.Entities;

namespace TOne.WhS.AccountBalance.Business
{
    public class AccountBalanceSettings : AccountTypeExtendedSettings
    {
        public override Guid ConfigId
        {
            get { throw new NotImplementedException(); }
        }

        public override string AccountSelector
        {
            get { throw new NotImplementedException(); }
        }

        public override IAccountManager GetAccountManager()
        {
            return new AccountBalanceManager();
        }
    }
}
