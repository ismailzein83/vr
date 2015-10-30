using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TOne.WhS.Sales.Entities.Queries
{
    public class RatePlanItemInput
    {
        public RatePlanItemFilter Filter { get; set; }

        public int FromRow { get; set; }

        public int ToRow { get; set; }
    }

    public class RatePlanItemFilter
    {
        public int CustomerId { get; set; }

        public char ZoneLetter { get; set; }

        public int CountryId { get; set; }
    }
}
