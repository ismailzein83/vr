using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TOne.WhS.Sales.Entities
{
    public abstract class BulkActionType
    {
        public abstract bool IsZoneApplicable(long zoneId);
    }
}
