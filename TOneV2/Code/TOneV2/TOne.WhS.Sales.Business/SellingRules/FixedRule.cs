using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TOne.WhS.Sales.Entities;

namespace TOne.WhS.Sales.Business.SellingRules
{
    public class FixedRule : SellingRuleSettings
    {
        public decimal FromRate { get; set; }
        public decimal ToRate { get; set; }
        public override void Execute(ISellingRuleExecutionContext context)
        {
            throw new NotImplementedException();
        }
    }
}
