using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TOne.WhS.AccountBalance.Entities
{
    public abstract class FinancialAccountDefinitionSettings
    {
        public abstract Guid ConfigId { get; }

        public abstract bool IsApplicableToCustomer { get; }

        public abstract bool IsApplicableToSupplier { get; }

        public virtual string RuntimeEditor { get; set; }
    }
}
