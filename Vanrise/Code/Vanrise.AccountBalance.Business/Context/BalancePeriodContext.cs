using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Vanrise.AccountBalance.Entities;

namespace Vanrise.AccountBalance.Business
{
    public class BalancePeriodContext : IBalancePeriodContext
    {
        public DateTime? LastPeriodDate { get; set; }

        public DateTime? NextPeriodDate { get; set; }
    }
}
