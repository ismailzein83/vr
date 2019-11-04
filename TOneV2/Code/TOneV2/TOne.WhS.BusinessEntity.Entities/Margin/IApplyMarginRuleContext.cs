using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TOne.WhS.BusinessEntity.Entities
{
    public interface IApplyMarginRuleContext
    {
        decimal Margin { get; }

        int? MarginCurrencyId { get; }

        DateTime? EffectiveDate { get; }

        Guid? MarginCategory { set; }
    }
}