using Retail.BusinessEntity.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Retail.BusinessEntity.Business
{
    public class AccountGenericFieldContext : IAccountGenericFieldContext
    {
        Account _account;
        public AccountGenericFieldContext(Account account)
        {
            _account = account;
        }
        public Account Account
        {
            get
            {
                return _account;
            }
        }
    }
}
