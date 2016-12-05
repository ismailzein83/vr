using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Vanrise.Invoice.Entities;

namespace Vanrise.Invoice.Business.Context
{
    public class InitialPeriodInfoContext : IInitialPeriodInfoContext
    {
        public string PartnerId { get; set; }
        public DateTime InitialStartDate { get; set; }
        public DateTime PartnerCreationDate { get; set; }
    }
}
