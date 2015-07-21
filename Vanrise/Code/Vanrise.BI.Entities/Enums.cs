using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Vanrise.BI.Entities
{
    public enum TimeDimensionType
    {
        Yearly = 0,
        Monthly = 1,
        Weekly = 2,
        Daily = 3,
        Hourly = 4
    }

    public enum MeasureType
    {
        DurationInMinutes = 0,
        Sale = 1,
        Cost = 2,
        Profit = 3,
        SuccessfulAttempts = 4,
        ACD = 5,
        PDD = 6
    }

    public enum EntityType
    {
        SaleZone = 0,
        Customer = 1,
        Supplier = 2
    }
    public enum ConfigurationType
    {
        Entity=0,
        Measure=1
    }
}
