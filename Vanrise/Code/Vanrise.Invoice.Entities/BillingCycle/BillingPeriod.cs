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
        public abstract List<BillingInterval> GetPeriod(IBillingPeriodContext context);
        public abstract string GetDescription();
    }
    public interface IBillingPeriodContext
    {
        DateTime? PreviousPeriodEndDate { get; set; }
        DateTime IssueDate { get;}
    }
}
