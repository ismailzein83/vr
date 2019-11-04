using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TOne.WhS.BusinessEntity.Entities
{
    public class ApplyMarginRuleContext : IApplyMarginRuleContext
    {
        public decimal Margin { get; set; }

        public int? MarginCurrencyId { get; set; }

        public DateTime? EffectiveDate { get; set; }

        public Guid? MarginCategory { get; set; }
    }
}