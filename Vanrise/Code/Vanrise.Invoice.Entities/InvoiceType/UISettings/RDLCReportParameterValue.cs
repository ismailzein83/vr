using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Vanrise.Invoice.Entities
{
    public abstract class RDLCReportParameterValue
    {
        public abstract Guid ConfigId { get; }
        public abstract dynamic Evaluate(IRDLCReportParameterValueContext context);
    }
}
