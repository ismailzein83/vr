using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TOne.WhS.Sales.Entities
{
    public abstract class ApplicableZoneType
    {
        protected IEnumerable<long> SelectedZoneIds { get; set; }

        public abstract IEnumerable<long> GetApplicableZoneIds();
    }
}
