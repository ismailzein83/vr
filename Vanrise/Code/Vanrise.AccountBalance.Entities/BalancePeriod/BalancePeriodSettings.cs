using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Vanrise.AccountBalance.Entities
{
    public abstract class BalancePeriodSettings
    {
        public int ConfigId { get; set; }

        public abstract void Execute(IBalancePeriodContext context);
    }
    public interface IBalancePeriodContext
    {
        DateTime? LastPeriodDate { get; }

        DateTime? NextPeriodDate { set; }
    }
}
