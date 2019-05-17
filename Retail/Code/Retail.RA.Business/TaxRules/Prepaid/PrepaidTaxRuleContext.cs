using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Vanrise.Rules;

namespace Retail.RA.Business
{
    public class PrepaidTaxRuleContext : IPrepaidTaxRuleContext
    {
        public decimal? TotalResidualAmount { get; set; }
        public decimal? TotalTopUpsAmount { get; set; }
        public decimal TotalTaxValue { set; get; }
        public IVRRule Rule { get; set; }
    }

    public interface IPrepaidTaxRuleContext : IRuleExecutionContext
    {
        decimal? TotalResidualAmount { get; }
        decimal? TotalTopUpsAmount { get; }
        decimal TotalTaxValue { get; set; }
    }
}
