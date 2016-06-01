using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Retail.BusinessEntity.Entities
{
    public class BaseAccount
    {
        public int AccountId { get; set; }

        public string Name { get; set; }

        public AccountType Type { get; set; }

        public AccountSettings Settings { get; set; }
    }

    public enum AccountType { Residential = 0, Enterprise = 1 }

    public class AccountSettings { }

    public class Account : BaseAccount
    {
        public int? ParentAccountId { get; set; }
    }

    public class AccountToEdit : BaseAccount
    {

    }
}
