using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Vanrise.NumberingPlan.Entities
{
    public abstract class BaseSaleCodeQueryHandler
    {
        public abstract IEnumerable<SaleCode> GetFilteredSaleCodes();
    }
}
