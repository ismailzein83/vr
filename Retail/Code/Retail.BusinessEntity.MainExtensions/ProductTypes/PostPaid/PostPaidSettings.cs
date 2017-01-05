using Retail.BusinessEntity.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Retail.BusinessEntity.MainExtensions.ProductTypes.PostPaid
{
    public class PostPaidSettings : ProductExtendedSettings
    {
        public Decimal CreditLimit { get; set; }

        public int CurrencyId { get; set; }
    }
}
