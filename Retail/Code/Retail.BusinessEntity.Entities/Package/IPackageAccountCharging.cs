using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Retail.BusinessEntity.Entities
{
    public interface IPackageAccountCharging
    {
        bool TryGetAccountRecurringChargingId(IPackageAccountChargingContext context);
    }

    public interface IPackageAccountChargingContext
    {
        int RecurringChargingId { set; }
    }
}
