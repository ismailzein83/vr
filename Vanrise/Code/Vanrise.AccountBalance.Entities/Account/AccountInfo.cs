using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Vanrise.Entities;

namespace Vanrise.AccountBalance.Entities
{
    public class AccountInfo
    {
        public const string BEInfoType = "VRAccountBalance_AccountInfo";
        public string Name { get; set; }

        public int CurrencyId { get; set; }

        public string StatusDescription { get; set; }
        public DateTime? BED { get; set; }
        public DateTime? EED { get; set; }
        public bool IsDeleted { get; set; }
        public VRAccountStatus Status { get; set; }

    }
}
