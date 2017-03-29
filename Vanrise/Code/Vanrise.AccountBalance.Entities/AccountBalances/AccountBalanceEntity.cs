using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Vanrise.AccountBalance.Entities
{
    public class AccountBalanceEntity
    {
        public long AccountBalanceId { get; set; }
        public string AccountId { get; set; }
        public Dictionary<string, AccountBalanceDetailObject> Items { get; set; }
    }
    public class AccountBalanceDetailObject
    {
        public Object Value { get; set; }
        public string Description { get; set; }
    }

}
