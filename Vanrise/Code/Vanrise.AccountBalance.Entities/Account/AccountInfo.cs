using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Vanrise.AccountBalance.Entities
{
    public class AccountInfo
    {
        public const string BEInfoType = "VRAccountBalance_AccountInfo";
        public string Name { get; set; }

        public int CurrencyId { get; set; }

        public string StatusDescription { get; set; }
    }
}
