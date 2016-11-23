using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Retail.BusinessEntity.Entities
{
    public interface IPackageServiceRecurringCharging
    {
        bool TryGetServiceRecurringChargingId(IPackageServiceRecurringChargingContext context);
    }

    public interface IPackageServiceRecurringChargingContext
    {
        Guid ServiceTypeId { get; }

        int RecurringChargingId { set; }
    }
}