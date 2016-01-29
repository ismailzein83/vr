using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TOne.WhS.Sales.Entities
{
    public interface ISellingRuleExecutionContext
    {
        Decimal? CustomerRate { get; }
        Decimal? ProductRate { get; }
        MarginStatus Status { set; }
    }
}
