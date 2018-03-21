using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Vanrise.Entities
{
    public class StatusDefinition
    {
        public Guid StatusDefinitionId { get; set; }

        public string Name { get; set; }

        public Guid BusinessEntityDefinitionId { get; set; }

        public StatusDefinitionSettings Settings { get; set; }

        public DateTime CreatedTime { get; set; }

        public int? CreatedBy { get; set; }

        public int? LastModifiedBy { get; set; }

        public DateTime? LastModifiedTime { get; set; }
    }

    public class StatusDefinitionSettings
    {
        public Guid StyleDefinitionId { get; set; }

        public bool HasInitialCharge { get; set; }

        public bool HasRecurringCharge { get; set; }

        public bool IsActive { get; set; }

        public bool IsInvoiceActive { get; set; }

        public bool IsAccountBalanceActive { get; set; }
    }
}
