using Retail.BusinessEntity.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Retail.BusinessEntity.MainExtensions.ContractTypes.PostPaid
{
    public class PostPaidSettings : ContractExtendedSettings
    {
        public Decimal CreditLimit { get; set; }

        public int CurrencyId { get; set; }
    }
}
