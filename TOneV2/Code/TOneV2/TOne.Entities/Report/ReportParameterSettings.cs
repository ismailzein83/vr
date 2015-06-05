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

        public bool RequiresSupllierId { get; set; }

        public bool RequiresCustomerAMUId { get; set; }

        public bool RequiresSupllierAMUId { get; set; }

        public bool RequiresGroupByCustomer { get; set; }


    }
}
