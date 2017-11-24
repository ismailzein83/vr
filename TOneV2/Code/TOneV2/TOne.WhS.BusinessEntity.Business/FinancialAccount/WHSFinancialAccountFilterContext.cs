using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TOne.WhS.BusinessEntity.Entities;

namespace TOne.WhS.BusinessEntity.Business
{
    public class WHSFinancialAccountFilterContext : IWHSFinancialAccountFilterContext
    {
        public int FinancialAccountId { get; set; }
        public Guid? InvoiceTypeId { get; set; }
        public Guid? BalanceAccountTypeId { get; set; }
       
    }
}
