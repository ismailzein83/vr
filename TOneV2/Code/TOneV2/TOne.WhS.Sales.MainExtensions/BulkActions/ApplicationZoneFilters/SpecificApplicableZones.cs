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
		public override Guid ConfigId { get { return new Guid("61D047D6-DF3D-4D74-9C2C-7CEA2907C2B3"); } }

        protected IEnumerable<long> SelectedZoneIds { get; set; }

        public override IEnumerable<long> GetApplicableZoneIds(IApplicableZoneIdsContext context)
        {
            return this.SelectedZoneIds;
        }
    }
}
