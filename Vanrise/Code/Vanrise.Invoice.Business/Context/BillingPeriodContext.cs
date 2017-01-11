using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Vanrise.Invoice.Entities;

namespace Vanrise.Invoice.Business.Context
{
    public class BillingPeriodContext : IBillingPeriodContext
    {
        public DateTime? PreviousPeriodEndDate { get; set; }
        public DateTime IssueDate { get; set; }
    }
}
