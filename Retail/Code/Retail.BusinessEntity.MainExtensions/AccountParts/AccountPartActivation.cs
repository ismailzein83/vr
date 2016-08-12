using Retail.BusinessEntity.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Retail.BusinessEntity.MainExtensions.AccountParts
{
    public class AccountPartActivation : AccountPartSettings
    {
        public const int ExtensionConfigId = 20;
        public AccountStatus Status { get; set; }

        public DateTime ActivationDate { get; set; }
    }
}
