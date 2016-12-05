using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Vanrise.Invoice.Entities
{
    public abstract class BillingPeriod
    {
        public abstract Guid ConfigId { get; }
        public abstract DateTime GetPeriod(DateTime fromDate);
    }
}
