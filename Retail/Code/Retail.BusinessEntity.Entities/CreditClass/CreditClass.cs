using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Retail.BusinessEntity.Entities
{
    public class CreditClass
    {
        public int CreditClassId { get; set; }

        public string Name { get; set; }

        public CreditClassSettings Settings { get; set; }
    }

    public class CreditClassSettings
    {
        public Decimal BalanceLimit { get; set; }

        public int CurrencyId { get; set; }
    }
}
