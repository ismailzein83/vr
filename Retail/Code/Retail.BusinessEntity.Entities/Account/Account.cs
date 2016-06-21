using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Retail.BusinessEntity.Entities
{
    public class BaseAccount
    {
        public long AccountId { get; set; }

        public string Name { get; set; }

        public int TypeId { get; set; }

        public AccountSettings Settings { get; set; }
    }

    public class AccountSettings
    {
        public AccountPartCollection Parts { get; set; }
    }

    public class Account : BaseAccount
    {
        public const string BUSINESSENTITY_DEFINITION_NAME = "Retail_BE_SubscriberAccount";

        public long? ParentAccountId { get; set; }
    }
    
    public class AccountToEdit : BaseAccount
    {

    }
}
