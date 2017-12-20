using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Retail.Teles.Entities
{
    public enum AccountEnterpriseDIDType { SN = 0, BT = 1 }
    public class AccountEnterpriseDID
    {
        public long? AccountId { get; set; }
        public string EnterpriseId { get; set; }
        public string ScreenNumber { get; set; }
        public int MaxCalls { get; set; }
        public AccountEnterpriseDIDType Type { get; set; }
    }
}
