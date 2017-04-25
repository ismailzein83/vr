using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Retail.BusinessEntity.Entities
{
    public class FinancialAccount : Vanrise.Entities.IDateEffectiveSettings
    {
        public Guid FinancialAccountDefinitionId { get; set; }

        public DateTime BED { get; set; }

        public DateTime? EED { get; set; }

        public FinancialAccountExtendedSettings ExtendedSettings { get; set; }
    }

    public abstract class FinancialAccountExtendedSettings
    {
    }
}
