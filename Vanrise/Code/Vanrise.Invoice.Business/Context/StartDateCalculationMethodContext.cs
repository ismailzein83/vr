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
        public DateTime InitialStartDate { get; set; }
        public DateTime PartnerCreatedDate { get; set; }
        public DateTime StartDate { get; set; }
    }
}
