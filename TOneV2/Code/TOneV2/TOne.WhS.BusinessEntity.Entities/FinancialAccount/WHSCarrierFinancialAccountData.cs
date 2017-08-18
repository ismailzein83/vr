using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TOne.WhS.BusinessEntity.Entities
{
    public class WHSCarrierFinancialAccountData
    {
        public WHSFinancialAccount FinancialAccount { get; set; }

        public int CurrencyId { get; set; }

        public WHSCarrierFinancialAccountBalanceData AccountBalanceData { get; set; }

        public WHSCarrierFinancialAccountInvoiceData InvoiceData { get; set; }
    }

    public class WHSCarrierFinancialAccountBalanceData
    {
        public Guid AccountTypeId { get; set; }

        public Decimal? CreditLimit { get; set; }
    }

    public class WHSCarrierFinancialAccountInvoiceData
    {
        public Guid InvoiceTypeId { get; set; }
    }
}
