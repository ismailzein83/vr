using Retail.BusinessEntity.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Retail.BusinessEntity.Business
{
    public class AccountServiceGenericFieldContext : IAccountServiceGenericFieldContext
    {
        Account _account;
        AccountService _accountService;
        public AccountServiceGenericFieldContext(Account account, AccountService accountService)
        {
            _account = account;
            _accountService = accountService;
        }
        public Account Account
        {
            get
            {
                return _account;
            }
        }

        public AccountService AccountService
        {
            get
            {
                return _accountService;
            }
        }
    }
}
