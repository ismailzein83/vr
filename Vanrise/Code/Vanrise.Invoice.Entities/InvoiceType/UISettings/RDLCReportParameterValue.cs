using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Vanrise.Invoice.Entities
{
    public abstract class RDLCReportParameterValue
    {
        public Guid ConfigId { get; set; }
        public abstract dynamic Evaluate(IRDLCReportParameterValueContext context);
    }
}
