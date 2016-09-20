using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Vanrise.AccountBalance.Entities
{
    public abstract class BalancePeriodSettings
    {
        public abstract  Guid ConfigId { get; }

        public abstract void Execute(IBalancePeriodContext context);
    }
    public interface IBalancePeriodContext
    {
        DateTime? LastPeriodDate { get; }

        DateTime? NextPeriodDate { set; }
    }
}
