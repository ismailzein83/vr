using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Retail.BusinessEntity.Entities
{
    public abstract class RecurringPeriodSettings
    {
        public abstract Guid ConfigId { get; }

        public abstract void Execute(IRecurringPeriodContext context);
    }

    public interface IRecurringPeriodContext
    {
        DateTime? LastPeriodDate { get; }

        DateTime? NextPeriodDate { set; }
    }
}
