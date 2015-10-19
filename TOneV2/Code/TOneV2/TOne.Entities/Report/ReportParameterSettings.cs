using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TOne.Entities
{
    public class ReportParameterSettings
    {
        public bool RequiresFromTime { get; set; }

        public bool RequiresToTime { get; set; }

        public bool RequiresCustomerId { get; set; }

        public bool RequiresSupplierId { get; set; }

        public bool RequiresCustomerAMUId { get; set; }

        public bool RequiresSupllierAMUId { get; set; }

        public bool RequiresGroupByCustomer { get; set; }

        public bool RequiresIsCost { get; set; }

        public bool RequiresCurrencyId { get; set; }

        public bool RequiresSupplierGroup { get; set; }

        public bool RequiresCustomerGroup { get; set; }

        public bool RequiresGroupBySupplier { get; set; }

        public bool RequiresIsService { get; set; }

        public bool RequiresIsCommission { get; set; }

        public bool RequiresServicesForCustomer { get; set; }

        public bool RequiresMargin { get; set; }
        public bool RequiresZoneId { get; set; }
        public bool RequiresIsExchange { get; set; }

        public bool RequiresTop { get; set; }

        public bool RequiresPageBreak { get; set; }

        public bool CustomerIdNotOptional { get; set; }

        public bool RequiresSingleCustomer { get; set; }
    }
}
