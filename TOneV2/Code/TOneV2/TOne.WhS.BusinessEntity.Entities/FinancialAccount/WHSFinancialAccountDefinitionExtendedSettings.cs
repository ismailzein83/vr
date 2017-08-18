using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TOne.WhS.BusinessEntity.Entities
{
    public abstract class WHSFinancialAccountDefinitionExtendedSettings
    {
        public abstract Guid ConfigId { get; }

        public virtual string RuntimeEditor { get; set; }

        public abstract bool IsApplicableToCustomer { get; }

        public abstract bool IsApplicableToSupplier { get; }
    }
}
