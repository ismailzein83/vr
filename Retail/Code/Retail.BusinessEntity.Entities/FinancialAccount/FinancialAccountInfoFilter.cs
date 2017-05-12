using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Retail.BusinessEntity.Entities
{
    public class FinancialAccountInfoFilter
    {
        public FinancialAccountEffective? FinancialAccountEffective { get; set; }
        public List<long> AccountIds { get; set; }
    }
}
