using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Retail.BusinessEntity.Entities
{
    public class FinancialAccountDetail
    {
        public string FinancialAccountDefinitionName { get; set; }
        public DateTime BED { get; set; }

        public DateTime? EED { get; set; }
    }
}
