using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Vanrise.Invoice.Entities;

namespace Vanrise.Invoice.Business
{
    public class StartDateCalculationMethodContext : IStartDateCalculationMethodContext
    {
        public BillingPeriod BillingPeriod { get; set; }
        public DateTime InitialStartDate { get; set; }
        public DateTime PartnerCreatedDate { get; set; }
        public DateTime FromDate { get; set; }
        public DateTime ToDate { get; set; }
    }
}
