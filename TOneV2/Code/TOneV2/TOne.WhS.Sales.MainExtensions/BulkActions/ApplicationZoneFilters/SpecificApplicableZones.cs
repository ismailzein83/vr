using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TOne.WhS.Sales.Entities;

namespace TOne.WhS.Sales.MainExtensions
{
    public class SpecificApplicableZones : BulkActionZoneFilter
    {
        protected IEnumerable<long> SelectedZoneIds { get; set; }

        public override IEnumerable<long> GetApplicableZoneIds(IApplicableZoneIdsContext context)
        {
            return this.SelectedZoneIds;
        }
    }
}
