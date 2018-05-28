using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TOne.WhS.BusinessEntity.Entities
{
    public class RecurringChargePeriod
    {
        public RecurringChargePeriodSettings Settings { get; set; }
    }
    public abstract class RecurringChargePeriodSettings
    {
        public abstract Guid ConfigId { get; }
        public abstract void Evaluate(IRecurringChargePeriodSettingsContext context);
    }
    public interface IRecurringChargePeriodSettingsContext
    {

    }
}
