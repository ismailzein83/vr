using Retail.BusinessEntity.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Retail.BusinessEntity.Business
{
    public class FinancialAccountFilterContext : IFinancialAccountFilterContext
    {
        public Guid AccountBEDefinitionId { get; set; }
        public long AccountId { get; set; }

        public string FinancialAccountId { get; set; }
    }
}
