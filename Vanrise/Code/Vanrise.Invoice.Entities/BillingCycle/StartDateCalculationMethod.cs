using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Vanrise.Invoice.Entities
{
    public abstract class StartDateCalculationMethod
    {
        public abstract Guid ConfigId { get; }
        public abstract void CalculateDate(IStartDateCalculationMethodContext context);
    }
    public interface IStartDateCalculationMethodContext
    {
         BillingPeriod BillingPeriod { get; }
         DateTime InitialStartDate { get;  }
         DateTime PartnerCreatedDate { get; }
         DateTime FromDate { set; }
         DateTime ToDate { set; }
    }
}
