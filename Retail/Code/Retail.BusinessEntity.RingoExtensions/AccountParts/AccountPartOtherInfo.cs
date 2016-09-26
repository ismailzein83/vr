using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Retail.BusinessEntity.Entities;

namespace Retail.BusinessEntity.RingoExtensions.AccountParts
{
    public class AccountPartOtherInfo : AccountPartSettings
    {
        public const int ExtensionConfigId = 29;
        public string CNIC { get; set; }
        public string TaxCode { get; set; }
        public bool IsTheft { get; set; }
        public DateTime MNPProcessDate { get; set; }

    }
}
