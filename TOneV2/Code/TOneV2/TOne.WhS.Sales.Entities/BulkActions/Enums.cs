using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TOne.WhS.Sales.Entities
{
    public enum SpecificApplicableZoneEntityType
    {
        Country = 0,
        Zone = 1
    }

    public enum RateBulkActionValidationResultType
    {
        Valid = 0,
        DoesNotExist = 1,
        Zero = 2,
        Negative = 3,
        EqualsCurrentNormalRate = 4
    }
}
